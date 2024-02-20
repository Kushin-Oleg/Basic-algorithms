#include <iostream>
#include <cmath>
#include <locale>

//функция нахождения подынтегральной функции
double f(double x) {
    return 1 / (1 - 0.49 * sin(x) * sin(x));
}

//функция для расчёта интеграла с помощью метода Симпсона
double simpson(double a, double b, int n) {
    double h = (b - a) / n;
    double sum = f(a) + f(b);
    for (int i = 1; i < n; i++) {
        if (i % 2 == 0) {
            sum += 2 * f(a + i * h);
        }
        else {
            sum += 4 * f(a + i * h);
        }
    }
    return (h / 3) * sum;
}

//функция для контроля погрешности по формуле Рунге
double runge(double a, double b, int n, double h) {
    double I1 = simpson(a, b, n);
    double I2 = simpson(a, b, 2 * n);
    return abs((I2 - I1) / (I2 - I1 * pow(2, 4)));
}

int main() {
    setlocale(LC_ALL, "Russian");
    double a, b;
    int n;
    double eps;

    std::cout << "Введите интервал интегрирования [a, b]: ";
    std::cin >> a >> b;
    std::cout << "Введите число разбиений n: ";
    std::cin >> n;
    std::cout << "Введите требуемую точность eps: ";
    std::cin >> eps;

    double h = (b - a) / n;
    double r = runge(a, b, n, h);

    while (r > eps) {
        n *= 2;
        h = (b - a) / n;
        r = runge(a, b, n, h);
    }

    double I = simpson(a, b, n);

    std::cout << "Интеграл: " << I;
}
