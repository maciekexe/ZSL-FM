namespace LacznoscFM.Core.Models
{
    /// <summary>
    /// Model klubu piłkarskiego.
    /// Nie zawiera jeszcze logiki finansowej ani transferowej.
    /// </summary>
    public class Club
    {
        /// <summary>Unikalny identyfikator klubu.</summary>
        public int Id { get; set; }

        /// <summary>Nazwa klubu.</summary>
        public string Name { get; set; } = string.Empty;

        //Dodać ligę, budżet i pozostałe dane po uzgodnieniu
        // schematu z osobą odpowiedzialną za bazę danych.
    }
}
