using System;
using System.Collections.Generic;

namespace ZSLFM.UI
{
    /// <summary>
    /// Niezależny od silnika Unity zapis koloru RGB (0-255) na potrzeby warstwy prezentacji i testów.
    /// </summary>
    public readonly struct RgbColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public RgbColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    public enum CrestShape
    {
        Shield,
        RoundedSquare,
        Circle,
        Sharp
    }

    public enum CrestPattern
    {
        Solid,
        VerticalSplit,
        HorizontalBand,
        DiagonalStripe
    }

    public enum JerseyPattern
    {
        Solid,
        VerticalStripes,
        HorizontalHoop,
        DiagonalSash,
        ContrastSleeves
    }

    public enum DifficultyTierType
    {
        HardcoreZero,
        SchoolClub,
        LeagueNewcomer,
        InvestorProject
    }

    public enum SquadBuildMode
    {
        GeneratedByDifficulty,
        KeepReplacedClub
    }

    /// <summary>
    /// Szkic darmowego Założyciela (gracz i znajomi) dodanego w drugim kroku kreatora klubu.
    /// </summary>
    public sealed class CustomPlayerDraft
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Position { get; }
        public int OverallSkill { get; }

        public CustomPlayerDraft(string firstName, string lastName, string position, int overallSkill)
        {
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            OverallSkill = overallSkill;
        }

        public string FullName => $"{FirstName} {LastName}";
    }

    /// <summary>
    /// Zdefiniowana paleta barw klubowych do wyboru w kreatorze lub dla istniejących klubów.
    /// </summary>
    public sealed class ClubColorPalette
    {
        public string Id { get; }
        public string DisplayName { get; }
        public RgbColor PrimaryGradient { get; }
        public RgbColor SecondaryGradient { get; }
        public RgbColor BaseDark { get; }
        public RgbColor Accent { get; }
        public RgbColor AccentLightText { get; }

        public ClubColorPalette(
            string id,
            string displayName,
            RgbColor primaryGradient,
            RgbColor secondaryGradient,
            RgbColor baseDark,
            RgbColor accent,
            RgbColor accentLightText)
        {
            Id = id;
            DisplayName = displayName;
            PrimaryGradient = primaryGradient;
            SecondaryGradient = secondaryGradient;
            BaseDark = baseDark;
            Accent = accent;
            AccentLightText = accentLightText;
        }
    }

    /// <summary>
    /// Spójny scenariusz startowy (poziom trudności): określa budżet, reputację, kadrę startową i limit OVR założycieli.
    /// </summary>
    public sealed class StarterBudgetTier
    {
        public string Id { get; }
        public DifficultyTierType DifficultyType { get; }
        public string DifficultyLabel { get; }
        public string Title { get; }
        public double Budget { get; }
        public int Reputation { get; }
        public int MaxFounderOvr { get; }
        public bool AllowsReplacedClubSquad { get; }
        public string Description { get; }

        public StarterBudgetTier(
            string id,
            DifficultyTierType difficultyType,
            string difficultyLabel,
            string title,
            double budget,
            int reputation,
            int maxFounderOvr,
            bool allowsReplacedClubSquad,
            string description)
        {
            Id = id;
            DifficultyType = difficultyType;
            DifficultyLabel = difficultyLabel;
            Title = title;
            Budget = budget;
            Reputation = reputation;
            MaxFounderOvr = maxFounderOvr;
            AllowsReplacedClubSquad = allowsReplacedClubSquad;
            Description = description;
        }
    }

    /// <summary>
    /// Gotowy istniejący klub możliwy do przejęcia lub opcjonalnego zastąpienia w lidze.
    /// </summary>
    public sealed class ExistingClubPreset
    {
        public string Id { get; }
        public string ClubName { get; }
        public string Monogram { get; }
        public double Budget { get; }
        public int Reputation { get; }
        public string Description { get; }
        public ClubColorPalette Palette { get; }
        public CrestShape CrestShape { get; }
        public CrestPattern CrestPattern { get; }
        public bool UsesDefaultZslLogo { get; }

        public ExistingClubPreset(
            string id,
            string clubName,
            string monogram,
            double budget,
            int reputation,
            string description,
            ClubColorPalette palette,
            CrestShape crestShape,
            CrestPattern crestPattern,
            bool usesDefaultZslLogo = false)
        {
            Id = id;
            ClubName = clubName;
            Monogram = monogram;
            Budget = budget;
            Reputation = reputation;
            Description = description;
            Palette = palette;
            CrestShape = crestShape;
            CrestPattern = crestPattern;
            UsesDefaultZslLogo = usesDefaultZslLogo;
        }
    }

    /// <summary>
    /// Wybrany lub stworzony w kreatorze klub przekazywany między Menu Głównym a Dashboardem.
    /// </summary>
    public sealed class ClubSetupSelection
    {
        public string ClubName { get; }
        public string Monogram { get; }
        public double Budget { get; }
        public double RemainingBudget { get; }
        public int Reputation { get; }
        public ClubColorPalette Palette { get; }
        public CrestShape CrestShape { get; }
        public CrestPattern CrestPattern { get; }
        public JerseyPattern JerseyPattern { get; }
        public bool IsCustomClub { get; }
        public bool UsesDefaultZslLogo { get; }
        public string CustomLogoFilePath { get; }

        public string City { get; }
        public string Nickname { get; }
        public string StadiumName { get; }
        public string ReplacedClubId { get; }
        public string ReplacedClubName { get; }
        public string RivalClubName { get; }
        public DifficultyTierType DifficultyType { get; }
        public SquadBuildMode SquadMode { get; }
        public IReadOnlyList<PlayerProfileDto> StartingSquad { get; }

        public ClubSetupSelection(
            string clubName,
            string monogram,
            double budget,
            int reputation,
            ClubColorPalette palette,
            CrestShape crestShape,
            CrestPattern crestPattern,
            bool isCustomClub,
            bool usesDefaultZslLogo,
            JerseyPattern jerseyPattern = JerseyPattern.Solid,
            string customLogoFilePath = "",
            string city = "",
            string nickname = "",
            string stadiumName = "",
            string replacedClubId = "",
            string replacedClubName = "",
            string rivalClubName = "",
            DifficultyTierType difficultyType = DifficultyTierType.LeagueNewcomer,
            SquadBuildMode squadMode = SquadBuildMode.KeepReplacedClub,
            IReadOnlyList<PlayerProfileDto> startingSquad = null,
            double? remainingBudget = null)
        {
            ClubName = clubName;
            Monogram = monogram;
            Budget = budget;
            RemainingBudget = remainingBudget ?? budget;
            Reputation = reputation;
            Palette = palette ?? throw new ArgumentNullException(nameof(palette));
            CrestShape = crestShape;
            CrestPattern = crestPattern;
            JerseyPattern = jerseyPattern;
            IsCustomClub = isCustomClub;
            UsesDefaultZslLogo = usesDefaultZslLogo;
            CustomLogoFilePath = customLogoFilePath ?? string.Empty;
            City = city ?? string.Empty;
            Nickname = nickname ?? string.Empty;
            StadiumName = stadiumName ?? string.Empty;
            ReplacedClubId = replacedClubId ?? string.Empty;
            ReplacedClubName = replacedClubName ?? string.Empty;
            RivalClubName = rivalClubName ?? string.Empty;
            DifficultyType = difficultyType;
            SquadMode = squadMode;
            StartingSquad = startingSquad;
        }
    }
}
