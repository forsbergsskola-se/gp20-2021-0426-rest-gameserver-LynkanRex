using System;
using System.Linq;
using System.Threading.Tasks;

namespace LameScooter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args[0].Any(char.IsDigit))
                throw new ArgumentException("Error: Station name cannot contain numbers!");    
            
            ILameScooterRental rental = new OfflineScooterRental();

            var count = await rental.GetScooterCountInStation(args[0]);
            Console.WriteLine("Number of scooters available at this station: " + count);
        }
    }
}
