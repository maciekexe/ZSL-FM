# 📊 Diagramy — Łączność FM

Diagramy w formacie Mermaid — renderują się automatycznie w podglądzie plików `.md` na GitHubie.

---

## 1. Diagram klas

Podstawowy model domenowy gry: menadżer, klub, zawodnik, liga, mecz, transfer i kontrakt.

```mermaid
classDiagram
    class Manager {
        +string Name
        +int Reputation
        +Club ManagedClub
        +hireStaff()
        +setTactic(Tactic)
    }

    class Club {
        +string Name
        +int Budget
        +int Reputation
        +List~Player~ Squad
        +List~Contract~ Contracts
        +addPlayer(Player)
        +removePlayer(Player)
        +calculateWageBill() int
    }

    class Player {
        +string Name
        +int Age
        +string Position
        +int Skill
        +int Morale
        +int Fitness
        +Contract CurrentContract
        +train()
        +updateMorale(int)
    }

    class Contract {
        +Player Player
        +Club Club
        +int Salary
        +Date StartDate
        +Date EndDate
        +bool IsExpired()
    }

    class League {
        +string Name
        +List~Club~ Clubs
        +List~Match~ Fixtures
        +Table Standings
        +generateFixtures()
        +updateStandings(Match)
    }

    class Match {
        +Club HomeClub
        +Club AwayClub
        +Date MatchDate
        +int HomeScore
        +int AwayScore
        +List~MatchEvent~ Events
        +simulate()
        +getResult() string
    }

    class MatchEvent {
        +int Minute
        +string EventType
        +Player InvolvedPlayer
        +string Description
    }

    class Transfer {
        +Player Player
        +Club FromClub
        +Club ToClub
        +int Fee
        +string Status
        +negotiate()
        +complete()
    }

    class Tactic {
        +string Formation
        +string Mentality
        +List~Player~ Lineup
    }

    Manager "1" --> "1" Club : zarządza
    Club "1" --> "*" Player : posiada skład
    Club "1" --> "*" Contract : ma kontrakty
    Player "1" --> "1" Contract : jest związany
    League "1" --> "*" Club : zawiera
    League "1" --> "*" Match : rozgrywa
    Match "1" --> "2" Club : rozgrywają
    Match "1" --> "*" MatchEvent : generuje
    Transfer "1" --> "1" Player : dotyczy
    Transfer "1" --> "2" Club : między klubami
    Manager "1" --> "1" Tactic : ustawia
    MatchEvent "1" --> "0..1" Player : dotyczy
```

---

## 2. Diagram aktywności — symulacja meczu

Przepływ jednej rozgrywki meczowej: od wyboru taktyki, przez symulację minut, do zapisania wyniku.

```mermaid
flowchart TD
    Start([Start]) --> A[Wybór taktyki przez menadżera]
    A --> B[Inicjalizacja meczu: składy, boisko]
    B --> C{Minuta 1-90}
    C --> D[Wygeneruj zdarzenie meczowe]
    D --> E{Typ zdarzenia}
    E -->|Gol| F[Zaktualizuj wynik]
    E -->|Kartka| G[Zaktualizuj status zawodnika]
    E -->|Kontuzja| H[Zdejmij zawodnika, zaproponuj zmianę]
    E -->|Brak akcji| I[Kontynuuj]
    F --> J[Zapisz zdarzenie w historii meczu]
    G --> J
    H --> J
    I --> J
    J --> K{Koniec 90 minut?}
    K -->|Nie| C
    K -->|Tak| L[Wygeneruj końcowy wynik]
    L --> M[Zaktualizuj tabelę ligową]
    M --> N[Zaktualizuj morale i formę zawodników]
    N --> O[Zapisz statystyki meczu do bazy]
    O --> End([Koniec])
```

---

## 3. Diagram aktywności — proces transferu zawodnika

```mermaid
flowchart TD
    Start([Start]) --> A[Menadżer wybiera zawodnika do transferu]
    A --> B{Zawodnik dostępny?}
    B -->|Nie| X[Odmowa - koniec]
    B -->|Tak| C[Złóż ofertę do klubu]
    C --> D{Klub akceptuje ofertę?}
    D -->|Nie| E[Negocjuj wyższą kwotę]
    E --> C
    D -->|Tak| F[Negocjacje z zawodnikiem: kontrakt, pensja]
    F --> G{Zawodnik akceptuje warunki?}
    G -->|Nie| H[Zmień warunki kontraktu]
    H --> F
    G -->|Tak| I[Sfinalizuj transfer]
    I --> J[Zaktualizuj budżet obu klubów]
    J --> K[Przenieś zawodnika do nowego klubu]
    K --> L[Zapisz nowy kontrakt]
    L --> End([Koniec])
    X --> End
```

---

*Diagramy można edytować bezpośrednio w blokach ```mermaid``` — każdy edytor obsługujący Mermaid (w tym podgląd GitHuba) wyrenderuje je automatycznie.*
