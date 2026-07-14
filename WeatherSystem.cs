using System;

namespace F2
{
    public class WeatherSystem
    {
        public int RainIntensity { get; private set; } // 0-100%
        public double CloudDensity { get; private set; } // 0-100%
        private Random _rng;

        // Trend pogodowy: -1 = poprawa, 0 = stabilnie, 1 = pogorszenie
        private int _weatherTrend = 0;
        private int _trendDuration = 0;
        private int _maxTrendDuration = 5;

        public WeatherSystem()
        {
            _rng = new Random();

            // Losowy start: 70% szans na słonecznie, 30% na pochmurno
            CloudDensity = _rng.Next(100) < 70 ? _rng.Next(0, 30) : _rng.Next(30, 60);
            CalculateIntensity();

            Console.WriteLine($"[POGODA] Start: {GetDescription()} ({RainIntensity}%)");
        }

        public void UpdateWeather()
        {
            // 1. Określ trend jeśli się skończył
            if (_trendDuration <= 0)
            {
                _weatherTrend = _rng.Next(-1, 2); // -1, 0, 1
                _trendDuration = _rng.Next(3, _maxTrendDuration + 1);
            }

            // 2. Oblicz zmianę w zależności od trendu
            double change;
            if (_weatherTrend == 1) // Pogarsza się
            {
                change = _rng.NextDouble() * 8.0 + 2.0; // +2% do +10%
            }
            else if (_weatherTrend == -1) // Poprawia się
            {
                change = -(_rng.NextDouble() * 6.0 + 2.0); // -2% do -8%
            }
            else // Stabilnie
            {
                change = (_rng.NextDouble() * 4.0) - 2.0; // -2% do +2%
            }

            // 3. Zmniejsz trendDuration
            _trendDuration--;

            // 4. Dodaj losowy element (10% szans na zmianę trendu)
            if (_rng.Next(100) < 10)
            {
                change += (_rng.NextDouble() * 10.0) - 5.0; // -5% do +5%
            }

            CloudDensity += change;

            // 5. Naturalne granice (0-100%) z "przyciąganiem" do środka
            if (CloudDensity < 0) CloudDensity = 0;
            if (CloudDensity > 100) CloudDensity = 100;

            // 6. Tendencja do stabilizacji (jeśli blisko krańców)
            if (CloudDensity > 80 && _weatherTrend == 1)
            {
                // Zmniejsz szansę na dalsze pogorszenie przy wysokim zachmurzeniu
                if (_rng.Next(100) < 60) _weatherTrend = -1;
            }
            else if (CloudDensity < 20 && _weatherTrend == -1)
            {
                // Zmniejsz szansę na dalszą poprawę przy bardzo małym zachmurzeniu
                if (_rng.Next(100) < 60) _weatherTrend = 1;
            }

            CalculateIntensity();
        }

        private void CalculateIntensity()
        {
            // Nowe progi - bardziej realistyczne
            if (CloudDensity < 40)
            {
                RainIntensity = 0; // Słonecznie/pochmurno
            }
            else if (CloudDensity < 55)
            {
                RainIntensity = _rng.Next(0, 10); // 0-10% - możliwe mżawki
            }
            else if (CloudDensity < 70)
            {
                RainIntensity = _rng.Next(10, 30); // 10-30% - lekki deszcz
            }
            else if (CloudDensity < 85)
            {
                RainIntensity = _rng.Next(30, 60); // 30-60% - deszcz
            }
            else
            {
                RainIntensity = _rng.Next(60, 100); // 60-100% - ulewa
            }
        }

        public string GetDescription()
        {
            if (RainIntensity == 0)
            {
                if (CloudDensity < 20) return "SŁONECZNIE";
                if (CloudDensity < 40) return "CZĘŚCIOWO PCHMURNO";
                return "POCHMURNO";
            }
            if (RainIntensity < 10) return "MGŁA/MŻAWKA";
            if (RainIntensity < 25) return "LEKKI DESZCZ";
            if (RainIntensity < 45) return "DESZCZ";
            if (RainIntensity < 70) return "MOCNY DESZCZ";
            return "ULEWA";
        }

        // Nowa metoda: szczegółowy raport pogodowy
        public void ShowWeatherReport()
        {
            Console.WriteLine("\n--- RAPORT POGODOWY ---");
            Console.WriteLine($"Zachmurzenie: {CloudDensity:F1}%");
            Console.WriteLine($"Intensywność opadów: {RainIntensity}%");
            Console.WriteLine($"Opis: {GetDescription()}");

            string trend = _weatherTrend switch
            {
                1 => "pogarsza się ",
                -1 => "poprawia się ",
                _ => "stabilna "
            };
            Console.WriteLine($"Trend: {trend} (za {_trendDuration} okrążeń)");
            Console.WriteLine("------------------------");
        }
    }
}