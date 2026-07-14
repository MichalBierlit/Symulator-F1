using System.Collections.Generic;

namespace F2
{
    public class F1GridFactory
    {
        public List<Driver> GetOfficialGrid()
        {
            var drivers = new List<Driver>();

            // 1. RED BULL RACING
            // Isack Hadjar awansuje do głównego zespołu
            drivers.Add(new Driver("Max Verstappen", 99, new Car("Red Bull Racing", 98)));
            drivers.Add(new Driver("Isack Hadjar", 85, new Car("Red Bull Racing", 98)));

            // 2. FERRARI
            drivers.Add(new Driver("Charles Leclerc", 94, new Car("Ferrari", 97)));
            drivers.Add(new Driver("Lewis Hamilton", 95, new Car("Ferrari", 97)));

            // 3. MCLAREN
            drivers.Add(new Driver("Lando Norris", 93, new Car("McLaren", 96)));
            drivers.Add(new Driver("Oscar Piastri", 89, new Car("McLaren", 96)));

            // 4. MERCEDES
            drivers.Add(new Driver("George Russell", 91, new Car("Mercedes", 94)));
            drivers.Add(new Driver("Kimi Antonelli", 83, new Car("Mercedes", 94)));

            // 5. ASTON MARTIN
            drivers.Add(new Driver("Fernando Alonso", 92, new Car("Aston Martin", 88)));
            drivers.Add(new Driver("Lance Stroll", 79, new Car("Aston Martin", 88)));

            // 6. ALPINE
            drivers.Add(new Driver("Pierre Gasly", 85, new Car("Alpine", 84)));
            drivers.Add(new Driver("Jack Doohan", 78, new Car("Alpine", 84)));

            // 7. WILLIAMS
            drivers.Add(new Driver("Alex Albon", 87, new Car("Williams", 83)));
            drivers.Add(new Driver("Carlos Sainz", 90, new Car("Williams", 83)));

            // 8. RB (Racing Bulls)
            // Debiutuje Arvid Lindblad
            drivers.Add(new Driver("Yuki Tsunoda", 84, new Car("RB Honda", 82)));
            drivers.Add(new Driver("Arvid Lindblad", 77, new Car("RB Honda", 82)));

            // 9. HAAS
            drivers.Add(new Driver("Esteban Ocon", 85, new Car("Haas", 81)));
            drivers.Add(new Driver("Ollie Bearman", 79, new Car("Haas", 81)));

            // 10. SAUBER (AUDI PRELUDE)
            drivers.Add(new Driver("Nico Hulkenberg", 86, new Car("Sauber", 78)));
            drivers.Add(new Driver("Gabriel Bortoleto", 80, new Car("Sauber", 78)));

            // 11. CADILLAC F1 TEAM
            drivers.Add(new Driver("Valtteri Bottas", 88, new Car("Cadillac F1", 86)));
            drivers.Add(new Driver("Sergio Perez", 85, new Car("Cadillac F1", 86)));

            return drivers;
        }
    }
}