using System;
using System.Threading;

namespace F2
{
    public class RaceStatus
    {
        public double TotalTime { get; set; }
        public int PitStopsCount { get; set; }
        public bool HasCrashed { get; set; }
        public ITireStrategy CurrentTyres { get; private set; }
        public double CurrentTyreHealth { get; private set; }

        private bool _lowTyreWarningShown = false;

        public RaceStatus()
        {
            TotalTime = 0;
            PitStopsCount = 0;
            HasCrashed = false;
            CurrentTyres = new SoftTyre();
            CurrentTyreHealth = 100.0;
        }

        public void SetStartingTyres(ITireStrategy newTyres)
        {
            CurrentTyres = newTyres;
            CurrentTyreHealth = 100.0;
        }

        public void SimulateLap(int driverSkill, int carPerformance, int rainIntensity, Random rng, bool isSafetyCar)
        {
            if (HasCrashed) return;

            // --- SAFETY CAR ---
            if (isSafetyCar)
            {
                TotalTime += 120.0; // Wolne tempo
                CurrentTyreHealth -= 0.5; // Minimalne zużycie
                if (CurrentTyreHealth < 0) CurrentTyreHealth = 0;
                return;
            }

            // --- 1. SPRAWDZENIE KRYTYCZNE (Opony 0%) ---
            if (CurrentTyreHealth <= 0)
            {
                HasCrashed = true;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" [CRASH] Opony wybuchły!");
                Console.ResetColor();
                return;
            }

            // --- 2. LOSOWY BŁĄD KIEROWCY (NOWOŚĆ) ---
            // Nawet na dobrych oponach można wypaść.
            // Bazowo: 0.3% szansy na okrążenie.
            // W deszczu: rośnie drastycznie (nawet do 2-3%).
            // Słabi kierowcy (skill < 80) mają większą szansę.

            double crashChance = 0.3;
            crashChance += (rainIntensity * 0.5); // Deszcz zwiększa ryzyko
            if (driverSkill < 85) crashChance += 0.2; // Słabi kierowcy częściej dzwonią

            if (rng.NextDouble() * 100 < crashChance)
            {
                HasCrashed = true;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" [CRASH] Błąd kierowcy / Poślizg!");
                Console.ResetColor();
                return;
            }

            // --- 3. STANDARDOWA FIZYKA ---
            double wear = CurrentTyres.CalculateWear(rainIntensity, rng);
            CurrentTyreHealth -= wear;

            // Ryzyko przy niskich oponach (<15%) - zwiększone
            if (CurrentTyreHealth < 15)
            {
                double risk = (15 - CurrentTyreHealth) * 2.0;
                if (rng.NextDouble() * 100 < risk)
                {
                    HasCrashed = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" [CRASH] Utrata przyczepności (zużyte opony)!");
                    Console.ResetColor();
                    return;
                }
            }

            if (CurrentTyreHealth < 0) CurrentTyreHealth = 0;

            // Czas okrążenia
            double lapTime = 90.0;
            lapTime -= (driverSkill * 0.05);
            lapTime -= (carPerformance * 0.05);
            lapTime += (rng.NextDouble() * 2.0) - 1.0;

            double tireFactor = CurrentTyres.CalculatePerformance(rainIntensity, CurrentTyreHealth);
            lapTime *= tireFactor;

            if (CurrentTyreHealth < 20)
            {
                lapTime += (20 - CurrentTyreHealth) * 0.08; // Mocniejsza kara za zużycie
            }

            TotalTime += lapTime;
        }

        public bool ChangeTyres(ITireStrategy newTyres, double pitTime)
        {
            if (PitStopsCount >= 6) return false;
            CurrentTyres = newTyres;
            CurrentTyreHealth = 100.0;
            TotalTime += pitTime;
            PitStopsCount++;
            _lowTyreWarningShown = false;
            return true;
        }
    }
}