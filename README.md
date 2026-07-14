# F1 Console Manager 2025 🏎️

Zaawansowany, obiektowy symulator zarządzania zespołem Formuły 1 napisany w języku C#. Projekt ten to nie tylko gra tekstowa, ale przede wszystkim **praktyczna implementacja zasad programowania zorientowanego obiektowo (OOP), wzorców projektowych oraz zaawansowanej logiki algorytmicznej**.

## 🧠 Architektura i Wzorce Projektowe

Kod został zaprojektowany z myślą o czystości, rozszerzalności i zasadach SOLID. W projekcie wykorzystano następujące wzorce i koncepcje:

* **Wzorzec Strategii (Strategy Pattern):**
  * Zaimplementowany w systemie opon (`ITireStrategy`). Pozwala to na dynamiczną zmianę algorytmów obliczających zużycie i osiągi bez modyfikowania głównego silnika gry. Konkretne klasy (`SoftTyre`, `WetTyre` itd.) implementują wspólny interfejs, realizując zasadę polimorfizmu.
* **Wzorzec Fabryki (Factory Pattern / Builder concept):**
  * Klasa `F1GridFactory` odpowiada za skomplikowany proces inicjalizacji całej stawki (tworzenie obiektów `Driver`, przypisywanie do nich obiektów `Car` i statystyk). Separuje to logikę tworzenia danych od logiki samej rozgrywki.
* **Hermetyzacja i Delegacja (Encapsulation & Delegation):**
  * Zmienne wewnętrzne (np. stan opon, punkty sezonu) są ściśle chronione przed niepowołanym dostępem z zewnątrz (prywatne settery). Klasa `Driver` działa jako fasada/pośrednik, delegując skomplikowane polecenia (np. `SetStartingTyres`) do podsystemów takich jak `RaceStatus`.
* **Single Responsibility Principle (Zasada Pojedynczej Odpowiedzialności):**
  * Kod jest podzielony na wysoce wyspecjalizowane moduły: `UserInterface` zajmuje się wyłącznie renderowaniem konsoli, `RaceStatus` to silnik fizyczny jednego auta, `RaceControlSystem` zarządza globalnymi zdarzeniami na torze, a `ChampionshipSystem` zlicza punkty.

## 💻 Wykorzystane Umiejętności Programistyczne (C# / .NET)

Projekt demonstruje biegłość w następujących obszarach języka C#:

* **LINQ (Language Integrated Query):**
  * Zaawansowane manipulacje kolekcjami (listami kierowców) z użyciem metod takich jak `.OrderBy()`, `.ThenByDescending()`, `.Where()`, `.Any()`, czy `.Count()`. Używane m.in. do dynamicznego sortowania tabeli wyników na żywo oraz klasyfikacji generalnej.
* **Systemy Prawdopodobieństwa i Algorytmy Decyzyjne:**
  * Implementacja złożonych drzew decyzyjnych dla sztucznej inteligencji (`MakeAIDecision`). Boty analizują dystans do mety, intensywność opadów oraz zużycie opon, wyliczając priorytety (bezpieczeństwo vs taktyka).
  * Algorytmy probabilistyczne korzystające z generatorów liczb pseudolosowych (`Random`) do symulowania wypadków, awarii, wahań tempa i kar sędziowskich w oparciu o statystyki (np. błędy sędziowskie z szansą 0.5%).
* **Zarządzanie Stanem (State Management):**
  * Śledzenie stanu pojedynczego wyścigu oraz całego sezonu, bezpieczne resetowanie zmiennych między sesjami bez wycieków pamięci.
* **Obsługa Wejścia/Wyjścia (I/O) Konsoli:**
  * Projektowanie interaktywnego UI w środowisku tekstowym. Reagowanie na pojedyncze wciśnięcia klawiszy (`Console.ReadKey`), dynamiczna zmiana kolorów (`ConsoleColor`) i czyszczenie bufora w celu uzyskania efektu płynnego "odświeżania" ekranu.

## 🌟 Mechaniki Gry

* **Bezlitosna Fizyka Opon:** Opony zużywają się dynamicznie na podstawie pogody, typu mieszanki i stylu jazdy. Jazda na oponach o wartości <= 0% kończy się natychmiastowym wypadkiem.
* **Dynamiczna Pogoda:** System śledzi zachmurzenie i intensywność opadów, co bezpośrednio modyfikuje współczynniki przyczepności.
* **Safety Car i Dyrekcja Wyścigu:** Autonomiczny system `RaceControl` wykrywa wypadki lub losowe incydenty, wypuszcza Samochód Bezpieczeństwa i automatycznie "ściska" stawkę (CompressField), niwelując różnice czasowe przy zachowaniu zakazu wyprzedzania.
* **Post-Race Penalties:** System symulujący decyzje sędziów (kary +5s / +10s doliczane do ostatecznego czasu), mogący zmienić ostateczny rezultat po przekroczeniu linii mety.

## 🛠️ Wymagania i Uruchomienie

Projekt został napisany w C# dla platformy .NET.

1. Wymagany zainstalowany **.NET SDK**.
2. Sklonuj to repozytorium na swój komputer.
3. W terminalu, w głównym folderze projektu wpisz:
   ```bash
   dotnet run
