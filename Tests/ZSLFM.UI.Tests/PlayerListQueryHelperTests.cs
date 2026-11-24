using System.Collections.Generic;
using Xunit;
using ZSLFM.UI;

namespace ZSLFM.UI.Tests
{
    public class PlayerListQueryHelperTests
    {
        private static PlayerProfileDto MakePlayer(
            string first,
            string last,
            string pos = "MID",
            int overall = 75,
            string club = "ZSŁ Gdańsk",
            double price = 100000.0)
        {
            return new PlayerProfileDto(
                first, last, pos, overall, club, price, 20, 1000.0,
                70, 70, 70, 70, 70, 70);
        }

        [Fact]
        public void FilterSquad_EmptyOrWhitespaceQuery_ReturnsAllNonNullPlayers()
        {
            var input = new List<PlayerProfileDto>
            {
                MakePlayer("Maciej", "Kamiński"),
                null,
                MakePlayer("Nataniel", "Żbikowski", "STR")
            };

            List<PlayerProfileDto> result = PlayerListQueryHelper.FilterSquad(input, "   ");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void FilterSquad_MatchesNameOrPositionCaseInsensitively()
        {
            var input = new List<PlayerProfileDto>
            {
                MakePlayer("Maciej", "Kamiński", "MID"),
                MakePlayer("Nataniel", "Żbikowski", "STR"),
                MakePlayer("Hubert", "Miłuch", "DEF")
            };

            Assert.Single(PlayerListQueryHelper.FilterSquad(input, "żbik"));
            Assert.Single(PlayerListQueryHelper.FilterSquad(input, "def"));
            Assert.Equal("Hubert Miłuch", PlayerListQueryHelper.FilterSquad(input, "def")[0].FullName);
        }

        [Fact]
        public void FilterMarket_CombinesPositionFilterAndClubOrNameQuery()
        {
            var input = new List<PlayerProfileDto>
            {
                MakePlayer("Camilo", "Mena", "MID", 75, "Lechia Gdańsk", 120000),
                MakePlayer("Maksym", "Khlan", "STR", 74, "Lechia Gdańsk", 95000),
                MakePlayer("Jude", "Bellingham", "MID", 96, "Real Madryt", 5500000)
            };

            List<PlayerProfileDto> lechiaMids = PlayerListQueryHelper.FilterMarket(input, "MID", "lechia");

            Assert.Single(lechiaMids);
            Assert.Equal("Camilo Mena", lechiaMids[0].FullName);
        }

        [Fact]
        public void FilterMarket_AllPosition_ReturnsMatchesAcrossPositions()
        {
            var input = new List<PlayerProfileDto>
            {
                MakePlayer("Camilo", "Mena", "MID", 75, "Lechia Gdańsk", 120000),
                MakePlayer("Maksym", "Khlan", "STR", 74, "Lechia Gdańsk", 95000),
                MakePlayer("Jude", "Bellingham", "MID", 96, "Real Madryt", 5500000)
            };

            List<PlayerProfileDto> result = PlayerListQueryHelper.FilterMarket(input, "ALL", "gdańsk");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void SortInPlace_ByName_SortsByLastNameThenFirstName()
        {
            var list = new List<PlayerProfileDto>
            {
                MakePlayer("Piotr", "Nowak"),
                MakePlayer("Adam", "Nowak"),
                MakePlayer("Jan", "Kowalski")
            };

            PlayerListQueryHelper.SortInPlace(list, SortField.Name, ascending: true);

            Assert.Equal("Jan Kowalski", list[0].FullName);
            Assert.Equal("Adam Nowak", list[1].FullName);
            Assert.Equal("Piotr Nowak", list[2].FullName);
        }

        [Fact]
        public void SortInPlace_ByOverallDescending_AndPriceAscending_OrdersCorrectly()
        {
            var list = new List<PlayerProfileDto>
            {
                MakePlayer("A", "Low", overall: 65, price: 30000),
                MakePlayer("B", "High", overall: 90, price: 500000),
                MakePlayer("C", "Mid", overall: 75, price: 90000)
            };

            PlayerListQueryHelper.SortInPlace(list, SortField.Overall, ascending: false);
            Assert.Equal(new[] { 90, 75, 65 }, new[] { list[0].OverallSkill, list[1].OverallSkill, list[2].OverallSkill });

            PlayerListQueryHelper.SortInPlace(list, SortField.Price, ascending: true);
            Assert.Equal(new[] { 30000.0, 90000.0, 500000.0 }, new[] { list[0].Price, list[1].Price, list[2].Price });
        }

        [Fact]
        public void SortInPlace_SortFieldNone_PreservesOrder()
        {
            var list = new List<PlayerProfileDto>
            {
                MakePlayer("B", "Beta"),
                MakePlayer("A", "Alpha")
            };

            PlayerListQueryHelper.SortInPlace(list, SortField.None, ascending: true);

            Assert.Equal("B Beta", list[0].FullName);
            Assert.Equal("A Alpha", list[1].FullName);
        }

        [Theory]
        [InlineData(99, "skill-high")]
        [InlineData(80, "skill-high")]
        [InlineData(79, "skill-mid")]
        [InlineData(65, "skill-mid")]
        [InlineData(64, "skill-low")]
        [InlineData(0, "skill-low")]
        public void GetSkillTierCssClass_ReturnsExpectedTierAtBoundaries(int skill, string expectedCss)
        {
            Assert.Equal(expectedCss, PlayerListQueryHelper.GetSkillTierCssClass(skill));
        }
    }
}
