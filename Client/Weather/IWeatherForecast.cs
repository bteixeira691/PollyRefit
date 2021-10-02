using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Weather
{
    public  interface IWeatherForecast
    {
        [Get("")]
        Task<List<ModelWeatherForecastClient>> GetWeatherForecast();
    }
}