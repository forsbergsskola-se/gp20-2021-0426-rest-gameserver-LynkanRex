using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LameScooter
{
    public class RealTimeScooterRental : ILameScooterRental
    {
        static readonly HttpClient HttpClient = new HttpClient();

        private string stationResponseBody;
        
        public async Task<int> GetScooterCountInStation(string stationName)
        {
            int result = 0;

            stationResponseBody = await GetListInfoFromNet();

            var stations = JsonConvert.DeserializeObject<Stations>(stationResponseBody);

            if (stations != null)
                foreach (var station in stations.stations)
                { 
                    if (station.name == stationName) 
                    {
                        return station.bikesAvailable;
                    }
                }
            return result;
        }

        private async Task<string> GetListInfoFromNet()
        {
            string bodyContent = "";
            
            HttpClient.BaseAddress = new Uri("https://raw.githubusercontent.com/marczaku/");
            
            HttpResponseMessage quickResponse =
                await HttpClient.GetAsync("GP20-2021-0426-Rest-Gameserver/main/assignments/scooters.json");

            quickResponse.EnsureSuccessStatusCode();

            bodyContent = await quickResponse.Content.ReadAsStringAsync();

            return bodyContent;
        }
    }
}