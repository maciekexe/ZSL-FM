# ⚽ Łączność FM

**Łączność FM** to gra menadżerska w stylu *Football Manager* — prowadź swój klub, zarządzaj transferami, taktyką i budżetem, a mecze śledź w prostej wizualizacji 2D.

\---

## 🎮 O projekcie

Łączność FM to symulator zarządzania klubem piłkarskim. Gracz wciela się w roli menadżera i odpowiada za:

* 🧑‍💼 Transfery i negocjacje z zawodnikami oraz klubami
* 📋 Taktykę i ustawienia meczowe
* 💰 Budżet klubu i finanse
* 📈 Rozwój zawodników i skauting
* 🏆 Rywalizację w lidze i pucharach

Mecze są rozgrywane w silniku symulacyjnym i wizualizowane w prostej grafice 2D (pozycje zawodników jako punkty na boisku), a nie w pełnym 3D — nacisk położony jest na głębię zarządzania, nie na animacje.

\---

## 🛠️ Stos technologiczny

|Warstwa|Technologia|
|-|-|
|Silnik gry / UI|**Unity** (C#)|
|Logika symulacji meczu i AI|**C#** (biblioteka .NET niezależna od Unity)|
|Baza danych (zawodnicy, ligi, kluby)|**SQLite**|
|Konfiguracja / dane wejściowe|**JSON**|
|Narzędzia pomocnicze (import danych, balansowanie)|**Python**|
|Kontrola wersji|**Git** / Git LFS (assety graficzne)|

\---

## 📂 Struktura repozytorium

```
LacznoscFM/
├── Assets/                 # Zasoby Unity (sceny, sprite'y, UI)
├── Scripts/
│   ├── UI/                 # Skrypty interfejsu (Unity)
│   └── Core/                # Czysta logika C# (silnik meczu, AI, ekonomia)
├── Data/
│   ├── Database/            # Pliki SQLite z bazą lig/klubów/zawodników
│   └── Config/               # Pliki JSON (ligi, ustawienia startowe)
├── Tools/                   # Skrypty Python do importu/generowania danych
├── Docs/                    # Dokumentacja projektu
└── README.md
```

\---

## 🚀 Wymagania

* [Unity](https://unity.com/) 2022 LTS lub nowszy
* .NET 6+ (dołączone w Unity)
* Python 3.10+ (opcjonalnie, do skryptów pomocniczych w `Tools/`)
* Git (z LFS dla dużych plików graficznych)

\---

## ⚙️ Instalacja i uruchomienie

1. Sklonuj repozytorium:

```bash
   git clone https://github.com/maciekexe/ZSL-FM.git
   cd ZSL-FM
   ```

2. Otwórz projekt w Unity Hub, wskazując folder repozytorium.
3. Poczekaj, aż Unity zaimportuje wszystkie assety.
4. Otwórz scenę startową w `Assets/Scenes/MainMenu.unity` i uruchom grę przyciskiem **Play**.

\---

## 🗺️ Roadmapa

* \[ ] Podstawowy silnik symulacji meczu
* \[ ] System transferów i negocjacji
* \[ ] Baza danych lig i klubów
* \[ ] Interfejs zarządzania kadrą
* \[ ] Wizualizacja 2D meczu
* \[ ] System finansów klubu
* \[ ] Zapis / wczytywanie gry
* \[ ] Tryb kariery wieloletniej

\---

## 🤝 Zasady współpracy 

Projekt rozwijany jest zespołowo. Aby utrzymać czystą architekturę i uniknąć długu technologicznego, każdego kontrybutora obowiązują bezwzględnie poniższe reguły:

1. **Ścisła Separacja Logiki (Core):** Kod w folderze `Scripts/Core/` to czyste środowisko C#. **Kategorycznie zabrania się** importowania i używania przestrzeni nazw `UnityEngine` w tej warstwie. Silnik meczowy ma być całkowicie niezależny, testowalny poza edytorem i opierać się wyłącznie na standardowych bibliotekach .NET.
2. **Architektura obiektów:** Modele domenowe zdefiniowane w warstwie Core (np. Manager, Player, Club) to czyste klasy (POCO) i **nie mogą** pod żadnym pozorem dziedziczyć po `MonoBehaviour`.
3. **Pasywna Warstwa Widoku:** Skrypty w folderze `Scripts/UI/` odpowiadają wyłącznie za wyświetlanie danych. Dziedziczą po `MonoBehaviour`, ale nie przeliczają logiki biznesowej — ich jedynym zadaniem jest subskrybowanie zdarzeń (Events/Delegates) płynących z warstwy `Core`.
4. Twórz nową gałąź (`feature/nazwa-funkcji`) dla każdej nowej funkcjonalności przed rozpoczęciem pracy.
5. Commituj małymi, czytelnymi paczkami i zawsze korzystaj z Pull Requestów przed wykonaniem merge'a do gałęzi `main`.
\---

## 📐 Architektura i Dokumentacja

Kluczowe założenia logiki biznesowej, diagramy klas UML oraz przepływy aktywności (np. symulacja meczu, transfery) znajdują się w folderze `Docs/`. 

* [Diagram Klas](Docs/DiagramKlas.png)
* [Diagram Aktywności - Symulacja Meczu](Docs/DiagramAktywnosci1.png)
* [Diagram Aktywności - Transfery](Docs/DiagramAktywnosci2.png)




