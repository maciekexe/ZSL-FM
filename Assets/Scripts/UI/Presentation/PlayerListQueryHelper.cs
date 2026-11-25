using System;
using System.Collections.Generic;

namespace ZSLFM.UI
{
    /// <summary>
    /// Kryterium sortowania kolumn w tabelach kadry oraz rynku transferowego.
    /// </summary>
    public enum SortField
    {
        None,
        Name,
        Club,
        Overall,
        Price
    }

    /// <summary>
    /// Czysta logika prezentacji list zawodników (filtrowanie, sortowanie i klasy progów umiejętności).
    /// Brak zależności od UnityEngine pozwala testować ją automatycznie poza edytorem.
    /// </summary>
    public static class PlayerListQueryHelper
    {
        public static List<PlayerProfileDto> FilterSquad(
            IReadOnlyList<PlayerProfileDto> players,
            string searchQuery)
        {
            var result = new List<PlayerProfileDto>();
            if (players == null) return result;

            string query = (searchQuery ?? string.Empty).Trim();
            foreach (PlayerProfileDto player in players)
            {
                if (player == null) continue;

                if (string.IsNullOrEmpty(query) ||
                    ContainsIgnoreCase(player.FullName, query) ||
                    ContainsIgnoreCase(player.Position, query))
                {
                    result.Add(player);
                }
            }

            return result;
        }

        public static List<PlayerProfileDto> FilterMarket(
            IReadOnlyList<PlayerProfileDto> players,
            string positionFilter,
            string searchQuery)
        {
            var result = new List<PlayerProfileDto>();
            if (players == null) return result;

            string normalizedPosition = string.IsNullOrWhiteSpace(positionFilter) ? "ALL" : positionFilter.Trim();
            string query = (searchQuery ?? string.Empty).Trim();

            foreach (PlayerProfileDto player in players)
            {
                if (player == null) continue;

                bool matchesPosition =
                    normalizedPosition.Equals("ALL", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(player.Position, normalizedPosition, StringComparison.OrdinalIgnoreCase);

                if (!matchesPosition) continue;

                bool matchesQuery =
                    string.IsNullOrEmpty(query) ||
                    ContainsIgnoreCase(player.FullName, query) ||
                    ContainsIgnoreCase(player.ClubName, query);

                if (matchesQuery)
                {
                    result.Add(player);
                }
            }

            return result;
        }

        public static void SortInPlace(List<PlayerProfileDto> list, SortField field, bool ascending)
        {
            if (list == null || field == SortField.None) return;

            list.Sort((a, b) =>
            {
                int comparison = 0;
                switch (field)
                {
                    case SortField.Name:
                        comparison = string.Compare(a.LastName, b.LastName, StringComparison.OrdinalIgnoreCase);
                        if (comparison == 0)
                        {
                            comparison = string.Compare(a.FirstName, b.FirstName, StringComparison.OrdinalIgnoreCase);
                        }
                        break;
                    case SortField.Club:
                        comparison = string.Compare(a.ClubName, b.ClubName, StringComparison.OrdinalIgnoreCase);
                        break;
                    case SortField.Overall:
                        comparison = a.OverallSkill.CompareTo(b.OverallSkill);
                        break;
                    case SortField.Price:
                        comparison = a.Price.CompareTo(b.Price);
                        break;
                }

                return ascending ? comparison : -comparison;
            });
        }

        /// <summary>
        /// Zwraca klasę CSS odpowiadającą poziomowi umiejętności zawodnika w UI Toolkit.
        /// </summary>
        public static string GetSkillTierCssClass(int value)
        {
            if (value >= 80) return "skill-high";
            if (value >= 65) return "skill-mid";
            return "skill-low";
        }

        private static bool ContainsIgnoreCase(string source, string value)
        {
            return !string.IsNullOrEmpty(source) &&
                   source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
