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
            LameScooterStationList[] stationList = JsonSerializer.Deserialize<LameScooterStationList[]>(await File.ReadAllTextAsync("scooters.json"));

            if (stationList != null)
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