using System;
using System.Collections.Generic;
using System.Linq;

namespace F2
{
    public class RaceControlSystem
    {
        public bool IsSafetyCarActive { get; private set; }
        public int SafetyCarLapsLeft { get; private set; }

        private Random _rng;
        private int _lastCrashedCount = 0;

        public RaceControlSystem()
        {
            IsSafetyCarActive = false;
            _rng = new Random();
        }

        public void Reset()
        {
            IsSafetyCarActive = false;
            SafetyCarLapsLeft = 0;
            _lastCrashedCount = 0;
        }

        public void CheckForIncidents(List<Driver> grid, int rainIntensity)
        {
            int currentCrashedCount = grid.Count(d => d.RaceStatus.HasCrashed);
            if (currentCrashedCount < _lastCrashedCount) _lastCrashedCount = currentCrashedCount;

            // --- OBSŁUGA AKTYWNEGO SC ---
            if (IsSafetyCarActive)
            {
                SafetyCarLapsLeft--;
                if (SafetyCarLapsLeft <= 0)
                {
                    IsSafetyCarActive = false;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n*** SAFETY CAR ZJEŻDŻA - WZNOWIENIE WYŚCIGU! ***");
                    Console.ResetColor();
                }
                else
                {
                    CompressField(grid);
                }
                _lastCrashedCount = currentCrashedCount;
                return;
            }

            // --- DECYZJA O WYJEŹDZIE SC ---
            bool deploySC = false;
            string reason = "";

            // 1. PRAWDZIWY WYPADEK (Ktoś się rozbił w tej turze)
            if (currentCrashedCount > _lastCrashedCount)
            {
                deploySC = true;
                reason = "POWAŻNY WYPADEK (Kierowca DNF)";
            }
            // 2. LOSOWE ZDARZENIE (Odłamki / Awaria bez DNF)
            // ZWIĘKSZONO SZANSĘ:
            // Bazowo: 3% + (2% * Deszcz). 
            // Na suchym: 3% co okrążenie. W ulewie: nawet 13%.
            else
            {
                int riskChance = 3 + (rainIntensity * 2);
                if (_rng.Next(100) < riskChance)
                {
                    deploySC = true;
                    reason = "ODŁAMKI NA TORZE / ZATRZYMANY BOLID";
                }
            }

            if (deploySC)
            {
                IsSafetyCarActive = true;
                SafetyCarLapsLeft = _rng.Next(3, 6); // Dłuższy SC (3-5 okrążeń)

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n" + new string('!', 40));
                Console.WriteLine($" !!! SAFETY CAR !!! ({reason})");
                Console.WriteLine(new string('!', 40));
                Console.ResetColor();

                CompressField(grid);
            }

            _lastCrashedCount = currentCrashedCount;
        }

        private void CompressField(List<Driver> grid)
        {
            var sortedGrid = grid
                .Where(d => !d.RaceStatus.HasCrashed)
                .OrderBy(d => d.RaceStatus.TotalTime)
                .ToList();

            if (sortedGrid.Count == 0) return;

            Console.WriteLine(" [RC] Stawka zbita za Safety Carem...");

            // Resetujemy różnice czasowe
            double prevCarTime = sortedGrid[0].RaceStatus.TotalTime;

            for (int i = 1; i < sortedGrid.Count; i++)
            {
                // Bardzo małe odstępy (0.2s - 0.5s)
                double gap = 0.2 + (_rng.NextDouble() * 0.3);
                sortedGrid[i].RaceStatus.TotalTime = prevCarTime + gap;
                prevCarTime = sortedGrid[i].RaceStatus.TotalTime;
            }
        }
    }
}