using Xunit;
using ZSLFM.UI;

namespace ZSLFM.UI.Tests
{
    public class ClubCreatorCatalogTests
    {
        [Fact]
        public void TryBuildCustomClub_EmptyName_FailsWithPolishValidationMessage()
        {
            bool ok = ClubCreatorCatalog.TryBuildCustomClub(
                rawClubName: "   ",
                rawMonogram: "FCL",
                palette: ClubCreatorCatalog.Palettes[0],
                crestShape: CrestShape.Shield,
                crestPattern: CrestPattern.VerticalSplit,
                budgetTier: ClubCreatorCatalog.BudgetTiers[0],
                selection: out ClubSetupSelection selection,
                validationError: out string error);

            Assert.False(ok);
            Assert.Null(selection);
            Assert.False(string.IsNullOrWhiteSpace(error));
        }

        [Fact]
        public void TryBuildCustomClub_SingleCharName_FailsValidation()
        {
            bool ok = ClubCreatorCatalog.TryBuildCustomClub(
                rawClubName: "A",
                rawMonogram: "",
                palette: ClubCreatorCatalog.Palettes[1],
                crestShape: CrestShape.Circle,
                crestPattern: CrestPattern.Solid,
                budgetTier: ClubCreatorCatalog.BudgetTiers[1],
                selection: out ClubSetupSelection selection,
                validationError: out string error);

            Assert.False(ok);
            Assert.Null(selection);
            Assert.Contains("2 znaki", error);
        }

        [Fact]
        public void TryBuildCustomClub_ValidInputs_NormalizesNameGeneratesMonogramAndAppliesTier()
        {
            StarterBudgetTier investorTier = ClubCreatorCatalog.BudgetTiers[3];
            ClubColorPalette bluePalette = ClubCreatorCatalog.Palettes[1];

            bool ok = ClubCreatorCatalog.TryBuildCustomClub(
                rawClubName: "  Akademia Futbolu Gdańsk  ",
                rawMonogram: "",
                palette: bluePalette,
                crestShape: CrestShape.RoundedSquare,
                crestPattern: CrestPattern.DiagonalStripe,
                budgetTier: investorTier,
                selection: out ClubSetupSelection selection,
                validationError: out string error);

            Assert.True(ok);
            Assert.Equal(string.Empty, error);
            Assert.NotNull(selection);
            Assert.Equal("Akademia Futbolu Gdańsk", selection.ClubName);
            Assert.Equal("AFG", selection.Monogram);
            Assert.Equal(2000000.0, selection.Budget);
            Assert.Equal(65, selection.Reputation);
            Assert.True(selection.IsCustomClub);
            Assert.False(selection.UsesDefaultZslLogo);
            Assert.Same(bluePalette, selection.Palette);
        }

        [Fact]
        public void TryBuildCustomClub_ExplicitMonogram_StripsNonAlphanumericAndTruncatesToThree()
        {
            bool ok = ClubCreatorCatalog.TryBuildCustomClub(
                rawClubName: "Łączność United",
                rawMonogram: "fc-ł12",
                palette: ClubCreatorCatalog.Palettes[0],
                crestShape: CrestShape.Sharp,
                crestPattern: CrestPattern.HorizontalBand,
                budgetTier: ClubCreatorCatalog.BudgetTiers[0],
                selection: out ClubSetupSelection selection,
                validationError: out _);

            Assert.True(ok);
            Assert.Equal("FCŁ", selection.Monogram);
        }

        [Fact]
        public void CreateFromExistingPreset_ZslGdansk_PreservesDefaultLogoAndBudget()
        {
            ExistingClubPreset zsl = ClubCreatorCatalog.ExistingClubs[0];

            ClubSetupSelection selection = ClubCreatorCatalog.CreateFromExistingPreset(zsl);

            Assert.Equal("ZSŁ Gdańsk", selection.ClubName);
            Assert.Equal(150000.0, selection.Budget);
            Assert.Equal(50, selection.Reputation);
            Assert.False(selection.IsCustomClub);
            Assert.True(selection.UsesDefaultZslLogo);
        }

        [Fact]
        public void ClubSessionState_SetAndClear_TracksActiveCareer()
        {
            ClubSessionState.Clear();
            Assert.False(ClubSessionState.HasActiveCareer);
            Assert.Null(ClubSessionState.ActiveSelection);

            ClubSetupSelection selection = ClubCreatorCatalog.CreateFromExistingPreset(ClubCreatorCatalog.ExistingClubs[1]);
            ClubSessionState.SetActiveSelection(selection);

            Assert.True(ClubSessionState.HasActiveCareer);
            Assert.Same(selection, ClubSessionState.ActiveSelection);

            ClubSessionState.Clear();
            Assert.False(ClubSessionState.HasActiveCareer);
        }
    }
}
