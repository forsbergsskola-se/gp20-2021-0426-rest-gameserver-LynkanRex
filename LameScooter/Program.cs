using System;
using System.Linq;
using System.Threading.Tasks;

namespace LameScooter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var count = 0;
            
            if (args[0].Any(char.IsDigit))
                throw new ArgumentException("Error: Station name cannot contain numbers!");

            if (args[1].ToLower() == "offline")
            {
                ILameScooterRental rental = new OfflineScooterRental();
                count = await rental.GetScooterCountInStation(args[0]);
            }
            else if (args[1].ToLower() == "realtime")
            {
                ILameScooterRental rental = new RealTimeScooterRental();
                count = await rental.GetScooterCountInStation(args[0]);
            }
            else if (args[1].ToLower() == "deprecated")
            {
                ILameScooterRental rental = new DeprecatedLameScooterRental();
                count = await rental.GetScooterCountInStation(args[0]);
            }
            else
            {
                throw new ArgumentException("Error: Incorrect parameter entry\n" +
                                            "valid arguments are: offline, online or deprecated\n" +
                                            "Also remember to use quotation marks, \"\", around the station name!\n");
            }
            
            Console.WriteLine("Number of scooters available at this station: " + count);
        }
    }
}
