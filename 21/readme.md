# Piscine videos

## namespace

```c++
#include <stdio.h>

int gl_var = 1;

// create a namespace and put symbols inside;
namespace Foo {
	int gl_var = 2;
	int f(void) { return 3; }
}

// namespace aliasing:
namespace Muf = Foo;

// scope resolution operator - ::
printf("namespace value - %d\n", Foo::gl_var);

// empty namespace refers to global scope
// might be useful for explicit use of variables
printf("global value - %d\n", ::gl_var);

```

## stdio streams

`std::endl` handles the difference between Unix and Microsoft OS families
difference on new line

## class and instance

Good practice is to separate class definition and declaration. Should double
check, but declaration goes to header file (mostly `.hpp`, but could be `.h`)
and definition to `.cpp`

Sample.hpp:
```c++

#ifndef SAMPLE_H
# define SAMPLE_H

class Sample {

public:

	int foo;

	Sample();
	~Sample();

	void	bar();
};

#endif
```

Sample.cpp:
```c++
#include <iostream>
#include "Sample.hpp"

Sample::Sample() {
	
	std::cout << "Construciton call" << std::endl;
}

Sample::~Sample() {
	
	std::cout << "Destructor call" << std::endl;
}
```

## member attributes and member function, this

Nothing complicated about member attributes and functions (very similar to
structures).

`this` is a pointer to the current instance that is always passed to all
functions (including constructor and destructor).

Sample.hpp:
```c++

#ifndef SAMPLE_H
# define SAMPLE_H

class Sample {

public:

	int foo;

	Sample();
	~Sample();

	void	bar();
};

#endif
```

Sample.cpp:
```c++
#include <iostream>
#include "Sample.hpp"

Sample::Sample() {
	
	std::cout << "Construciton call" << std::endl;

	this->foo = 42;

	this->bar();
}

Sample::~Sample() {
	
	std::cout << "Destructor call" << std::endl;
}
```

## initialization list

Special syntax to initialize attributes of the object.

Sample.hpp:
```c++

#ifndef SAMPLE_H
# define SAMPLE_H

class Sample {

public:

	char	a1;
	int		a2;
	float	a3;

	Sample(char p1, int p2, float p3);
	~Sample();
};

#endif
```

Sample.cpp:
```c++
#include <iostream>
#include "Sample.hpp"

Sample::Sample(char p1, int p2, float p3): a1(p1), a2(p2), a3(p3) {
	
	std::cout << "Construciton call" << std::endl;
}

Sample::~Sample() {
	
	std::cout << "Destructor call" << std::endl;
}
```

## const

`const` in definition or/and declaration of a method states that the function
will not alter the current object (that is will not have any attributes
assignments).

Sample.hpp:
```c++

#ifndef SAMPLE_H
# define SAMPLE_H

class Sample {

public:

	int		foo;

	Sample(char p1, int p2, float p3);
	~Sample();

	void	bar() const;
};

#endif
```

Sample.cpp:
```c++
#include <iostream>
#include "Sample.hpp"

Sample::Sample(char p1, int p2, float p3): a1(p1), a2(p2), a3(p3) {
	
	std::cout << "Construciton call" << std::endl;
}

Sample::~Sample() {
	
	std::cout << "Destructor call" << std::endl;
}

void	Sample::bar() const {

	std::cout << "value of foo - " << this->foo << std::endl;
}
```

Other use of `const` includes constant variables:
```c++
int const var;
```

It's a good practice in general to use `const` as much as possible, which makes
the code more robust in the future.

## visibility

Nothing fancy, just added the use of `private` keyword usage in class, which
makes attributes and methods invisible and unaccessable from the outside. Good
practice is to use as much private as possible.

## class vs struct

Very similar definition indeed, however, by default struct's attributes are
public, while class has them private by default.

## accessors

Getters and setters provide interface to the user, while all attributes are
best to be private. Good practice is to even overuse them. Sometimes only
getter might be useful, look particular context.

Sample.hpp:
```c++
#ifndef SAMPLE_H
# define SAMPLE_H

class Sample {

public:

	Sample(char p1, int p2, float p3);
	~Sample();

	int		getFoo const ();
	void	setFoo (int v);

private:

	int		foo;
};

#endif
```

## non member attributes and non member functions

That is class attributes and methods. To declare precede it with a keyword
`static` in class definition. The only way to initialize class attribute is
outside class (no matter if it is private or public, check below).

Sample.hpp:
```c++

#ifndef SAMPLE_H
# define SAMPLE_H

class Sample {

public:

	int		foo;

	Sample();
	~Sample();

	static int	getNumInst();;
};

private:

	static int	_nbInst;

#endif
```

Sample.cpp:
```c++
#include <iostream>
#include "Sample.hpp"

...

int	Sample::getNumInst() {

	return Sample::_nbInst;
}

int	Sample::_nbInst = 0;
```

## pointers to members

Welcome to the weird declaration and usage of pointers to instance attributes
and methods.

```c++
int	main() {

	Sample	instance;
	Sample*	instancep = &instance;

	// The only diffirence in the definition is the class resolution preceded to
	// the pointer identifier

	int		Sample::*p = NULL;
	void	(Sample::*f)() const;

	// here is the pointer assignment, which actually does not specify which
	// instance it refers to, that will be specified when accessgin the
	// attribute

	p = &Sample::foo;

	// assigning value to the attribute through pointer, first indicate which
	// insstance to refer to, then use .* operator to access the attribute

	instance.*p = 21;

	// same throught the pointer to the instance, this time operator ->* is
	// used

	instancep->*p = 21;

	// funciton pointer assignment, again without specifying which instance it
	// is referred to

	f = &Sample::bar;

	// same as with attributes first specify with instance is referred, then
	// corresponding operators are used to access the function from stack
	// variable and from a pointer

	(instance.*f)();
	(instancep->*f)();

	return 0;
}
```
