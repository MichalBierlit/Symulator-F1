using System;

namespace F2
{
    public interface ITireStrategy
    {
        double CalculatePerformance(int rain, double currentHealth);
        double CalculateWear(int rain, Random rng);
        string Name { get; }
        ConsoleColor Color { get; }
    }

    // --- SOFT ---
    public class SoftTyre : ITireStrategy
    {
        public string Name => "Soft";
        public ConsoleColor Color => ConsoleColor.Red;

        public double CalculatePerformance(int rain, double health)
        {
            if (rain > 30) return 1.40;
            if (rain > 15) return 1.25;

            double degradation = (100 - health) * 0.0015;
            return 0.95 + degradation;
        }

        public double CalculateWear(int rain, Random rng)
        {
            if (rain > 30) return 5.0 + rng.NextDouble() * 2.0;
            if (rain > 15) return 7.0 + rng.NextDouble() * 2.0;

            // **WIĘKSZE ZUŻYCIE NA KOŃCU ŻYCIA OPON**
            double baseWear = 10.0;
            return baseWear + (rng.NextDouble() * 4.0 - 2.0);
        }
    }

    // --- MEDIUM ---
    public class MediumTyre : ITireStrategy
    {
        public string Name => "Medium";
        public ConsoleColor Color => ConsoleColor.Yellow;

        public double CalculatePerformance(int rain, double health)
        {
            if (rain > 40) return 1.30;
            if (rain > 20) return 1.15;

            double degradation = (100 - health) * 0.0012;
            return 1.00 + degradation;
        }

        public double CalculateWear(int rain, Random rng)
        {
            if (rain > 30) return 4.0 + rng.NextDouble() * 2.0;
            if (rain > 15) return 5.0 + rng.NextDouble() * 2.0;
            return 6.0 + rng.NextDouble() * 2.0;
        }
    }

    // --- HARD ---
    public class HardTyre : ITireStrategy
    {
        public string Name => "Hard";
        public ConsoleColor Color => ConsoleColor.White;

        public double CalculatePerformance(int rain, double health)
        {
            if (rain > 50) return 1.20;
            if (rain > 25) return 1.10;

            double degradation = (100 - health) * 0.0008;
            return 1.05 + degradation;
        }

        public double CalculateWear(int rain, Random rng)
        {
            if (rain > 30) return 2.0 + rng.NextDouble() * 1.0;
            return 3.0 + rng.NextDouble() * 1.0;
        }
    }

    // --- WET --- (NAJWAŻNIEJSZE POPRAWKI)
    public class WetTyre : ITireStrategy
    {
        public string Name => "Wet";
        public ConsoleColor Color => ConsoleColor.Cyan;

        public double CalculatePerformance(int rain, double health)
        {
            // OPONY DESZCZOWE: Im więcej deszczu, tym SZYBCIEJ!
            if (rain > 60) return 0.92;
            if (rain > 40) return 0.95;
            if (rain > 20) return 1.00;

            // Na suchym - KATASTROFALNIE WOLNE
            double dryPenalty = (20 - rain) * 0.03;
            return 1.20 + dryPenalty;
        }

        public double CalculateWear(int rain, Random rng)
        {
            // **NA SUCHYM ZUŻYWAJĄ SIĘ BARDZO SZYBKO**
            if (rain < 20) return 20.0 + rng.NextDouble() * 10.0;
            return 1.5 + rng.NextDouble() * 1.0;
        }
    }
}