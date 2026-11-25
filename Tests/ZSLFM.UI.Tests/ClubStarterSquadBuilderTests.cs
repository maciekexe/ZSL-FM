using System.Collections.Generic;
using System.Linq;
using Xunit;
using ZSLFM.UI;

namespace ZSLFM.UI.Tests
{
    public class ClubStarterSquadBuilderTests
    {
        [Fact]
        public void TryCreateFounderDraft_RejectsOverMaxOvrForDifficultyOrOverLimit()
        {
            Assert.False(ClubStarterSquadBuilder.TryCreateFounderDraft(
                "Jan", "Kowalski", "STR", overallSkill: 70, maxAllowedOvr: 58, currentFounderCount: 0,
                out _, out string ovrError));
            Assert.Contains("58", ovrError);

            Assert.False(ClubStarterSquadBuilder.TryCreateFounderDraft(
                "Jan", "Kowalski", "STR", overallSkill: 55, maxAllowedOvr: 58, currentFounderCount: 4,
                out _, out string countError));
            Assert.Contains("4", countError);
        }

        [Fact]
        public void TryCreateFounderDraft_RejectsInvalidPositionOrShortName()
        {
            Assert.False(ClubStarterSquadBuilder.TryCreateFounderDraft("J", "Nowak", "MID", 55, 65, 0, out _, out _));
            Assert.False(ClubStarterSquadBuilder.TryCreateFounderDraft("Jan", "Nowak", "CAM", 55, 65, 0, out _, out _));
        }

        [Fact]
        public void BuildStartingSquad_HardcoreZero_Creates12AmateursPlusFreeFounders()
        {
            StarterBudgetTier hardcore = ClubCreatorCatalog.BudgetTiers[0];
            Assert.True(ClubStarterSquadBuilder.TryCreateFounderDraft("Maciej", "Kapitan", "MID", 58, hardcore.MaxFounderOvr, 0, out var founder, out _));

            List<PlayerProfileDto> squad = ClubStarterSquadBuilder.BuildStartingSquadForDifficulty(
                clubName: "FC Osiedle",
                difficulty: hardcore,
                keepReplacedClubSquad: false,
                replacedClubName: "",
                founders: new[] { founder });

            Assert.Equal(13, squad.Count);
            Assert.Contains(squad, p => p.FullName == "Maciej Kapitan" && p.OverallSkill == 58);

            var amateurs = squad.Where(p => p.FullName != "Maciej Kapitan").ToList();
            Assert.Equal(12, amateurs.Count);
            Assert.All(amateurs, p => Assert.InRange(p.OverallSkill, 42, 52));
            Assert.All(amateurs, p => Assert.InRange(p.Age, 18, 26));
        }

        [Fact]
        public void BuildStartingSquad_SchoolClub_Creates12YoungStudentsPlusFreeFounders()
        {
            StarterBudgetTier school = ClubCreatorCatalog.BudgetTiers[1];
            Assert.True(ClubStarterSquadBuilder.TryCreateFounderDraft("Karol", "Uczeń", "STR", 64, school.MaxFounderOvr, 0, out var founder, out _));

            List<PlayerProfileDto> squad = ClubStarterSquadBuilder.BuildStartingSquadForDifficulty(
                clubName: "UKS Szkolny",
                difficulty: school,
                keepReplacedClubSquad: false,
                replacedClubName: "",
                founders: new[] { founder });

            Assert.Equal(13, squad.Count);
            Assert.Contains(squad, p => p.FullName == "Karol Uczeń" && p.OverallSkill == 64);

            var students = squad.Where(p => p.FullName != "Karol Uczeń").ToList();
            Assert.Equal(12, students.Count);
            Assert.All(students, p => Assert.InRange(p.Age, 15, 17));
            Assert.All(students, p => Assert.InRange(p.OverallSkill, 48, 58));
        }

        [Fact]
        public void BuildStartingSquad_InvestorProject_CreatesBalanced18PlayerSquad()
        {
            StarterBudgetTier investor = ClubCreatorCatalog.BudgetTiers[3];

            List<PlayerProfileDto> squad = ClubStarterSquadBuilder.BuildStartingSquadForDifficulty(
                clubName: "FC Inwestor",
                difficulty: investor,
                keepReplacedClubSquad: false,
                replacedClubName: "",
                founders: null);

            Assert.Equal(18, squad.Count);
            Assert.Equal(2, squad.Count(p => p.Position == "GK"));
            Assert.Equal(6, squad.Count(p => p.Position == "DEF"));
            Assert.Equal(6, squad.Count(p => p.Position == "MID"));
            Assert.Equal(4, squad.Count(p => p.Position == "STR"));
            Assert.All(squad, p => Assert.InRange(p.OverallSkill, 64, 76));
        }

        [Theory]
        [InlineData(0)] // HardcoreZero
        [InlineData(1)] // SchoolClub
        public void TryBuildCustomClub_KeepReplacedOnHardcoreOrSchool_FailsValidation(int tierIndex)
        {
            StarterBudgetTier tier = ClubCreatorCatalog.BudgetTiers[tierIndex];

            bool ok = ClubCreatorCatalog.TryBuildCustomClub(
                rawClubName: "KS Hallera",
                rawMonogram: "KSH",
                palette: ClubCreatorCatalog.Palettes[0],
                crestShape: CrestShape.Shield,
                crestPattern: CrestPattern.VerticalSplit,
                jerseyPattern: JerseyPattern.Solid,
                budgetTier: tier,
                city: "Gdańsk",
                nickname: "",
                stadiumName: "",
                replacedClub: ClubCreatorCatalog.ExistingClubs[0],
                rivalClub: null,
                customLogoFilePath: "",
                keepReplacedClubSquad: true,
                founders: null,
                selection: out _,
                validationError: out string error);

            Assert.False(ok);
            Assert.Contains("Średnim i Łatwym", error);
        }

        [Fact]
        public void TryBuildCustomClub_KeepReplacedOnMediumWithoutReplacedClub_FailsValidation()
        {
            StarterBudgetTier medium = ClubCreatorCatalog.BudgetTiers[2];

            bool ok = ClubCreatorCatalog.TryBuildCustomClub(
                rawClubName: "KS Hallera",
                rawMonogram: "KSH",
                palette: ClubCreatorCatalog.Palettes[0],
                crestShape: CrestShape.Shield,
                crestPattern: CrestPattern.VerticalSplit,
                jerseyPattern: JerseyPattern.Solid,
                budgetTier: medium,
                city: "Gdańsk",
                nickname: "",
                stadiumName: "",
                replacedClub: null,
                rivalClub: null,
                customLogoFilePath: "",
                keepReplacedClubSquad: true,
                founders: null,
                selection: out _,
                validationError: out string error);

            Assert.False(ok);
            Assert.Contains("wybierz najpierw klub", error);
        }

        [Fact]
        public void TryBuildCustomClub_HardcoreWithFreeFounder_KeepsZeroBudgetAndAddsFounderForFree()
        {
            StarterBudgetTier hardcore = ClubCreatorCatalog.BudgetTiers[0];
            Assert.True(ClubStarterSquadBuilder.TryCreateFounderDraft("Hubert", "Obrońca", "DEF", 58, hardcore.MaxFounderOvr, 0, out var founder, out _));

            bool ok = ClubCreatorCatalog.TryBuildCustomClub(
                rawClubName: "KS Hallera",
                rawMonogram: "KSH",
                palette: ClubCreatorCatalog.Palettes[0],
                crestShape: CrestShape.Shield,
                crestPattern: CrestPattern.VerticalSplit,
                jerseyPattern: JerseyPattern.DiagonalSash,
                budgetTier: hardcore,
                city: "Gdańsk",
                nickname: "Łącznościowcy",
                stadiumName: "Stadion Hallera",
                replacedClub: null,
                rivalClub: ClubCreatorCatalog.ExistingClubs[1],
                customLogoFilePath: "",
                keepReplacedClubSquad: false,
                founders: new[] { founder },
                selection: out ClubSetupSelection selection,
                validationError: out string error);

            Assert.True(ok);
            Assert.Equal(string.Empty, error);
            Assert.Equal(0.0, selection.Budget);
            Assert.Equal(0.0, selection.RemainingBudget);
            Assert.Equal(DifficultyTierType.HardcoreZero, selection.DifficultyType);
            Assert.Equal(13, selection.StartingSquad.Count);
        }
    }
}
