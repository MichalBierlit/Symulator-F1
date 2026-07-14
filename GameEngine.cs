using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace F2
{
    public class GameEngine
    {
        private List<Driver> _grid;
        private List<RaceData> _calendar;
        private List<Driver> _playerDrivers;
        private Random _rng;

        private UserInterface _ui;
        private WeatherSystem _weather;
        private RaceControlSystem _raceControl;
        private ChampionshipSystem _championship;
        private PitStopMinigame _pitStopGame;
        private F1GridFactory _gridFactory;

        public GameEngine()
        {
            _rng = new Random();
            _ui = new UserInterface();
            _weather = new WeatherSystem();
            _raceControl = new RaceControlSystem();
            _championship = new ChampionshipSystem();
            _pitStopGame = new PitStopMinigame();
            _gridFactory = new F1GridFactory();

            _grid = _gridFactory.GetOfficialGrid();

            _calendar = new List<RaceData>
            {
                new RaceData("GP BAHRAINU", 41),
                new RaceData("GP ARABII", 43),
                new RaceData("GP AUSTRALII", 38),
                new RaceData("GP JAPONII", 39),
                new RaceData("GP MONACO", 46)
            };
        }

        public void Run()
        {
            SetupGame();
            foreach (var race in _calendar) RunRaceWeekend(race);
            _ui.ShowSeasonEnd();
        }

        private void SetupGame()
        {
            _ui.ShowWelcomeScreen(_calendar.Count);
            var teams = _grid.Select(d => d.Car.TeamName).Distinct().ToList();
            _ui.ShowTeamSelection(teams);
            int choice = _ui.GetIntInput(1, teams.Count);
            string selectedTeam = teams[choice - 1];
            _playerDrivers = _grid.Where(d => d.Car.TeamName == selectedTeam).ToList();
            foreach (var d in _playerDrivers) d.IsPlayer = true;
            _ui.ShowTeamConfirmation(selectedTeam, _playerDrivers[0].Name, _playerDrivers[1].Name);
        }

        private void RunRaceWeekend(RaceData race)
        {
            foreach (var d in _grid) d.ResetForRace();
            RunQualifying();
            _ui.ShowQualifyingResults(race.Name, _grid);
            _ui.WaitForRaceStart(race.Laps);
            RunRaceSession(race);
            _ui.ShowRaceEnd(race.Name);
            _raceControl.Reset();
            ProcessRaceResults();
        }

        private void RunQualifying()
        {
            Console.Clear();
            Console.WriteLine("Trwają kwalifikacje...");
            Thread.Sleep(1000);
            foreach (var d in _grid) d.RunQualifyingLap(_rng);
            _grid = _grid.OrderBy(d => d.QualifyingTime).ToList();
        }

        private void RunRaceSession(RaceData race)
        {
            _weather.UpdateWeather();
            Console.Clear();
            Console.WriteLine($"\n WARUNKI NA STARCIE: {_weather.GetDescription()}");
            Thread.Sleep(1500);

            // Startowy dobór opon
            foreach (var d in _grid)
            {
                if (d.IsPlayer)
                {
                    Console.WriteLine($"\nDecyzja dla: {d.Name}");
                    ITireStrategy chosen = _ui.GetStartingTyreChoice();
                    d.SetStartingTyres(chosen);
                }
                else d.PickStartingTyres(_weather.RainIntensity, _rng);
            }

            // PĘTLA WYŚCIGU - ZMIENIONA KOLEJNOŚĆ
            for (int lap = 1; lap <= race.Laps; lap++)
            {
                // 1. Czyszczenie ekranu na początku okrążenia
                Console.Clear();

                // 2. Nagłówek i informacje
                _ui.ShowRaceHeader(race.Name, lap, race.Laps);
                _weather.UpdateWeather();
                _raceControl.CheckForIncidents(_grid, _weather.RainIntensity);

                _ui.ShowTrackStatus(
                    _raceControl.IsSafetyCarActive,
                    _raceControl.SafetyCarLapsLeft,
                    _weather.GetDescription(),
                    (int)_weather.CloudDensity
                );

                // 3. DECYZJE GRACZA (przed jazdą i tabelą)
                Console.WriteLine("\n--- TWOJE DECYZJE PRZED OKRĄŻENIEM ---");
                foreach (var driver in _playerDrivers)
                {
                    HandlePlayerTurn(driver);
                }

                // 4. DECYZJE AI
                Console.WriteLine("\n--- DECYZJE POZOSTAŁYCH KIEROWCÓW ---");

                bool anyMessages = false;

                foreach (var d in _grid)
                {
                    if (!d.IsPlayer)
                    {
                        d.MakeAIDecision(lap, race.Laps, _raceControl.IsSafetyCarActive, _weather.RainIntensity);
                        // Jeśli MakeAIDecision coś wyświetla, ustaw flagę
                    }
                }

                // SPRAWDŹ CZY COŚ SIĘ WYŚWIETLIŁO
                // (prosta heurystyka - jeśli nie ma wypadków i jest deszcz, to pewnie były decyzje)
                if (_weather.RainIntensity > 20 || _grid.Any(d => !d.IsPlayer && d.RaceStatus.PitStopsCount > 0))
                {
                    anyMessages = true;
                }

                // ZAMIAST Thread.Sleep UŻYJ ENTERA TYLKO GDY BYŁY KOMUNIKATY
                if (anyMessages)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\n[ENTER] ");
                    Console.ResetColor();
                    Console.WriteLine("Kontynuuj...");
                    Console.ReadLine();
                }
                else
                {
                    // Jeśli nie było komunikatów, od razu przejdź dalej (bez pauzy)
                    Console.WriteLine(); // Tylko pusta linia dla estetyki
                }


                // 5. JAZDA WSZYSTKICH KIEROWCÓW
                Console.WriteLine("\n--- SYMULACJA OKRĄŻENIA ---");
                Thread.Sleep(800);
                foreach (var d in _grid)
                {
                    d.DriveLap(_rng, _weather.RainIntensity,_raceControl.IsSafetyCarActive);
                }

                // 6. TABELA PO OKRĄŻENIU
                var sortedGrid = _grid
                    .OrderBy(d => d.RaceStatus.HasCrashed ? 1 : 0)
                    .ThenBy(d => d.RaceStatus.TotalTime)
                    .ToList();

                _ui.ShowLiveTable(sortedGrid, sortedGrid[0]);

                // 7. PAUZA PRZED NASTĘPNYM OKRĄŻENIEM
                if (lap < race.Laps)
                {
                    Console.WriteLine("\n[ENTER] Przejdź do następnego okrążenia...");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("\n[ENTER] Zakończ wyścig...");
                    Console.ReadLine();
                }
            }
        }

        private void HandlePlayerTurn(Driver driver)
        {
            if (driver.RaceStatus.HasCrashed)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" > {driver.Name}: ROZBITY (Pomiń)");
                Console.ResetColor();
                return;
            }

            _ui.ShowDriverControls(driver);

            bool validChoice = false;
            while (!validChoice)
            {
                var key = Console.ReadKey(true).Key;
                ITireStrategy t = null;

                if (key == ConsoleKey.D1) // 1. Jazda
                {
                    Console.WriteLine(" -> Kontynuujesz jazdę");
                    validChoice = true;
                }
                else
                {
                    switch (key)
                    {
                        case ConsoleKey.D2: t = new SoftTyre(); break;
                        case ConsoleKey.D3: t = new MediumTyre(); break;
                        case ConsoleKey.D4: t = new HardTyre(); break;
                        case ConsoleKey.D5: t = new WetTyre(); break;
                    }

                    if (t != null)
                    {
                        Console.WriteLine($" -> PIT STOP: {t.Name.ToUpper()}");
                        Thread.Sleep(500);

                        double pitTime = _pitStopGame.Run();
                        bool success = driver.ChangeTyres(t, pitTime);

                        if (!success)
                        {
                            Console.WriteLine("   BŁĄD: Limit pit stopów!");
                            Thread.Sleep(1000);
                        }

                        validChoice = true;
                    }
                }
            }
        }

        private void ProcessRaceResults()
        {
            foreach (var d in _grid) if (!d.RaceStatus.HasCrashed) d.ApplyPostRacePenalty();

            var finalResults = _grid
                .OrderBy(d => d.RaceStatus.HasCrashed ? 1 : 0)
                .ThenBy(d => d.RaceStatus.TotalTime)
                .ToList();

            _championship.AwardPoints(finalResults);
            _championship.ShowStandings(_grid);
            _championship.ShowConstructorsStandings(_grid);
        }
    }
}