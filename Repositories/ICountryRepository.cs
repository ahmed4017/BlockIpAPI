using BlockIpAPI.Models;

namespace BlockIpAPI.Repositories
{
    public interface ICountryRepository
    {
        bool AddBlockedCountry(string code, string name); bool RemoveBlockedCountry(string code);
        bool IsCountryBlocked(string code);
        IEnumerable<Country> GetBlockedCountries();

        bool AddTempBlock(string code, DateTime expiry);
        bool IsTempBlocked(string code);
        void RemoveExpiredTempBlocks();
    }
}
