using System;
using System.Collections.Generic;
using System.Linq;

namespace F2
{
    public class ChampionshipSystem
    {
        private readonly int[] _pointsSystem = { 25, 18, 15, 12, 10, 8, 6, 4, 2, 1 };

        public void AwardPoints(List<Driver> raceResults)
        {
            for (int i = 0; i < raceResults.Count; i++)
            {
                if (i < _pointsSystem.Length && !raceResults[i].RaceStatus.HasCrashed)
                {
                    raceResults[i].SeasonPoints += _pointsSystem[i];
                }
            }
        }

        public void ShowStandings(List<Driver> allDrivers)
        {
            Console.WriteLine("\n=== KLASYFIKACJA KIEROWCÓW ===");
            var sorted = allDrivers.OrderByDescending(d => d.SeasonPoints).ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                string marker = sorted[i].IsPlayer ? "*" : " ";
                Console.WriteLine($"{i + 1,2}. {marker} {sorted[i].Name,-20} - {sorted[i].SeasonPoints,3} pkt");
            }
        }

        // --- NOWA METODA: KLASYFIKACJA KONSTRUKTORÓW ---
        public void ShowConstructorsStandings(List<Driver> allDrivers)
        {
            Console.WriteLine("\n=== KLASYFIKACJA KONSTRUKTORÓW ===");

            // 1. Grupujemy kierowców po nazwie zespołu
            var teams = allDrivers
                .GroupBy(d => d.Car.TeamName)
                .Select(group => new
                {
                    TeamName = group.Key,
                    // Sumujemy punkty kierowców w tym zespole
                    TotalPoints = group.Sum(d => d.SeasonPoints),
                    // Sprawdzamy, czy gracz jest w tym zespole (żeby go podświetlić)
                    IsPlayerTeam = group.Any(d => d.IsPlayer)
                })
                .OrderByDescending(t => t.TotalPoints) // Sortujemy od najlepszego
                .ToList();

            // 2. Wyświetlamy tabelę
            for (int i = 0; i < teams.Count; i++)
            {
                var t = teams[i];

                if (t.IsPlayerTeam) Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($"{i + 1,2}. {t.TeamName,-20} - {t.TotalPoints,3} pkt");

                Console.ResetColor();
            }

            Console.WriteLine("------------------------------");
            Console.WriteLine("[ENTER] Zakończ weekend...");
            Console.ReadLine();
        }
    }
}