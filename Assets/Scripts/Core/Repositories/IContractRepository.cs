using ZSLFM.Core.Models;
using System;
using System.Collections.Generic;
using ZSLFM.Core.Repositories;

namespace ZSLFM.Core.Repositories
{
    public interface IContractRepository : IRepository<Contract>
    {
        /// <summary>Aktywny kontrakt zawodnika lub null (wolny agent).</summary>
        Contract GetActiveByPlayer(int playerId);
        IReadOnlyList<Contract> GetActiveByClub(int clubId);
        /// <summary>Aktywne kontrakty wygasające przed podaną datą.</summary>
        IReadOnlyList<Contract> GetExpiringBefore(DateTime date);
    }
}