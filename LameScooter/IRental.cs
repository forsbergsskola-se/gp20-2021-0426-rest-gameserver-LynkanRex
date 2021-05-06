using System.Threading.Tasks;

namespace LameScooter
{
    public interface IRental
    {
        Task<int> GetScooterCountInStation(string stationName);
    }
}