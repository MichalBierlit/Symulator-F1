using System;
using System.Collections.Generic;

namespace F2
{
    public class UserInterface
    {
        // --- Metody powitalne i konfiguracyjne ---
        public void ShowWelcomeScreen(int totalRaces)
        {
            Console.Clear();
            Console.WriteLine("==========================================");
            Console.WriteLine("       F1 TEAM MANAGER 2025: SEASON       ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Kalendarz: {totalRaces} wyścigów.");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("[ENTER] Dalej...");
            Console.ReadLine();
        }

        public void ShowTeamSelection(List<string> teams)
        {
            Console.Clear();
            Console.WriteLine("\nWYBIERZ ZESPÓŁ DO PROWADZENIA:");
            for (int i = 0; i < teams.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {teams[i]}");
            }
        }

        public int GetIntInput(int min, int max)
        {
            int choice = 0;
            while (choice < min || choice > max)
            {
                Console.Write($"Wybierz ({min}-{max}): ");
                if (!int.TryParse(Console.ReadLine(), out choice))
                    choice = 0;
            }
            return choice;
        }

        public void ShowTeamConfirmation(string teamName, string driver1, string driver2)
        {
            Console.Clear();
            Console.WriteLine($"\nGRATULACJE! Jesteś nowym szefem {teamName}.");
            Console.WriteLine($"Twoi kierowcy: {driver1} oraz {driver2}");
            Console.WriteLine("[ENTER] ROZPOCZNIJ SEZON");
            Console.ReadLine();
        }

        // --- Kwalifikacje ---
        public void ShowQualifyingResults(string raceName, List<Driver> grid)
        {
            Console.Clear();
            Console.WriteLine($"=== {raceName} - WYNIKI KWALIFIKACJI ===");
            for (int i = 0; i < grid.Count; i++)
            {
                if (grid[i].IsPlayer)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;

                string gap = i == 0 ? "POLE" : $"+{grid[i].QualifyingTime - grid[0].QualifyingTime:F3}s";
                Console.WriteLine($"{i + 1,2}. {grid[i].Name,-20} ({gap})");
            }
            Console.ResetColor();
        }

        public void WaitForRaceStart(int laps)
        {
            Console.WriteLine($"\nDYSTANS WYŚCIGU: {laps} okrążeń.");
            Console.WriteLine("[ENTER] START WYŚCIGU");
            Console.ReadLine();
        }

        // --- Wyścig ---
        public void ShowRaceHeader(string raceName, int currentLap, int totalLaps)
        {
            // TYLKO nagłówek, bez czyszczenia ekranu
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($" {raceName} | OKRĄŻENIE {currentLap}/{totalLaps} ".PadRight(60));
            Console.ResetColor();
        }

        public void ShowTrackStatus(bool isSC, int scLapsLeft, string weatherDesc, int cloudDensity)
        {
            Console.Write("Status toru: ");
            if (isSC)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($" SAFETY CAR ({scLapsLeft} okr.) ");
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ZIELONA FLAGA ");
            }
            Console.ResetColor();

            Console.Write(" | Pogoda: ");
            if (cloudDensity > 70)
                Console.ForegroundColor = ConsoleColor.Cyan;
            else
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine($"{weatherDesc} ({cloudDensity}%)");
            Console.ResetColor();
        }

        public void ShowDriverControls(Driver d)
        {
            Console.Write($"\n{d.Name}: ");
            PrintColoredText($"{d.RaceStatus.CurrentTyres.Name} ", d.RaceStatus.CurrentTyres.Color);

            // Pokaż stan opon z kolorami
            if (d.RaceStatus.CurrentTyreHealth > 50)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (d.RaceStatus.CurrentTyreHealth > 20)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Red;

            Console.Write($"[{d.RaceStatus.CurrentTyreHealth:F0}%]");
            Console.ResetColor();

            Console.Write($" | Pit: {d.RaceStatus.PitStopsCount}");

            // Dodaj ostrzeżenie jeśli opony są zużyte
            ShowTyreWarning(d);

            Console.WriteLine("\n  1: Jazda  2: Soft  3: Medium  4: Hard  5: Wet");

            // Dodaj sugestię przy bardzo niskich oponach
            if (d.RaceStatus.CurrentTyreHealth < 15)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  SUGESTIA: Zmień opony! (<15%)");
                Console.ResetColor();
            }

            Console.Write("  Wybór: ");
        }




        public void ShowLiveTable(List<Driver> sortedGrid, Driver leader)
        {
            Console.WriteLine("\nPOZ | KIEROWCA             | GAP       | OPONA (STAN)  | PIT");
            Console.WriteLine(new string('-', 70));

            for (int i = 0; i < sortedGrid.Count; i++)
            {
                var d = sortedGrid[i];

                if (d.RaceStatus.HasCrashed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    // DNF - bez info o oponach i pit stopach
                    Console.WriteLine($"{i + 1,2}. | {d.Name,-20} | {"DNF",-10} | {"DNF",-12} | {"DNF",-3}");
                    Console.ResetColor();
                    continue;
                }

                if (d.IsPlayer)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;

                string gapStr;
                if (i == 0)
                    gapStr = "LIDER";
                else
                {
                    double gap = d.RaceStatus.TotalTime - leader.RaceStatus.TotalTime;
                    gapStr = $"+{gap:F1}s";
                }

                // Opony z procentem
                string tyreInfo = $"{d.RaceStatus.CurrentTyres.Name} ({d.RaceStatus.CurrentTyreHealth:F0}%)";

                // Pit stopy w formacie "użyte/maks"
                string pitInfo = $"{d.RaceStatus.PitStopsCount}/4";

                Console.Write($"{i + 1,2}. | {d.Name,-20} | {gapStr,-10} | ");

                // Kolorowe opony
                PrintColoredText($"{tyreInfo,-12}", d.RaceStatus.CurrentTyres.Color);

                // Pit stopy - kolor w zależności od ilości
                Console.Write(" | ");
                if (d.RaceStatus.PitStopsCount >= 3)
                    Console.ForegroundColor = ConsoleColor.Yellow; // Prawie wykorzystane
                else if (d.RaceStatus.PitStopsCount == 4)
                    Console.ForegroundColor = ConsoleColor.Red; // Wykorzystane wszystkie

                Console.Write($"{pitInfo,-3}");

                // Przywróć kolor
                if (d.IsPlayer)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine();
            }
            Console.ResetColor();
        }
        public void ShowTyreWarning(Driver d)
        {
            if (d.RaceStatus.CurrentTyreHealth < 10)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  OSTRZEŻENIE: {d.RaceStatus.CurrentTyreHealth:F0}% opon!");

                if (d.RaceStatus.CurrentTyreHealth < 5)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  KRYTYCZNE: Następne okrążenie może zakończyć się wypadkiem!");
                }
                Console.ResetColor();
            }
        }



        // --- Koniec wyścigu i sezonu ---
        public void ShowRaceEnd(string raceName)
        {
            Console.Clear();
            Console.WriteLine($"=== KONIEC GP {raceName} ===");
            Console.WriteLine("[ENTER] Zobacz wyniki...");
            Console.ReadLine();
        }

        public void ShowSeasonEnd()
        {
            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine("           KONIEC SEZONU 2025                 ");
            Console.WriteLine("==============================================");
            Console.WriteLine("\nDziękujemy za grę!");
            Console.WriteLine("[ENTER] Wyjdź");
            Console.ReadLine();
        }

        // --- Wybór opon na start ---
        public ITireStrategy GetStartingTyreChoice()
        {
            Console.WriteLine("\nWYBIERZ OPONY NA START:");
            Console.WriteLine("  1. Soft ");
            Console.WriteLine("  2. Medium ");
            Console.WriteLine("  3. Hard ");
            Console.WriteLine("  4. Wet ");
            

            int choice = GetIntInput(1, 4);
            switch (choice)
            {
                case 1: return new SoftTyre();
                case 2: return new MediumTyre();
                case 3: return new HardTyre();
                case 4: return new WetTyre();
                default: return new SoftTyre();
            }
        }

        // --- Metody pomocnicze ---
        private void PrintColoredText(string text, ConsoleColor c)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(text);
            Console.ForegroundColor = old;
        }

        // --- NOWA METODA: pokaż prosty komunikat ---
        public void ShowMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"\n> {message}");
            Console.ResetColor();
        }

        // --- NOWA METODA: potwierdzenie decyzji ---
        public void ShowDecisionConfirmation(string driverName, string decision)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  {driverName}: {decision}");
            Console.ResetColor();
        }

        // --- NOWA METODA: pokaż tylko tabelę bez nagłówków (do szybkiego podglądu) ---
        public void ShowCompactTable(List<Driver> sortedGrid, Driver leader)
        {
            Console.WriteLine("\n--- AKTUALNA KLASYFIKACJA ---");
            for (int i = 0; i < Math.Min(10, sortedGrid.Count); i++) // tylko top 10
            {
                var d = sortedGrid[i];
                string marker = d.IsPlayer ? "*" : " ";
                if (d.IsPlayer) Console.ForegroundColor = ConsoleColor.Green;

                if (d.RaceStatus.HasCrashed)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{i + 1,2}.{marker} {d.Name,-18} DNF");
                }
                else
                {
                    double gap = d.RaceStatus.TotalTime - leader.RaceStatus.TotalTime;
                    string gapStr = i == 0 ? "LIDER" : $"+{gap:F1}s";

                    if (d.IsPlayer) Console.ForegroundColor = ConsoleColor.Green;
                    else Console.ForegroundColor = ConsoleColor.Gray;

                    Console.Write($"{i + 1,2}.{marker} {d.Name,-18} {gapStr,-8} ");
                    PrintColoredText(d.RaceStatus.CurrentTyres.Name, d.RaceStatus.CurrentTyres.Color);
                    Console.WriteLine($" ({d.RaceStatus.CurrentTyreHealth:F0}%)");
                }
            }
            Console.ResetColor();
        }

        // --- NOWA METODA: pokaż tylko kierowców gracza ---
        public void ShowPlayerDriversInfo(List<Driver> playerDrivers)
        {
            Console.WriteLine("\n--- TWOI KIEROWCY ---");
            foreach (var d in playerDrivers)
            {
                Console.Write($"  {d.Name}: ");
                PrintColoredText($"{d.RaceStatus.CurrentTyres.Name} ", d.RaceStatus.CurrentTyres.Color);
                Console.WriteLine($"[{d.RaceStatus.CurrentTyreHealth:F0}%] | Pit: {d.RaceStatus.PitStopsCount} | Czas: {d.RaceStatus.TotalTime:F1}s");
            }
        }
    }
}