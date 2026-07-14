namespace F2
{


    public interface IDrivingStrategy
    {
        double ApplyStyle(Random rng);
        string Name { get; }
    }




}


