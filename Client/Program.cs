using Client.Weather;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath($"C:\\Users\\dakar\\source\\repos\\ProjectPollyRefit\\Client")
                .AddJsonFile($"appsettings.json");

            var config = builder.Build();

            var apiWeatherClient = new WeatherForecastClient(config);

           await apiWeatherClient.GetWeather2();
        }
    }
}
