# C++

# Contents

+ [General](#general)
+ [Variables and data types](#variables-and-data-types)
+ [Flows](#flows)
+ [Libraries](#libraries)

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

Assigning containers like strings and vector perform deep copying, that is
contents of the containers rather than pointer to container.

## Data types

Common types:
```c++
#include <string>

int number = 1;
double d = -1.5;
char symbol = 'A';
string str = "example"; // requires string header file
bool status = false;
```

String can also be compared with `==`, `!=`, `>`, `<` operators.

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

# Flows

[Alternative logical operators available in standard
C++](https://en.cppreference.com/w/cpp/language/operator_alternative)

`auto` keyword can be used so that compiler can decide what is a type of inner
objects.

## while and do-while

```c++
int n = 5;
int sum = 0;

while (n > 0) {
	sum	+= n;
	n--;
}

sum = 0;

do {
	sum += n;
	n--;
} while(n > 0);
```

## for loop

Iterate over all elements of a container(must specify the type of inner
elements):
```c++
string x = "abcd";
vector<int> nums = {1, 2, 3};
for (char c : x) {
	cout << c << ",";
}
for (int c : nums) {
	cout << c << ",";
}
for (auto c : nums) {
	cout << c << ",";
}
```

# Libraries

## iostream

All objects of iostream library are defined in std namespace. iostream library
doesnt provide a way to accept keyboard input without user having to press
enter.

`<<` - insertion operator, inserts a string of characters into the character
device. `cout` - standard output stream which outputs on the screen. Expression
is a valid cout element too.

Simple example and example with multiple arguments:
```c++
int sum = 441;
cout << sum;
cout << "total sum is " << sum;
```

    > cout is able to recognize the actual type of its elements:
        char Char = 'X';
        cout << Char << " " < (int)Char;
    > endl manipulator works just like adding '\n'

`>>` - extraction operator. `cin` - need to explicitely specify the variable
that can store the data. Recognized the input: int is converted through smth
like atoi, string as string, character - first entered character is saved.

Input is parsed via whitespaces. Example belows assigns `5` and `7` to variables
`x` and `y` when user enters line `5 7`:
```
int x, y;
cin >> x >> y;
```

    > manipulator - switches the stream to work on a different mode (function
        that changes one of the output stream's properties - basefield)
        - hex - changes th stream to hexadecimal mode:
            cout << "number is hex << hex << byte
        starts its work from the point it was placed at and continues even after
        the end of the cout statement - finishes ony with another manipulator
        canceling  its action; manipulator name may be in conflict with variable,
        may be fixed with namespace
    > dec (as in decimal) is a default manipulator, oct (as in octal) switches
        to octal
    > setbase(positive_integer) - manipulator, which directly instructs the
        stream on what base to use (three cases before were special cases);
        requires header file iomanip (special cases do not)
        #include <iomanip>
        cout << stebase(16) << number;
    > float value representation can be switched between fixed and scientific
        by manipulators: fixed, scientific; default - automatic

## string

## vector

## map

```c++
map<string, int> a = {{"one", 1}, {"two", 2}};

for (auto i : a) {
	cout << i.first << " " < i.second << endl;
}
```

## algorithm

Count number of elements in a container:
```c++
vector<int> nums = {1, 5, 2, 5, 3};
quantity = count(begin(nums), end(nums), 5);
cout << "found " << quantity << " fives";
```

Sort elements:
```c++
vector<int> nums = {1, 5, 2, 5, 3};
sort(begin(nums), end(nums));
```
