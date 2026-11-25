using System;

namespace ZSLFM.Core.Models
{
    /// <summary>Kontrakt zawodnika z klubem. Czysta klasa POCO.</summary>
    public class Contract
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int ClubId { get; set; }
        public decimal WeeklyWage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? ReleaseClause { get; set; }
        public bool IsActive { get; set; }

        /// <summary>Pozostała długość kontraktu w miesiącach względem daty w grze.</summary>
        public int GetRemainingMonths(DateTime currentDate)
        {
            if (currentDate >= EndDate) return 0;
            int months = (EndDate.Year - currentDate.Year) * 12 + EndDate.Month - currentDate.Month;
            if (EndDate.Day < currentDate.Day) months--;
            return months < 0 ? 0 : months;
        }

        public bool IsExpired(DateTime currentDate) => currentDate >= EndDate;
    }
}