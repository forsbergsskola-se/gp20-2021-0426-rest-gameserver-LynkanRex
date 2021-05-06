using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LameScooter
{
    public class DeprecatedLameScooterRental : ILameScooterRental
    {
        public async Task<int> GetScooterCountInStation(string stationName)
        {
            List<string> lines = new List<string>();
            var count = 0;

            using (StreamReader sr = new StreamReader("scooters.txt"))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (Regex.IsMatch(line, stationName, RegexOptions.IgnoreCase))
                    {
                        lines.Add(line);
                    }
                }
            }
            
            if(lines.Count == 0)
                throw new SystemException($"Station named '{stationName}' not found, did you spell it right?");
            
            foreach (var foundLine in lines)
            {
                string[] splitLines = foundLine.Split(" : ");

                count = Convert.ToInt32(splitLines[1]);
                return count;
            }
            return count;
        }
    }
}
