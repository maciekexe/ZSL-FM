using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZSLFM.UI
{
    /// <summary>
    /// Pasywny kontroler Menu Głównego i 2-etapowego Kreatora Klubu (M-4 / M-5).
    /// Krok 1: Miejsce w lidze, personalizacja, paleta HSV, herb/PNG, koszulka 2D i Poziom Trudności.
    /// Krok 2: Darmowi Założyciele (z limitem OVR) oraz opcjonalne przejęcie kadry (Średni/Łatwy).
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

        // Krok 1 / Krok 2 Kreatora
        private Button _btnWizardStep1;
        private Button _btnWizardStep2;
        private VisualElement _creatorStep1Panel;
        private VisualElement _creatorStep2Panel;
        private Button _btnNextCreatorStep;
        private Button _btnPrevCreatorStep;

        // Pola personalizacji i ligi (Krok 1)
        private ScrollView _existingClubsList;
        private VisualElement _replacedClubChips;
        private VisualElement _rivalClubChips;
        private TextField _inputClubName;
        private TextField _inputClubMonogram;
        private TextField _inputClubCity;
        private TextField _inputClubNickname;
        private TextField _inputClubStadium;
        private TextField _inputCustomLogoPath;
        private Button _btnLoadLogoPng;
        private Button _btnResetCustomLogo;
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

        // Krok 2: Podsumowanie trudności, przejęcie kadry i Założyciele
        private Label _lblStep2DifficultySummary;
        private Label _lblStep2SquadInfo;
        private VisualElement _keepReplacedBox;
        private Toggle _toggleKeepReplacedSquad;
        private Label _lblFoundersCount;
        private TextField _inputCustomPlayerFirst;
        private TextField _inputCustomPlayerLast;
        private SliderInt _sliderCustomPlayerOvr;
        private Label _lblCustomPlayerOvr;
        private Button _btnAddCustomPlayer;
        private VisualElement _customPlayersList;

        // Słowniki przycisków wyboru
        private readonly Dictionary<CrestShape, Button> _shapeButtons = new Dictionary<CrestShape, Button>();
        private readonly Dictionary<CrestPattern, Button> _patternButtons = new Dictionary<CrestPattern, Button>();
        private readonly Dictionary<JerseyPattern, Button> _jerseyButtons = new Dictionary<JerseyPattern, Button>();
        private readonly Dictionary<string, Button> _positionButtons = new Dictionary<string, Button>();
        private readonly List<VisualElement> _existingClubCards = new List<VisualElement>();
        private readonly List<VisualElement> _tierCards = new List<VisualElement>();
        private readonly List<Button> _replacedChipButtons = new List<Button>();
        private readonly List<Button> _rivalChipButtons = new List<Button>();
        private readonly List<CustomPlayerDraft> _founders = new List<CustomPlayerDraft>();

        // Podgląd na żywo (prawa kolumna)
        private VisualElement _previewCrest;
        private VisualElement _previewJerseyBox;
        private Label _previewClubName;
        private Label _previewClubType;
        private Label _previewMetaLine;
        private Label _previewBudget;
        private Label _previewReputation;
        private Label _previewDescription;
        private Button _btnStartCareer;

        // Bieżący stan formularza kreatora
        private bool _isCustomMode;
        private bool _isEditingAccentColor;
        private RgbColor _primaryColor = new RgbColor(118, 22, 14);
        private RgbColor _accentColor = new RgbColor(201, 31, 16);
        private ExistingClubPreset _selectedExistingPreset;
        private ExistingClubPreset _selectedReplacedClub;
        private ExistingClubPreset _selectedRivalClub;
        private ClubColorPalette _selectedPalette;
        private CrestShape _selectedShape = CrestShape.Shield;
        private CrestPattern _selectedPattern = CrestPattern.VerticalSplit;
        private JerseyPattern _selectedJerseyPattern = JerseyPattern.Solid;
        private StarterBudgetTier _selectedBudgetTier;
        private string _selectedFounderPos = "MID";
        private Texture2D _customUploadedLogoTexture;
        private string _customLogoFilePath = string.Empty;

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
            _selectedReplacedClub = null;
            _selectedRivalClub = ClubCreatorCatalog.ExistingClubs[1];
            _selectedPalette = ClubPaletteBuilder.BuildCustomPalette(_primaryColor, _accentColor);
            _selectedBudgetTier = ClubCreatorCatalog.BudgetTiers[0]; // Domyślnie Hardcore (0 PLN)

            InitializeElements(root);
            SetupMainNavigation();
            SetupModeSwitcher();
            SetupWizardNavigation();
            BuildExistingClubsList();
            BuildLeagueAndRivalChips();
            SetupCustomCreatorInputs();
            SetupColorPickerControls();
            SetupCrestAndJerseyButtons();
            SetupCustomLogoUpload();
            BuildDifficultyTiers();
            SetupFoundersStep();

            if (_btnStartCareer != null)
            {
                _btnStartCareer.clicked += HandleStartCareer;
            }

            RefreshColorSlotsUi();
            _colorPicker.SetColor(_primaryColor, notify: false);
            RefreshContinueButtonState();
            RefreshStep2DifficultyUi();
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

            DisposeCustomUploadedTexture();
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

            _btnWizardStep1 = root.Q<Button>("BtnWizardStep1");
            _btnWizardStep2 = root.Q<Button>("BtnWizardStep2");
            _creatorStep1Panel = root.Q<VisualElement>("CreatorStep1Panel");
            _creatorStep2Panel = root.Q<VisualElement>("CreatorStep2Panel");
            _btnNextCreatorStep = root.Q<Button>("BtnNextCreatorStep");
            _btnPrevCreatorStep = root.Q<Button>("BtnPrevCreatorStep");

            _existingClubsList = root.Q<ScrollView>("ExistingClubsList");
            _replacedClubChips = root.Q<VisualElement>("ReplacedClubChips");
            _rivalClubChips = root.Q<VisualElement>("RivalClubChips");
            _inputClubName = root.Q<TextField>("InputClubName");
            _inputClubMonogram = root.Q<TextField>("InputClubMonogram");
            _inputClubCity = root.Q<TextField>("InputClubCity");
            _inputClubNickname = root.Q<TextField>("InputClubNickname");
            _inputClubStadium = root.Q<TextField>("InputClubStadium");
            _inputCustomLogoPath = root.Q<TextField>("InputCustomLogoPath");
            _btnLoadLogoPng = root.Q<Button>("BtnLoadLogoPng");
            _btnResetCustomLogo = root.Q<Button>("BtnResetCustomLogo");
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

            _lblStep2DifficultySummary = root.Q<Label>("LblStep2DifficultySummary");
            _lblStep2SquadInfo = root.Q<Label>("LblStep2SquadInfo");
            _keepReplacedBox = root.Q<VisualElement>("KeepReplacedBox");
            _toggleKeepReplacedSquad = root.Q<Toggle>("ToggleKeepReplacedSquad");
            _lblFoundersCount = root.Q<Label>("LblFoundersCount");
            _inputCustomPlayerFirst = root.Q<TextField>("InputCustomPlayerFirst");
            _inputCustomPlayerLast = root.Q<TextField>("InputCustomPlayerLast");
            _sliderCustomPlayerOvr = root.Q<SliderInt>("SliderCustomPlayerOvr");
            _lblCustomPlayerOvr = root.Q<Label>("LblCustomPlayerOvr");
            _btnAddCustomPlayer = root.Q<Button>("BtnAddCustomPlayer");
            _customPlayersList = root.Q<VisualElement>("CustomPlayersList");

            _shapeButtons.Clear();
            RegisterMapButton(_shapeButtons, root.Q<Button>("BtnShapeShield"), CrestShape.Shield);
            RegisterMapButton(_shapeButtons, root.Q<Button>("BtnShapeRounded"), CrestShape.RoundedSquare);
            RegisterMapButton(_shapeButtons, root.Q<Button>("BtnShapeCircle"), CrestShape.Circle);
            RegisterMapButton(_shapeButtons, root.Q<Button>("BtnShapeSharp"), CrestShape.Sharp);

            _patternButtons.Clear();
            RegisterMapButton(_patternButtons, root.Q<Button>("BtnPatternSolid"), CrestPattern.Solid);
            RegisterMapButton(_patternButtons, root.Q<Button>("BtnPatternSplit"), CrestPattern.VerticalSplit);
            RegisterMapButton(_patternButtons, root.Q<Button>("BtnPatternBand"), CrestPattern.HorizontalBand);
            RegisterMapButton(_patternButtons, root.Q<Button>("BtnPatternDiagonal"), CrestPattern.DiagonalStripe);

            _jerseyButtons.Clear();
            RegisterMapButton(_jerseyButtons, root.Q<Button>("BtnJerseySolid"), JerseyPattern.Solid);
            RegisterMapButton(_jerseyButtons, root.Q<Button>("BtnJerseyStripes"), JerseyPattern.VerticalStripes);
            RegisterMapButton(_jerseyButtons, root.Q<Button>("BtnJerseyHoop"), JerseyPattern.HorizontalHoop);
            RegisterMapButton(_jerseyButtons, root.Q<Button>("BtnJerseySash"), JerseyPattern.DiagonalSash);
            RegisterMapButton(_jerseyButtons, root.Q<Button>("BtnJerseySleeves"), JerseyPattern.ContrastSleeves);

            _positionButtons.Clear();
            RegisterMapButton(_positionButtons, root.Q<Button>("BtnPosGK"), "GK");
            RegisterMapButton(_positionButtons, root.Q<Button>("BtnPosDEF"), "DEF");
            RegisterMapButton(_positionButtons, root.Q<Button>("BtnPosMID"), "MID");
            RegisterMapButton(_positionButtons, root.Q<Button>("BtnPosSTR"), "STR");

            _previewCrest = root.Q<VisualElement>("PreviewCrest");
            _previewJerseyBox = root.Q<VisualElement>("PreviewJerseyBox");
            _previewClubName = root.Q<Label>("PreviewClubName");
            _previewClubType = root.Q<Label>("PreviewClubType");
            _previewMetaLine = root.Q<Label>("PreviewMetaLine");
            _previewBudget = root.Q<Label>("PreviewBudget");
            _previewReputation = root.Q<Label>("PreviewReputation");
            _previewDescription = root.Q<Label>("PreviewDescription");
            _btnStartCareer = root.Q<Button>("BtnStartCareer");
        }

        private static void RegisterMapButton<TKey>(Dictionary<TKey, Button> map, Button button, TKey key)
        {
            if (button != null) map[key] = button;
        }

        private void SetupMainNavigation()
        {
            if (_btnNavNewCareer != null) _btnNavNewCareer.clicked += () => ShowMainPanel(showNewCareer: true);
            if (_btnNavSettings != null) _btnNavSettings.clicked += () => ShowMainPanel(showNewCareer: false);
            if (_btnNavContinue != null)
            {
                _btnNavContinue.clicked += () =>
                {
                    if (ClubSessionState.HasActiveCareer) LoadDashboardScene();
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
            ToggleHidden(_newCareerPanel, !showNewCareer);
            ToggleHidden(_settingsPanel, showNewCareer);

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
            if (_btnModeExisting != null) _btnModeExisting.clicked += () => SwitchCareerMode(customMode: false);
            if (_btnModeCustom != null) _btnModeCustom.clicked += () => SwitchCareerMode(customMode: true);
        }

        private void SwitchCareerMode(bool customMode)
        {
            _isCustomMode = customMode;
            HideCreatorError();

            ToggleHidden(_existingClubSection, customMode);
            ToggleHidden(_customClubSection, !customMode);

            if (_btnModeExisting != null && _btnModeCustom != null)
            {
                if (customMode)
                {
                    _btnModeCustom.AddToClassList("mode-tab--active");
                    _btnModeExisting.RemoveFromClassList("mode-tab--active");
                    SetWizardStep(step: 1);
                }
                else
                {
                    _btnModeExisting.AddToClassList("mode-tab--active");
                    _btnModeCustom.RemoveFromClassList("mode-tab--active");
                }
            }

            RefreshPreview();
        }

        private void SetupWizardNavigation()
        {
            if (_btnWizardStep1 != null) _btnWizardStep1.clicked += () => SetWizardStep(1);
            if (_btnWizardStep2 != null) _btnWizardStep2.clicked += TryAdvanceToStep2;
            if (_btnNextCreatorStep != null) _btnNextCreatorStep.clicked += TryAdvanceToStep2;
            if (_btnPrevCreatorStep != null) _btnPrevCreatorStep.clicked += () => SetWizardStep(1);
        }

        private void TryAdvanceToStep2()
        {
            string rawName = _inputClubName != null ? _inputClubName.value : string.Empty;
            string normalized = ClubCreatorCatalog.NormalizeClubName(rawName);
            if (normalized.Length < 2)
            {
                ShowCreatorError("Podaj nazwę swojego klubu (min. 2 znaki), zanim przejdziesz do Założycieli.");
                return;
            }

            HideCreatorError();
            SetWizardStep(2);
        }

        private void SetWizardStep(int step)
        {
            bool showStep1 = step == 1;
            ToggleHidden(_creatorStep1Panel, !showStep1);
            ToggleHidden(_creatorStep2Panel, showStep1);

            if (_btnWizardStep1 != null && _btnWizardStep2 != null)
            {
                if (showStep1)
                {
                    _btnWizardStep1.AddToClassList("wizard-step-tab--active");
                    _btnWizardStep2.RemoveFromClassList("wizard-step-tab--active");
                }
                else
                {
                    _btnWizardStep2.AddToClassList("wizard-step-tab--active");
                    _btnWizardStep1.RemoveFromClassList("wizard-step-tab--active");
                }
            }

            RefreshStep2DifficultyUi();
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
                    foreach (VisualElement c in _existingClubCards) c.RemoveFromClassList("club-option-card--selected");
                    card.AddToClassList("club-option-card--selected");
                    RefreshPreview();
                });

                _existingClubCards.Add(card);
                _existingClubsList.Add(card);
            }
        }

        private void BuildLeagueAndRivalChips()
        {
            if (_replacedClubChips != null)
            {
                _replacedClubChips.Clear();
                _replacedChipButtons.Clear();

                Button noReplaceBtn = new Button { text = "Nie zastępuj (Nowy klub)" };
                noReplaceBtn.AddToClassList("chip-btn");
                noReplaceBtn.AddToClassList("chip-btn--active");
                noReplaceBtn.clicked += () =>
                {
                    foreach (Button b in _replacedChipButtons) b.RemoveFromClassList("chip-btn--active");
                    noReplaceBtn.AddToClassList("chip-btn--active");
                    _selectedReplacedClub = null;
                    HideCreatorError();
                    RefreshStep2DifficultyUi();
                    RefreshPreview();
                };
                _replacedChipButtons.Add(noReplaceBtn);
                _replacedClubChips.Add(noReplaceBtn);

                foreach (ExistingClubPreset preset in ClubCreatorCatalog.ExistingClubs)
                {
                    Button btn = new Button { text = preset.ClubName };
                    btn.AddToClassList("chip-btn");
                    btn.clicked += () =>
                    {
                        foreach (Button b in _replacedChipButtons) b.RemoveFromClassList("chip-btn--active");
                        btn.AddToClassList("chip-btn--active");
                        _selectedReplacedClub = preset;
                        HideCreatorError();
                        RefreshStep2DifficultyUi();
                        RefreshPreview();
                    };
                    _replacedChipButtons.Add(btn);
                    _replacedClubChips.Add(btn);
                }
            }

            BuildClubChipGroup(_rivalClubChips, _rivalChipButtons, _selectedRivalClub, preset =>
            {
                _selectedRivalClub = preset;
                RefreshPreview();
            });
        }

        private static void BuildClubChipGroup(
            VisualElement container,
            List<Button> buttonList,
            ExistingClubPreset initialSelection,
            System.Action<ExistingClubPreset> onSelect)
        {
            if (container == null) return;
            container.Clear();
            buttonList.Clear();

            foreach (ExistingClubPreset preset in ClubCreatorCatalog.ExistingClubs)
            {
                Button btn = new Button { text = preset.ClubName };
                btn.AddToClassList("chip-btn");
                if (preset == initialSelection) btn.AddToClassList("chip-btn--active");

                btn.clicked += () =>
                {
                    foreach (Button b in buttonList) b.RemoveFromClassList("chip-btn--active");
                    btn.AddToClassList("chip-btn--active");
                    onSelect(preset);
                };

                buttonList.Add(btn);
                container.Add(btn);
            }
        }

        private void SetupCustomCreatorInputs()
        {
            RegisterRefreshOnInput(_inputClubName);
            RegisterRefreshOnInput(_inputClubMonogram);
            RegisterRefreshOnInput(_inputClubCity);
            RegisterRefreshOnInput(_inputClubNickname);
            RegisterRefreshOnInput(_inputClubStadium);
        }

        private void RegisterRefreshOnInput(TextField field)
        {
            if (field == null) return;
            field.RegisterValueChangedCallback(_ =>
            {
                HideCreatorError();
                RefreshPreview();
            });
        }

        private void SetupColorPickerControls()
        {
            if (_btnSelectPrimaryColor != null) _btnSelectPrimaryColor.clicked += () => SelectColorSlot(editAccent: false);
            if (_btnSelectAccentColor != null) _btnSelectAccentColor.clicked += () => SelectColorSlot(editAccent: true);

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
            if (_swatchPrimaryColor != null) _swatchPrimaryColor.style.backgroundColor = ClubThemeMapper.ToUnityColor(_primaryColor);
            if (_swatchAccentColor != null) _swatchAccentColor.style.backgroundColor = ClubThemeMapper.ToUnityColor(_accentColor);
            if (_hexPrimaryColor != null) _hexPrimaryColor.text = ClubPaletteBuilder.ToHex(_primaryColor);
            if (_hexAccentColor != null) _hexAccentColor.text = ClubPaletteBuilder.ToHex(_accentColor);
        }

        private void SetupCrestAndJerseyButtons()
        {
            BindChipDictionary(_shapeButtons, shape =>
            {
                _selectedShape = shape;
                RefreshPreview();
            });

            BindChipDictionary(_patternButtons, pattern =>
            {
                _selectedPattern = pattern;
                RefreshPreview();
            });

            BindChipDictionary(_jerseyButtons, jersey =>
            {
                _selectedJerseyPattern = jersey;
                RefreshPreview();
            });
        }

        private static void BindChipDictionary<TKey>(Dictionary<TKey, Button> map, System.Action<TKey> onSelect)
        {
            foreach (KeyValuePair<TKey, Button> pair in map)
            {
                TKey capturedKey = pair.Key;
                pair.Value.clicked += () =>
                {
                    foreach (Button b in map.Values) b.RemoveFromClassList("chip-btn--active");
                    pair.Value.AddToClassList("chip-btn--active");
                    onSelect(capturedKey);
                };
            }
        }

        private void SetupCustomLogoUpload()
        {
            if (_btnLoadLogoPng != null) _btnLoadLogoPng.clicked += HandleLoadCustomLogo;

            if (_btnResetCustomLogo != null)
            {
                _btnResetCustomLogo.clicked += () =>
                {
                    _customLogoFilePath = string.Empty;
                    if (_inputCustomLogoPath != null) _inputCustomLogoPath.value = string.Empty;
                    DisposeCustomUploadedTexture();
                    HideCreatorError();
                    RefreshPreview();
                };
            }
        }

        private void HandleLoadCustomLogo()
        {
            string path = _inputCustomLogoPath != null ? _inputCustomLogoPath.value : string.Empty;

#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(path))
            {
                path = EditorUtility.OpenFilePanel("Wybierz własny herb klubu (PNG / JPG)", "", "png,jpg,jpeg");
                if (string.IsNullOrWhiteSpace(path)) return;
                if (_inputCustomLogoPath != null) _inputCustomLogoPath.value = path;
            }
#endif

            if (!CustomLogoLoader.TryLoadTextureFromFile(path, out Texture2D loadedTex, out string error))
            {
                ShowCreatorError(error);
                return;
            }

            DisposeCustomUploadedTexture();
            _customUploadedLogoTexture = loadedTex;
            _customLogoFilePath = path.Trim().Trim('"');
            HideCreatorError();
            RefreshPreview();
        }

        private void DisposeCustomUploadedTexture()
        {
            if (_customUploadedLogoTexture != null)
            {
                Destroy(_customUploadedLogoTexture);
                _customUploadedLogoTexture = null;
            }
        }

        private void BuildDifficultyTiers()
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

                VisualElement titleGroup = new VisualElement();
                titleGroup.AddToClassList("tier-title-group");

                Label badgeLabel = new Label(tier.DifficultyLabel);
                badgeLabel.AddToClassList("tier-badge");
                Label titleLabel = new Label(tier.Title);
                titleLabel.AddToClassList("tier-title");

                titleGroup.Add(badgeLabel);
                titleGroup.Add(titleLabel);

                Label budgetLabel = new Label($"{tier.Budget:N0} PLN");
                budgetLabel.AddToClassList("tier-budget");

                topRow.Add(titleGroup);
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

                    // Usuwamy założycieli przekraczających nowy limit OVR przy obniżeniu trudności
                    _founders.RemoveAll(f => f.OverallSkill > tier.MaxFounderOvr);
                    RenderFoundersList();
                    RefreshStep2DifficultyUi();
                    RefreshPreview();
                });

                _tierCards.Add(card);
                _budgetTiersContainer.Add(card);
            }
        }

        private void SetupFoundersStep()
        {
            BindChipDictionary(_positionButtons, pos => _selectedFounderPos = pos);

            if (_sliderCustomPlayerOvr != null)
            {
                _sliderCustomPlayerOvr.RegisterValueChangedCallback(evt => UpdateFounderSliderLabel(evt.newValue));
            }

            if (_btnAddCustomPlayer != null)
            {
                _btnAddCustomPlayer.clicked += HandleAddFounder;
            }
        }

        private void RefreshStep2DifficultyUi()
        {
            StarterBudgetTier tier = _selectedBudgetTier ?? ClubCreatorCatalog.BudgetTiers[0];

            if (_lblStep2DifficultySummary != null)
            {
                _lblStep2DifficultySummary.text = $"{tier.DifficultyLabel} — {tier.Title} ({tier.Budget:N0} PLN)";
            }

            if (_lblStep2SquadInfo != null)
            {
                _lblStep2SquadInfo.text = $"{tier.Description} Możesz dodać za darmo do {ClubStarterSquadBuilder.MaxFoundersCount} Założycieli (maks. OVR {tier.MaxFounderOvr}).";
            }

            bool canKeepReplaced = tier.AllowsReplacedClubSquad && _selectedReplacedClub != null;
            ToggleHidden(_keepReplacedBox, !canKeepReplaced);
            if (!canKeepReplaced && _toggleKeepReplacedSquad != null)
            {
                _toggleKeepReplacedSquad.value = false;
            }
            else if (canKeepReplaced && _toggleKeepReplacedSquad != null)
            {
                _toggleKeepReplacedSquad.label = $"Przejmij kadrę klubu {_selectedReplacedClub.ClubName} zamiast nowej (tylko Średni/Łatwy)";
            }

            if (_sliderCustomPlayerOvr != null)
            {
                _sliderCustomPlayerOvr.lowValue = ClubStarterSquadBuilder.MinOverall;
                _sliderCustomPlayerOvr.highValue = tier.MaxFounderOvr;
                if (_sliderCustomPlayerOvr.value > tier.MaxFounderOvr)
                {
                    _sliderCustomPlayerOvr.value = tier.MaxFounderOvr;
                }
                UpdateFounderSliderLabel(_sliderCustomPlayerOvr.value);
            }

            UpdateFoundersCountLabel();
        }

        private void UpdateFounderSliderLabel(int ovr)
        {
            StarterBudgetTier tier = _selectedBudgetTier ?? ClubCreatorCatalog.BudgetTiers[0];
            if (_lblCustomPlayerOvr != null)
            {
                _lblCustomPlayerOvr.text = $"OVR: {ovr} (maks. {tier.MaxFounderOvr})";
            }
        }

        private void HandleAddFounder()
        {
            StarterBudgetTier tier = _selectedBudgetTier ?? ClubCreatorCatalog.BudgetTiers[0];
            int ovr = _sliderCustomPlayerOvr != null ? _sliderCustomPlayerOvr.value : ClubStarterSquadBuilder.MinOverall;
            string first = _inputCustomPlayerFirst != null ? _inputCustomPlayerFirst.value : string.Empty;
            string last = _inputCustomPlayerLast != null ? _inputCustomPlayerLast.value : string.Empty;

            if (!ClubStarterSquadBuilder.TryCreateFounderDraft(
                firstName: first,
                lastName: last,
                position: _selectedFounderPos,
                overallSkill: ovr,
                maxAllowedOvr: tier.MaxFounderOvr,
                currentFounderCount: _founders.Count,
                draft: out CustomPlayerDraft draft,
                validationError: out string error))
            {
                ShowCreatorError(error);
                return;
            }

            _founders.Add(draft);
            if (_inputCustomPlayerFirst != null) _inputCustomPlayerFirst.value = string.Empty;
            if (_inputCustomPlayerLast != null) _inputCustomPlayerLast.value = string.Empty;

            HideCreatorError();
            RenderFoundersList();
        }

        private void RenderFoundersList()
        {
            if (_customPlayersList == null) return;
            _customPlayersList.Clear();

            for (int i = 0; i < _founders.Count; i++)
            {
                int index = i;
                CustomPlayerDraft draft = _founders[i];

                VisualElement row = new VisualElement();
                row.AddToClassList("custom-player-row");

                Label info = new Label($"{draft.Position} • {draft.FullName} • OVR {draft.OverallSkill} (Założyciel)");
                info.AddToClassList("custom-player-info");

                Button removeBtn = new Button { text = "Usuń" };
                removeBtn.AddToClassList("custom-player-remove-btn");
                removeBtn.clicked += () =>
                {
                    _founders.RemoveAt(index);
                    RenderFoundersList();
                };

                row.Add(info);
                row.Add(removeBtn);
                _customPlayersList.Add(row);
            }

            UpdateFoundersCountLabel();
        }

        private void UpdateFoundersCountLabel()
        {
            if (_lblFoundersCount != null)
            {
                _lblFoundersCount.text = $"Dodano: {_founders.Count} / {ClubStarterSquadBuilder.MaxFoundersCount}";
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
                    metaLine: string.Empty,
                    budget: preset.Budget,
                    reputation: preset.Reputation,
                    description: preset.Description,
                    palette: preset.Palette,
                    shape: preset.CrestShape,
                    pattern: preset.CrestPattern,
                    jerseyPattern: JerseyPattern.Solid,
                    monogram: preset.Monogram,
                    useDefaultZslLogo: preset.UsesDefaultZslLogo,
                    customTex: null);
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

            string city = _inputClubCity != null && !string.IsNullOrWhiteSpace(_inputClubCity.value)
                ? _inputClubCity.value.Trim()
                : "Gdańsk";
            string stadium = _inputClubStadium != null && !string.IsNullOrWhiteSpace(_inputClubStadium.value)
                ? _inputClubStadium.value.Trim()
                : $"Stadion {previewName}";
            string leagueSlotText = _selectedReplacedClub != null
                ? $"Zastępuje: {_selectedReplacedClub.ClubName}"
                : "Nowy zespół w lidze";
            string rivalName = _selectedRivalClub != null ? _selectedRivalClub.ClubName : "Lechia Gdańsk";

            ApplyPreviewValues(
                clubName: previewName,
                clubTypeText: $"{tier.DifficultyLabel} • {leagueSlotText}",
                metaLine: $"{city} • {stadium}\nRywal: {rivalName}",
                budget: tier.Budget,
                reputation: tier.Reputation,
                description: tier.Description,
                palette: _selectedPalette,
                shape: _selectedShape,
                pattern: _selectedPattern,
                jerseyPattern: _selectedJerseyPattern,
                monogram: monogram,
                useDefaultZslLogo: false,
                customTex: _customUploadedLogoTexture);
        }

        private void ApplyPreviewValues(
            string clubName,
            string clubTypeText,
            string metaLine,
            double budget,
            int reputation,
            string description,
            ClubColorPalette palette,
            CrestShape shape,
            CrestPattern pattern,
            JerseyPattern jerseyPattern,
            string monogram,
            bool useDefaultZslLogo,
            Texture2D customTex)
        {
            if (_previewClubName != null) _previewClubName.text = clubName;
            if (_previewClubType != null) _previewClubType.text = clubTypeText;
            if (_previewMetaLine != null)
            {
                _previewMetaLine.text = metaLine;
                ToggleHidden(_previewMetaLine, string.IsNullOrWhiteSpace(metaLine));
            }
            if (_previewBudget != null) _previewBudget.text = $"{budget:N0} PLN";
            if (_previewReputation != null) _previewReputation.text = reputation.ToString();
            if (_previewDescription != null) _previewDescription.text = description;

            ClubCrestRenderer.Render(_previewCrest, shape, pattern, palette, monogram, useDefaultZslLogo, customUploadedTexture: customTex);
            ClubJerseyRenderer.Render(_previewJerseyBox, palette, jerseyPattern);
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
            else
            {
                StarterBudgetTier tier = _selectedBudgetTier ?? ClubCreatorCatalog.BudgetTiers[0];
                bool keepReplaced = tier.AllowsReplacedClubSquad &&
                                    _selectedReplacedClub != null &&
                                    _toggleKeepReplacedSquad != null &&
                                    _toggleKeepReplacedSquad.value;

                if (!ClubCreatorCatalog.TryBuildCustomClub(
                    rawClubName: _inputClubName != null ? _inputClubName.value : string.Empty,
                    rawMonogram: _inputClubMonogram != null ? _inputClubMonogram.value : string.Empty,
                    palette: _selectedPalette,
                    crestShape: _selectedShape,
                    crestPattern: _selectedPattern,
                    jerseyPattern: _selectedJerseyPattern,
                    budgetTier: tier,
                    city: _inputClubCity != null ? _inputClubCity.value : string.Empty,
                    nickname: _inputClubNickname != null ? _inputClubNickname.value : string.Empty,
                    stadiumName: _inputClubStadium != null ? _inputClubStadium.value : string.Empty,
                    replacedClub: _selectedReplacedClub,
                    rivalClub: _selectedRivalClub,
                    customLogoFilePath: _customLogoFilePath,
                    keepReplacedClubSquad: keepReplaced,
                    founders: _founders,
                    selection: out selection,
                    validationError: out string error))
                {
                    ShowCreatorError(error);
                    return;
                }
            }

            ClubSessionState.SetActiveSelection(selection);
            Debug.Log($"[MainMenu] Rozpoczęto karierę klubem: {selection.ClubName} (Trudność: {selection.DifficultyType}, Budżet: {selection.RemainingBudget:N0} PLN, Kadra: {selection.StartingSquad?.Count ?? 0} graczy).");
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

        private static void ToggleHidden(VisualElement element, bool hide)
        {
            if (element == null) return;
            if (hide) element.AddToClassList("hidden");
            else element.RemoveFromClassList("hidden");
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
