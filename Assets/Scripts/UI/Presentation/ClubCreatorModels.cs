using System;

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
    /// Pakiet budżetu startowego w kreatorze własnego klubu.
    /// </summary>
    public sealed class StarterBudgetTier
    {
        public string Id { get; }
        public string Title { get; }
        public double Budget { get; }
        public int Reputation { get; }
        public string Description { get; }

        public StarterBudgetTier(string id, string title, double budget, int reputation, string description)
        {
            Id = id;
            Title = title;
            Budget = budget;
            Reputation = reputation;
            Description = description;
        }
    }

    /// <summary>
    /// Gotowy istniejący klub możliwy do przejęcia w Menu Głównym.
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
        public int Reputation { get; }
        public ClubColorPalette Palette { get; }
        public CrestShape CrestShape { get; }
        public CrestPattern CrestPattern { get; }
        public bool IsCustomClub { get; }
        public bool UsesDefaultZslLogo { get; }

        public ClubSetupSelection(
            string clubName,
            string monogram,
            double budget,
            int reputation,
            ClubColorPalette palette,
            CrestShape crestShape,
            CrestPattern crestPattern,
            bool isCustomClub,
            bool usesDefaultZslLogo)
        {
            ClubName = clubName;
            Monogram = monogram;
            Budget = budget;
            Reputation = reputation;
            Palette = palette ?? throw new ArgumentNullException(nameof(palette));
            CrestShape = crestShape;
            CrestPattern = crestPattern;
            IsCustomClub = isCustomClub;
            UsesDefaultZslLogo = usesDefaultZslLogo;
        }
    }
}
