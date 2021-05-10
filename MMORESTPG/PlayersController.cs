using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MMORESTPG
{
    public class PlayersController : IRepository
    {
        private IRepository repository;
        
        [HttpPost]
        [HttpPut]
        public async Task<Player> Get(Guid id)
        {
            return new Player();
        }

        public async Task<Player[]> GetAll()
        {
            return new Player[0];
        }

        public Task<Player> Create(Player player) => throw new NotImplementedException();

        public async Task<Player> Create(NewPlayer player)
        {
            return new Player();
        }

        public async Task<Player> Modify(Guid id, ModifiedPlayer player)
        {
            return new Player();
        }

        public async Task<Player> Delete(Guid id)
        {
            return new Player();
        }

        public PlayersController(IRepository repository)
        {
            this.repository = repository;
        }
    }
}