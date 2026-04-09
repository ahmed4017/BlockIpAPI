using BlockIpAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlockIpAPI.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet("blocked-attempts")]
        public IActionResult GetLogs(int page = 1, int pageSize = 10)
        {
            var allLogs = _logService.GetLogs().ToList();
            var total = allLogs.Count;
            var logs = allLogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Ok(new { total, page, pageSize, data = logs });
        }
    }
}
