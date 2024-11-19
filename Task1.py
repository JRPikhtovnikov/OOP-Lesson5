import json
from itertools import combinations


def open_file(file_name: str):
    with open(file_name, 'r', encoding='utf-8') as file:
        library = json.load(file)
    return library


def cities_in_round(library: list, x_of_shield: float, y_of_shield: float, rad_of_shield: float) -> list | str:
    protected_cities = []
    for city in library:
        if (city["x"] - x_of_shield) ** 2 + (city["y"] - y_of_shield) ** 2 <= rad_of_shield ** 2:
            protected_cities.append(city["city"])
    if protected_cities:
        return " ".join(protected_cities)
    return "Ни один город не защищен."


def find_optimal_shield_position(library: list, max_radius: float) -> tuple:
    optimal_x, optimal_y, optimal_radius = 0, 0, 0
    max_cities = []

    for city1, city2 in combinations(library, 2):
        x1, y1 = city1["x"], city1["y"]
        x2, y2 = city2["x"], city2["y"]

        center_x, center_y = (x1 + x2) / 2, (y1 + y2) / 2
        radius = ((x1 - x2) ** 2 + (y1 - y2) ** 2) ** 0.5 / 2

        if radius > max_radius:
            continue

        protected_cities = cities_in_round(library, center_x, center_y, radius)

        if len(protected_cities) > len(max_cities):
            max_cities = protected_cities
            optimal_x, optimal_y, optimal_radius = center_x, center_y, radius

    return optimal_x, optimal_y, optimal_radius, max_cities


def get_coords() -> tuple:
    try:
        x_of_shield, y_of_shield, rad_of_shield = list(map(float, input("Введите координаты и радиус щита: ").split()))
        return x_of_shield, y_of_shield, rad_of_shield
    except ValueError:
        print("Ошибка: вводите координаты и радиус в формате 'x y r'.")
        return 0, 0, 0


def main():
    library = open_file("cities.json")

    user_shield_coords = get_coords()
    print(f"Города, попадающие под щит с указаными данными: \n"
          f"{cities_in_round(open_file("cities.json"), 
           user_shield_coords[0], user_shield_coords[1], user_shield_coords[2])} \n")

    optimal_x, optimal_y, optimal_radius, max_cities = find_optimal_shield_position(library, 100)

    print("Оптимальное место для щита: \n"
    f"Координаты: ({optimal_x}, {optimal_y}) \n"
    f"Радиус: {optimal_radius} \n"
    f"Защищенные города: {max_cities}")

if __name__ == '__main__':
    main()