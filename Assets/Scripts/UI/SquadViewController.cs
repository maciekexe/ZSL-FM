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
    /// - Nie implementuje logiki biznesowej ani silnika meczowego (te należą do Core).
    /// - Obsługuje dynamiczne motywy klubowe (logo, gradient całego tła, akcenty kolorystyczne),
    ///   wyszukiwanie na żywo, sortowanie kolumn, profil zawodnika oraz oferty transferowe.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SquadViewController : MonoBehaviour
    {
        private const double ClubBudget = 150000.0;

        /// <summary>
        /// Model konfiguracji wizualnej dla wybranego klubu.
        /// Pozwala automatycznie zmieniać herb, gradient całego ekranu oraz akcenty UI dla każdego klubu.
        /// </summary>
        [Serializable]
        public class ClubThemeDto
        {
            public string ClubName { get; }
            public Color PrimaryGradientColor { get; }
            public Color SecondaryGradientColor { get; }
            public Color BaseDarkColor { get; }
            public Color AccentColor { get; }
            public Color AccentLightTextColor { get; }
            public Texture2D LogoTexture { get; }

            public ClubThemeDto(
                string clubName,
                Color primaryGradientColor,
                Color secondaryGradientColor,
                Color baseDarkColor,
                Color accentColor,
                Color accentLightTextColor,
                Texture2D logoTexture = null)
            {
                ClubName = clubName;
                PrimaryGradientColor = primaryGradientColor;
                SecondaryGradientColor = secondaryGradientColor;
                BaseDarkColor = baseDarkColor;
                AccentColor = accentColor;
                AccentLightTextColor = accentLightTextColor;
                LogoTexture = logoTexture;
            }
        }

        /// <summary>
        /// Model DTO prezentujący pełny profil zawodnika na potrzeby UI.
        /// W przyszłości zasilany bezpośrednio z modeli POCO Core / bazy SQLite.
        /// </summary>
        [Serializable]
        public class PlayerProfileDto
        {
            public string FirstName { get; }
            public string LastName { get; }
            public string Position { get; }
            public int OverallSkill { get; }
            public string ClubName { get; }
            public double Price { get; }
            public int Age { get; }
            public double Wage { get; }

            // 6 Atrybutów stylu Football Managera (0-99)
            public int Pace { get; }
            public int Shooting { get; }
            public int Passing { get; }
            public int Dribbling { get; }
            public int Defending { get; }
            public int Physicality { get; }

            public PlayerProfileDto(string firstName, string lastName, string position, int overallSkill,
                string clubName, double price, int age, double wage,
                int pace, int shooting, int passing, int dribbling, int defending, int physicality)
            {
                FirstName = firstName;
                LastName = lastName;
                Position = position;
                OverallSkill = overallSkill;
                ClubName = clubName;
                Price = price;
                Age = age;
                Wage = wage;
                Pace = pace;
                Shooting = shooting;
                Passing = passing;
                Dribbling = dribbling;
                Defending = defending;
                Physicality = physicality;
            }

            public string FullName => $"{FirstName} {LastName}";
        }

        private enum SortField
        {
            None,
            Name,
            Club,
            Overall,
            Price
        }

        private UIDocument _uiDocument;
        private VisualElement _rootContainer;
        private VisualElement _clubLogoElement;
        private Label _clubTitleLabel;
        private Label _clubSubtitleLabel;
        private Label _clubBudgetLabel;
        private Label _sectionTitle;
        private Label _sectionSubtitle;
        private TextField _playerSearchInput;

        // Kontenery zakładek
        private VisualElement _squadContainer;
        private VisualElement _transferMarketContainer;
        private ScrollView _playersContainer;
        private ScrollView _marketPlayersContainer;

        // Boczny panel profilu piłkarza (Drawer)
        private VisualElement _playerDetailsDrawer;
        private Label _drawerTitle;
        private Label _detailName;
        private Label _detailClub;
        private Label _detailPosBadge;
        private Label _detailOverallBadge;
        private Label _detailAge;
        private Label _detailWage;
        private Label _detailValue;

        // Paski atrybutów w Drawerze
        private VisualElement _barPac, _barSho, _barPas, _barDri, _barDef, _barPhy;
        private Label _valPac, _valSho, _valPas, _valDri, _valDef, _valPhy;

        // Przyciski i filtry
        private readonly List<Button> _navButtons = new List<Button>();
        private Button _activeNavButton;
        private readonly List<Button> _filterButtons = new List<Button>();
        private Button _activeFilterButton;
        private string _activePositionFilter = "ALL";
        private string _searchQuery = "";

        // Sortowanie
        private SortField _squadSort = SortField.None;
        private bool _squadSortAsc = false;
        private SortField _marketSort = SortField.None;
        private bool _marketSortAsc = false;

        // Przyciski sortowania w nagłówkach
        private Button _sortSquadName, _sortSquadOverall;
        private Button _sortMarketName, _sortMarketClub, _sortMarketOverall, _sortMarketPrice;

        // Baza danych zawodników i motyw dla UI
        private List<PlayerProfileDto> _squadPlayers;
        private List<PlayerProfileDto> _marketPlayers;
        private VisualElement _selectedRowElement;
        private bool _isShowingSquadTab = true;
        private ClubThemeDto _currentTheme;
        private Texture2D _runtimeGradientTexture;

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
            SetupSearch();
            SetupSorting(root);
            SetupMarketFilters(root);
            SetupDrawer(root);

            InitializeData();

            // Zastosowanie domyślnego motywu makiety ZSŁ Gdańsk (kolory pobrane z herbu)
            ApplyClubTheme(CreateDefaultZslTheme());

            RenderSquadList();
            RenderMarketList();
        }

        private void OnDisable()
        {
            if (_runtimeGradientTexture != null)
            {
                Destroy(_runtimeGradientTexture);
                _runtimeGradientTexture = null;
            }
        }

        private void InitializeElements(VisualElement root)
        {
            _rootContainer = root.Q<VisualElement>("Root");
            _clubLogoElement = root.Q<VisualElement>("ClubLogo");
            _clubTitleLabel = root.Q<Label>("ClubTitle");
            _clubSubtitleLabel = root.Q<Label>("ClubSubtitle");
            _clubBudgetLabel = root.Q<Label>("ClubBudget");

            _sectionTitle = root.Q<Label>("SectionTitle");
            _sectionSubtitle = root.Q<Label>("SectionSubtitle");
            _playerSearchInput = root.Q<TextField>("PlayerSearchInput");

            _squadContainer = root.Q<VisualElement>("SquadContainer");
            _transferMarketContainer = root.Q<VisualElement>("TransferMarketContainer");

            _playersContainer = root.Q<ScrollView>("PlayersContainer");
            _marketPlayersContainer = root.Q<ScrollView>("MarketPlayersContainer");

            // Elementy Drawera profilu
            _playerDetailsDrawer = root.Q<VisualElement>("PlayerDetailsDrawer");
            _drawerTitle = root.Q<Label>(className: "drawer-title");
            _detailName = root.Q<Label>("DetailPlayerName");
            _detailClub = root.Q<Label>("DetailClubName");
            _detailPosBadge = root.Q<Label>("DetailPosBadge");
            _detailOverallBadge = root.Q<Label>("DetailOverallBadge");
            _detailAge = root.Q<Label>("DetailAge");
            _detailWage = root.Q<Label>("DetailWage");
            _detailValue = root.Q<Label>("DetailValue");

            // Paski i etykiety statystyk
            _barPac = root.Q<VisualElement>("BarPac");
            _barSho = root.Q<VisualElement>("BarSho");
            _barPas = root.Q<VisualElement>("BarPas");
            _barDri = root.Q<VisualElement>("BarDri");
            _barDef = root.Q<VisualElement>("BarDef");
            _barPhy = root.Q<VisualElement>("BarPhy");

            _valPac = root.Q<Label>("ValPac");
            _valSho = root.Q<Label>("ValSho");
            _valPas = root.Q<Label>("ValPas");
            _valDri = root.Q<Label>("ValDri");
            _valDef = root.Q<Label>("ValDef");
            _valPhy = root.Q<Label>("ValPhy");
        }

        #region DYNAMICZNY MOTYW KLUBU (LOGO, GRADIENT, AKCENTY)

        private ClubThemeDto CreateDefaultZslTheme()
        {
            return new ClubThemeDto(
                clubName: "ZSŁ Gdańsk",
                primaryGradientColor: new Color32(118, 22, 14, 255),   // Głęboka czerwień herbu ZSŁ
                secondaryGradientColor: new Color32(36, 44, 47, 255),  // Grafit lwów z herbu (#3A4548 przyciemniony)
                baseDarkColor: new Color32(16, 20, 22, 255),           // Ciemny grafit bazowy
                accentColor: new Color32(201, 31, 16, 255),            // Czerwień akcentowa ZSŁ (#C91F10)
                accentLightTextColor: new Color32(252, 165, 165, 255)  // Jasnoczerwony odcień tekstu
            );
        }

        /// <summary>
        /// Nakłada kolory, gradient całego tła oraz logo wybranego klubu na interfejs.
        /// Umożliwia zmianę całej oprawy wizualnej w zależności od prowadzonego klubu.
        /// </summary>
        public void ApplyClubTheme(ClubThemeDto theme)
        {
            if (theme == null) return;
            _currentTheme = theme;

            if (_clubTitleLabel != null) _clubTitleLabel.text = theme.ClubName;

            if (_clubLogoElement != null && theme.LogoTexture != null)
            {
                _clubLogoElement.style.backgroundImage = new StyleBackground(theme.LogoTexture);
            }

            // Generowanie płynnego gradientu na całe tło dashboardu z kolorów klubu
            if (_rootContainer != null)
            {
                if (_runtimeGradientTexture != null)
                {
                    Destroy(_runtimeGradientTexture);
                }

                _runtimeGradientTexture = GenerateDiagonalGradientTexture(
                    theme.PrimaryGradientColor,
                    theme.SecondaryGradientColor,
                    theme.BaseDarkColor
                );
                _rootContainer.style.backgroundImage = new StyleBackground(_runtimeGradientTexture);
            }

            // Aktualizacja akcentów tekstowych
            if (_clubSubtitleLabel != null) _clubSubtitleLabel.style.color = theme.AccentColor;
            if (_clubBudgetLabel != null) _clubBudgetLabel.style.color = theme.AccentColor;
            if (_drawerTitle != null) _drawerTitle.style.color = theme.AccentColor;
            if (_detailValue != null) _detailValue.style.color = theme.AccentLightTextColor;

            ApplyActiveNavThemeStyle();
            ApplyActiveFilterThemeStyle();
        }

        private Texture2D GenerateDiagonalGradientTexture(Color topLeft, Color mid, Color bottomRight)
        {
            const int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                // W Unity oś Y tekstury zaczyna się na dole (0 = dół, size-1 = góra)
                float ny = 1f - (y / (float)(size - 1));
                for (int x = 0; x < size; x++)
                {
                    float nx = x / (float)(size - 1);
                    float t = Mathf.Clamp01((nx + ny) * 0.5f);

                    Color c = t < 0.38f
                        ? Color.Lerp(topLeft, mid, t / 0.38f)
                        : Color.Lerp(mid, bottomRight, (t - 0.38f) / 0.62f);

                    pixels[y * size + x] = c;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        private void ApplyActiveNavThemeStyle()
        {
            if (_currentTheme == null) return;

            foreach (Button btn in _navButtons)
            {
                if (btn == _activeNavButton)
                {
                    Color bg = _currentTheme.AccentColor;
                    bg.a = 0.18f;
                    btn.style.backgroundColor = bg;
                    btn.style.color = _currentTheme.AccentLightTextColor;
                    btn.style.borderLeftColor = _currentTheme.AccentColor;
                }
                else
                {
                    btn.style.backgroundColor = StyleKeyword.Null;
                    btn.style.color = StyleKeyword.Null;
                    btn.style.borderLeftColor = StyleKeyword.Null;
                }
            }
        }

        private void ApplyActiveFilterThemeStyle()
        {
            if (_currentTheme == null) return;

            foreach (Button btn in _filterButtons)
            {
                if (btn == _activeFilterButton)
                {
                    Color bg = _currentTheme.AccentColor;
                    bg.a = 0.25f;
                    btn.style.backgroundColor = bg;
                    btn.style.borderTopColor = _currentTheme.AccentColor;
                    btn.style.borderBottomColor = _currentTheme.AccentColor;
                    btn.style.borderLeftColor = _currentTheme.AccentColor;
                    btn.style.borderRightColor = _currentTheme.AccentColor;
                    btn.style.color = _currentTheme.AccentLightTextColor;
                }
                else
                {
                    btn.style.backgroundColor = StyleKeyword.Null;
                    btn.style.borderTopColor = StyleKeyword.Null;
                    btn.style.borderBottomColor = StyleKeyword.Null;
                    btn.style.borderLeftColor = StyleKeyword.Null;
                    btn.style.borderRightColor = StyleKeyword.Null;
                    btn.style.color = StyleKeyword.Null;
                }
            }
        }

        #endregion

        #region NAWIGACJA I ZAKŁADKI

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
                _activeNavButton = btnSquad;
                btnSquad.clicked += () => SwitchTab(btnSquad, "Kadra Zespołu", $"{(_currentTheme != null ? _currentTheme.ClubName : "ZSŁ Gdańsk")} • Pierwszy Skład i Rezerwy", showSquad: true, showMarket: false);
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

        private void SwitchTab(Button clickedButton, string title, string subtitle, bool showSquad, bool showMarket)
        {
            foreach (Button btn in _navButtons)
            {
                btn.RemoveFromClassList("nav-button--active");
            }
            clickedButton.AddToClassList("nav-button--active");
            _activeNavButton = clickedButton;
            ApplyActiveNavThemeStyle();

            if (_sectionTitle != null) _sectionTitle.text = title;
            if (_sectionSubtitle != null) _sectionSubtitle.text = subtitle;

            _isShowingSquadTab = showSquad;

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

            if (showSquad) RenderSquadList();

            DeselectCurrentRow();
            HideDrawer();

            Debug.Log($"[Łączność FM UI] Przełączono widok na: {title}");
        }

        #endregion

        #region WYSZUKIWARKA NA ŻYWO (SEARCH)

        private void SetupSearch()
        {
            if (_playerSearchInput == null) return;

            _playerSearchInput.RegisterValueChangedCallback(evt =>
            {
                _searchQuery = (evt.newValue ?? "").Trim();
                if (_isShowingSquadTab) RenderSquadList();
                else RenderMarketList();
            });
        }

        #endregion

        #region SORTOWANIE KOLUMN

        private void SetupSorting(VisualElement root)
        {
            _sortSquadName = root.Q<Button>("SortSquadName");
            _sortSquadOverall = root.Q<Button>("SortSquadOverall");

            _sortMarketName = root.Q<Button>("SortMarketName");
            _sortMarketClub = root.Q<Button>("SortMarketClub");
            _sortMarketOverall = root.Q<Button>("SortMarketOverall");
            _sortMarketPrice = root.Q<Button>("SortMarketPrice");

            if (_sortSquadName != null)
                _sortSquadName.clicked += () => ToggleSquadSort(SortField.Name, _sortSquadName, "ZAWODNIK");

            if (_sortSquadOverall != null)
                _sortSquadOverall.clicked += () => ToggleSquadSort(SortField.Overall, _sortSquadOverall, "OVERALL");

            if (_sortMarketName != null)
                _sortMarketName.clicked += () => ToggleMarketSort(SortField.Name, _sortMarketName, "ZAWODNIK");

            if (_sortMarketClub != null)
                _sortMarketClub.clicked += () => ToggleMarketSort(SortField.Club, _sortMarketClub, "KLUB");

            if (_sortMarketOverall != null)
                _sortMarketOverall.clicked += () => ToggleMarketSort(SortField.Overall, _sortMarketOverall, "OVERALL");

            if (_sortMarketPrice != null)
                _sortMarketPrice.clicked += () => ToggleMarketSort(SortField.Price, _sortMarketPrice, "WARTOŚĆ");
        }

        private void ToggleSquadSort(SortField field, Button headerBtn, string label)
        {
            if (_squadSort == field) _squadSortAsc = !_squadSortAsc;
            else { _squadSort = field; _squadSortAsc = false; }

            ResetSquadHeaderLabels();
            headerBtn.text = $"{label} {(_squadSortAsc ? "^" : "v")}";
            headerBtn.AddToClassList("col-header-btn--active");

            RenderSquadList();
        }

        private void ToggleMarketSort(SortField field, Button headerBtn, string label)
        {
            if (_marketSort == field) _marketSortAsc = !_marketSortAsc;
            else { _marketSort = field; _marketSortAsc = (field == SortField.Price); }

            ResetMarketHeaderLabels();
            headerBtn.text = $"{label} {(_marketSortAsc ? "^" : "v")}";
            headerBtn.AddToClassList("col-header-btn--active");

            RenderMarketList();
        }

        private void ResetSquadHeaderLabels()
        {
            if (_sortSquadName != null) { _sortSquadName.text = "ZAWODNIK <>"; _sortSquadName.RemoveFromClassList("col-header-btn--active"); }
            if (_sortSquadOverall != null) { _sortSquadOverall.text = "OVERALL <>"; _sortSquadOverall.RemoveFromClassList("col-header-btn--active"); }
        }

        private void ResetMarketHeaderLabels()
        {
            if (_sortMarketName != null) { _sortMarketName.text = "ZAWODNIK <>"; _sortMarketName.RemoveFromClassList("col-header-btn--active"); }
            if (_sortMarketClub != null) { _sortMarketClub.text = "KLUB <>"; _sortMarketClub.RemoveFromClassList("col-header-btn--active"); }
            if (_sortMarketOverall != null) { _sortMarketOverall.text = "OVERALL <>"; _sortMarketOverall.RemoveFromClassList("col-header-btn--active"); }
            if (_sortMarketPrice != null) { _sortMarketPrice.text = "WARTOŚĆ <>"; _sortMarketPrice.RemoveFromClassList("col-header-btn--active"); }
        }

        #endregion

        #region BOCZNY PANEL PROFILU ZAWODNIKA (DRAWER)

        private void SetupDrawer(VisualElement root)
        {
            Button btnClose = root.Q<Button>("BtnCloseDetails");
            if (btnClose != null)
            {
                btnClose.clicked += () =>
                {
                    HideDrawer();
                    DeselectCurrentRow();
                };
            }
        }

        private void ShowPlayerDetails(PlayerProfileDto player, VisualElement rowElement)
        {
            if (_playerDetailsDrawer == null || player == null) return;

            DeselectCurrentRow();
            _selectedRowElement = rowElement;
            if (_selectedRowElement != null)
            {
                _selectedRowElement.AddToClassList("player-row--selected");
                if (_currentTheme != null)
                {
                    Color bg = _currentTheme.AccentColor;
                    bg.a = 0.14f;
                    _selectedRowElement.style.backgroundColor = bg;
                    _selectedRowElement.style.borderLeftColor = _currentTheme.AccentColor;
                    _selectedRowElement.style.borderTopColor = _currentTheme.AccentColor;
                    _selectedRowElement.style.borderBottomColor = _currentTheme.AccentColor;
                    _selectedRowElement.style.borderRightColor = _currentTheme.AccentColor;
                }
            }

            if (_detailName != null) _detailName.text = player.FullName;
            if (_detailClub != null) _detailClub.text = player.ClubName;
            if (_detailPosBadge != null) _detailPosBadge.text = player.Position;
            if (_detailAge != null) _detailAge.text = $"{player.Age} lat";
            if (_detailWage != null) _detailWage.text = $"{player.Wage:N0} PLN";
            if (_detailValue != null) _detailValue.text = $"{player.Price:N0} PLN";

            if (_detailOverallBadge != null)
            {
                _detailOverallBadge.text = player.OverallSkill.ToString();
                _detailOverallBadge.RemoveFromClassList("skill-high");
                _detailOverallBadge.RemoveFromClassList("skill-mid");
                _detailOverallBadge.RemoveFromClassList("skill-low");

                if (player.OverallSkill >= 80) _detailOverallBadge.AddToClassList("skill-high");
                else if (player.OverallSkill >= 65) _detailOverallBadge.AddToClassList("skill-mid");
                else _detailOverallBadge.AddToClassList("skill-low");
            }

            SetStatBar(_barPac, _valPac, player.Pace);
            SetStatBar(_barSho, _valSho, player.Shooting);
            SetStatBar(_barPas, _valPas, player.Passing);
            SetStatBar(_barDri, _valDri, player.Dribbling);
            SetStatBar(_barDef, _valDef, player.Defending);
            SetStatBar(_barPhy, _valPhy, player.Physicality);

            _playerDetailsDrawer.RemoveFromClassList("hidden");
        }

        private void SetStatBar(VisualElement bar, Label valLabel, int statValue)
        {
            if (valLabel != null) valLabel.text = statValue.ToString();
            if (bar != null)
            {
                bar.style.width = new Length(Mathf.Clamp(statValue, 5, 100), LengthUnit.Percent);
                if (statValue >= 80) bar.style.backgroundColor = new StyleColor(new Color32(16, 185, 129, 255));
                else if (statValue >= 65) bar.style.backgroundColor = new StyleColor(new Color32(245, 158, 11, 255));
                else bar.style.backgroundColor = new StyleColor(new Color32(100, 116, 139, 255));
            }
        }

        private void HideDrawer()
        {
            if (_playerDetailsDrawer != null) _playerDetailsDrawer.AddToClassList("hidden");
        }

        private void DeselectCurrentRow()
        {
            if (_selectedRowElement != null)
            {
                _selectedRowElement.RemoveFromClassList("player-row--selected");
                _selectedRowElement.style.backgroundColor = StyleKeyword.Null;
                _selectedRowElement.style.borderLeftColor = StyleKeyword.Null;
                _selectedRowElement.style.borderTopColor = StyleKeyword.Null;
                _selectedRowElement.style.borderBottomColor = StyleKeyword.Null;
                _selectedRowElement.style.borderRightColor = StyleKeyword.Null;
                _selectedRowElement = null;
            }
        }

        #endregion

        #region RENDEROWANIE KADRY

        public void RenderSquadList()
        {
            if (_playersContainer == null || _squadPlayers == null) return;

            _playersContainer.Clear();

            List<PlayerProfileDto> filtered = _squadPlayers.FindAll(p =>
                string.IsNullOrEmpty(_searchQuery) ||
                p.FullName.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.Position.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0);

            SortList(filtered, _squadSort, _squadSortAsc);

            foreach (PlayerProfileDto player in filtered)
            {
                VisualElement row = BuildPlayerRowElement(player);
                _playersContainer.Add(row);
            }
        }

        private VisualElement BuildPlayerRowElement(PlayerProfileDto player)
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("player-row");

            Label posBadge = new Label(player.Position);
            posBadge.AddToClassList("player-pos-badge");
            row.Add(posBadge);

            Label nameLabel = new Label(player.FullName);
            nameLabel.AddToClassList("player-name-label");
            row.Add(nameLabel);

            Label skillBadge = new Label(player.OverallSkill.ToString());
            skillBadge.AddToClassList("player-skill-badge");

            if (player.OverallSkill >= 80) skillBadge.AddToClassList("skill-high");
            else if (player.OverallSkill >= 65) skillBadge.AddToClassList("skill-mid");
            else skillBadge.AddToClassList("skill-low");

            row.Add(skillBadge);

            row.RegisterCallback<ClickEvent>(evt =>
            {
                ShowPlayerDetails(player, row);
            });

            return row;
        }

        #endregion

        #region RENDEROWANIE RYNKU TRANSFEROWEGO

        private void SetupMarketFilters(VisualElement root)
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
                ApplyActiveFilterThemeStyle();

                _activePositionFilter = positionKey;
                RenderMarketList();
                Debug.Log($"[Rynek Transferowy] Zastosowano filtr pozycji: {positionKey}");
            };
        }

        private void RenderMarketList()
        {
            if (_marketPlayersContainer == null || _marketPlayers == null) return;

            _marketPlayersContainer.Clear();

            List<PlayerProfileDto> filtered = _marketPlayers.FindAll(p =>
                (_activePositionFilter == "ALL" || p.Position.Equals(_activePositionFilter, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(_searchQuery) ||
                 p.FullName.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                 p.ClubName.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0));

            SortList(filtered, _marketSort, _marketSortAsc);

            foreach (PlayerProfileDto player in filtered)
            {
                VisualElement row = BuildMarketPlayerRowElement(player);
                _marketPlayersContainer.Add(row);
            }
        }

        private VisualElement BuildMarketPlayerRowElement(PlayerProfileDto player)
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("market-row");

            Label posBadge = new Label(player.Position);
            posBadge.AddToClassList("player-pos-badge");
            row.Add(posBadge);

            Label nameLabel = new Label(player.FullName);
            nameLabel.AddToClassList("player-name-label");
            row.Add(nameLabel);

            Label clubLabel = new Label(player.ClubName);
            clubLabel.AddToClassList("club-pill");
            row.Add(clubLabel);

            Label skillBadge = new Label(player.OverallSkill.ToString());
            skillBadge.AddToClassList("player-skill-badge");
            if (player.OverallSkill >= 80) skillBadge.AddToClassList("skill-high");
            else if (player.OverallSkill >= 65) skillBadge.AddToClassList("skill-mid");
            else skillBadge.AddToClassList("skill-low");
            row.Add(skillBadge);

            Label priceLabel = new Label($"{player.Price:N0} PLN");
            priceLabel.AddToClassList("price-tag");
            row.Add(priceLabel);

            Button bidButton = new Button();
            bidButton.text = "Złóż ofertę";
            bidButton.AddToClassList("btn-bid");
            if (_currentTheme != null)
            {
                bidButton.style.backgroundColor = _currentTheme.AccentColor;
            }
            bidButton.clicked += () => HandleTransferBid(player);
            row.Add(bidButton);

            row.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.target == bidButton) return;
                ShowPlayerDetails(player, row);
            });

            return row;
        }

        private void HandleTransferBid(PlayerProfileDto player)
        {
            if (player.Price <= ClubBudget)
            {
                double remaining = ClubBudget - player.Price;
                Debug.Log($"[Rynek Transferowy] Sukces! Złożono ofertę zakupu za zawodnika {player.FullName} ({player.ClubName}) na kwotę {player.Price:N0} PLN. Mieści się w budżecie 150 000 PLN! (Pozostałoby w budżecie: {remaining:N0} PLN).");
            }
            else
            {
                double shortage = player.Price - ClubBudget;
                Debug.LogWarning($"[Rynek Transferowy] Brak funduszy! Oferta za {player.FullName} ({player.ClubName}) wynosi {player.Price:N0} PLN i przekracza budżet klubu (150 000 PLN) o {shortage:N0} PLN!");
            }
        }

        #endregion

        #region SORTOWANIE LOGICZNE

        private void SortList(List<PlayerProfileDto> list, SortField field, bool ascending)
        {
            if (field == SortField.None) return;

            list.Sort((a, b) =>
            {
                int comparison = 0;
                switch (field)
                {
                    case SortField.Name:
                        comparison = string.Compare(a.LastName, b.LastName, StringComparison.OrdinalIgnoreCase);
                        if (comparison == 0) comparison = string.Compare(a.FirstName, b.FirstName, StringComparison.OrdinalIgnoreCase);
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

        #endregion

        #region BAZA DANYCH POCZĄTKOWYCH

        private void InitializeData()
        {
            _squadPlayers = new List<PlayerProfileDto>
            {
                new PlayerProfileDto("Maciej", "Kamiński", "MID", 90, "ZSŁ Gdańsk", 450000.0, 18, 2500.0, 88, 86, 94, 92, 82, 84),
                new PlayerProfileDto("Nataniel", "Żbikowski", "STR", 90, "ZSŁ Gdańsk", 500000.0, 18, 2500.0, 94, 92, 82, 88, 50, 86),
                new PlayerProfileDto("Hubert", "Miłuch", "DEF", 90, "ZSŁ Gdańsk", 420000.0, 18, 2500.0, 84, 62, 85, 80, 94, 92),
                new PlayerProfileDto("Karol", "Stolc", "DEF", 65, "ZSŁ Gdańsk", 45000.0, 18, 500.0, 66, 45, 62, 60, 68, 70),

                new PlayerProfileDto("Kacper", "Nowak", "GK", 75, "ZSŁ Gdańsk", 90000.0, 20, 1200.0, 60, 30, 65, 55, 76, 78),
                new PlayerProfileDto("Tomasz", "Zieliński", "GK", 63, "ZSŁ Gdańsk", 35000.0, 17, 400.0, 55, 25, 58, 50, 64, 65),
                new PlayerProfileDto("Michał", "Wiśniewski", "DEF", 72, "ZSŁ Gdańsk", 75000.0, 22, 1000.0, 72, 48, 68, 65, 74, 75),
                new PlayerProfileDto("Jakub", "Wójcik", "DEF", 69, "ZSŁ Gdańsk", 55000.0, 19, 700.0, 68, 42, 64, 62, 71, 72),
                new PlayerProfileDto("Mateusz", "Kowalczyk", "DEF", 70, "ZSŁ Gdańsk", 60000.0, 21, 850.0, 70, 44, 66, 63, 72, 73),
                new PlayerProfileDto("Oskar", "Kozłowski", "DEF", 64, "ZSŁ Gdańsk", 40000.0, 18, 450.0, 64, 40, 60, 58, 66, 67),
                new PlayerProfileDto("Piotr", "Lewandowski", "MID", 77, "ZSŁ Gdańsk", 110000.0, 23, 1400.0, 76, 72, 79, 78, 68, 72),
                new PlayerProfileDto("Szymon", "Szymański", "MID", 74, "ZSŁ Gdańsk", 85000.0, 20, 1100.0, 75, 70, 76, 75, 65, 70),
                new PlayerProfileDto("Dawid", "Woźniak", "MID", 68, "ZSŁ Gdańsk", 50000.0, 19, 650.0, 68, 62, 70, 69, 60, 65),
                new PlayerProfileDto("Kamil", "Dąbrowski", "MID", 64, "ZSŁ Gdańsk", 40000.0, 18, 450.0, 65, 58, 66, 65, 58, 62),
                new PlayerProfileDto("Filip", "Mazur", "MID", 71, "ZSŁ Gdańsk", 70000.0, 21, 900.0, 72, 66, 73, 72, 62, 68),
                new PlayerProfileDto("Jan", "Krawczyk", "STR", 76, "ZSŁ Gdańsk", 105000.0, 22, 1300.0, 78, 77, 68, 74, 42, 75),
                new PlayerProfileDto("Mikołaj", "Kaczmarek", "STR", 69, "ZSŁ Gdańsk", 55000.0, 19, 700.0, 74, 71, 62, 68, 38, 70),
                new PlayerProfileDto("Igor", "Nowicki", "STR", 66, "ZSŁ Gdańsk", 45000.0, 18, 500.0, 72, 68, 60, 65, 36, 67)
            };

            _marketPlayers = new List<PlayerProfileDto>
            {
                new PlayerProfileDto("Camilo", "Mena", "MID", 75, "Lechia Gdańsk", 120000.0, 22, 1800.0, 84, 72, 74, 80, 52, 70),
                new PlayerProfileDto("Maksym", "Khlan", "STR", 74, "Lechia Gdańsk", 95000.0, 21, 1500.0, 82, 75, 70, 78, 45, 68),
                new PlayerProfileDto("Rifet", "Kapić", "MID", 73, "Lechia Gdańsk", 85000.0, 28, 1400.0, 70, 71, 78, 74, 64, 72),
                new PlayerProfileDto("Bogdan", "Sarnavskyi", "GK", 72, "Lechia Gdańsk", 75000.0, 29, 1300.0, 58, 28, 65, 52, 74, 75),
                new PlayerProfileDto("Elias", "Olsson", "DEF", 71, "Lechia Gdańsk", 70000.0, 21, 1200.0, 70, 42, 66, 62, 73, 76),

                new PlayerProfileDto("Karol", "Czubak", "STR", 74, "Arka Gdynia", 110000.0, 24, 1700.0, 78, 78, 64, 70, 40, 82),
                new PlayerProfileDto("Hubert", "Adamczyk", "MID", 72, "Arka Gdynia", 80000.0, 26, 1350.0, 74, 73, 76, 75, 55, 68),
                new PlayerProfileDto("Kacper", "Skóra", "STR", 70, "Arka Gdynia", 70000.0, 21, 1100.0, 80, 71, 66, 73, 42, 66),
                new PlayerProfileDto("Martin", "Dobrotka", "DEF", 71, "Arka Gdynia", 65000.0, 39, 1000.0, 60, 45, 68, 60, 75, 74),
                new PlayerProfileDto("Paweł", "Lenarcik", "GK", 70, "Arka Gdynia", 60000.0, 29, 950.0, 56, 25, 62, 50, 72, 72),

                new PlayerProfileDto("Lucas", "Beraldo", "DEF", 84, "PSG", 950000.0, 20, 25000.0, 78, 55, 79, 76, 86, 82),
                new PlayerProfileDto("Bradley", "Barcola", "STR", 86, "PSG", 1900000.0, 21, 45000.0, 92, 82, 80, 89, 44, 74),
                new PlayerProfileDto("Vitinha", "Silva", "MID", 89, "PSG", 2800000.0, 24, 60000.0, 80, 80, 91, 90, 76, 78),
                new PlayerProfileDto("Gianluigi", "Donnarumma", "GK", 88, "PSG", 3200000.0, 25, 75000.0, 65, 30, 75, 60, 90, 88),

                new PlayerProfileDto("Antonio", "Rüdiger", "DEF", 87, "Real Madryt", 2100000.0, 31, 55000.0, 86, 52, 72, 68, 88, 90),
                new PlayerProfileDto("Thibaut", "Courtois", "GK", 90, "Real Madryt", 3500000.0, 32, 85000.0, 68, 35, 78, 62, 92, 89),
                new PlayerProfileDto("Jude", "Bellingham", "MID", 96, "Real Madryt", 5500000.0, 21, 120000.0, 86, 90, 92, 94, 84, 88),
                new PlayerProfileDto("Vinicius", "Junior", "STR", 95, "Real Madryt", 6000000.0, 24, 110000.0, 98, 89, 85, 96, 42, 78),

                new PlayerProfileDto("Dayot", "Upamecano", "DEF", 85, "Bayern Monachium", 1400000.0, 25, 40000.0, 85, 48, 70, 72, 87, 88),
                new PlayerProfileDto("Joshua", "Kimmich", "MID", 88, "Bayern Monachium", 2600000.0, 29, 65000.0, 74, 76, 92, 86, 84, 82),
                new PlayerProfileDto("Harry", "Kane", "STR", 93, "Bayern Monachium", 4200000.0, 31, 100000.0, 75, 96, 88, 84, 48, 86)
            };
        }

        #endregion
    }
}
