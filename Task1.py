import json
from itertools import combinations


def open_file(file_name: str):
    with open(file_name, 'r', encoding='utf-8') as file:
        return json.load(file)


def cities_in_round(library: list, x_of_shield: float, y_of_shield: float, rad_of_shield: float) -> list:
    protected_cities = []
    for city in library:
        if (city["x"] - x_of_shield) ** 2 + (city["y"] - y_of_shield) ** 2 <= rad_of_shield ** 2:
            protected_cities.append(city["city"])
    return protected_cities


def find_not_defended(library: list, x_of_shield: float, y_of_shield: float, rad_of_shield: float) -> list:
    defended_cities = cities_in_round(library, x_of_shield, y_of_shield, rad_of_shield)
    not_defended = [city for city in library if city["city"] not in defended_cities]
    return not_defended


def is_city_defended(library: list, city_name: str, x_of_shield: float, y_of_shield: float, rad_of_shield: float) -> bool:
    for city in library:
        if city["city"] == city_name:
            return (city["x"] - x_of_shield) ** 2 + (city["y"] - y_of_shield) ** 2 <= rad_of_shield ** 2
    return False


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


def user_action(library: list):
    actions = {
        "FindNotDefended": lambda: find_not_defended_action(library),
        "Defended": lambda: defended_action(library),
        "OptimalDefendPosition": lambda: optimal_defend_position_action(library),
        "Exit": lambda: exit_action()
    }

    while True:
        print("\nВыберите действие:")
        for key in actions.keys():
            print(f"- {key}")

        action = input("> ").strip()

        if action in actions:
            actions[action]()
        else:
            print("Неизвестная команда. Попробуйте снова.")


def find_not_defended_action(library: list):
    try:
        x_of_shield, y_of_shield, rad_of_shield = map(float, input("Введите координаты и радиус щита (x y r): ").split())
        not_defended_cities = find_not_defended(library, x_of_shield, y_of_shield, rad_of_shield)
        if not_defended_cities:
            print("Незащищенные города:")
            for city in not_defended_cities:
                print(f"{city['city']} ({city['x']}, {city['y']})")
        else:
            print("Все города защищены.")
    except ValueError:
        print("Ошибка ввода! Укажите данные в формате x y r.")


def defended_action(library: list):
    try:
        city_name = input("Введите название города: ")
        x_of_shield, y_of_shield, rad_of_shield = map(float, input("Введите координаты и радиус щита (x y r): ").split())
        if is_city_defended(library, city_name, x_of_shield, y_of_shield, rad_of_shield):
            print(f"Город {city_name} защищён.")
        else:
            print(f"Город {city_name} не защищён.")
    except ValueError:
        print("Ошибка ввода! Укажите данные в формате x y r.")


def optimal_defend_position_action(library: list):
    try:
        max_radius = float(input("Введите максимальный радиус щита: "))
        optimal_x, optimal_y, optimal_radius, max_cities = find_optimal_shield_position(library, max_radius)
        print("Оптимальное место для щита:")
        print(f"Координаты: ({optimal_x}, {optimal_y})")
        print(f"Радиус: {optimal_radius}")
        print(f"Защищенные города: {', '.join(max_cities)}")
    except ValueError:
        print("Ошибка ввода! Укажите допустимое значение радиуса.")


def exit_action():
    print("Выход из программы.")
    exit()


def main():
    library = open_file("cities.json")
    user_action(library)


if __name__ == "__main__":
    main()
