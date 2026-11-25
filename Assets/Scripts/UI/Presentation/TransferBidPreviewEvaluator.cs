using System;

namespace ZSLFM.UI
{
    /// <summary>
    /// Wynik wstępnej walidacji oferty transferowej na potrzeby podglądu UI (do czasu podpięcia Core z K-5).
    /// </summary>
    public readonly struct TransferBidPreviewResult
    {
        public bool IsAffordable { get; }
        public string PlayerFullName { get; }
        public string ClubName { get; }
        public double Price { get; }
        public double ClubBudget { get; }
        public double Difference { get; }

        public TransferBidPreviewResult(
            bool isAffordable,
            string playerFullName,
            string clubName,
            double price,
            double clubBudget,
            double difference)
        {
            IsAffordable = isAffordable;
            PlayerFullName = playerFullName;
            ClubName = clubName;
            Price = price;
            ClubBudget = clubBudget;
            Difference = difference;
        }
    }

    /// <summary>
    /// Tymczasowa ocena oferty po stronie UI przed podpięciem właściwej logiki transferów z warstwy Core.
    /// </summary>
    public static class TransferBidPreviewEvaluator
    {
        public static TransferBidPreviewResult Evaluate(PlayerProfileDto player, double clubBudget)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

            bool isAffordable = player.Price <= clubBudget;
            double difference = isAffordable
                ? clubBudget - player.Price
                : player.Price - clubBudget;

            return new TransferBidPreviewResult(
                isAffordable: isAffordable,
                playerFullName: player.FullName,
                clubName: player.ClubName,
                price: player.Price,
                clubBudget: clubBudget,
                difference: difference);
        }
    }
}
