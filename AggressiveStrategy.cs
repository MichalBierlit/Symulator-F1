using F2;

public class AggressiveStrategy : IDrivingStrategy
{
    public string Name => "Brawurowy";
    // Szybki (odejmuje czas), ale mniej stabilny
    public double ApplyStyle(Random rng) => 0.98 - (rng.NextDouble() * 0.03);
}