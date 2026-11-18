# 📋 Łączność FM – Podział obowiązków i harmonogram

> Deadline końcowy projektu: **24.03.2027**.

## 1. Zespoły i role

| Zespół                             | Osoby           | Obszar                                                                                     | Warstwa w repo                                       |
| ---------------------------------- | --------------- | ------------------------------------------------------------------------------------------ | ---------------------------------------------------- |
| **Zespół 1 – Frontend & Dane**     | Maciek, Hubert  | UI, wizualizacja 2D, spójność projektu, baza danych, narzędzia danych                      | `Scripts/UI/`, `Assets/`, `Data/`, `Tools/`, `Docs/` |
| **Zespół 2 – Backend & Mechaniki** | Karol, Nataniel | Silnik symulacji, logika, funkcjonalny kalendarz wydarzeń, mechanika transferów i finansów | `Scripts/Core/` (czysty C#, **bez `UnityEngine`**)   |

### Podział w ramach zespołów

| Osoba        | Główny obszar                                                                                                                                                                   | Dodatkowo             |
| ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------- |
| **Maciek**   | Frontend: menu główne, widok meczu 2D, ekran taktyki, styl UI. **Spójność projektu**: architektura, code review, dokumentacja UML, łączenie obszarów projektu, zarządzanie repo | Integracja UI         |
| **Hubert**   | Frontend: ekrany kadry, transferów, finansów i kalendarza. **Baza danych i obsługa danych**: funkcjonalność bazy danych oraz ich transferu                                      | Zapis/wczytywanie gry |
| **Karol**    | **Transfery** (negocjacje, oferty, okno transferowe), **kalendarz wydarzeń** (sezon, terminarz, ligi/puchary), **finanse klubu**                                                | Nadzór kreatywny      |
| **Nataniel** | **Symulacja meczów** (silnik, wyniki, zdarzenia), **taktyka** (formacje, style gry), **zarządzanie składem** (kadra, kontuzje, rozwój)                                          | Nadzór kreatywny      |

---

## 2. Taski zespołowe (dla całych grup)

### 🟦 Zespół 1 (Maciek + Hubert)

| ID   | Task                                                                                   | Wykonują        | Deadline   |
| ---- | -------------------------------------------------------------------------------------- | --------------- | ---------- |
| Z1-1 | Konfiguracja projektu Unity, struktura folderów, Git LFS                               | Maciek          | 31.10.2026 |
| Z1-2 | Ustalenie stylu graficznego UI (kolory, fonty, komponenty wielokrotnego użytku)        | Maciek          | 04.11.2026 |
| Z1-3 | Stworzenie funkcjonalnej bazy danych                                                   | Hubert          | 04.11.2026 |
| Z1-4 | Makiety wszystkich ekranów (menu, kadra, transfery, taktyka, finanse, kalendarz, mecz) | Maciek, Hubert  | 25.11.2026 |
| Z1-5 | Implementacja ekranów wg makiet i podłączenie z resztą projektu                        | Maciek, Hubert  | 09.12.2026 |
| Z1-6 | Wizualizacja 2D meczu                                                                  | Maciek          | 10.02.2027 |
| Z1-7 | Integracja wszystkich elementów projektu                                               | Maciek, Hubert  | 24.02.2027 |
| Z1-8 | Testy jednostkowe i optymalizacyjne                                                    | Maciek, Hubert  | 10.03.2027 |
| Z1-9 | Finalna dokumentacja i oddanie projektu                                                | Maciek, Hubert  | 24.03.2027 |

### 🟥 Zespół 2 (Karol + Nataniel)

| ID   | Task                                            | Wykonują         | Deadline   |
| ---- | ----------------------------------------------- | ---------------- | ---------- |
| Z2-1 | Szkielet projektu                               | Karol, Nataniel  | 04.11.2026 |
| Z2-2 | Mechanika zarządzania składem                   | Nataniel         | 25.11.2026 |
| Z2-3 | Mechanika kalendarza (upływ czasu, wydarzenia)  | Karol            | 25.11.2026 |
| Z2-4 | Mechanika transferów                            | Karol            | 06.01.2027 |
| Z2-5 | Mechanika taktyki                               | Nataniel         | 06.01.2027 |
| Z2-6 | Finanse                                         | Karol            | 10.02.2027 |
| Z2-7 | Silnik symulacji meczów                         | Nataniel         | 10.02.2027 |
| Z2-8 | Testy użytkowe                                  | Karol, Nataniel  | 17.03.2027 |

---

## 3. Taski indywidualne

### 👤 Maciek – Frontend + Spójność projektu

| ID   | Task                                                                                         | Dotyczy    | Deadline   |
| ---- | -------------------------------------------------------------------------------------------- | ---------- | ---------- |
| M-1  | Repozytorium: ochrona gałęzi `main`, szablon Pull Requestu, konwencja nazw gałęzi i commitów | Z1-1       | 31.10.2026 |
| M-2  | Dokument zasad architektury (Core/UI, zakaz `UnityEngine` w Core), checklista code review    | Z1-1, Z2-1 | 04.11.2026 |
| M-3  | System stylu UI: motyw, komponenty (przycisk, tabela, karta zawodnika)                       | Z1-2       | 04.11.2026 |
| M-4  | Makiety: menu główne, ekran taktyki, widok meczu                                             | Z1-4       | 25.11.2026 |
| M-5  | Menu główne + nawigacja między scenami                                                       | Z1-5       | 09.12.2026 |
| M-6  | Ekran taktyki (formacja, ustawienia meczowe) na danych próbnych                              | Z1-5       | 09.12.2026 |
| M-7  | Wizualizacja 2D meczu (boisko, punkty zawodników, odtwarzanie zdarzeń)                       | Z1-6       | 10.02.2027 |
| M-8  | Podpięcie menu i ekranu taktyki do eventów Core (warstwa „binding"), integracja obszarów projektu, bez logiki biznesowej w UI | Z1-7 | 24.02.2027 |
| M-9  | Testy i optymalizacja UI oraz warstwy „binding"                                              | Z1-8       | 10.03.2027 |
| M-10 | Aktualizacja diagramów UML i README (roadmapa, instrukcja uruchomienia)                      | Z1-9       | 17.03.2027 |
| M-11 | Build końcowy i oddanie projektu                                                             | Z1-9       | 24.03.2027 |
| M-12 | Cykliczne code review PR-ów (min. raz na tydzień) i pilnowanie spójności nazw/API            | wszystkie  | ciągle     |

### 👤 Hubert – Frontend + Baza danych i obsługa danych

| ID   | Task                                                                                         | Dotyczy | Deadline   |
| ---- | -------------------------------------------------------------------------------------------- | ------- | ---------- |
| H-1  | Projekt schematu SQLite (zawodnicy, kluby, ligi, kontrakty, transfery, sezony) + diagram ERD | Z1-3    | 31.10.2026 |
| H-2  | Funkcjonalna baza: tworzenie bazy, implementacja repozytoriów (wg interfejsów od Karola), struktura plików JSON (`Data/Config/`) | Z1-3 | 04.11.2026 |
| H-3  | Skrypty Python w `Tools/`: import i generowanie danych startowych (zawodnicy, kluby)         | Z1-3    | 18.11.2026 |
| H-4  | Makiety: kadra, transfery, finanse, kalendarz                                                | Z1-4    | 25.11.2026 |
| H-5  | Ekran kadry (lista zawodników, szczegóły, sortowanie/filtry) na danych próbnych              | Z1-5    | 09.12.2026 |
| H-6  | Ekran transferów (lista, wyszukiwarka, oferty) na danych próbnych                            | Z1-5    | 09.12.2026 |
| H-7  | Ekran kalendarza i tabeli ligowej na danych próbnych                                         | Z1-5    | 09.12.2026 |
| H-8  | Ekran finansów klubu na danych próbnych                                                      | Z1-5    | 09.12.2026 |
| H-9  | Podłączenie ekranów kadry, transferów, kalendarza i finansów do Core i bazy                  | Z1-7    | 24.02.2027 |
| H-10 | Zapis / wczytywanie gry (serializacja stanu gry)                                             | Z1-7    | 24.02.2027 |
| H-11 | Testy bazy: migracje, kopie zapasowe, weryfikacja spójności danych                           | Z1-8    | 10.03.2027 |
| H-12 | Dokumentacja bazy i danych (ERD, format JSON, skrypty) oraz oddanie projektu                 | Z1-9    | 24.03.2027 |

### 👤 Karol – Transfery, kalendarz, finanse, nadzór kreatywny (wizja)

| ID   | Task                                                                                        | Dotyczy    | Deadline   |
| ---- | ------------------------------------------------------------------------------------------- | ---------- | ---------- |
| K-1  | Interfejsy repozytoriów (dla warstwy danych Huberta)                                        | Z2-1       | 31.10.2026 |
| K-2  | Modele POCO: zawodnik, klub, kontrakt (atrybuty, wartość rynkowa, pensja, długość kontraktu) | Z2-1       | 04.11.2026 |
| K-3  | Dokument wizji gry: zasady, pętla rozgrywki, tryby, ton i styl                              | nadzór     | 11.11.2026 |
| K-4  | Kalendarz: sezon, terminarz ligowy, upływ czasu, daty wydarzeń                              | Z2-3       | 25.11.2026 |
| K-5  | Transfery v0: wyceny zawodników, składanie ofert                                            | Z2-4       | 16.12.2026 |
| K-6  | Negocjacje (AI klubów i zawodników: akceptacja, kontroferta, odrzucenie), okno transferowe, skauting | Z2-4 | 06.01.2027 |
| K-7  | Puchary i rozgrywki dodatkowe w kalendarzu, kolejność wydarzeń, awanse/spadki               | Z2-3       | 06.01.2027 |
| K-8  | Finanse: budżet, pensje, przychody, bilans sezonu, limity transferowe                       | Z2-6       | 10.02.2027 |
| K-9  | Balans ekonomii (ceny, pensje, przychody)                                                   | Z2-6       | 24.02.2027 |
| K-10 | Testy jednostkowe: transfery, kalendarz, finanse                                            | Z2-8       | 03.03.2027 |
| K-11 | Testy użytkowe (wspólnie z Natanielem) i poprawki błędów                                    | Z2-8       | 17.03.2027 |
| K-12 | Opis mechanik (transfery, kalendarz, finanse) do dokumentacji końcowej                      | Z1-9       | 24.03.2027 |

### 👤 Nataniel – Symulacja meczów, taktyka, skład, nadzór kreatywny (balans)

| ID   | Task                                                                    | Dotyczy    | Deadline   |
| ---- | ----------------------------------------------------------------------- | ---------- | ---------- |
| N-1  | Modele POCO: manager, mecz, sezon + system zdarzeń (Events/Delegates) dla UI | Z2-1  | 04.11.2026 |
| N-2  | Zarządzanie składem: kadra, pierwsza jedenastka, rezerwa, kontuzje, forma, zmęczenie | Z2-2 | 25.11.2026 |
| N-3  | Projekt algorytmu symulacji meczu (założenia, wzory, parametry)         | Z2-7       | 02.12.2026 |
| N-4  | Silnik meczu v0: wynik na podstawie siły drużyn                         | Z2-7       | 16.12.2026 |
| N-5  | Rozwój zawodników (progresja, regres, wiek, potencjał)                  | Z2-2       | 16.12.2026 |
| N-6  | Taktyka: formacje, style gry, role zawodników, wpływ na symulację       | Z2-5       | 06.01.2027 |
| N-7  | Silnik meczu v1: zdarzenia (gole, kartki, kontuzje, zmiany) + wersja próbna danych pozycji dla 2D | Z2-7 | 20.01.2027 |
| N-8  | Silnik meczu v2: pozycje zawodników w czasie (dane dla wizualizacji 2D) | Z2-7       | 10.02.2027 |
| N-9  | Balans meczów (symulacje masowe, statystyki realistyczności wyników)    | Z2-7       | 24.02.2027 |
| N-10 | Testy jednostkowe: silnik meczu, taktyka, skład                         | Z2-8       | 03.03.2027 |
| N-11 | Testy użytkowe (wspólnie z Karolem) i poprawki błędów                   | Z2-8       | 17.03.2027 |
| N-12 | Opis mechanik (symulacja, taktyka, skład) do dokumentacji końcowej      | Z1-9       | 24.03.2027 |

---

## 4. Zależności między osobami (kto na kogo czeka)

| Zależność                  | Opis                                                                                       | Termin gotowości                  |
| -------------------------- | ------------------------------------------------------------------------------------------ | --------------------------------- |
| Karol → Hubert             | Interfejsy repozytoriów (K-1) potrzebne do H-2                                             | 31.10.2026                        |
| Hubert → Karol, Nataniel   | Schemat bazy (H-1) potrzebny do modeli i zapytań (K-2, N-1)                                | 31.10.2026                        |
| Nataniel → Maciek, Hubert  | System zdarzeń Core (N-1) potrzebny do ekranów i bindingu (M-8, H-9)                       | 04.11.2026                        |
| Hubert → Nataniel          | Dane startowe (H-3) potrzebne do testowania zarządzania składem (N-2)                      | 18.11.2026                        |
| Maciek → Nataniel          | Makieta ekranu taktyki (M-4) potrzebna do mechaniki taktyki (N-6)                          | 25.11.2026                        |
| Nataniel → Hubert          | API zarządzania składem (N-2) potrzebne do ekranu kadry (H-5, H-9)                         | 25.11.2026                        |
| Karol → Hubert             | Dane kalendarza (K-4, K-7) potrzebne do ekranu kalendarza (H-7, H-9)                       | 25.11.2026 (K-7: 06.01.2027)      |
| Karol → Hubert             | API transferów (K-5, K-6) potrzebne do ekranu transferów (H-6, H-9)                        | 16.12.2026 (wersja v0), 06.01.2027 |
| Nataniel → Maciek          | API taktyki (N-6) potrzebne do podpięcia ekranu taktyki (M-8)                              | 06.01.2027                        |
| Nataniel → Maciek          | Dane pozycji zawodników z silnika meczu (N-7, N-8) potrzebne do M-7                        | 20.01.2027 (wersja próbna), 10.02.2027 (finalna) |
| Karol → Hubert             | API finansów (K-8) potrzebne do ekranu finansów (H-8, H-9)                                 | 20.01.2027 (API), 10.02.2027 (pełny model) |
| Wszyscy → Maciek, Hubert   | Gotowe mechaniki Core (Z2-2…Z2-7) potrzebne do integracji (Z1-7)                           | 10.02.2027                        |
