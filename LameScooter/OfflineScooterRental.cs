using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LameScooter
{
    public class OfflineScooterRental : ILameScooterRental
    {
        public async Task<int> GetScooterCountInStation(string stationName)
        {
            
            LameScooterStationList[] stationList = JsonSerializer.Deserialize<LameScooterStationList[]>(await File.ReadAllTextAsync("scooters.json"));
            
            if (stationList != null)
            {
                foreach (var entry in stationList)
                {
                    if (entry.name == stationName)
                    {
                        return entry.bikesAvailable;
                    } 
                }
            }
            else
            {
                throw new SystemException($"Station named '{stationName}' not found, did you spell it right?\n");
            }
            return 0;
        }
    }

    // public class StationDataDeserializer
    // {
    //     public async Task<LameScooterStationList[]> DeserializeStationList(stationList)
    //     {
    //          
    //
    //         return JsonSerializer.Deserialize<LameScooterStationList[]>(await File.ReadAllTextAsync("scooters.json"));
    //     }
    // }
}