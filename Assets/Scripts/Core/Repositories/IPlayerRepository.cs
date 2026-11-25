using ZSLFM.Core.Models;
using System.Collections.Generic;
using ZSLFM.Core.Repositories;

namespace ZSLFM.Core.Repositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        IReadOnlyList<Player> GetByClub(int clubId);
        IReadOnlyList<Player> GetFreeAgents();
        IReadOnlyList<Player> GetByPosition(Position position);
        /// <summary>Wyszukiwanie po fragmencie imienia/nazwiska (ekran transferów).</summary>
        IReadOnlyList<Player> SearchByName(string text);
    }
}