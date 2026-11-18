namespace LacznoscFM.Core.Models
{
    /// <summary>
    /// Model zawodnika. Wspólny model wykorzystywany m.in. przez skład,
    /// taktykę, symulację meczu i transfery.
    /// Szczegółowe atrybuty zostaną ustalone przed implementacją mechanik.
    /// </summary>
    public class Player
    {
        /// <summary>Unikalny identyfikator zawodnika.</summary>
        public int Id { get; set; }

        /// <summary>Imię i nazwisko zawodnika.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Klub, do którego zawodnik jest przypisany.</summary>
        public int? ClubId { get; set; }

        /// <summary>Wartość rynkowa – szczegóły wyceny do ustalenia.</summary>
        public decimal MarketValue { get; set; }

        /// <summary>Pensja zawodnika – jednostka okresu do ustalenia.</summary>
        public decimal Salary { get; set; }

        // Dodanie innych parametrów (do uzgodnienia w trakcie projektu) np. wiek, pozycję, atrybuty,
        // potencjał, formę, zmęczenie, stan zdrowia itp.
    }
}
