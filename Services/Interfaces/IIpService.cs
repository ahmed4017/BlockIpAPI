using BlockIpAPI.DTOs;

namespace BlockIpAPI.Services.Interfaces
{
    public interface IIpService
    {
        Task<CountryResponseDto> GetCountryByIpAsync(string ip);
    }
}
