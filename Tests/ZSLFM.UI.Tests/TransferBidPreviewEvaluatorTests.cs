using System;
using Xunit;
using ZSLFM.UI;

namespace ZSLFM.UI.Tests
{
    public class TransferBidPreviewEvaluatorTests
    {
        private static PlayerProfileDto MakeMarketPlayer(double price)
        {
            return new PlayerProfileDto(
                "Camilo", "Mena", "MID", 75, "Lechia Gdańsk", price, 22, 1800.0,
                84, 72, 74, 80, 52, 70);
        }

        [Fact]
        public void Evaluate_PriceBelowBudget_IsAffordableWithRemainingDifference()
        {
            TransferBidPreviewResult result = TransferBidPreviewEvaluator.Evaluate(MakeMarketPlayer(120000.0), 150000.0);

            Assert.True(result.IsAffordable);
            Assert.Equal(30000.0, result.Difference);
            Assert.Equal("Camilo Mena", result.PlayerFullName);
        }

        [Fact]
        public void Evaluate_PriceEqualToBudget_IsAffordableWithZeroDifference()
        {
            TransferBidPreviewResult result = TransferBidPreviewEvaluator.Evaluate(MakeMarketPlayer(150000.0), 150000.0);

            Assert.True(result.IsAffordable);
            Assert.Equal(0.0, result.Difference);
        }

        [Fact]
        public void Evaluate_PriceAboveBudget_IsNotAffordableWithShortageDifference()
        {
            TransferBidPreviewResult result = TransferBidPreviewEvaluator.Evaluate(MakeMarketPlayer(950000.0), 150000.0);

            Assert.False(result.IsAffordable);
            Assert.Equal(800000.0, result.Difference);
        }

        [Fact]
        public void Evaluate_NullPlayer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => TransferBidPreviewEvaluator.Evaluate(null, 150000.0));
        }

        [Fact]
        public void SampleDataProvider_ReturnsConsistentSquadAndMarketLists()
        {
            var squad = UiSampleSquadDataProvider.CreateSampleSquadPlayers();
            var market = UiSampleSquadDataProvider.CreateSampleMarketPlayers();

            Assert.Equal(18, squad.Count);
            Assert.Equal(21, market.Count);
            Assert.All(squad, p => Assert.False(string.IsNullOrWhiteSpace(p.FullName)));
            Assert.All(market, p => Assert.True(p.Price > 0));
        }
    }
}
