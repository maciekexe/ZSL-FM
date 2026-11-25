using System;
using System.Collections.Generic;
using System.Text;

namespace ZSLFM.UI
{
    /// <summary>
    /// Katalog gotowych palet barw, pakietów budżetu startowego oraz istniejących klubów na potrzeby Menu Głównego.
    /// Zawiera także walidację formularza kreatora własnego klubu.
    /// </summary>
    public static class ClubCreatorCatalog
    {
        public const int MaxClubNameLength = 28;
        public const int MaxMonogramLength = 3;

        public static IReadOnlyList<ClubColorPalette> Palettes { get; } = new[]
        {
            new ClubColorPalette(
                id: "zsl-red",
                displayName: "Czerwień & Grafit",
                primaryGradient: new RgbColor(118, 22, 14),
                secondaryGradient: new RgbColor(36, 44, 47),
                baseDark: new RgbColor(16, 20, 22),
                accent: new RgbColor(201, 31, 16),
                accentLightText: new RgbColor(252, 165, 165)),
            new ClubColorPalette(
                id: "baltic-blue",
                displayName: "Bałtycki Błękit",
                primaryGradient: new RgbColor(16, 56, 112),
                secondaryGradient: new RgbColor(22, 35, 52),
                baseDark: new RgbColor(12, 17, 26),
                accent: new RgbColor(37, 99, 235),
                accentLightText: new RgbColor(147, 197, 253)),
            new ClubColorPalette(
                id: "emerald-pitch",
                displayName: "Murawa & Szmaragd",
                primaryGradient: new RgbColor(13, 78, 54),
                secondaryGradient: new RgbColor(24, 40, 36),
                baseDark: new RgbColor(12, 20, 18),
                accent: new RgbColor(16, 185, 129),
                accentLightText: new RgbColor(167, 243, 208)),
            new ClubColorPalette(
                id: "amber-steel",
                displayName: "Stal & Bursztyn",
                primaryGradient: new RgbColor(120, 63, 12),
                secondaryGradient: new RgbColor(40, 36, 30),
                baseDark: new RgbColor(18, 17, 16),
                accent: new RgbColor(245, 158, 11),
                accentLightText: new RgbColor(253, 230, 138)),
            new ClubColorPalette(
                id: "royal-violet",
                displayName: "Królewski Fiolet",
                primaryGradient: new RgbColor(76, 29, 115),
                secondaryGradient: new RgbColor(36, 28, 48),
                baseDark: new RgbColor(17, 14, 24),
                accent: new RgbColor(139, 92, 246),
                accentLightText: new RgbColor(221, 214, 254))
        };

        public static IReadOnlyList<StarterBudgetTier> BudgetTiers { get; } = new[]
        {
            new StarterBudgetTier(
                id: "hardcore-zero",
                difficultyType: DifficultyTierType.HardcoreZero,
                difficultyLabel: "HARDCORE",
                title: "Od zera (Amatorzy)",
                budget: 0.0,
                reputation: 15,
                maxFounderOvr: 58,
                allowsReplacedClubSquad: false,
                description: "0 PLN na start, 12 darmowych amatorów (OVR 42-52). Szukasz pierwszych sponsorów."),
            new StarterBudgetTier(
                id: "school-club",
                difficultyType: DifficultyTierType.SchoolClub,
                difficultyLabel: "TRUDNY",
                title: "Klub Szkolny",
                budget: 200000.0,
                reputation: 35,
                maxFounderOvr: 65,
                allowsReplacedClubSquad: false,
                description: "200 tys. PLN + 12 młodych wychowanków (15-17 lat, OVR 48-58). Budujesz drużynę od podstaw."),
            new StarterBudgetTier(
                id: "newcomer",
                difficultyType: DifficultyTierType.LeagueNewcomer,
                difficultyLabel: "ŚREDNI",
                title: "Beniaminek Ligi",
                budget: 750000.0,
                reputation: 50,
                maxFounderOvr: 75,
                allowsReplacedClubSquad: true,
                description: "750 tys. PLN i zbalansowana kadra 18 graczy (lub przejęcie zastępowanej drużyny)."),
            new StarterBudgetTier(
                id: "investor",
                difficultyType: DifficultyTierType.InvestorProject,
                difficultyLabel: "ŁATWY",
                title: "Projekt Inwestora",
                budget: 2000000.0,
                reputation: 65,
                maxFounderOvr: 85,
                allowsReplacedClubSquad: true,
                description: "2 mln PLN od sponsora i mocna kadra 18 graczy (lub przejęcie zastępowanej drużyny).")
        };

        public static IReadOnlyList<ExistingClubPreset> ExistingClubs { get; } = new[]
        {
            new ExistingClubPreset(
                id: "zsl-gdansk",
                clubName: "ZSŁ Gdańsk",
                monogram: "ZSŁ",
                budget: 150000.0,
                reputation: 50,
                description: "Szkolna duma z Gdańska. Młoda kadra z dużym potencjałem.",
                palette: Palettes[0],
                crestShape: CrestShape.Shield,
                crestPattern: CrestPattern.VerticalSplit,
                usesDefaultZslLogo: true),
            new ExistingClubPreset(
                id: "lechia-gdansk",
                clubName: "Lechia Gdańsk",
                monogram: "LGD",
                budget: 300000.0,
                reputation: 60,
                description: "Biało-zieloni z Gdańska. Solidny budżet na walkę o awans.",
                palette: Palettes[2],
                crestShape: CrestShape.Shield,
                crestPattern: CrestPattern.DiagonalStripe),
            new ExistingClubPreset(
                id: "arka-gdynia",
                clubName: "Arka Gdynia",
                monogram: "ARK",
                budget: 250000.0,
                reputation: 55,
                description: "Żółto-niebieska ekipa znad morza z walecznym składem.",
                palette: Palettes[3],
                crestShape: CrestShape.RoundedSquare,
                crestPattern: CrestPattern.HorizontalBand),
            new ExistingClubPreset(
                id: "psg",
                clubName: "PSG",
                monogram: "PSG",
                budget: 5000000.0,
                reputation: 95,
                description: "Paryski gigant z ogromnymi środkami na gwiazdy.",
                palette: Palettes[1],
                crestShape: CrestShape.Circle,
                crestPattern: CrestPattern.VerticalSplit),
            new ExistingClubPreset(
                id: "real-madryt",
                clubName: "Real Madryt",
                monogram: "RMA",
                budget: 8000000.0,
                reputation: 98,
                description: "Królewscy — najwyższa reputacja i rekordowy budżet.",
                palette: Palettes[4],
                crestShape: CrestShape.Circle,
                crestPattern: CrestPattern.DiagonalStripe)
        };

        public static ClubSetupSelection CreateFromExistingPreset(ExistingClubPreset preset)
        {
            if (preset == null) throw new ArgumentNullException(nameof(preset));

            var squad = ClubStarterSquadBuilder.BuildInheritedSquad(
                clubName: preset.ClubName,
                sourceClubName: preset.ClubName);

            return new ClubSetupSelection(
                clubName: preset.ClubName,
                monogram: preset.Monogram,
                budget: preset.Budget,
                reputation: preset.Reputation,
                palette: preset.Palette,
                crestShape: preset.CrestShape,
                crestPattern: preset.CrestPattern,
                isCustomClub: false,
                usesDefaultZslLogo: preset.UsesDefaultZslLogo,
                replacedClubId: preset.Id,
                replacedClubName: preset.ClubName,
                difficultyType: DifficultyTierType.LeagueNewcomer,
                squadMode: SquadBuildMode.KeepReplacedClub,
                startingSquad: squad,
                remainingBudget: preset.Budget);
        }

        public static bool TryBuildCustomClub(
            string rawClubName,
            string rawMonogram,
            ClubColorPalette palette,
            CrestShape crestShape,
            CrestPattern crestPattern,
            StarterBudgetTier budgetTier,
            out ClubSetupSelection selection,
            out string validationError)
        {
            return TryBuildCustomClub(
                rawClubName: rawClubName,
                rawMonogram: rawMonogram,
                palette: palette,
                crestShape: crestShape,
                crestPattern: crestPattern,
                jerseyPattern: JerseyPattern.Solid,
                budgetTier: budgetTier,
                city: string.Empty,
                nickname: string.Empty,
                stadiumName: string.Empty,
                replacedClub: null,
                rivalClub: ExistingClubs[1],
                customLogoFilePath: string.Empty,
                keepReplacedClubSquad: false,
                founders: null,
                selection: out selection,
                validationError: out validationError);
        }

        public static bool TryBuildCustomClub(
            string rawClubName,
            string rawMonogram,
            ClubColorPalette palette,
            CrestShape crestShape,
            CrestPattern crestPattern,
            JerseyPattern jerseyPattern,
            StarterBudgetTier budgetTier,
            string city,
            string nickname,
            string stadiumName,
            ExistingClubPreset replacedClub,
            ExistingClubPreset rivalClub,
            string customLogoFilePath,
            bool keepReplacedClubSquad,
            IReadOnlyList<CustomPlayerDraft> founders,
            out ClubSetupSelection selection,
            out string validationError)
        {
            string normalizedName = NormalizeClubName(rawClubName);
            if (string.IsNullOrEmpty(normalizedName))
            {
                selection = null;
                validationError = "Podaj nazwę swojego klubu (np. FC Łączność).";
                return false;
            }

            if (normalizedName.Length < 2)
            {
                selection = null;
                validationError = "Nazwa klubu musi mieć co najmniej 2 znaki.";
                return false;
            }

            StarterBudgetTier effectiveTier = budgetTier ?? BudgetTiers[0];
            if (keepReplacedClubSquad)
            {
                if (!effectiveTier.AllowsReplacedClubSquad)
                {
                    selection = null;
                    validationError = $"Przejęcie kadry zastępowanego klubu jest dostępne tylko na poziomie Średnim i Łatwym.";
                    return false;
                }

                if (replacedClub == null)
                {
                    selection = null;
                    validationError = "Aby przejąć kadrę zastępowanego klubu, wybierz najpierw klub do zastąpienia w Kroku 1.";
                    return false;
                }
            }

            if (founders != null)
            {
                if (founders.Count > ClubStarterSquadBuilder.MaxFoundersCount)
                {
                    selection = null;
                    validationError = $"Maksymalna liczba Założycieli to {ClubStarterSquadBuilder.MaxFoundersCount}.";
                    return false;
                }

                foreach (CustomPlayerDraft founder in founders)
                {
                    if (founder != null && founder.OverallSkill > effectiveTier.MaxFounderOvr)
                    {
                        selection = null;
                        validationError = $"Założyciel {founder.FullName} ma OVR {founder.OverallSkill}, co przekracza limit ({effectiveTier.MaxFounderOvr}) dla poziomu {effectiveTier.DifficultyLabel}.";
                        return false;
                    }
                }
            }

            string monogram = NormalizeMonogram(rawMonogram, normalizedName);
            ClubColorPalette effectivePalette = palette ?? Palettes[0];
            string cleanCity = NormalizeOptionalField(city, "Gdańsk");
            string cleanNickname = NormalizeOptionalField(nickname, "");
            string cleanStadium = NormalizeOptionalField(stadiumName, $"Stadion {normalizedName}");
            string rivalName = rivalClub != null ? rivalClub.ClubName : string.Empty;
            string replacedId = replacedClub != null ? replacedClub.Id : string.Empty;
            string replacedName = replacedClub != null ? replacedClub.ClubName : string.Empty;

            SquadBuildMode effectiveSquadMode = keepReplacedClubSquad
                ? SquadBuildMode.KeepReplacedClub
                : SquadBuildMode.GeneratedByDifficulty;

            var startingSquad = ClubStarterSquadBuilder.BuildStartingSquadForDifficulty(
                clubName: normalizedName,
                difficulty: effectiveTier,
                keepReplacedClubSquad: keepReplacedClubSquad,
                replacedClubName: replacedName,
                founders: founders);

            selection = new ClubSetupSelection(
                clubName: normalizedName,
                monogram: monogram,
                budget: effectiveTier.Budget,
                reputation: effectiveTier.Reputation,
                palette: effectivePalette,
                crestShape: crestShape,
                crestPattern: crestPattern,
                isCustomClub: true,
                usesDefaultZslLogo: false,
                jerseyPattern: jerseyPattern,
                customLogoFilePath: (customLogoFilePath ?? string.Empty).Trim(),
                city: cleanCity,
                nickname: cleanNickname,
                stadiumName: cleanStadium,
                replacedClubId: replacedId,
                replacedClubName: replacedName,
                rivalClubName: rivalName,
                difficultyType: effectiveTier.DifficultyType,
                squadMode: effectiveSquadMode,
                startingSquad: startingSquad,
                remainingBudget: effectiveTier.Budget);

            validationError = string.Empty;
            return true;
        }

        private static string NormalizeOptionalField(string raw, string fallback)
        {
            if (string.IsNullOrWhiteSpace(raw)) return fallback;
            string trimmed = raw.Trim();
            return trimmed.Length > 36 ? trimmed.Substring(0, 36).TrimEnd() : trimmed;
        }

        public static string NormalizeClubName(string rawClubName)
        {
            if (string.IsNullOrWhiteSpace(rawClubName)) return string.Empty;

            string trimmed = rawClubName.Trim();
            return trimmed.Length > MaxClubNameLength
                ? trimmed.Substring(0, MaxClubNameLength).TrimEnd()
                : trimmed;
        }

        public static string NormalizeMonogram(string rawMonogram, string fallbackClubName)
        {
            string cleaned = CleanLettersOrDigits(rawMonogram);
            if (!string.IsNullOrEmpty(cleaned))
            {
                return cleaned.Length > MaxMonogramLength
                    ? cleaned.Substring(0, MaxMonogramLength)
                    : cleaned;
            }

            string[] words = (fallbackClubName ?? string.Empty).Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (string word in words)
            {
                string letters = CleanLettersOrDigits(word);
                if (letters.Length > 0)
                {
                    sb.Append(letters[0]);
                    if (sb.Length >= MaxMonogramLength) break;
                }
            }

            if (sb.Length == 1)
            {
                string allLetters = CleanLettersOrDigits(fallbackClubName);
                if (allLetters.Length >= 2)
                {
                    return allLetters.Substring(0, Math.Min(MaxMonogramLength, allLetters.Length));
                }
            }

            return sb.Length > 0 ? sb.ToString() : "FM";
        }

        private static string CleanLettersOrDigits(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var sb = new StringBuilder();
            foreach (char c in input.Trim())
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(char.ToUpperInvariant(c));
                }
            }

            return sb.ToString();
        }
    }
}
