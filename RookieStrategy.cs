namespace F2
{
    public class RookieStrategy : IDrivingStrategy
    {
        public string Name => "Ostrożny";
        // Wolniejszy (dodaje czas)
        public double ApplyStyle(Random rng) => 1.02 - (rng.NextDouble() * 0.01);
    }
}
