namespace F2
{
    public struct RaceData
    {
        public string Name { get; set; }
        public int Laps { get; set; }

        public RaceData(string name, int laps)
        {
            Name = name;
            Laps = laps;
        }
    }
}