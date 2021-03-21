using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Manager.App.Data
{
    public class WeatherForecastService
    {

        private readonly TokenProvider _store;
        public HttpClient Client { get; }

        public WeatherForecastService(IHttpClientFactory clientFactory,
            TokenProvider tokenProvider)
        {
            Client = clientFactory.CreateClient();
            _store = tokenProvider;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var token = _store.AccessToken;



            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://localhost:6001/api/clusters/PS-PreProd-01/applications/FXP-Service/keys/URP:Logs");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await Client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<WeatherForecast[]>(await response.Content.ReadAsStringAsync());
            //var rng = new Random();
            //return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = startDate.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //}).ToArray());
        }
    }
}
