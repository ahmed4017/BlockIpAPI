using BlockIpAPI.DTOs;
using BlockIpAPI.Services.Interfaces;
using Newtonsoft.Json;

namespace BlockIpAPI.Services.Implementations
{
    public class IpApiResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("isp")]
        public string Isp { get; set; }
    }

    public class IpService : IIpService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public IpService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<CountryResponseDto> GetCountryByIpAsync(string ip)
        {
            var baseUrl = _config["IpApi:BaseUrl"]; // "http://ip-api.com/json/"
            var url = $"{baseUrl}{ip}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"URL: {url}");
                Console.WriteLine($"Response: {content}");

                var data = JsonConvert.DeserializeObject<IpApiResponse>(content);

                if (data == null || data.Status != "success")
                    return new CountryResponseDto { Code = "Unknown", Name = "Unknown" };

                return new CountryResponseDto
                {
                    Code = data.CountryCode,
                    Name = data.Country,
                    Isp = data.Isp
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new CountryResponseDto { Code = "Unknown", Name = "Unknown" };
            }
        }
    }
}