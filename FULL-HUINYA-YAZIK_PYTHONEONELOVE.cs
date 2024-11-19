using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

class Task {
    class City {
        public string CityName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    static List<City> OpenFile(string fileName) {
        try {
            string json = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<List<City>>(json) ?? new List<City>();
        }
        catch (Exception ex) {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            return new List<City>();
        }
    }

    static string CitiesInRound(List<City> library, double xOfShield, double yOfShield, double radOfShield) {
        var protectedCities = library
            .Where(city => Math.Pow(city.X - xOfShield, 2) + Math.Pow(city.Y - yOfShield, 2) <= Math.Pow(radOfShield, 2))
            .Select(city => city.CityName)
            .ToList();

        return protectedCities.Count != 0
            ? string.Join(", ", protectedCities)
            : "Ни один город не защищен.";
    }

    static (double optimalX, double optimalY, double optimalRadius, List<string> maxCities) FindOptimalShieldPosition(List<City> library, double maxRadius) {
        double optimalX = 0, optimalY = 0, optimalRadius = 0;
        List<string> maxCities = new();

        for (int i = 0; i < library.Count; i++) {
            for (int j = i + 1; j < library.Count; j++) {
                var city1 = library[i];
                var city2 = library[j];

                double x1 = city1.X, y1 = city1.Y;
                double x2 = city2.X, y2 = city2.Y;

                double centerX = (x1 + x2) / 2;
                double centerY = (y1 + y2) / 2;
                double radius = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)) / 2;

                if (radius > maxRadius)
                    continue;

                var protectedCities = library
                    .Where(city => Math.Pow(city.X - centerX, 2) + Math.Pow(city.Y - centerY, 2) <= Math.Pow(radius, 2))
                    .Select(city => city.CityName)
                    .ToList();

                if (protectedCities.Count > maxCities.Count) {
                    maxCities = protectedCities;
                    optimalX = centerX;
                    optimalY = centerY;
                    optimalRadius = radius;
                }
            }
        }

        foreach (var city in library) {
            var protectedCities = library
                .Where(c => Math.Pow(c.X - city.X, 2) + Math.Pow(c.Y - city.Y, 2) <= Math.Pow(maxRadius, 2))
                .Select(c => c.CityName)
                .ToList();

            if (protectedCities.Count > maxCities.Count) {
                maxCities = protectedCities;
                optimalX = city.X;
                optimalY = city.Y;
                optimalRadius = maxRadius;
            }
        }

        return (optimalX, optimalY, optimalRadius, maxCities);
    }

    static (double xOfShield, double yOfShield, double radOfShield) GetCoords() {
        try {
            Console.WriteLine("Введите координаты и радиус щита (x y r): ");
            string[] inputs = Console.ReadLine()?.Split(' ');

            return (double.Parse(inputs[0]), double.Parse(inputs[1]), double.Parse(inputs[2]));
        }
        catch {
            Console.WriteLine("Ошибка: вводите координаты и радиус в формате 'x y r'.");
            return (0, 0, 0);
        }
    }

    static void Main() {
        var library = OpenFile("../../../cities.json");

        var userShieldCoords = GetCoords();
        Console.WriteLine($"Города, попадающие под щит с указанными данными:\n" +
                          $"{CitiesInRound(library, userShieldCoords.xOfShield, userShieldCoords.yOfShield, userShieldCoords.radOfShield)}\n");

        var (optimalX, optimalY, optimalRadius, maxCities) = FindOptimalShieldPosition(library, 100);

        Console.WriteLine("Оптимальное место для щита: \n" +
        $"Координаты: ({optimalX}, {optimalY})\n" +
        $"Радиус: {optimalRadius}\n" +
        $"Защищенные города: {string.Join(" ", maxCities)}");
    }
}
