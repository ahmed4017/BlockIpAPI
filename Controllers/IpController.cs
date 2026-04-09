using BlockIpAPI.Models;
using BlockIpAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BlockIpAPI.Repositories;
namespace BlockIpAPI.Controllers
{
    [ApiController]
    [Route("api/ip")]
    public class IpController : ControllerBase
    {
        private readonly IIpService _ipService;
        private readonly ICountryRepository _repo;
        private readonly ILogService _logService;

        public IpController(IIpService ipService, ICountryRepository repo, ILogService logService)
        {
            _ipService = ipService;
            _repo = repo;
            _logService = logService;
        }

        private bool IsValidIp(string ip)
        {
            return System.Net.IPAddress.TryParse(ip, out _);
        }


        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup(string? ipAddress)
        {
            ipAddress ??= HttpContext.Connection.RemoteIpAddress?.ToString();

            

            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1" || ipAddress == "127.0.0.1")
            {
                return BadRequest("Could not determine caller IP");
            }

            if (!IsValidIp(ipAddress))
                return BadRequest("Invalid IP");

            var result = await _ipService.GetCountryByIpAsync(ipAddress);

            if (result == null)
                return StatusCode(500, "IP Service Failed");

            return Ok(result);
        }



        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ip) || ip == "::1" || ip == "127.0.0.1")
            {
                ip = "8.8.8.8";
            }

            var country = await _ipService.GetCountryByIpAsync(ip);

            if (country == null || string.IsNullOrEmpty(country.Code))
                return StatusCode(500, "IP Service Failed");

            var isBlocked = _repo.IsCountryBlocked(country.Code);

            _logService.Log(new BlockedAttemptLog
            {
                IpAddress = ip,
                CountryCode = country.Code,
                Timestamp = DateTime.UtcNow,
                IsBlocked = isBlocked,
                UserAgent = Request.Headers["User-Agent"]
            });

            return Ok(new
            {
                ip,
                country = country.Code,
                isBlocked
            });
        }
    }
}
