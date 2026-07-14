using System;
using System.Diagnostics;
using System.Threading;

namespace F2
{
    public class PitStopMinigame
    {
        private Random _rng;
        // Klawisze pod lewą ręką (Q, W, E, A, S, D, F, Z, X, C)
        private readonly char[] _validKeys = { 'Q', 'W', 'E', 'A', 'S', 'D', 'F', 'Z', 'X', 'C' };

        public PitStopMinigame()
        {
            _rng = new Random();
        }

        public double Run()
        {
            // --- KROK 1: INSTRUKCJA ---
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n========================================");
            Console.WriteLine("       PRZYGOTOWANIE DO PIT-STOPU       ");
            Console.WriteLine("========================================");
            Console.ResetColor();

            Console.WriteLine("\nZASADY:");
            Console.WriteLine("1. Za chwilę zobaczysz kolejno 4 koła do wymiany.");
            Console.WriteLine("2. Przy każdym kole pojawi się wymagany klawisz.");
            Console.WriteLine("3. Musisz wcisnąć go jak najszybciej.");
            Console.WriteLine("4. BŁĄD = +2 sekundy kary!");

            Console.WriteLine("\nMOŻLIWE KLAWISZE (Lewa ręka):");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" [Q] [W] [E]");
            Console.WriteLine(" [A] [S] [D] [F]");
            Console.WriteLine(" [Z] [X] [C]");
            Console.ResetColor();

            Console.WriteLine("\nNaciśnij [ENTER], gdy będziesz gotowy...");
            Console.ReadLine();

            // --- KROK 2: ODLICZANIE ---
            Console.Clear();
            Console.WriteLine("\nMECHANICY WYBIEGAJĄ...");
            Thread.Sleep(800);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("3...");
            Thread.Sleep(800);
            Console.WriteLine("2...");
            Thread.Sleep(800);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1...");
            Thread.Sleep(800);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("START!");
            Console.ResetColor();
            Thread.Sleep(200);

            // --- KROK 3: MINIGRA (WŁAŚCIWA LOGIKA) ---
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n !!! ZMIANA OPON - OGIEŃ !!! \n");
            Console.ResetColor();

            double totalTime = 20.0; // Czas bazowy (dojazd, wyjazd, podnośniki)
            Stopwatch sw = new Stopwatch();
            string[] tires = { "LEWY PRZOD", "PRAWY PRZOD", "LEWY TYL", "PRAWY TYL" };

            foreach (var tire in tires)
            {
                // Losujemy klawisz
                char keyToPress = _validKeys[_rng.Next(_validKeys.Length)];

                Console.Write($" {tire,-12} | WYMAGANY: ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"[{keyToPress}]");
                Console.ResetColor();

                sw.Restart();
                bool correct = false;

                while (!correct)
                {
                    // Czekamy na klawisz (true ukrywa znak w konsoli)
                    var input = Console.ReadKey(true);
                    char pressedChar = char.ToUpper(input.KeyChar);

                    if (pressedChar == keyToPress)
                    {
                        correct = true;
                        sw.Stop();

                        double reactionTime = sw.Elapsed.TotalSeconds;
                        totalTime += reactionTime;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($" -> OK! (+{reactionTime:F2}s)");
                        Console.ResetColor();
                    }
                    else
                    {
                        // Błąd
                        totalTime += 2.0;
                        Console.Write(" ");
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" BŁĄD! (+2.0s) ");
                        Console.ResetColor();
                    }
                }
            }

            // --- KROK 4: WYNIK ---
            Console.WriteLine("\n----------------------------------------");
            Console.Write(" CZAS CAŁKOWITY: ");

            if (totalTime < 24.0) Console.ForegroundColor = ConsoleColor.Green;      // Szybko
            else if (totalTime < 27.0) Console.ForegroundColor = ConsoleColor.Yellow; // Średnio
            else Console.ForegroundColor = ConsoleColor.Red;                          // Wolno

            Console.WriteLine($"{totalTime:F2}s");
            Console.ResetColor();
            Console.WriteLine("----------------------------------------");

            Console.WriteLine("Wracasz na tor...");
            Thread.Sleep(1500);

            return totalTime;
        }
    }
}