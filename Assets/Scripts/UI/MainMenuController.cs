using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Pasywny kontroler Menu Głównego i Kreatora Klubu (M-4 / M-5).
    /// Obsługuje nawigację, wybór gotowego klubu, kreator własnego klubu z paletą HSV na żywo
    /// oraz przejście do sceny Dashboardu (SampleScene).
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string dashboardSceneName = "SampleScene";

        private UIDocument _uiDocument;
        private ClubThemeApplier _themeApplier;
        private HsvColorPickerView _colorPicker;

        // Nawigacja główna
        private Button _btnNavNewCareer;
        private Button _btnNavContinue;
        private Button _btnNavSettings;
        private Button _btnNavQuit;
        private VisualElement _newCareerPanel;
        private VisualElement _settingsPanel;

        // Przełącznik trybu Nowej Kariery
        private Button _btnModeExisting;
        private Button _btnModeCustom;
        private VisualElement _existingClubSection;
        private VisualElement _customClubSection;

        // Listy i pola kreatora
        private ScrollView _existingClubsList;
        private TextField _inputClubName;
        private TextField _inputClubMonogram;
        private VisualElement _budgetTiersContainer;
        private Label _creatorErrorLabel;

        // Paleta kolorów (Wariant B)
        private Button _btnSelectPrimaryColor;
        private Button _btnSelectAccentColor;
        private Button _btnSwapColors;
        private VisualElement _swatchPrimaryColor;
        private VisualElement _swatchAccentColor;
        private Label _hexPrimaryColor;
        private Label _hexAccentColor;
        private VisualElement _quickSwatchesGrid;

        // Przyciski kształtu i wzoru herbu
        private readonly Dictionary<CrestShape, Button> _shapeButtons = new Dictionary<CrestShape, Button>();
        private readonly Dictionary<CrestPattern, Button> _patternButtons = new Dictionary<CrestPattern, Button>();
        private readonly List<VisualElement> _existingClubCards = new List<VisualElement>();
        private readonly List<VisualElement> _tierCards = new List<VisualElement>();

        // Podgląd na żywo (prawa kolumna)
        private VisualElement _previewCrest;
        private Label _previewClubName;
        private Label _previewClubType;
        private Label _previewBudget;
        private Label _previewReputation;
        private Label _previewDescription;
        private Button _btnStartCareer;

        // Bieżący wybór UI
        private bool _isCustomMode;
        private bool _isEditingAccentColor;
        private RgbColor _primaryColor = new RgbColor(118, 22, 14);
        private RgbColor _accentColor = new RgbColor(201, 31, 16);
        private ExistingClubPreset _selectedExistingPreset;
        private ClubColorPalette _selectedPalette;
        private CrestShape _selectedShape = CrestShape.Shield;
        private CrestPattern _selectedPattern = CrestPattern.VerticalSplit;
        private StarterBudgetTier _selectedBudgetTier;

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                Debug.LogWarning("[MainMenuController] Komponent UIDocument nie posiada jeszcze zainicjalizowanego VisualElement.");
                return;
            }

            VisualElement root = _uiDocument.rootVisualElement;
            _themeApplier = new ClubThemeApplier(root);
            _colorPicker = new HsvColorPickerView(root);
            _colorPicker.ColorChanged += OnPickerColorChanged;

            _selectedExistingPreset = ClubCreatorCatalog.ExistingClubs[0];
            _selectedPalette = ClubPaletteBuilder.BuildCustomPalette(_primaryColor, _accentColor);
            _selectedBudgetTier = ClubCreatorCatalog.BudgetTiers[0];

            InitializeElements(root);
            SetupMainNavigation();
            SetupModeSwitcher();
            BuildExistingClubsList();
            SetupCustomCreatorInputs();
            SetupColorPickerControls();
            SetupShapeAndPatternButtons();
            BuildBudgetTiers();

            if (_btnStartCareer != null)
            {
                _btnStartCareer.clicked += HandleStartCareer;
            }

            RefreshColorSlotsUi();
            _colorPicker.SetColor(_primaryColor, notify: false);
            RefreshContinueButtonState();
            RefreshPreview();
        }

        private void OnDisable()
        {
            if (_colorPicker != null)
            {
                _colorPicker.ColorChanged -= OnPickerColorChanged;
                _colorPicker.Dispose();
                _colorPicker = null;
            }

            _themeApplier?.Dispose();
            _themeApplier = null;
        }

        private void InitializeElements(VisualElement root)
        {
            _btnNavNewCareer = root.Q<Button>("BtnNavNewCareer");
            _btnNavContinue = root.Q<Button>("BtnNavContinue");
            _btnNavSettings = root.Q<Button>("BtnNavSettings");
            _btnNavQuit = root.Q<Button>("BtnNavQuit");
            _newCareerPanel = root.Q<VisualElement>("NewCareerPanel");
            _settingsPanel = root.Q<VisualElement>("SettingsPanel");

            _btnModeExisting = root.Q<Button>("BtnModeExisting");
            _btnModeCustom = root.Q<Button>("BtnModeCustom");
            _existingClubSection = root.Q<VisualElement>("ExistingClubSection");
            _customClubSection = root.Q<VisualElement>("CustomClubSection");

            _existingClubsList = root.Q<ScrollView>("ExistingClubsList");
            _inputClubName = root.Q<TextField>("InputClubName");
            _inputClubMonogram = root.Q<TextField>("InputClubMonogram");
            _budgetTiersContainer = root.Q<VisualElement>("BudgetTiersContainer");
            _creatorErrorLabel = root.Q<Label>("CreatorErrorLabel");

            _btnSelectPrimaryColor = root.Q<Button>("BtnSelectPrimaryColor");
            _btnSelectAccentColor = root.Q<Button>("BtnSelectAccentColor");
            _btnSwapColors = root.Q<Button>("BtnSwapColors");
            _swatchPrimaryColor = root.Q<VisualElement>("SwatchPrimaryColor");
            _swatchAccentColor = root.Q<VisualElement>("SwatchAccentColor");
            _hexPrimaryColor = root.Q<Label>("HexPrimaryColor");
            _hexAccentColor = root.Q<Label>("HexAccentColor");
            _quickSwatchesGrid = root.Q<VisualElement>("QuickSwatchesGrid");

            _shapeButtons.Clear();
            RegisterShapeButton(root.Q<Button>("BtnShapeShield"), CrestShape.Shield);
            RegisterShapeButton(root.Q<Button>("BtnShapeRounded"), CrestShape.RoundedSquare);
            RegisterShapeButton(root.Q<Button>("BtnShapeCircle"), CrestShape.Circle);
            RegisterShapeButton(root.Q<Button>("BtnShapeSharp"), CrestShape.Sharp);

            _patternButtons.Clear();
            RegisterPatternButton(root.Q<Button>("BtnPatternSolid"), CrestPattern.Solid);
            RegisterPatternButton(root.Q<Button>("BtnPatternSplit"), CrestPattern.VerticalSplit);
            RegisterPatternButton(root.Q<Button>("BtnPatternBand"), CrestPattern.HorizontalBand);
            RegisterPatternButton(root.Q<Button>("BtnPatternDiagonal"), CrestPattern.DiagonalStripe);

            _previewCrest = root.Q<VisualElement>("PreviewCrest");
            _previewClubName = root.Q<Label>("PreviewClubName");
            _previewClubType = root.Q<Label>("PreviewClubType");
            _previewBudget = root.Q<Label>("PreviewBudget");
            _previewReputation = root.Q<Label>("PreviewReputation");
            _previewDescription = root.Q<Label>("PreviewDescription");
            _btnStartCareer = root.Q<Button>("BtnStartCareer");
        }

        private void SetupMainNavigation()
        {
            if (_btnNavNewCareer != null)
            {
                _btnNavNewCareer.clicked += () => ShowMainPanel(showNewCareer: true);
            }

            if (_btnNavSettings != null)
            {
                _btnNavSettings.clicked += () => ShowMainPanel(showNewCareer: false);
            }

            if (_btnNavContinue != null)
            {
                _btnNavContinue.clicked += () =>
                {
                    if (!ClubSessionState.HasActiveCareer) return;
                    LoadDashboardScene();
                };
            }

            if (_btnNavQuit != null)
            {
                _btnNavQuit.clicked += () =>
                {
                    Debug.Log("[MainMenu] Zamykanie aplikacji.");
                    Application.Quit();
                };
            }
        }

        private void RefreshContinueButtonState()
        {
            if (_btnNavContinue == null) return;

            bool hasCareer = ClubSessionState.HasActiveCareer;
            _btnNavContinue.SetEnabled(hasCareer);
            if (hasCareer) _btnNavContinue.RemoveFromClassList("menu-btn--disabled");
            else _btnNavContinue.AddToClassList("menu-btn--disabled");
        }

        private void ShowMainPanel(bool showNewCareer)
        {
            if (_newCareerPanel != null)
            {
                if (showNewCareer) _newCareerPanel.RemoveFromClassList("hidden");
                else _newCareerPanel.AddToClassList("hidden");
            }

            if (_settingsPanel != null)
            {
                if (showNewCareer) _settingsPanel.AddToClassList("hidden");
                else _settingsPanel.RemoveFromClassList("hidden");
            }

            if (_btnNavNewCareer != null && _btnNavSettings != null)
            {
                if (showNewCareer)
                {
                    _btnNavNewCareer.AddToClassList("menu-btn--active");
                    _btnNavSettings.RemoveFromClassList("menu-btn--active");
                }
                else
                {
                    _btnNavSettings.AddToClassList("menu-btn--active");
                    _btnNavNewCareer.RemoveFromClassList("menu-btn--active");
                }
            }
        }

        private void SetupModeSwitcher()
        {
            if (_btnModeExisting != null)
            {
                _btnModeExisting.clicked += () => SwitchCareerMode(customMode: false);
            }

            if (_btnModeCustom != null)
            {
                _btnModeCustom.clicked += () => SwitchCareerMode(customMode: true);
            }
        }

        private void SwitchCareerMode(bool customMode)
        {
            _isCustomMode = customMode;
            HideCreatorError();

            if (_existingClubSection != null)
            {
                if (customMode) _existingClubSection.AddToClassList("hidden");
                else _existingClubSection.RemoveFromClassList("hidden");
            }

            if (_customClubSection != null)
            {
                if (customMode) _customClubSection.RemoveFromClassList("hidden");
                else _customClubSection.AddToClassList("hidden");
            }

            if (_btnModeExisting != null && _btnModeCustom != null)
            {
                if (customMode)
                {
                    _btnModeCustom.AddToClassList("mode-tab--active");
                    _btnModeExisting.RemoveFromClassList("mode-tab--active");
                }
                else
                {
                    _btnModeExisting.AddToClassList("mode-tab--active");
                    _btnModeCustom.RemoveFromClassList("mode-tab--active");
                }
            }

            RefreshPreview();
        }

        private void BuildExistingClubsList()
        {
            if (_existingClubsList == null) return;
            _existingClubsList.Clear();
            _existingClubCards.Clear();

            foreach (ExistingClubPreset preset in ClubCreatorCatalog.ExistingClubs)
            {
                VisualElement card = new VisualElement();
                card.AddToClassList("club-option-card");
                if (preset == _selectedExistingPreset) card.AddToClassList("club-option-card--selected");

                VisualElement left = new VisualElement();
                left.AddToClassList("club-option-left");

                VisualElement miniCrest = new VisualElement();
                miniCrest.AddToClassList("club-option-badge");
                ClubCrestRenderer.Render(
                    miniCrest,
                    preset.CrestShape,
                    preset.CrestPattern,
                    preset.Palette,
                    preset.Monogram,
                    preset.UsesDefaultZslLogo);
                left.Add(miniCrest);

                VisualElement textBox = new VisualElement();
                Label nameLabel = new Label(preset.ClubName);
                nameLabel.AddToClassList("club-option-name");
                Label metaLabel = new Label($"Reputacja: {preset.Reputation}");
                metaLabel.AddToClassList("club-option-meta");
                textBox.Add(nameLabel);
                textBox.Add(metaLabel);
                left.Add(textBox);

                Label budgetLabel = new Label($"{preset.Budget:N0} PLN");
                budgetLabel.AddToClassList("club-option-budget");

                card.Add(left);
                card.Add(budgetLabel);

                card.RegisterCallback<ClickEvent>(_ =>
                {
                    _selectedExistingPreset = preset;
                    foreach (VisualElement c in _existingClubCards)
                    {
                        c.RemoveFromClassList("club-option-card--selected");
                    }
                    card.AddToClassList("club-option-card--selected");
                    RefreshPreview();
                });

                _existingClubCards.Add(card);
                _existingClubsList.Add(card);
            }
        }

        private void SetupCustomCreatorInputs()
        {
            if (_inputClubName != null)
            {
                _inputClubName.RegisterValueChangedCallback(_ =>
                {
                    HideCreatorError();
                    RefreshPreview();
                });
            }

            if (_inputClubMonogram != null)
            {
                _inputClubMonogram.RegisterValueChangedCallback(_ => RefreshPreview());
            }
        }

        private void SetupColorPickerControls()
        {
            if (_btnSelectPrimaryColor != null)
            {
                _btnSelectPrimaryColor.clicked += () => SelectColorSlot(editAccent: false);
            }

            if (_btnSelectAccentColor != null)
            {
                _btnSelectAccentColor.clicked += () => SelectColorSlot(editAccent: true);
            }

            if (_btnSwapColors != null)
            {
                _btnSwapColors.clicked += () =>
                {
                    RgbColor prevPrimary = _primaryColor;
                    _primaryColor = _accentColor;
                    _accentColor = prevPrimary;
                    _selectedPalette = ClubPaletteBuilder.BuildCustomPalette(_primaryColor, _accentColor);

                    RefreshColorSlotsUi();
                    _colorPicker?.SetColor(_isEditingAccentColor ? _accentColor : _primaryColor, notify: false);
                    RefreshPreview();
                };
            }

            if (_quickSwatchesGrid != null)
            {
                _quickSwatchesGrid.Clear();
                foreach (RgbColor swatch in ClubPaletteBuilder.QuickSwatches)
                {
                    Button btn = new Button();
                    btn.AddToClassList("quick-swatch-btn");
                    btn.style.backgroundColor = ClubThemeMapper.ToUnityColor(swatch);
                    btn.clicked += () => _colorPicker?.SetColor(swatch, notify: true);
                    _quickSwatchesGrid.Add(btn);
                }
            }
        }

        private void SelectColorSlot(bool editAccent)
        {
            _isEditingAccentColor = editAccent;

            if (_btnSelectPrimaryColor != null && _btnSelectAccentColor != null)
            {
                if (editAccent)
                {
                    _btnSelectAccentColor.AddToClassList("color-slot-btn--active");
                    _btnSelectPrimaryColor.RemoveFromClassList("color-slot-btn--active");
                }
                else
                {
                    _btnSelectPrimaryColor.AddToClassList("color-slot-btn--active");
                    _btnSelectAccentColor.RemoveFromClassList("color-slot-btn--active");
                }
            }

            _colorPicker?.SetColor(editAccent ? _accentColor : _primaryColor, notify: false);
        }

        private void OnPickerColorChanged(RgbColor newColor)
        {
            if (_isEditingAccentColor) _accentColor = newColor;
            else _primaryColor = newColor;

            _selectedPalette = ClubPaletteBuilder.BuildCustomPalette(_primaryColor, _accentColor);
            RefreshColorSlotsUi();
            RefreshPreview();
        }

        private void RefreshColorSlotsUi()
        {
            if (_swatchPrimaryColor != null)
            {
                _swatchPrimaryColor.style.backgroundColor = ClubThemeMapper.ToUnityColor(_primaryColor);
            }

            if (_swatchAccentColor != null)
            {
                _swatchAccentColor.style.backgroundColor = ClubThemeMapper.ToUnityColor(_accentColor);
            }

            if (_hexPrimaryColor != null) _hexPrimaryColor.text = ClubPaletteBuilder.ToHex(_primaryColor);
            if (_hexAccentColor != null) _hexAccentColor.text = ClubPaletteBuilder.ToHex(_accentColor);
        }

        private void SetupShapeAndPatternButtons()
        {
            foreach (KeyValuePair<CrestShape, Button> pair in _shapeButtons)
            {
                CrestShape shape = pair.Key;
                pair.Value.clicked += () =>
                {
                    _selectedShape = shape;
                    foreach (Button b in _shapeButtons.Values) b.RemoveFromClassList("chip-btn--active");
                    pair.Value.AddToClassList("chip-btn--active");
                    RefreshPreview();
                };
            }

            foreach (KeyValuePair<CrestPattern, Button> pair in _patternButtons)
            {
                CrestPattern pattern = pair.Key;
                pair.Value.clicked += () =>
                {
                    _selectedPattern = pattern;
                    foreach (Button b in _patternButtons.Values) b.RemoveFromClassList("chip-btn--active");
                    pair.Value.AddToClassList("chip-btn--active");
                    RefreshPreview();
                };
            }
        }

        private void RegisterShapeButton(Button button, CrestShape shape)
        {
            if (button != null) _shapeButtons[shape] = button;
        }

        private void RegisterPatternButton(Button button, CrestPattern pattern)
        {
            if (button != null) _patternButtons[pattern] = button;
        }

        private void BuildBudgetTiers()
        {
            if (_budgetTiersContainer == null) return;
            _budgetTiersContainer.Clear();
            _tierCards.Clear();

            foreach (StarterBudgetTier tier in ClubCreatorCatalog.BudgetTiers)
            {
                VisualElement card = new VisualElement();
                card.AddToClassList("tier-card");
                if (tier == _selectedBudgetTier) card.AddToClassList("tier-card--active");

                VisualElement topRow = new VisualElement();
                topRow.AddToClassList("tier-top");

                Label titleLabel = new Label(tier.Title);
                titleLabel.AddToClassList("tier-title");
                Label budgetLabel = new Label($"{tier.Budget:N0} PLN");
                budgetLabel.AddToClassList("tier-budget");

                topRow.Add(titleLabel);
                topRow.Add(budgetLabel);

                Label descLabel = new Label(tier.Description);
                descLabel.AddToClassList("tier-desc");

                card.Add(topRow);
                card.Add(descLabel);

                card.RegisterCallback<ClickEvent>(_ =>
                {
                    _selectedBudgetTier = tier;
                    foreach (VisualElement t in _tierCards) t.RemoveFromClassList("tier-card--active");
                    card.AddToClassList("tier-card--active");
                    RefreshPreview();
                });

                _tierCards.Add(card);
                _budgetTiersContainer.Add(card);
            }
        }

        private void RefreshPreview()
        {
            if (!_isCustomMode)
            {
                ExistingClubPreset preset = _selectedExistingPreset ?? ClubCreatorCatalog.ExistingClubs[0];
                ApplyPreviewValues(
                    clubName: preset.ClubName,
                    clubTypeText: "Istniejący Klub",
                    budget: preset.Budget,
                    reputation: preset.Reputation,
                    description: preset.Description,
                    palette: preset.Palette,
                    shape: preset.CrestShape,
                    pattern: preset.CrestPattern,
                    monogram: preset.Monogram,
                    useDefaultZslLogo: preset.UsesDefaultZslLogo);
                return;
            }

            string rawName = _inputClubName != null ? _inputClubName.value : string.Empty;
            string previewName = string.IsNullOrWhiteSpace(rawName)
                ? "Nowy Klub"
                : ClubCreatorCatalog.NormalizeClubName(rawName);
            string monogram = ClubCreatorCatalog.NormalizeMonogram(
                _inputClubMonogram != null ? _inputClubMonogram.value : string.Empty,
                previewName);
            StarterBudgetTier tier = _selectedBudgetTier ?? ClubCreatorCatalog.BudgetTiers[0];

            ApplyPreviewValues(
                clubName: previewName,
                clubTypeText: "Własny Klub (Kreator)",
                budget: tier.Budget,
                reputation: tier.Reputation,
                description: tier.Description,
                palette: _selectedPalette,
                shape: _selectedShape,
                pattern: _selectedPattern,
                monogram: monogram,
                useDefaultZslLogo: false);
        }

        private void ApplyPreviewValues(
            string clubName,
            string clubTypeText,
            double budget,
            int reputation,
            string description,
            ClubColorPalette palette,
            CrestShape shape,
            CrestPattern pattern,
            string monogram,
            bool useDefaultZslLogo)
        {
            if (_previewClubName != null) _previewClubName.text = clubName;
            if (_previewClubType != null) _previewClubType.text = clubTypeText;
            if (_previewBudget != null) _previewBudget.text = $"{budget:N0} PLN";
            if (_previewReputation != null) _previewReputation.text = reputation.ToString();
            if (_previewDescription != null) _previewDescription.text = description;

            ClubCrestRenderer.Render(_previewCrest, shape, pattern, palette, monogram, useDefaultZslLogo);
            _themeApplier?.ApplyTheme(ClubThemeMapper.ToClubThemeDto(clubName, palette));

            if (_btnStartCareer != null && palette != null)
            {
                _btnStartCareer.style.backgroundColor = ClubThemeMapper.ToUnityColor(palette.Accent);
            }
        }

        private void HandleStartCareer()
        {
            ClubSetupSelection selection;
            if (!_isCustomMode)
            {
                ExistingClubPreset preset = _selectedExistingPreset ?? ClubCreatorCatalog.ExistingClubs[0];
                selection = ClubCreatorCatalog.CreateFromExistingPreset(preset);
            }
            else if (!ClubCreatorCatalog.TryBuildCustomClub(
                rawClubName: _inputClubName != null ? _inputClubName.value : string.Empty,
                rawMonogram: _inputClubMonogram != null ? _inputClubMonogram.value : string.Empty,
                palette: _selectedPalette,
                crestShape: _selectedShape,
                crestPattern: _selectedPattern,
                budgetTier: _selectedBudgetTier,
                selection: out selection,
                validationError: out string error))
            {
                ShowCreatorError(error);
                return;
            }

            ClubSessionState.SetActiveSelection(selection);
            Debug.Log($"[MainMenu] Rozpoczęto karierę klubem: {selection.ClubName} (Budżet: {selection.Budget:N0} PLN, Reputacja: {selection.Reputation}).");
            LoadDashboardScene();
        }

        private void LoadDashboardScene()
        {
            if (!Application.CanStreamedLevelBeLoaded(dashboardSceneName))
            {
                ShowCreatorError($"Scena '{dashboardSceneName}' nie jest dodana w File -> Build Profiles.");
                return;
            }

            SceneManager.LoadScene(dashboardSceneName);
        }

        private void ShowCreatorError(string message)
        {
            if (_creatorErrorLabel == null) return;
            _creatorErrorLabel.text = message;
            _creatorErrorLabel.RemoveFromClassList("hidden");
        }

        private void HideCreatorError()
        {
            if (_creatorErrorLabel == null) return;
            _creatorErrorLabel.text = string.Empty;
            _creatorErrorLabel.AddToClassList("hidden");
        }
    }
}
