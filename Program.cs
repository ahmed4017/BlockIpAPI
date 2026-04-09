using BlockIpAPI.Repositories;
using BlockIpAPI.BackgroundServices;
using BlockIpAPI.Services.Implementations;
using BlockIpAPI.Services.Interfaces;

namespace BlockIpAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<ICountryRepository, CountryRepository>();
            builder.Services.AddSingleton<ILogService, LogService>();

            builder.Services.AddHttpClient<IIpService, IpService>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
            builder.Services.AddHostedService<TempBlockCleanupService>();
            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
