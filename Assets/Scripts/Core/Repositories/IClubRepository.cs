using ZSLFM.Core.Models;
using System.Collections.Generic;
using ZSLFM.Core.Repositories;

namespace ZSLFM.Core.Repositories
{
    public interface IClubRepository : IRepository<Club>
    {
        IReadOnlyList<Club> GetByLeague(int leagueId);
        /// <summary>Zwraca klub o danej nazwie lub null.</summary>
        Club GetByName(string name);
    }
}