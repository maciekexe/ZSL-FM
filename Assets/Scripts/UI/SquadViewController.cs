using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Pasywny kontroler główny widoku kadry oraz rynku transferowego (SquadView) w Unity UI Toolkit.
    /// Zgodnie z wytycznymi architektonicznymi projektu (AGENTS.md):
    /// - Spina niezależne komponenty UI (ClubThemeApplier, PlayerDetailsDrawerView, SquadListView, TransferMarketView).
    /// - Nie implementuje logiki biznesowej ani silnika meczowego — reaguje wyłącznie na zdarzenia widoków.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SquadViewController : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _clubLogoElement;
        private Label _clubBudgetLabel;
        private Label _clubReputationLabel;
        private Label _sectionTitle;
        private Label _sectionSubtitle;
        private TextField _playerSearchInput;

        private ClubThemeApplier _themeApplier;
        private PlayerDetailsDrawerView _detailsDrawerView;
        private SquadListView _squadListView;
        private TransferMarketView _transferMarketView;

        private readonly List<Button> _navButtons = new List<Button>();
        private Button _activeNavButton;

        private List<PlayerProfileDto> _squadPlayers;
        private List<PlayerProfileDto> _marketPlayers;
        private double _activeClubBudget = UiSampleSquadDataProvider.DefaultClubBudget;
        private Texture2D _customSessionLogoTexture;
        private string _searchQuery = string.Empty;
        private bool _isShowingSquadTab = true;

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                Debug.LogWarning("[SquadViewController] Komponent UIDocument nie posiada jeszcze zainicjalizowanego VisualElement.");
                return;
            }

            VisualElement root = _uiDocument.rootVisualElement;
            InitializeComponents(root);
            SetupNavigation(root);
            SetupSearch(root);

            _marketPlayers = UiSampleSquadDataProvider.CreateSampleMarketPlayers();
            ApplyInitialClubSession();

            RenderSquadList();
            RenderMarketList();
        }

        private void OnDisable()
        {
            if (_customSessionLogoTexture != null)
            {
                Destroy(_customSessionLogoTexture);
                _customSessionLogoTexture = null;
            }

            _themeApplier?.Dispose();
            _themeApplier = null;
        }

        /// <summary>
        /// Nakłada kolory, gradient całego tła oraz logo wybranego klubu na interfejs.
        /// </summary>
        public void ApplyClubTheme(ClubThemeDto theme)
        {
            if (_themeApplier == null || theme == null) return;

            _themeApplier.ApplyTheme(theme);
            _themeApplier.ApplyActiveNavStyle(_navButtons, _activeNavButton);
            _transferMarketView?.RefreshFilterTheme();
        }

        public void RenderSquadList()
        {
            _squadListView?.Render(_squadPlayers, _searchQuery);
        }

        private void RenderMarketList()
        {
            _transferMarketView?.Render(_marketPlayers, _searchQuery);
        }

        private void ApplyInitialClubSession()
        {
            ClubSetupSelection selection = ClubSessionState.ActiveSelection;
            if (selection == null)
            {
                _squadPlayers = UiSampleSquadDataProvider.CreateSampleSquadPlayers();
                _activeClubBudget = UiSampleSquadDataProvider.DefaultClubBudget;
                ApplyClubTheme(ClubThemeApplier.CreateDefaultZslTheme());
                return;
            }

            _squadPlayers = selection.StartingSquad != null
                ? new List<PlayerProfileDto>(selection.StartingSquad)
                : UiSampleSquadDataProvider.CreateSampleSquadPlayers();

            _activeClubBudget = selection.RemainingBudget;
            if (_clubBudgetLabel != null) _clubBudgetLabel.text = $"{selection.RemainingBudget:N0} PLN";
            if (_clubReputationLabel != null) _clubReputationLabel.text = selection.Reputation.ToString();

            if (_sectionSubtitle != null)
            {
                string extra = !string.IsNullOrWhiteSpace(selection.City) && !string.IsNullOrWhiteSpace(selection.StadiumName)
                    ? $"{selection.City} • {selection.StadiumName}"
                    : "Pierwszy Skład i Rezerwy";
                _sectionSubtitle.text = $"{selection.ClubName} • {extra}";
            }

            ApplyClubTheme(ClubThemeMapper.ToClubThemeDto(selection));

            Texture2D customTex = null;
            if (!string.IsNullOrWhiteSpace(selection.CustomLogoFilePath) &&
                CustomLogoLoader.TryLoadTextureFromFile(selection.CustomLogoFilePath, out Texture2D loaded, out _))
            {
                _customSessionLogoTexture = loaded;
                customTex = loaded;
            }

            if (_clubLogoElement != null && !selection.UsesDefaultZslLogo && customTex == null)
            {
                _clubLogoElement.style.width = 44;
                _clubLogoElement.style.height = 44;
            }

            ClubCrestRenderer.Render(_clubLogoElement, selection, customUploadedTexture: customTex);
        }

        private void InitializeComponents(VisualElement root)
        {
            _clubLogoElement = root.Q<VisualElement>("ClubLogo");
            _clubBudgetLabel = root.Q<Label>("ClubBudget");
            _clubReputationLabel = root.Q<Label>("ClubReputation");
            _sectionTitle = root.Q<Label>("SectionTitle");
            _sectionSubtitle = root.Q<Label>("SectionSubtitle");
            _playerSearchInput = root.Q<TextField>("PlayerSearchInput");

            _themeApplier = new ClubThemeApplier(root);
            _detailsDrawerView = new PlayerDetailsDrawerView(root);
            _squadListView = new SquadListView(root);
            _transferMarketView = new TransferMarketView(root, _themeApplier);

            _squadListView.PlayerSelected += OnPlayerSelected;
            _transferMarketView.PlayerSelected += OnPlayerSelected;
            _transferMarketView.BidRequested += OnTransferBidRequested;
        }

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
                btnSquad.clicked += () =>
                {
                    string clubName = _themeApplier?.CurrentTheme != null ? _themeApplier.CurrentTheme.ClubName : "ZSŁ Gdańsk";
                    SwitchTab(btnSquad, "Kadra Zespołu", $"{clubName} • Pierwszy Skład i Rezerwy", showSquad: true, showMarket: false);
                };
            }

            if (btnTransfers != null)
            {
                _navButtons.Add(btnTransfers);
                btnTransfers.clicked += () =>
                    SwitchTab(btnTransfers, "Rynek Transferowy", "Dostępni zawodnicy na rynku transferowym", showSquad: false, showMarket: true);
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
            _themeApplier?.ApplyActiveNavStyle(_navButtons, _activeNavButton);

            if (_sectionTitle != null) _sectionTitle.text = title;
            if (_sectionSubtitle != null) _sectionSubtitle.text = subtitle;

            _isShowingSquadTab = showSquad;
            _squadListView?.SetVisible(showSquad);
            _transferMarketView?.SetVisible(showMarket);

            if (showSquad) RenderSquadList();
            if (showMarket) RenderMarketList();

            _detailsDrawerView?.DeselectCurrentRow();
            _detailsDrawerView?.Hide();

            Debug.Log($"[Łączność FM UI] Przełączono widok na: {title}");
        }

        private void SetupSearch(VisualElement root)
        {
            if (_playerSearchInput == null) return;

            _playerSearchInput.RegisterValueChangedCallback(evt =>
            {
                _searchQuery = (evt.newValue ?? string.Empty).Trim();
                if (_isShowingSquadTab) RenderSquadList();
                else RenderMarketList();
            });
        }

        private void OnPlayerSelected(PlayerProfileDto player, VisualElement row)
        {
            _detailsDrawerView?.Show(player, row, _themeApplier?.CurrentTheme);
        }

        private void OnTransferBidRequested(PlayerProfileDto player)
        {
            TransferBidPreviewResult preview = TransferBidPreviewEvaluator.Evaluate(
                player,
                _activeClubBudget);

            if (preview.IsAffordable)
            {
                Debug.Log($"[Rynek Transferowy] Sukces! Złożono ofertę zakupu za zawodnika {preview.PlayerFullName} ({preview.ClubName}) na kwotę {preview.Price:N0} PLN. Mieści się w budżecie {preview.ClubBudget:N0} PLN! (Pozostałoby w budżecie: {preview.Difference:N0} PLN).");
            }
            else
            {
                Debug.LogWarning($"[Rynek Transferowy] Brak funduszy! Oferta za {preview.PlayerFullName} ({preview.ClubName}) wynosi {preview.Price:N0} PLN i przekracza budżet klubu ({preview.ClubBudget:N0} PLN) o {preview.Difference:N0} PLN!");
            }
        }
    }
}
