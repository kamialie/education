# Piscine videos

## ad-hoc polymorphism (function overloading)

+ Overloaded methods MUST change the argument list
+ Overloaded methods CAN change the return type
+ Overloaded methods CAN change the access modifier
+ Overloaded methods CAN declare new or broader checked exceptions
+ A method can be overloaded in the same class or in a subclass

Since can't access stream class, overload output function
```c++
std::ostream &	operator<<(std::ostream &o, Integer const &rhs) {
	o << rhs.getValue();
	return o;
}
```

## operator overloading

Example:
```c++
Integer	Integer+(Integer const &rhs) const{
	return Integer(this->_n + rhs.getValue());
}
```

## canonical form

Must have for canonical complience:
+ at least default constructor
+ copy constructor
+ destructor
+ assignment operator overload

Good to have:
+ output function overload
+ string serialization
