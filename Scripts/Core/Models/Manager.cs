namespace LacznoscFM.Core.Models
{
    /// <summary>
    /// Model menedżera/gracza prowadzącego klub.
    /// To model danych, a nie kontroler interfejsu Unity.
    /// </summary>
    public class Manager
    {
        public int Id { get; set; }

        /// <summary>Nazwa lub imię i nazwisko menedżera.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Klub prowadzony przez menedżera.</summary>
        public int? ClubId { get; set; }

        // Dodać dalsze informacje o menedżerze, jeśli będą potrzebne (może xd).
    }
}
