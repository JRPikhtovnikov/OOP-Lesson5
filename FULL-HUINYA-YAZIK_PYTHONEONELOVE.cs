using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;
using System.Text.Json;

public class City {
    public string CityName { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}


class Program {
    static List<City> OpenFile(string fileName) {
        string json = File.ReadAllText(fileName);
        return JsonSerializer.Deserialize<List<City>>(json);
    }

    static List<string> CitiesInRound(List<City> cities, double x, double y, double radius) {
        return cities.Where(city => Math.Pow(city.X - x, 2) + Math.Pow(city.Y - y, 2) <= Math.Pow(radius, 2))
                     .Select(city => city.CityName)
                     .ToList();
    }

    static List<City> FindNotDefended(List<City> cities, double x, double y, double radius) {
        var defendedCities = CitiesInRound(cities, x, y, radius);
        return cities.Where(city => !defendedCities.Contains(city.CityName)).ToList();
    }

    static bool IsCityDefended(List<City> cities, string cityName, double x, double y, double radius) {
        var city = cities.FirstOrDefault(c => c.CityName.Equals(cityName, StringComparison.OrdinalIgnoreCase));
        if (city != null) {
            return Math.Pow(city.X - x, 2) + Math.Pow(city.Y - y, 2) <= Math.Pow(radius, 2);
        }
        return false;
    }

    static (double x, double y, double radius, List<string> cities) FindOptimalShieldPosition(List<City> cities, double maxRadius) {
        double optimalX = 0, optimalY = 0, optimalRadius = 0;
        List<string> maxCities = new List<string>();

        for (int i = 0; i < cities.Count; i++) {
            for (int j = i + 1; j < cities.Count; j++) {
                var city1 = cities[i];
                var city2 = cities[j];

                double centerX = (city1.X + city2.X) / 2;
                double centerY = (city1.Y + city2.Y) / 2;
                double radius = Math.Sqrt(Math.Pow(city1.X - city2.X, 2) + Math.Pow(city1.Y - city2.Y, 2)) / 2;

                if (radius > maxRadius)
                    continue;

                var protectedCities = CitiesInRound(cities, centerX, centerY, radius);

                if (protectedCities.Count > maxCities.Count) {
                    maxCities = protectedCities;
                    optimalX = centerX;
                    optimalY = centerY;
                    optimalRadius = radius;
                }
            }
        }

        return (optimalX, optimalY, optimalRadius, maxCities);
    }

    static void UserAction(List<City> cities) {
        var actions = new Dictionary<string, Action>
        {
            { "FindNotDefended", () => FindNotDefendedAction(cities) },
            { "Defended", () => DefendedAction(cities) },
            { "OptimalDefendPosition", () => OptimalDefendPositionAction(cities) },
            { "Exit", () => ExitAction() }
        };

        while (true) {
            WriteLine("\nВыберите действие:");
            foreach (var action in actions.Keys) {
                WriteLine($"- {action}");
            }

            string userInput = ReadLine()?.Trim();
            if (actions.ContainsKey(userInput)) {
                actions[userInput].Invoke();
            }
            else {
                WriteLine("Неизвестная команда. Попробуйте снова.");
            }
        }
    }

    static void FindNotDefendedAction(List<City> cities) {
        Write("Введите координаты и радиус щита (x y r): ");
        var input = ReadLine()?.Split();
        if (input != null && input.Length == 3 &&
            double.TryParse(input[0], out double x) &&
            double.TryParse(input[1], out double y) &&
            double.TryParse(input[2], out double radius)) {
            var notDefendedCities = FindNotDefended(cities, x, y, radius);
            if (notDefendedCities.Any()) {
                WriteLine("Незащищенные города:");
                foreach (var city in notDefendedCities) {
                    WriteLine($"{city.CityName} ({city.X}, {city.Y})");
                }
            }
            else {
                WriteLine("Все города защищены.");
            }
        }
        else {
            WriteLine("Ошибка ввода! Укажите данные в формате x y r.");
        }
    }

    static void DefendedAction(List<City> cities) {
        Write("Введите название города: ");
        string cityName = ReadLine();
        Write("Введите координаты и радиус щита (x y r): ");
        var input = ReadLine()?.Split();
        if (input != null && input.Length == 3 &&
            double.TryParse(input[0], out double x) &&
            double.TryParse(input[1], out double y) &&
            double.TryParse(input[2], out double radius)) {
            if (IsCityDefended(cities, cityName, x, y, radius)) {
                WriteLine($"Город {cityName} защищён.");
            }
            else {
                WriteLine($"Город {cityName} не защищён.");
            }
        }
        else {
            WriteLine("Ошибка ввода! Укажите данные в формате x y r.");
        }
    }

    static void OptimalDefendPositionAction(List<City> cities) {
        Write("Введите максимальный радиус щита: ");
        if (double.TryParse(ReadLine(), out double maxRadius)) {
            var (x, y, radius, defendedCities) = FindOptimalShieldPosition(cities, maxRadius);
            WriteLine("Оптимальное место для щита:\n" +
            $"Координаты: ({x}, {y})\n" +
            $"Радиус: {radius}OptimalDefendPosition\n" +
            $"Защищенные города: {string.Join(", ", defendedCities)}");
        }
        else {
            WriteLine("Ошибка ввода! Укажите допустимое значение радиуса.");
        }
    }

    static void ExitAction() {
        WriteLine("Выход из программы.");
        Environment.Exit(0);
    }

    static void Main() {
        var cities = OpenFile("../../../cities.json");
        UserAction(cities);
    }
}
