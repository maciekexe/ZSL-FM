**Kontekst Projektu**



**Jesteś agentem programistycznym wspierającym rozwój gry "Łączność FM" – symulatora menedżera piłkarskiego stworzonego w silniku Unity 2D.**



**Bezwzględne Zasady Architektury (CRITICAL)**



**Zanim napiszesz lub zmodyfikujesz jakikolwiek kod, musisz upewnić się, że nie łamiesz poniższych reguł separacji:**



**Warstwa Logiki (Core) -> Assets/Scripts/Core/**



**Skrypty w tym folderze to czysty kod C# (modele domenowe POCO, algorytmy, AI).**



**ZAKAZ: Kategorycznie zabrania się używania using UnityEngine;, dziedziczenia po MonoBehaviour czy korzystania z jakichkolwiek klas specyficznych dla silnika Unity. Ta warstwa musi być testowalna poza edytorem.**



**Warstwa Widoku (UI) -> Assets/Scripts/UI/**



**Skrypty w tym folderze odpowiadają wyłącznie za interfejs graficzny i dziedziczą po MonoBehaviour.**



**ZAKAZ: Nie implementuj tu żadnej logiki biznesowej ani matematyki. Interfejs jest pasywny – ma jedynie wywoływać komendy z warstwy Core i subskrybować jej zdarzenia (np. za pomocą Action lub EventHandler), aby odświeżać widok w systemie UI Toolkit.**



**Baza Danych i Narzędzia -> Data/Database/ oraz Tools/**



**Dane statyczne (zawodnicy, kluby) trzymane są w pliku SQLite (zsl\_fm.db).**



**Do operacji narzędziowych na bazie poza środowiskiem Unity używamy skryptów Python (w folderze Tools/).**



**Jeśli użytkownik poprosi o nową mechanikę (np. "dodaj system kontuzji"), najpierw zaplanuj czyste klasy w Core, a dopiero potem zaproponuj pasywny skrypt UI do wyświetlenia tego w grze.**

