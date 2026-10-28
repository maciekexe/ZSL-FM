namespace ZSLFM.Core.Models
{
    /// <summary>Klub piłkarski. Czysta klasa POCO.</summary>
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Country { get; set; }
        public int LeagueId { get; set; }
        public int Reputation { get; set; }          // 0-100
        public int StadiumCapacity { get; set; }

        // Finanse (szczegółową logikę dodasz w zadaniu K-8)
        public decimal Budget { get; set; }
        public decimal TransferBudget { get; set; }
        public decimal WageBudget { get; set; }      // limit tygodniowy
    }
}