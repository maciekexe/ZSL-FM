using System;
using System.Collections.Generic;

namespace ZSLFM.UI
{
    /// <summary>
    /// Buduje początkową kadrę w kreatorze klubu na podstawie wybranego poziomu trudności
    /// oraz darmowych Założycieli (gracz i znajomi).
    /// </summary>
    public static class ClubStarterSquadBuilder
    {
        public const int MaxFoundersCount = 4;
        public const int FullSquadSize = 18;
        public const int AmateurAndYouthSquadSize = 12;
        public const int MinOverall = 45;

        private static readonly string[] FirstNames =
        {
            "Adam", "Bartosz", "Cezary", "Dominik", "Eryk", "Fabian", "Grzegorz",
            "Hubert", "Igor", "Jakub", "Kacper", "Łukasz", "Maciej", "Nataniel",
            "Oskar", "Patryk", "Rafał", "Szymon", "Tomasz", "Wiktor"
        };

        private static readonly string[] LastNames =
        {
            "Bąk", "Chmielewski", "Dąbrowski", "Górski", "Jankowski", "Kaczmarek",
            "Kamiński", "Kowalczyk", "Kowalski", "Kozłowski", "Krawczyk", "Lewandowski",
            "Mazur", "Michalski", "Nowak", "Pawlak", "Piotrowski", "Sikora", "Wójcik", "Zieliński"
        };

        private static readonly (string Position, int TargetCount)[] FullSquadQuotas =
        {
            ("GK", 2),
            ("DEF", 6),
            ("MID", 6),
            ("STR", 4)
        };

        private static readonly (string Position, int TargetCount)[] CompactSquadQuotas =
        {
            ("GK", 2),
            ("DEF", 4),
            ("MID", 4),
            ("STR", 2)
        };

        public static bool TryCreateFounderDraft(
            string firstName,
            string lastName,
            string position,
            int overallSkill,
            int maxAllowedOvr,
            int currentFounderCount,
            out CustomPlayerDraft draft,
            out string validationError)
        {
            if (currentFounderCount >= MaxFoundersCount)
            {
                draft = null;
                validationError = $"Możesz dodać maksymalnie {MaxFoundersCount} Założycieli do jednego klubu.";
                return false;
            }

            string cleanFirst = (firstName ?? string.Empty).Trim();
            string cleanLast = (lastName ?? string.Empty).Trim();
            string normalizedPos = (position ?? string.Empty).Trim().ToUpperInvariant();

            if (cleanFirst.Length < 2 || cleanLast.Length < 2)
            {
                draft = null;
                validationError = "Podaj imię i nazwisko Założyciela (min. 2 znaki).";
                return false;
            }

            if (normalizedPos != "GK" && normalizedPos != "DEF" && normalizedPos != "MID" && normalizedPos != "STR")
            {
                draft = null;
                validationError = "Wybierz poprawną pozycję (GK, DEF, MID lub STR).";
                return false;
            }

            if (overallSkill < MinOverall || overallSkill > maxAllowedOvr)
            {
                draft = null;
                validationError = $"Dla wybranego poziomu trudności Założyciel może mieć OVR od {MinOverall} do {maxAllowedOvr}.";
                return false;
            }

            draft = new CustomPlayerDraft(cleanFirst, cleanLast, normalizedPos, overallSkill);
            validationError = string.Empty;
            return true;
        }

        public static List<PlayerProfileDto> BuildStartingSquadForDifficulty(
            string clubName,
            StarterBudgetTier difficulty,
            bool keepReplacedClubSquad,
            string replacedClubName,
            IReadOnlyList<CustomPlayerDraft> founders,
            int seed = 2026)
        {
            string targetClub = string.IsNullOrWhiteSpace(clubName) ? "Nowy Klub" : clubName.Trim();
            StarterBudgetTier effectiveTier = difficulty ?? ClubCreatorCatalog.BudgetTiers[0];
            var rng = new Random(seed);
            List<PlayerProfileDto> founderDtos = ConvertFounderDrafts(founders, targetClub, rng);

            if (keepReplacedClubSquad &&
                effectiveTier.AllowsReplacedClubSquad &&
                !string.IsNullOrWhiteSpace(replacedClubName))
            {
                List<PlayerProfileDto> inherited = BuildFromReplacedClub(targetClub, replacedClubName, rng);
                var combined = new List<PlayerProfileDto>(founderDtos);
                combined.AddRange(inherited);
                return combined;
            }

            switch (effectiveTier.DifficultyType)
            {
                case DifficultyTierType.HardcoreZero:
                    return PrependFounders(
                        founderDtos,
                        GenerateBalancedSquad(
                            targetClub, Array.Empty<PlayerProfileDto>(), CompactSquadQuotas, AmateurAndYouthSquadSize,
                            minOvr: 42, maxOvrExclusive: 53, minAge: 18, maxAgeExclusive: 27, rng));

                case DifficultyTierType.SchoolClub:
                    return PrependFounders(
                        founderDtos,
                        GenerateBalancedSquad(
                            targetClub, Array.Empty<PlayerProfileDto>(), CompactSquadQuotas, AmateurAndYouthSquadSize,
                            minOvr: 48, maxOvrExclusive: 59, minAge: 15, maxAgeExclusive: 18, rng));

                case DifficultyTierType.InvestorProject:
                    return GenerateBalancedSquad(
                        targetClub, founderDtos, FullSquadQuotas, FullSquadSize,
                        minOvr: 64, maxOvrExclusive: 77, minAge: 18, maxAgeExclusive: 30, rng);

                case DifficultyTierType.LeagueNewcomer:
                default:
                    return GenerateBalancedSquad(
                        targetClub, founderDtos, FullSquadQuotas, FullSquadSize,
                        minOvr: 58, maxOvrExclusive: 71, minAge: 17, maxAgeExclusive: 30, rng);
            }
        }

        public static List<PlayerProfileDto> BuildInheritedSquad(string clubName, string sourceClubName, int seed = 2026)
        {
            string targetClub = string.IsNullOrWhiteSpace(clubName) ? "ZSŁ Gdańsk" : clubName.Trim();
            return BuildFromReplacedClub(targetClub, sourceClubName, new Random(seed));
        }

        private static List<PlayerProfileDto> PrependFounders(
            List<PlayerProfileDto> founders,
            List<PlayerProfileDto> baseSquad)
        {
            var combined = new List<PlayerProfileDto>(founders.Count + baseSquad.Count);
            combined.AddRange(founders);
            combined.AddRange(baseSquad);
            return combined;
        }

        private static List<PlayerProfileDto> ConvertFounderDrafts(
            IReadOnlyList<CustomPlayerDraft> founders,
            string targetClub,
            Random rng)
        {
            var list = new List<PlayerProfileDto>();
            if (founders == null) return list;

            foreach (CustomPlayerDraft draft in founders)
            {
                if (draft == null) continue;
                list.Add(CreatePlayerDto(
                    draft.FirstName,
                    draft.LastName,
                    draft.Position,
                    draft.OverallSkill,
                    targetClub,
                    age: 18,
                    rng: rng));
            }

            return list;
        }

        private static List<PlayerProfileDto> BuildFromReplacedClub(string targetClub, string replacedClubName, Random rng)
        {
            var inherited = new List<PlayerProfileDto>();

            if (string.IsNullOrWhiteSpace(replacedClubName) ||
                string.Equals(replacedClubName, "ZSŁ Gdańsk", StringComparison.OrdinalIgnoreCase))
            {
                foreach (PlayerProfileDto p in UiSampleSquadDataProvider.CreateSampleSquadPlayers())
                {
                    inherited.Add(CloneForClub(p, targetClub));
                }
                return inherited;
            }

            foreach (PlayerProfileDto p in UiSampleSquadDataProvider.CreateSampleMarketPlayers())
            {
                if (string.Equals(p.ClubName, replacedClubName, StringComparison.OrdinalIgnoreCase))
                {
                    inherited.Add(CloneForClub(p, targetClub));
                }
            }

            return inherited.Count >= FullSquadSize
                ? inherited
                : GenerateBalancedSquad(targetClub, inherited, FullSquadQuotas, FullSquadSize, 58, 71, 17, 30, rng);
        }

        private static List<PlayerProfileDto> GenerateBalancedSquad(
            string targetClub,
            IReadOnlyList<PlayerProfileDto> existingPlayers,
            (string Position, int TargetCount)[] quotas,
            int targetSize,
            int minOvr,
            int maxOvrExclusive,
            int minAge,
            int maxAgeExclusive,
            Random rng)
        {
            var squad = new List<PlayerProfileDto>(existingPlayers);
            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["GK"] = 0,
                ["DEF"] = 0,
                ["MID"] = 0,
                ["STR"] = 0
            };

            foreach (PlayerProfileDto p in squad)
            {
                if (counts.ContainsKey(p.Position))
                {
                    counts[p.Position]++;
                }
            }

            foreach ((string pos, int quota) in quotas)
            {
                while (counts[pos] < quota)
                {
                    squad.Add(GenerateRandomPlayer(targetClub, pos, minOvr, maxOvrExclusive, minAge, maxAgeExclusive, rng));
                    counts[pos]++;
                }
            }

            while (squad.Count < targetSize)
            {
                squad.Add(GenerateRandomPlayer(targetClub, "MID", minOvr, maxOvrExclusive, minAge, maxAgeExclusive, rng));
            }

            return squad;
        }

        private static PlayerProfileDto GenerateRandomPlayer(
            string clubName,
            string position,
            int minOvr,
            int maxOvrExclusive,
            int minAge,
            int maxAgeExclusive,
            Random rng)
        {
            string first = FirstNames[rng.Next(FirstNames.Length)];
            string last = LastNames[rng.Next(LastNames.Length)];
            int ovr = rng.Next(minOvr, maxOvrExclusive);
            int age = rng.Next(minAge, maxAgeExclusive);

            return CreatePlayerDto(first, last, position, ovr, clubName, age, rng);
        }

        private static PlayerProfileDto CreatePlayerDto(
            string firstName,
            string lastName,
            string position,
            int ovr,
            string clubName,
            int age,
            Random rng)
        {
            double price = EstimateMarketValue(ovr);
            double wage = Math.Max(200.0, Math.Round(price * 0.012 / 50.0) * 50.0);

            int pac = ClampAttr(ovr + rng.Next(-6, 7));
            int sho = ClampAttr(position == "STR" ? ovr + 3 : (position == "GK" ? ovr - 35 : ovr - 6));
            int pas = ClampAttr(position == "MID" ? ovr + 4 : ovr - 4);
            int dri = ClampAttr(position == "MID" || position == "STR" ? ovr + 2 : ovr - 10);
            int def = ClampAttr(position == "DEF" || position == "GK" ? ovr + 3 : (position == "STR" ? ovr - 28 : ovr - 8));
            int phy = ClampAttr(ovr + rng.Next(-5, 6));

            return new PlayerProfileDto(
                firstName, lastName, position, ovr, clubName,
                price, age, wage,
                pac, sho, pas, dri, def, phy);
        }

        private static double EstimateMarketValue(int ovr)
        {
            if (ovr <= 50) return 10000.0;
            if (ovr <= 60) return 20000.0 + (ovr - 50) * 2000.0;
            if (ovr <= 70) return 40000.0 + (ovr - 60) * 5000.0;
            if (ovr <= 80) return 90000.0 + (ovr - 70) * 12000.0;
            return 210000.0 + (ovr - 80) * 30000.0;
        }

        private static PlayerProfileDto CloneForClub(PlayerProfileDto src, string clubName)
        {
            return new PlayerProfileDto(
                src.FirstName, src.LastName, src.Position, src.OverallSkill,
                clubName, src.Price, src.Age, src.Wage,
                src.Pace, src.Shooting, src.Passing, src.Dribbling, src.Defending, src.Physicality);
        }

        private static int ClampAttr(int value)
        {
            if (value < 20) return 20;
            if (value > 99) return 99;
            return value;
        }
    }
}
