namespace LacznoscFM.Core.Models
{
    /// <summary>
    /// Model pojedynczego meczu.
    /// Na tym etapie przechowuje wyłącznie podstawowe informacje.
    /// </summary>
    public class Match
    {
        public int Id { get; set; }
        public int HomeClubId { get; set; }
        public int AwayClubId { get; set; }

        /// <summary>Wynik gospodarzy. Null oznacza brak ustalonego wyniku.</summary>
        public int? HomeScore { get; set; }

        /// <summary>Wynik gości. Null oznacza brak ustalonego wyniku.</summary>
        public int? AwayScore { get; set; }

        /// <summary>Czy mecz został już rozegrany.</summary>
        public bool IsPlayed { get; set; }

        // TODO: Dodać datę, rozgrywki, status i dane zdarzeń meczowych.
    }
}
