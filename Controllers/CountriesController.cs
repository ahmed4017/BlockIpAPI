









////////////////////////////////
using BlockIpAPI.DTOs;
using BlockIpAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BlockIpAPI.Repositories;

namespace BlockIpAPI.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepository _repo;
        private readonly IIpService _ipService;

        public CountriesController(ICountryRepository repo, IIpService ipService)
        {
            _repo = repo;
            _ipService = ipService;
        }

        [HttpPost("block")]
        public IActionResult BlockCountry(BlockCountryDto dto)
        {
            if (string.IsNullOrEmpty(dto.CountryCode))
                return BadRequest("Country code is required");

            var code = dto.CountryCode.ToUpper();

            if (code.Length != 2)
                return BadRequest("Invalid country code");

            // CHANGED: removed GetCountryName() call, using code as name fallback
            // reason: GetCountryName() had a hardcoded dictionary of only ~60 countries
            // any country outside that list would get its code as the name (e.g. KZ -> "KZ")
            var added = _repo.AddBlockedCountry(code, code);
            if (!added)
                return Conflict("Already blocked");

            return Ok(new { code });
        }

        [HttpDelete("block/{code}")]
        public IActionResult Unblock(string code)
        {
            var removed = _repo.RemoveBlockedCountry(code.ToUpper());
            if (!removed)
                return NotFound();

            return Ok();
        }

        [HttpGet("blocked")]
        public IActionResult GetBlocked(string? search, int page = 1, int pageSize = 10)
        {
            var query = _repo.GetBlockedCountries();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.Code.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var total = query.Count();

            var result = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Ok(new
            {
                total,
                page,
                pageSize,
                data = result
            });
        }

        [HttpPost("temporal-block")]
        public IActionResult TempBlock(TempBlockDto dto)
        {
            if (string.IsNullOrEmpty(dto.CountryCode))
                return BadRequest("Country code is required");

            if (dto.CountryCode.Length != 2)
                return BadRequest("Invalid country code");

            if (dto.DurationMinutes < 1 || dto.DurationMinutes > 1440)
                return BadRequest("Duration must be between 1 and 1440 minutes");

            var expiry = DateTime.UtcNow.AddMinutes(dto.DurationMinutes);
            var added = _repo.AddTempBlock(dto.CountryCode.ToUpper(), expiry);

            if (!added)
                return Conflict("Already temp blocked");

            return Ok(new { expiresAt = expiry });
        }

        // REMOVED: entire GetCountryName() method deleted
        // it was a 60-entry hardcoded dictionary that didn't belong in the controller
    }
}