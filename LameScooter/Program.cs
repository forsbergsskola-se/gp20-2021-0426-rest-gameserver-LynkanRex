using System;
using System.Threading.Tasks;

namespace LameScooter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ILameScooterRental rental = new OfflineScooterRental();

            var count = await rental.GetScooterCountInStation(args[0]);
            Console.WriteLine("Number of scooters available at this station: " + count);
        }
    }
}
