#include <iostream>

int main(void) {
    std::cout << "Enter a number: ";
    int x{0};
    std::cin >> x;
    std::cout << "You entered " << x << '\n';
    return (0);
}