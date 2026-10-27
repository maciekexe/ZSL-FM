using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Pasywny kontroler widoku kadry zespołu (SquadView) zrealizowany w Unity UI Toolkit.
    /// Zgodnie z wytycznymi architektonicznymi projektu (AGENTS.md):
    /// - Odpowiada wyłącznie za warstwę prezentacji (UI).
    /// - Nie implementuje mechanik meczowych ani logiki biznesowej (te należą do Core).
    /// - Rejestruje zdarzenia użytkownika i dynamicznie generuje elementy DOM w kontenerze PlayersContainer.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SquadViewController : MonoBehaviour
    {
        /// <summary>
        /// Prosty model widoku (View Model / DTO) dla wiersza zawodnika na liście.
        /// W przyszłości dane te będą zasilane z bazy SQLite lub zdarzeń domeny Core.
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

        private UIDocument _uiDocument;
        private Label _sectionTitle;
        private ScrollView _playersContainer;
        private readonly List<Button> _navButtons = new List<Button>();

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                Debug.LogWarning("[SquadViewController] Komponent UIDocument nie posiada jeszcze zainicjalizowanego VisualElement.");
                return;
            }

            VisualElement root = _uiDocument.rootVisualElement;

            // Pobieramy referencje do kluczowych elementów drzewa UXML
            _sectionTitle = root.Q<Label>("SectionTitle");
            _playersContainer = root.Q<ScrollView>("PlayersContainer");

            // Konfigurujemy interaktywny panel boczny (Sidebar)
            SetupNavigation(root);

            // Generujemy wiersze dla zawodników
            RenderSquadList();
        }

        /// <summary>
        /// Podpina zdarzenia kliknięcia pod przyciski menu bocznego i zarządza aktywnym stanem wizualnym.
        /// </summary>
        private void SetupNavigation(VisualElement root)
        {
            _navButtons.Clear();

            Button btnSquad = root.Q<Button>("BtnSquad");
            Button btnTactics = root.Q<Button>("BtnTactics");
            Button btnTransfers = root.Q<Button>("BtnTransfers");
            Button btnFinances = root.Q<Button>("BtnFinances");

            RegisterNavButton(btnSquad, "Kadra Zespołu", "Otwarto widok: Kadra Zespołu (ZSŁ Gdańsk)");
            RegisterNavButton(btnTactics, "Taktyka Meczowa", "Otwarto widok: Taktyka Meczowa");
            RegisterNavButton(btnTransfers, "Rynek Transferowy", "Otwarto widok: Rynek Transferowy");
            RegisterNavButton(btnFinances, "Budżet i Finanse", "Otwarto widok: Finanse Klubu");
        }

        private void RegisterNavButton(Button button, string sectionTitleText, string logMessage)
        {
            if (button == null) return;

            _navButtons.Add(button);
            button.clicked += () =>
            {
                // Usuwamy klasę aktywnego przycisku z pozostałych i dodajemy do klikniętego
                foreach (Button btn in _navButtons)
                {
                    btn.RemoveFromClassList("nav-button--active");
                }
                button.AddToClassList("nav-button--active");

                // Zmiana tekstu nagłówka w widoku
                if (_sectionTitle != null)
                {
                    _sectionTitle.text = sectionTitleText;
                }

                // Log diagnostyczny w konsoli Unity
                Debug.Log($"[Łączność FM UI] {logMessage}");
            };
        }

        /// <summary>
        /// Czyści kontener i tworzy w pętli elementy VisualElement reprezentujące poszczególnych graczy.
        /// Odpowiednik metody map() generującej listę komponentów w React/JSX.
        /// </summary>
        public void RenderSquadList()
        {
            if (_playersContainer == null)
            {
                Debug.LogError("[SquadViewController] Kontener 'PlayersContainer' nie został odnaleziony w szablonie UXML!");
                return;
            }

            _playersContainer.Clear();

            List<PlayerViewModel> players = LoadInitialPlayerData();

            foreach (PlayerViewModel player in players)
            {
                VisualElement playerRow = BuildPlayerRowElement(player);
                _playersContainer.Add(playerRow);
            }
        }

        /// <summary>
        /// Tworzy pojedynczy wiersz zawodnika wraz z ostylowanymi etykietami.
        /// </summary>
        private VisualElement BuildPlayerRowElement(PlayerViewModel player)
        {
            // Kontener główny wiersza zawodnika (.player-row)
            VisualElement row = new VisualElement();
            row.AddToClassList("player-row");

            // Plakietka z pozycją (GK, DEF, MID, STR)
            Label posBadge = new Label(player.Position);
            posBadge.AddToClassList("player-pos-badge");
            row.Add(posBadge);

            // Etykieta z pełnym imieniem i nazwiskiem gracza
            Label nameLabel = new Label($"{player.FirstName} {player.LastName}");
            nameLabel.AddToClassList("player-name-label");
            row.Add(nameLabel);

            // Plakietka z oceną ogólną (OverallSkill) wraz z dynamicznym kodem kolorystycznym
            Label skillBadge = new Label(player.OverallSkill.ToString());
            skillBadge.AddToClassList("player-skill-badge");

            if (player.OverallSkill >= 80)
            {
                // Kolor zielony dla graczy o statusie gwiazdy
                skillBadge.AddToClassList("skill-high");
            }
            else if (player.OverallSkill >= 65)
            {
                // Kolor bursztynowy / pomarańczowy dla solidnych graczy ligowych
                skillBadge.AddToClassList("skill-mid");
            }
            else
            {
                // Stonowany szary dla młodych graczy lub głębokich rezerw
                skillBadge.AddToClassList("skill-low");
            }

            row.Add(skillBadge);

            return row;
        }

        /// <summary>
        /// Pobiera wstępne dane zawodników (główny skład ZSŁ Gdańsk z bazy zaktualizowany o żądane statystyki
        /// oraz kilkunastu graczy rezerwowych).
        /// </summary>
        private List<PlayerViewModel> LoadInitialPlayerData()
        {
            return new List<PlayerViewModel>
            {
                // Główny trzon zespołu ZSŁ Gdańsk z zadanymi statystykami:
                new PlayerViewModel("Maciej", "Kamiński", "MID", 90),
                new PlayerViewModel("Nataniel", "Żbikowski", "STR", 90),
                new PlayerViewModel("Hubert", "Miłuch", "DEF", 90),
                new PlayerViewModel("Karol", "Stolc", "DEF", 65),

                // Skład uzupełniający i rezerwy:
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
    }
}
