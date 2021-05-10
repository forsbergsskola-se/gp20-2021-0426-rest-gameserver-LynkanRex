using System;
using System.Threading.Tasks;

namespace MMORESTPG
{
    public class FileRepository : IRepository
    {
        public string fileName = "game-dev.txt";
        
        public Task<Player> Get(Guid id) => throw new NotImplementedException();

        public Task<Player[]> GetAll() => throw new NotImplementedException();

        public Task<Player> Create(Player player) => throw new NotImplementedException();

        public Task<Player> Modify(Guid id, ModifiedPlayer player) => throw new NotImplementedException();

        public Task<Player> Delete(Guid id) => throw new NotImplementedException();
    }
}