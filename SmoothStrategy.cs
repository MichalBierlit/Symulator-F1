namespace F2
{
    public class SmoothStrategy : IDrivingStrategy
    {
        public string Name => "Płynny";
        // Stabilny, przewidywalny czas
        public double ApplyStyle(Random rng) => 1.00 - (rng.NextDouble() * 0.01);
    }
}
