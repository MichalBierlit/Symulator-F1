namespace F2
{
    public class Car
    {
        public string TeamName { get; set; }
        public int Performance { get; set; } // Tego pola brakowało

        // Ten konstruktor jest wymagany przez F1GridFactory
        public Car(string teamName, int performance)
        {
            TeamName = teamName;
            Performance = performance;
        }
    }
}