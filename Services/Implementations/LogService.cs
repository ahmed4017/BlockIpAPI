using BlockIpAPI.Models;
using BlockIpAPI.Services.Interfaces;
using System.Collections.Concurrent;

public class LogService : ILogService
{
    private readonly ConcurrentBag<BlockedAttemptLog> _logs = new();

    public void Log(BlockedAttemptLog log)
    {
        _logs.Add(log);
    }

    public IEnumerable<BlockedAttemptLog> GetLogs()
    {
        return _logs.OrderByDescending(x => x.Timestamp);
    }
}