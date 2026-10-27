using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Pasywny kontroler widoku kadry oraz rynku transferowego (SquadView) w Unity UI Toolkit.
    /// Zgodnie z wytycznymi architektonicznymi projektu (AGENTS.md):
    /// - Odpowiada wyłącznie za warstwę prezentacji (UI) i dziedziczy po MonoBehaviour.
    /// - Nie implementuje logiki biznesowej ani matematyki symulacyjnej (te należą do Core).
    /// - Obsługuje przełączanie zakładek, filtrowanie oraz akcje interfejsu (np. oferty transferowe).
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SquadViewController : MonoBehaviour
    {
        private const double ClubBudget = 150000.0;

        /// <summary>
        /// Model widoku dla zawodnika z kadry własnej.
        /// </summary>
        [Serializable]
        public class PlayerViewModel
        {
            public string FirstName { get; }
            public string LastName { get; }
            public string Position { get; }
            public int OverallSkill { get; }

            public PlayerViewModel(string firstName, string lastName, string position, int overallSkill)
            {
                FirstName = firstName;
                LastName = lastName;
                Position = position;
                OverallSkill = overallSkill;
            }
        }

        /// <summary>
        /// Model widoku dla zawodnika na rynku transferowym.
        /// </summary>
        [Serializable]
        public class MarketPlayerViewModel
        {
            public string FirstName { get; }
            public string LastName { get; }
            public string Position { get; }
            public int OverallSkill { get; }
            public string ClubName { get; }
            public double Price { get; }

            public MarketPlayerViewModel(string firstName, string lastName, string position, int overallSkill, string clubName, double price)
            {
                FirstName = firstName;
                LastName = lastName;
                Position = position;
                OverallSkill = overallSkill;
                ClubName = clubName;
                Price = price;
            }
        }

        private UIDocument _uiDocument;
        private Label _sectionTitle;
        private Label _sectionSubtitle;

        // Kontenery zakładek
        private VisualElement _squadContainer;
        private VisualElement _transferMarketContainer;
        private ScrollView _playersContainer;
        private ScrollView _marketPlayersContainer;

        // Przyciski nawigacji
        private readonly List<Button> _navButtons = new List<Button>();

        // Filtry rynku transferowego
        private readonly List<Button> _filterButtons = new List<Button>();
        private string _activePositionFilter = "ALL";
        private List<MarketPlayerViewModel> _allMarketPlayers;

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                Debug.LogWarning("[SquadViewController] Komponent UIDocument nie posiada jeszcze zainicjalizowanego VisualElement.");
                return;
            }

            VisualElement root = _uiDocument.rootVisualElement;

            InitializeElements(root);
            SetupNavigation(root);
            SetupMarketFilters(root);

            // Inicjalizacja danych
            RenderSquadList();
            InitializeMarketData();
            RenderMarketList();
        }

        private void InitializeElements(VisualElement root)
        {
            _sectionTitle = root.Q<Label>("SectionTitle");
            _sectionSubtitle = root.Q<Label>("SectionSubtitle");

            _squadContainer = root.Q<VisualElement>("SquadContainer");
            _transferMarketContainer = root.Q<VisualElement>("TransferMarketContainer");

            _playersContainer = root.Q<ScrollView>("PlayersContainer");
            _marketPlayersContainer = root.Q<ScrollView>("MarketPlayersContainer");
        }

        /// <summary>
        /// Konfiguruje przełączanie zakładek w menu bocznym.
        /// Odpowiednik routingu / zakładek w aplikacji Reactowej.
        /// </summary>
        private void SetupNavigation(VisualElement root)
        {
            _navButtons.Clear();

            Button btnSquad = root.Q<Button>("BtnSquad");
            Button btnTactics = root.Q<Button>("BtnTactics");
            Button btnTransfers = root.Q<Button>("BtnTransfers");
            Button btnFinances = root.Q<Button>("BtnFinances");

            if (btnSquad != null)
            {
                _navButtons.Add(btnSquad);
                btnSquad.clicked += () => SwitchTab(btnSquad, "Kadra Zespołu", "ZSŁ Gdańsk • Pierwszy Skład i Rezerwy", showSquad: true, showMarket: false);
            }

            if (btnTransfers != null)
            {
                _navButtons.Add(btnTransfers);
                btnTransfers.clicked += () => SwitchTab(btnTransfers, "Rynek Transferowy", "Dostępni zawodnicy na rynku transferowym", showSquad: false, showMarket: true);
            }

            if (btnTactics != null)
            {
                _navButtons.Add(btnTactics);
                btnTactics.clicked += () =>
                {
                    SwitchTab(btnTactics, "Taktyka Meczowa", "Formacja i dyspozycje zespołu", showSquad: false, showMarket: false);
                    Debug.Log("[Łączność FM UI] Widok taktyki jest w przygotowaniu.");
                };
            }

            if (btnFinances != null)
            {
                _navButtons.Add(btnFinances);
                btnFinances.clicked += () =>
                {
                    SwitchTab(btnFinances, "Budżet i Finanse", "Przychody, wydatki i bilans transferowy", showSquad: false, showMarket: false);
                    Debug.Log("[Łączność FM UI] Widok finansów jest w przygotowaniu.");
                };
            }
        }

        /// <summary>
        /// Przełącza widoczność kontenerów za pomocą klasy CSS/USS .hidden.
        /// </summary>
        private void SwitchTab(Button clickedButton, string title, string subtitle, bool showSquad, bool showMarket)
        {
            // Aktualizacja aktywnego przycisku w menu
            foreach (Button btn in _navButtons)
            {
                btn.RemoveFromClassList("nav-button--active");
            }
            clickedButton.AddToClassList("nav-button--active");

            // Aktualizacja nagłówków
            if (_sectionTitle != null) _sectionTitle.text = title;
            if (_sectionSubtitle != null) _sectionSubtitle.text = subtitle;

            // Zarządzanie widocznością kontenerów
            if (_squadContainer != null)
            {
                if (showSquad) _squadContainer.RemoveFromClassList("hidden");
                else _squadContainer.AddToClassList("hidden");
            }

            if (_transferMarketContainer != null)
            {
                if (showMarket)
                {
                    _transferMarketContainer.RemoveFromClassList("hidden");
                    RenderMarketList();
                }
                else
                {
                    _transferMarketContainer.AddToClassList("hidden");
                }
            }

            Debug.Log($"[Łączność FM UI] Przełączono widok na: {title}");
        }

        #region KADRA ZESPOŁU (SQUAD)

        public void RenderSquadList()
        {
            if (_playersContainer == null) return;

            _playersContainer.Clear();
            List<PlayerViewModel> players = LoadInitialPlayerData();

            foreach (PlayerViewModel player in players)
            {
                VisualElement row = BuildPlayerRowElement(player);
                _playersContainer.Add(row);
            }
        }

        private VisualElement BuildPlayerRowElement(PlayerViewModel player)
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("player-row");

            Label posBadge = new Label(player.Position);
            posBadge.AddToClassList("player-pos-badge");
            row.Add(posBadge);

            Label nameLabel = new Label($"{player.FirstName} {player.LastName}");
            nameLabel.AddToClassList("player-name-label");
            row.Add(nameLabel);

            Label skillBadge = new Label(player.OverallSkill.ToString());
            skillBadge.AddToClassList("player-skill-badge");

            if (player.OverallSkill >= 80) skillBadge.AddToClassList("skill-high");
            else if (player.OverallSkill >= 65) skillBadge.AddToClassList("skill-mid");
            else skillBadge.AddToClassList("skill-low");

            row.Add(skillBadge);
            return row;
        }

        private List<PlayerViewModel> LoadInitialPlayerData()
        {
            return new List<PlayerViewModel>
            {
                new PlayerViewModel("Maciej", "Kamiński", "MID", 90),
                new PlayerViewModel("Nataniel", "Żbikowski", "STR", 90),
                new PlayerViewModel("Hubert", "Miłuch", "DEF", 90),
                new PlayerViewModel("Karol", "Stolc", "DEF", 65),

                new PlayerViewModel("Kacper", "Nowak", "GK", 75),
                new PlayerViewModel("Tomasz", "Zieliński", "GK", 63),
                new PlayerViewModel("Michał", "Wiśniewski", "DEF", 72),
                new PlayerViewModel("Jakub", "Wójcik", "DEF", 69),
                new PlayerViewModel("Mateusz", "Kowalczyk", "DEF", 70),
                new PlayerViewModel("Oskar", "Kozłowski", "DEF", 64),
                new PlayerViewModel("Piotr", "Lewandowski", "MID", 77),
                new PlayerViewModel("Szymon", "Szymański", "MID", 74),
                new PlayerViewModel("Dawid", "Woźniak", "MID", 68),
                new PlayerViewModel("Kamil", "Dąbrowski", "MID", 64),
                new PlayerViewModel("Filip", "Mazur", "MID", 71),
                new PlayerViewModel("Jan", "Krawczyk", "STR", 76),
                new PlayerViewModel("Mikołaj", "Kaczmarek", "STR", 69),
                new PlayerViewModel("Igor", "Nowicki", "STR", 66)
            };
        }

        #endregion

        #region RYNEK TRANSFEROWY (TRANSFER MARKET)

        private void SetupMarketFilters(VisualElement root)
        {
            _filterButtons.Clear();

            BindFilterButton(root.Q<Button>("FilterAll"), "ALL");
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

                _activePositionFilter = positionKey;
                RenderMarketList();
                Debug.Log($"[Rynek Transferowy] Zastosowano filtr pozycji: {positionKey}");
            };
        }

        private void RenderMarketList()
        {
            if (_marketPlayersContainer == null || _allMarketPlayers == null) return;

            _marketPlayersContainer.Clear();

            List<MarketPlayerViewModel> filteredList = _allMarketPlayers.FindAll(p =>
                _activePositionFilter == "ALL" || p.Position.Equals(_activePositionFilter, StringComparison.OrdinalIgnoreCase));

            foreach (MarketPlayerViewModel player in filteredList)
            {
                VisualElement row = BuildMarketPlayerRowElement(player);
                _marketPlayersContainer.Add(row);
            }
        }

        private VisualElement BuildMarketPlayerRowElement(MarketPlayerViewModel player)
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("market-row");

            // Pozycja
            Label posBadge = new Label(player.Position);
            posBadge.AddToClassList("player-pos-badge");
            row.Add(posBadge);

            // Imię i nazwisko
            Label nameLabel = new Label($"{player.FirstName} {player.LastName}");
            nameLabel.AddToClassList("player-name-label");
            row.Add(nameLabel);

            // Klub
            Label clubLabel = new Label(player.ClubName);
            clubLabel.AddToClassList("club-pill");
            row.Add(clubLabel);

            // Ocena ogólna (OverallSkill)
            Label skillBadge = new Label(player.OverallSkill.ToString());
            skillBadge.AddToClassList("player-skill-badge");
            if (player.OverallSkill >= 80) skillBadge.AddToClassList("skill-high");
            else if (player.OverallSkill >= 65) skillBadge.AddToClassList("skill-mid");
            else skillBadge.AddToClassList("skill-low");
            row.Add(skillBadge);

            // Wycena w PLN
            Label priceLabel = new Label($"{player.Price:N0} PLN");
            priceLabel.AddToClassList("price-tag");
            row.Add(priceLabel);

            // Przycisk akcji: "Złóż ofertę"
            Button bidButton = new Button();
            bidButton.text = "Złóż ofertę";
            bidButton.AddToClassList("btn-bid");
            bidButton.clicked += () => HandleTransferBid(player);
            row.Add(bidButton);

            return row;
        }

        /// <summary>
        /// Obsługa próby złożenia oferty transferowej z walidacją budżetu (150 000 PLN).
        /// </summary>
        private void HandleTransferBid(MarketPlayerViewModel player)
        {
            string fullName = $"{player.FirstName} {player.LastName}";

            if (player.Price <= ClubBudget)
            {
                double remaining = ClubBudget - player.Price;
                Debug.Log($"[Rynek Transferowy] Sukces! Złożono ofertę zakupu za zawodnika {fullName} ({player.ClubName}) na kwotę {player.Price:N0} PLN. Mieści się w budżecie 150 000 PLN! (Pozostałoby w budżecie: {remaining:N0} PLN).");
            }
            else
            {
                double shortage = player.Price - ClubBudget;
                Debug.LogWarning($"[Rynek Transferowy] Brak funduszy! Oferta za {fullName} ({player.ClubName}) wynosi {player.Price:N0} PLN i przekracza budżet klubu (150 000 PLN) o {shortage:N0} PLN!");
            }
        }

        private void InitializeMarketData()
        {
            _allMarketPlayers = new List<MarketPlayerViewModel>
            {
                // Zawodnicy w zasięgu budżetu klubu (<= 150 000 PLN) - np. Lechia Gdańsk, Arka Gdynia
                new MarketPlayerViewModel("Camilo", "Mena", "MID", 75, "Lechia Gdańsk", 120000.0),
                new MarketPlayerViewModel("Maksym", "Khlan", "STR", 74, "Lechia Gdańsk", 95000.0),
                new MarketPlayerViewModel("Rifet", "Kapić", "MID", 73, "Lechia Gdańsk", 85000.0),
                new MarketPlayerViewModel("Bogdan", "Sarnavskyi", "GK", 72, "Lechia Gdańsk", 75000.0),
                new MarketPlayerViewModel("Elias", "Olsson", "DEF", 71, "Lechia Gdańsk", 70000.0),

                new MarketPlayerViewModel("Karol", "Czubak", "STR", 74, "Arka Gdynia", 110000.0),
                new MarketPlayerViewModel("Hubert", "Adamczyk", "MID", 72, "Arka Gdynia", 80000.0),
                new MarketPlayerViewModel("Kacper", "Skóra", "STR", 70, "Arka Gdynia", 70000.0),
                new MarketPlayerViewModel("Martin", "Dobrotka", "DEF", 71, "Arka Gdynia", 65000.0),
                new MarketPlayerViewModel("Paweł", "Lenarcik", "GK", 70, "Arka Gdynia", 60000.0),

                // Gwiazdy światowe poza naszym budżetem (> 150 000 PLN) - PSG, Real Madryt, Bayern Monachium
                new MarketPlayerViewModel("Lucas", "Beraldo", "DEF", 84, "PSG", 950000.0),
                new MarketPlayerViewModel("Bradley", "Barcola", "STR", 86, "PSG", 1900000.0),
                new MarketPlayerViewModel("Vitinha", "Silva", "MID", 89, "PSG", 2800000.0),
                new MarketPlayerViewModel("Gianluigi", "Donnarumma", "GK", 88, "PSG", 3200000.0),

                new MarketPlayerViewModel("Antonio", "Rüdiger", "DEF", 87, "Real Madryt", 2100000.0),
                new MarketPlayerViewModel("Thibaut", "Courtois", "GK", 90, "Real Madryt", 3500000.0),
                new MarketPlayerViewModel("Jude", "Bellingham", "MID", 96, "Real Madryt", 5500000.0),
                new MarketPlayerViewModel("Vinicius", "Junior", "STR", 95, "Real Madryt", 6000000.0),

                new MarketPlayerViewModel("Dayot", "Upamecano", "DEF", 85, "Bayern Monachium", 1400000.0),
                new MarketPlayerViewModel("Joshua", "Kimmich", "MID", 88, "Bayern Monachium", 2600000.0),
                new MarketPlayerViewModel("Harry", "Kane", "STR", 93, "Bayern Monachium", 4200000.0)
            };
        }

        #endregion
    }
}
