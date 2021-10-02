using Microsoft.Extensions.Configuration;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Weather
{
    public class WeatherForecastClient
    {
        private IConfiguration _configuration;
        private IWeatherForecast _weatherForecast;
        public WeatherForecastClient(IConfiguration configuration)
        {
            _configuration = configuration;
            string urlBase = _configuration.GetSection(
                "APIWeatherForecast:UrlBase").Value;

            _weatherForecast = RestService.For<IWeatherForecast>(urlBase);

        }

        public async Task GetWeather()
        {

            await Policy
               .Handle<HttpRequestException>(
                   ex => ex.InnerException.Message.Any())
               .RetryAsync(10, async (exception, retryCount) =>
               {
                   var lastColour = Console.ForegroundColor;
                   Console.ForegroundColor = ConsoleColor.Green;
                   await Console.Out.WriteLineAsync("Execução de RetryPolicy...");
                   Console.ForegroundColor = lastColour;

               }).ExecuteAsync(async () => Console.Out.WriteLineAsync("listWeather: " +
            JsonSerializer.Serialize(await _weatherForecast.GetWeatherForecast())));

        }

        public async Task GetWeather2()
        {
            var retry = Policy.Handle<HttpRequestException>(ex => ex.InnerException.Message.Any()).RetryAsync(5, async (exception, retryCount) =>
            {
               var lastColour = Console.ForegroundColor;
               Console.ForegroundColor = ConsoleColor.Green;
               await Console.Out.WriteLineAsync("Execução de RetryPolicy...");
               Console.ForegroundColor = lastColour;

            });

            var _circuitBreakerPolicy = GetCircuitBreaker();

            var policy = Policy.WrapAsync(retry, _circuitBreakerPolicy);


            try
            {
                Console.WriteLine($"Circuit State: {_circuitBreakerPolicy.CircuitState}");

                await policy.ExecuteAsync(async () => Console.Out.WriteLineAsync("listWeather: " +
                JsonSerializer.Serialize(await _weatherForecast.GetWeatherForecast())));

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
      


        public static AsyncCircuitBreakerPolicy GetCircuitBreaker()
        {
            var circuitBreakerPolicy = Policy.Handle<Exception>()
              .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1),
              (ex, t) =>
              {
                  Console.WriteLine("Circuit broken!");
              },
              () =>
              {
                  Console.WriteLine("Circuit Reset!");
              });

            return circuitBreakerPolicy;
        }

    }
}
