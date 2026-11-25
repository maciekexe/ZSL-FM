using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Komponent UI obsługujący zakładkę Rynku Transferowego (filtry pozycji, sortowanie, przyciski ofert).
    /// Zdarzenia kliknięcia wiersza oraz złożenia oferty przekazuje na zewnątrz bez wykonywania logiki biznesowej.
    /// </summary>
    public sealed class TransferMarketView
    {
        private readonly VisualElement _container;
        private readonly ScrollView _marketPlayersContainer;
        private readonly ClubThemeApplier _themeApplier;
        private readonly List<Button> _filterButtons = new List<Button>();

        private readonly Button _sortNameBtn;
        private readonly Button _sortClubBtn;
        private readonly Button _sortOverallBtn;
        private readonly Button _sortPriceBtn;

        private Button _activeFilterButton;
        private string _activePositionFilter = "ALL";
        private SortField _sortField = SortField.None;
        private bool _sortAscending;
        private IReadOnlyList<PlayerProfileDto> _lastPlayers;
        private string _lastSearchQuery = string.Empty;

        public event Action<PlayerProfileDto, VisualElement> PlayerSelected;
        public event Action<PlayerProfileDto> BidRequested;

        public TransferMarketView(VisualElement root, ClubThemeApplier themeApplier)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            _themeApplier = themeApplier;

            _container = root.Q<VisualElement>("TransferMarketContainer");
            _marketPlayersContainer = root.Q<ScrollView>("MarketPlayersContainer");

            _sortNameBtn = root.Q<Button>("SortMarketName");
            _sortClubBtn = root.Q<Button>("SortMarketClub");
            _sortOverallBtn = root.Q<Button>("SortMarketOverall");
            _sortPriceBtn = root.Q<Button>("SortMarketPrice");

            if (_sortNameBtn != null) _sortNameBtn.clicked += () => ToggleSort(SortField.Name);
            if (_sortClubBtn != null) _sortClubBtn.clicked += () => ToggleSort(SortField.Club);
            if (_sortOverallBtn != null) _sortOverallBtn.clicked += () => ToggleSort(SortField.Overall);
            if (_sortPriceBtn != null) _sortPriceBtn.clicked += () => ToggleSort(SortField.Price);

            SetupPositionFilters(root);
        }

        public void SetVisible(bool visible)
        {
            if (_container == null) return;
            if (visible) _container.RemoveFromClassList("hidden");
            else _container.AddToClassList("hidden");
        }

        public void RefreshFilterTheme()
        {
            _themeApplier?.ApplyActiveFilterStyle(_filterButtons, _activeFilterButton);
        }

        public void Render(IReadOnlyList<PlayerProfileDto> players, string searchQuery)
        {
            _lastPlayers = players;
            _lastSearchQuery = searchQuery ?? string.Empty;

            if (_marketPlayersContainer == null || players == null) return;

            _marketPlayersContainer.Clear();

            List<PlayerProfileDto> filtered = PlayerListQueryHelper.FilterMarket(players, _activePositionFilter, _lastSearchQuery);
            PlayerListQueryHelper.SortInPlace(filtered, _sortField, _sortAscending);

            foreach (PlayerProfileDto player in filtered)
            {
                _marketPlayersContainer.Add(BuildRow(player));
            }
        }

        private void SetupPositionFilters(VisualElement root)
        {
            _filterButtons.Clear();

            Button filterAll = root.Q<Button>("FilterAll");
            _activeFilterButton = filterAll;

            BindFilterButton(filterAll, "ALL");
            BindFilterButton(root.Q<Button>("FilterGK"), "GK");
            BindFilterButton(root.Q<Button>("FilterDEF"), "DEF");
            BindFilterButton(root.Q<Button>("FilterMID"), "MID");
            BindFilterButton(root.Q<Button>("FilterSTR"), "STR");
        }

        private void BindFilterButton(Button button, string positionKey)
        {
            if (button == null) return;

            _filterButtons.Add(button);
            button.clicked += () =>
            {
                foreach (Button btn in _filterButtons)
                {
                    btn.RemoveFromClassList("filter-btn--active");
                }

                button.AddToClassList("filter-btn--active");
                _activeFilterButton = button;
                RefreshFilterTheme();

                _activePositionFilter = positionKey;
                if (_lastPlayers != null)
                {
                    Render(_lastPlayers, _lastSearchQuery);
                }

                Debug.Log($"[Rynek Transferowy] Zastosowano filtr pozycji: {positionKey}");
            };
        }

        private void ToggleSort(SortField field)
        {
            if (_sortField == field) _sortAscending = !_sortAscending;
            else
            {
                _sortField = field;
                _sortAscending = field == SortField.Price;
            }

            PlayerUiElementFactory.UpdateHeaderButtonLabel(_sortNameBtn, "ZAWODNIK", _sortField == SortField.Name, _sortAscending);
            PlayerUiElementFactory.UpdateHeaderButtonLabel(_sortClubBtn, "KLUB", _sortField == SortField.Club, _sortAscending);
            PlayerUiElementFactory.UpdateHeaderButtonLabel(_sortOverallBtn, "OVERALL", _sortField == SortField.Overall, _sortAscending);
            PlayerUiElementFactory.UpdateHeaderButtonLabel(_sortPriceBtn, "WARTOŚĆ", _sortField == SortField.Price, _sortAscending);

            if (_lastPlayers != null)
            {
                Render(_lastPlayers, _lastSearchQuery);
            }
        }

        private VisualElement BuildRow(PlayerProfileDto player)
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("market-row");

            row.Add(PlayerUiElementFactory.CreatePositionBadge(player.Position));

            Label nameLabel = new Label(player.FullName);
            nameLabel.AddToClassList("player-name-label");
            row.Add(nameLabel);

            Label clubLabel = new Label(player.ClubName);
            clubLabel.AddToClassList("club-pill");
            row.Add(clubLabel);

            row.Add(PlayerUiElementFactory.CreateSkillBadge(player.OverallSkill));

            Label priceLabel = new Label($"{player.Price:N0} PLN");
            priceLabel.AddToClassList("price-tag");
            row.Add(priceLabel);

            Button bidButton = new Button { text = "Złóż ofertę" };
            bidButton.AddToClassList("btn-bid");
            if (_themeApplier?.CurrentTheme != null)
            {
                bidButton.style.backgroundColor = _themeApplier.CurrentTheme.AccentColor;
            }

            bidButton.clicked += () => BidRequested?.Invoke(player);
            row.Add(bidButton);

            row.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.target == bidButton) return;
                PlayerSelected?.Invoke(player, row);
            });

            return row;
        }
    }
}
