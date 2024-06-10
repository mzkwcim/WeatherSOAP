using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace WeatherSOAP
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private const string apiKey = "xxx"; 

        static async Task Main(string[] args)
        {
            string location = "Poznan,POL";
            string startDate = "1999-07-26";
            string endDate = "1999-07-31";

            string url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{location}/{startDate}/{endDate}?key={apiKey}";

            try
            {
                string response = await GetWeatherData(url);
                ParseAndDisplayWeatherData(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }

            Console.WriteLine("Naciśnij dowolny klawisz, aby zakończyć...");
            Console.ReadKey();
        }

        static async Task<string> GetWeatherData(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        static void ParseAndDisplayWeatherData(string jsonData)
        {
            JObject weatherData = JObject.Parse(jsonData);

            Console.WriteLine($"Lokalizacja: {weatherData["resolvedAddress"]}");
            Console.WriteLine($"Strefa czasowa: {weatherData["timezone"]}");

            foreach (var day in weatherData["days"])
            {
                Console.WriteLine($"\nData: {day["datetime"]}");
                Console.WriteLine($"Temperatura: {TemperatureConverter((double)day["temp"])} °F");
                Console.WriteLine($"Opis: {day["description"]}");
                Console.WriteLine($"Wilgotność: {day["humidity"]}%");
                Console.WriteLine($"Wiatr: {day["windspeed"]} km/h");
            }
        }

        private static double TemperatureConverter(double temperature)
        {
            return (temperature - 32) * (5 / 9);
        }
    }
}
