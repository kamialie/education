# C++

# Contents

+ [General](#general)
+ [Variables and data types](#variables-and-data-types)

# General

Hello world example:
```c++
#include <iostream>
using namespace std;

int main(void) {
	cout << "Hello, world";
	return (0);
}
```

## Compiler

Common flags:
+ `-pedantic-errors` flag to disable compiler extensions for gcc/g++
+ `-std=c++11` to indicate what standard to support - need this for uniform
initialization
+ `-Wall -Weffc++ -Wextra -Wsign-conversion` - gcc/g++ flags to increase warning
level
+ `-Werror` - gcc/g++ flag to treat warnings as errors

## Comments

Multiline comments (cannot be nested):
```c++
/* This is a multiline comment
 * with some text 1
 * with some text 2
 */
```

Proper use of comments
+ at the library, program, or function level describe how
+ inside the library, program, or function level describe how
+ at the statement level describe why
+ show why one decision was made over the other


# Variables and data types

C++ supports 3 ways to initialize a variable:
+ `int width = 5;` - copy initialization
+ `int width(5);` - direct initialization
+ `int width{5};` - uniform(brace) initialization, good for disallowing
"narrowing" conversions, for example int width{4.5} wil cause a compilation
error because of type error (copy and direct would just drop fractional part)
+ `int width{};` - zero initialization

Best practices:
+ favor uniform initialization
+ use explicit zero initialization for direct usage:
```c++
int x{0};
std::cout << x;
```
+ use zero initialization if the value is later replaced:
```c++
int x{};
std::cin >> x;
```

Initializing multiple variables, valid examples:
```c++
int a = 5, b = 5;
int c(5), d(5);
int e{5}, f{5};
```

## Data types

Common types:
```c++
#include <string>

int number = 1;
double d = -1.5;
char symbol = 'A';
string str = "example"; // requires string header file
boolen status = false;
```

### Vector

```c++
#include <vector>

vector<int> nums = {1, 2, 3};
cout << nums.size()
```

---

### Map

```c++
#include <map>

map<string, int> n_to_v;
n_to_v["one"] = 1;
n_to_v["two"] = 2;
cout << "number is" << n_to_v["two"];
```

---

### Struct

```c+++
#include <vector>
#include <string>

Struct Person {
	string name;
	int age;
}

vector<Person> staff;
staff.push_back({'Jack', 22});
staff.push_back({'Io', 15});
cout << "Io's age is " << staff[1].age;
```
