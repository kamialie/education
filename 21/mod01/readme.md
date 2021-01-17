# Piscine videos

## new and delete

`new` keyword is used to allocated memory to any object, including custom
classes. Returns pointed to allocated memory. For array allocation just append
square brackets and desired number inside. Note that array allocation does not
accept parameters, thus constructor without parameters is called.

`delete` keyword is used to free up memory allocated by `new`. Nothing fancy,
except new delete syntax for arrays:

```c++
class Some {

	//blabla
};

int	main() {

	Some	*object = new Some;
	Some	*arrayOfObjects = new Some[21];

	delete object;
	delete [] arrayOfObjects;

	return 0;
}
```

## references

References act like an alias to the variable or always dereferenced pointer.
References can not point to nothing, thus, the only way to create a reference
is also initialize in the same line.

```c++
#include <iostream>

int	main() {
	int	one = 1;
	int	&two = one;

	std::cout << one << ' ' << two << std::endl;

	two = 2;

	std::cout << one << ' ' << two << std::endl;

	return 0;
}
```

References also can be used to pass parameter as a reference:

```c++
#include <iostream>
#include <string>

void	modifyString(std::string &str) {
	
	str += " and here";
}

void	outputString(std::string const &str) {
	
	std::cout << std << std::endl;
}

int	main() {

	std::string str = "hey, there";

	modifyString(str);
	outputString(str);

	return 0;
}
```

Last useful usage is returning reference to an object from function. When doing
is that return can be used as an lvalue that can be assigned a value:

```
...

int	main() {
	...
	student.getLoginRef() = "new login";
	...
}
```

Summary. References are a good choice when you know that the objects always
exists, however, if that is not the case or could change or won't always be
there, use pointers.

## filestreams

`ifsteam` and `ofstream` (stand for input and output file stream) are used to
read and write to/from files:

```c++
#include <iostream>
#include <fstream>

int	main() {
	std::ifstream	ifs("numbers");
	unsigned int	dst;
	unsigned int	dst2;
	ifs >> dst >> dst2;

	std::cout << dst << " " << dst2 << std::endl;
	ifs,.close();

	std:ofstream	ofs("test");

	ofs << "random text" << std::endl;
	ofs.close();

	return 0;
}
```
