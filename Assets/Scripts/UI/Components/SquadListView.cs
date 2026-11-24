using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Komponent UI obsługujący zakładkę Kadra Zespołu (tabela zawodników oraz nagłówki sortowania).
    /// Nie przechowuje stanu domenowego — emituje zdarzenie wyboru zawodnika.
    /// </summary>
    public sealed class SquadListView
    {
        private readonly VisualElement _container;
        private readonly ScrollView _playersContainer;
        private readonly Button _sortNameBtn;
        private readonly Button _sortOverallBtn;

        private SortField _sortField = SortField.None;
        private bool _sortAscending;
        private IReadOnlyList<PlayerProfileDto> _lastPlayers;
        private string _lastSearchQuery = string.Empty;

        public event Action<PlayerProfileDto, VisualElement> PlayerSelected;

        public SquadListView(VisualElement root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            _container = root.Q<VisualElement>("SquadContainer");
            _playersContainer = root.Q<ScrollView>("PlayersContainer");
            _sortNameBtn = root.Q<Button>("SortSquadName");
            _sortOverallBtn = root.Q<Button>("SortSquadOverall");

            if (_sortNameBtn != null) _sortNameBtn.clicked += () => ToggleSort(SortField.Name);
            if (_sortOverallBtn != null) _sortOverallBtn.clicked += () => ToggleSort(SortField.Overall);
        }

        public void SetVisible(bool visible)
        {
            if (_container == null) return;
            if (visible) _container.RemoveFromClassList("hidden");
            else _container.AddToClassList("hidden");
        }

        public void Render(IReadOnlyList<PlayerProfileDto> players, string searchQuery)
        {
            _lastPlayers = players;
            _lastSearchQuery = searchQuery ?? string.Empty;

            if (_playersContainer == null || players == null) return;

            _playersContainer.Clear();

            List<PlayerProfileDto> filtered = PlayerListQueryHelper.FilterSquad(players, _lastSearchQuery);
            PlayerListQueryHelper.SortInPlace(filtered, _sortField, _sortAscending);

            if (filtered.Count == 0)
            {
                string emptyMessage = players.Count == 0
                    ? "Kadra zespołu jest pusta — przejdź do zakładki Transfery, aby zakontraktować zawodników."
                    : "Brak zawodników pasujących do wyszukiwania.";

                Label emptyLabel = new Label(emptyMessage);
                emptyLabel.style.color = new StyleColor(new UnityEngine.Color32(148, 163, 184, 255));
                emptyLabel.style.fontSize = 13;
                emptyLabel.style.unityTextAlign = UnityEngine.TextAnchor.MiddleCenter;
                emptyLabel.style.paddingTop = 28;
                emptyLabel.style.paddingBottom = 28;
                emptyLabel.style.whiteSpace = WhiteSpace.Normal;
                _playersContainer.Add(emptyLabel);
                return;
            }

            foreach (PlayerProfileDto player in filtered)
            {
                _playersContainer.Add(BuildRow(player));
            }
        }

        private void ToggleSort(SortField field)
        {
            if (_sortField == field) _sortAscending = !_sortAscending;
            else
            {
                _sortField = field;
                _sortAscending = false;
            }

            PlayerUiElementFactory.UpdateHeaderButtonLabel(_sortNameBtn, "ZAWODNIK", _sortField == SortField.Name, _sortAscending);
            PlayerUiElementFactory.UpdateHeaderButtonLabel(_sortOverallBtn, "OVERALL", _sortField == SortField.Overall, _sortAscending);

            if (_lastPlayers != null)
            {
                Render(_lastPlayers, _lastSearchQuery);
            }
        }

        private VisualElement BuildRow(PlayerProfileDto player)
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("player-row");

            row.Add(PlayerUiElementFactory.CreatePositionBadge(player.Position));

            Label nameLabel = new Label(player.FullName);
            nameLabel.AddToClassList("player-name-label");
            row.Add(nameLabel);

            row.Add(PlayerUiElementFactory.CreateSkillBadge(player.OverallSkill));

            row.RegisterCallback<ClickEvent>(_ => PlayerSelected?.Invoke(player, row));
            return row;
        }
    }
}
