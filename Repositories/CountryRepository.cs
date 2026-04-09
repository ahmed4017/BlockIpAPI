using BlockIpAPI.Models;
using System.Collections.Concurrent;
using BlockIpAPI.Repositories;


public class CountryRepository : ICountryRepository
{
    private readonly ConcurrentDictionary<string, Country> _blocked = new();
    private readonly ConcurrentDictionary<string, DateTime> _tempBlocked = new();

    public bool AddBlockedCountry(string code, string name)
    {
        return _blocked.TryAdd(code, new Country
        {
            Code = code,
            Name = name
        });
    }

    public bool RemoveBlockedCountry(string code)
    {
        return _blocked.TryRemove(code, out _);
    }

    public bool IsCountryBlocked(string code)
    {
        return _blocked.ContainsKey(code) || IsTempBlocked(code);
    }

    public IEnumerable<Country> GetBlockedCountries()
    {
        return _blocked.Values;
    }

    public bool AddTempBlock(string code, DateTime expiry)
    {
        return _tempBlocked.TryAdd(code, expiry);
    }

    public bool IsTempBlocked(string code)
    {
        if (_tempBlocked.TryGetValue(code, out var expiry))
        {
            return expiry > DateTime.UtcNow;
        }
        return false;
    }

    public void RemoveExpiredTempBlocks()
    {
        foreach (var item in _tempBlocked)
        {
            if (item.Value <= DateTime.UtcNow)
            {
                _tempBlocked.TryRemove(item.Key, out _);
            }
        }
    }
}