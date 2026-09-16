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
   git clone https://github.com/twoja-organizacja/lacznosc-fm.git
   cd lacznosc-fm
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

## 🤝 Współpraca

Projekt rozwijany przez zespół. Zasady współpracy:

1. Twórz nową gałąź (`feature/nazwa-funkcji`) dla każdej funkcjonalności.
2. Commituj małymi, czytelnymi zmianami.
3. Otwieraj Pull Request i poproś o code review przed merge do `main`.
4. Logikę symulacji (folder `Scripts/Core`) trzymaj niezależną od Unity — powinna dać się testować bez otwierania edytora.

\---

## 📬 Kontakt

Masz pytania lub sugestie? Otwórz Issue w repozytorium.

