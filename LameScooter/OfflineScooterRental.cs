using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LameScooter
{
    public class OfflineScooterRental : ILameScooterRental
    {
        async Task<int> IRental.GetScooterCountInStation(string stationName)
        {
            var result = 0;
            LameScooterStationList[] stationList = JsonSerializer.Deserialize<LameScooterStationList[]>(File.ReadAllText("scooters.json"));
            
            foreach (var entry in stationList)
            {
                if (entry.name == stationName)
                {
                    return entry.bikesAvailable;
                }
            }
            return 0;
        }
    }
}