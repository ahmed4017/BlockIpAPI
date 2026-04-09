using BlockIpAPI.Repositories;
namespace BlockIpAPI.BackgroundServices

{
    public class TempBlockCleanupService : BackgroundService
    {
        private readonly ICountryRepository _repo;

        public TempBlockCleanupService(ICountryRepository repo)
        {
            _repo = repo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _repo.RemoveExpiredTempBlocks();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
