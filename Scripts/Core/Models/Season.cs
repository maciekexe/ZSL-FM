namespace LacznoscFM.Core.Models
{
    /// <summary>
    /// Model sezonu rozgrywkowego.
    /// Terminarz i upływ czasu będą obsługiwane przez moduł Calendar.
    /// </summary>
    public class Season
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Ustalenie oraz dodanie listy rozgrywek oraz ich formaty
    }
}
