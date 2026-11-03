namespace LacznoscFM.Core.Models
{
    /// <summary>
    /// Model kontraktu zawodnika.
    /// Szczegółowe zasady kontraktów będą częścią mechaniki transferów.
    /// </summary>
    public class Contract
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int ClubId { get; set; }

        /// <summary>Wynagrodzenie zawodnika</summary>
        public decimal Salary { get; set; }

        /// <summary>Długość kontraktu</summary>
        public int Duration { get; set; }

        // Ustalić sposób przechowywania daty rozpoczęcia i zakończenia.
    }
}
