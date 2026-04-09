using BlockIpAPI.Models;

namespace BlockIpAPI.Services.Interfaces
{
    public interface ILogService
    {
        void Log(BlockedAttemptLog log);
        IEnumerable<BlockedAttemptLog> GetLogs();
    }
}
