using System;

namespace F2
{
    public class Driver
    {
        public string Name { get; set; }
        public int Skill { get; set; }
        public int SeasonPoints { get; set; }
        public Car Car { get; set; }
        public RaceStatus RaceStatus { get; set; }
        public bool IsPlayer { get; set; }
        public double QualifyingTime { get; set; }

        private int _rainThreshold = 30;
        private Random _personalRng;

        public Driver(string name, int skill, Car car, ITireStrategy startingTyre = null)
        {
            Name = name;
            Skill = skill;
            Car = car;
            RaceStatus = new RaceStatus();
            IsPlayer = false;
            _personalRng = new Random(name.GetHashCode());

            // Losowy próg deszczu dla AI (20-50)
            _rainThreshold = 20 + _personalRng.Next(0, 31);

            if (startingTyre != null)
            {
                RaceStatus.SetStartingTyres(startingTyre);
                RaceStatus.PitStopsCount = 0;
            }
        }

        public void ResetForRace()
        {
            RaceStatus = new RaceStatus();
        }

        public void RunQualifyingLap(Random rng)
        {
            double baseTime = 80.0;
            double carFactor = (100 - Car.Performance) * 0.5;
            double skillFactor = (100 - Skill) * 0.3;
            double randomVar = rng.NextDouble() * 1.5;
            QualifyingTime = baseTime + carFactor + skillFactor + randomVar;
        }

        public void PickStartingTyres(int rainIntensity, Random rng)
        {
            ITireStrategy chosen;

            if (rainIntensity > 40)
            {
                chosen = new WetTyre();
            }
            else if (rainIntensity > 20)
            {
                if (rng.Next(100) < 70) chosen = new WetTyre();
                else chosen = new MediumTyre();
            }
            else
            {
                int choice = rng.Next(100);
                if (choice < 60) chosen = new SoftTyre();
                else if (choice < 85) chosen = new MediumTyre();
                else chosen = new HardTyre();
            }

            RaceStatus.SetStartingTyres(chosen);
            RaceStatus.PitStopsCount = 0;
        }
        public void SetStartingTyres(ITireStrategy tyres)
        {
            // Przekazujemy polecenie do RaceStatus
            RaceStatus.SetStartingTyres(tyres);
        }
        // **POPRAWIONE - AI ZMIENIA OPONY PRZED 0%**
        public void MakeAIDecision(int currentLap, int totalLaps, bool isSafetyCarActive, int rainIntensity)
        {
            if (RaceStatus.HasCrashed || RaceStatus.PitStopsCount >= 4)
                return;

            var currentTyres = RaceStatus.CurrentTyres;
            var health = RaceStatus.CurrentTyreHealth;

            // **WAŻNE: AI ZMIENIA OPONY PRZY 25%, NIE CZEKA DO 0%**
            double safetyThreshold = 25.0; // AI zmienia przy 25%, aby uniknąć ryzyka

            // Jeśli opony poniżej progu bezpieczeństwa - PIERWSZY PRIORYTET
            if (health < safetyThreshold && !(currentTyres is WetTyre && rainIntensity > 30))
            {
                ITireStrategy newTyre = SelectDryTyreForLaps(totalLaps - currentLap, rainIntensity);
                if (RaceStatus.ChangeTyres(newTyre, 20.0))
                {
                    Console.WriteLine($"{Name} zmienia opony (tylko {health:F0}%)");
                }
                return;
            }

            // 1. SPRAWDŹ CZY POTRZEBA OPON DESZCZOWYCH
            bool isWetTyre = currentTyres is WetTyre;

            if (rainIntensity > _rainThreshold && !isWetTyre)
            {
                if (RaceStatus.ChangeTyres(new WetTyre(), 20.0))
                {
                    Console.WriteLine($"{Name} zmienia na opony deszczowe");
                }
                return;
            }

            // 2. ZMIANA Z OPON DESZCZOWYCH NA SUCHE
            if (rainIntensity < 15 && isWetTyre)
            {
                ITireStrategy dryTyre = SelectDryTyreForLaps(totalLaps - currentLap, 0);
                if (RaceStatus.ChangeTyres(dryTyre, 20.0))
                {
                    Console.WriteLine($"{Name} wraca na opony suche");
                }
                return;
            }

            // 3. OSTATNIE 10 OKRĄŻEŃ - MOŻLIWOŚĆ FINISZU NA SOFTACH
            if (totalLaps - currentLap <= 10 && health > 40 && !isWetTyre)
            {
                if (!(currentTyres is SoftTyre))
                {
                    if (RaceStatus.ChangeTyres(new SoftTyre(), 20.0))
                    {
                        Console.WriteLine($"{Name} zmienia na Softy na finisz");
                    }
                }
            }

        }

        private ITireStrategy SelectDryTyreForLaps(int lapsLeft, int rainIntensity)
        {
            if (rainIntensity > 20) return new MediumTyre(); // W deszczu bezpieczniejsze

            if (lapsLeft <= 12) return new SoftTyre();
            if (lapsLeft <= 25) return new MediumTyre();
            return new HardTyre();
        }

        public void DriveLap(Random rng, int rainIntensity, bool isSafetyCar)
        {
            // Przekazujemy flagę SC do fizyki
            RaceStatus.SimulateLap(this.Skill, this.Car.Performance, rainIntensity, rng, isSafetyCar);
        }

        public bool ChangeTyres(ITireStrategy newTyres, double pitTime)
        {
            return RaceStatus.ChangeTyres(newTyres, pitTime);
        }

        public void ApplyPostRacePenalty() { }
    }
}