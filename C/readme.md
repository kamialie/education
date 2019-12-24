### Info about bit representation

converting numbers from positive to negative:
* flip bits then add 1
* example - 5(0101) to negative 5(0101 is 1010 flipped + 1 equals 1011)
* reason is easy addition - -5 + 1 = -4 = 1011 + 0001 = 1100 - flip back is 0011 then add 1 is 0100 which is 4

floating point representaion:
* three fields(32 bits, 64):
    1. lowest 23(52) bits - mantissa
    2. 8(11) bits - exponent
    3. most significant - sign bit
* IEEE floating point standard


# Tools

## Valgrind

[Valgrind documentation](http://valgrind.org/docs/manual/manual.html)

Valgrind is a collection of tools, Memcheck is the default tool, while others can be added.

Compile code with **-g** or **-ggdb3** flags (for gcc) to add extra information that valgrind will use.

Run program as follows: valgrind \<program_name\> \<arguments...\>

- number on the left (valgrind output) is the process ID
- add **--track-origins=yes** to have extra information on uninitialized values
- some errors may not be detected by Memcheck, for example stack address that was destroyed, but later created again by other function; use **-fsanitize=address** compilation flag to detect those
- **--vgdb=full --vgdb-error=0** - run valgrind with this options and it will tell you how to start gdb with valgrind (can start gdb in editor buffer or different terminal), pass **continue** to gdb to let it advance till valgrind detects an error
- [monitor commands available](http://valgrind.org/docs/manual/mc-manual.html#mc-manual.monitor-commands)
- **--leak-check=full** to have more verbose info about leaking memory (checkout **monitor leak_check_fill reachable any** command when ran with gdb)
- valgrind provides header files with macros, that lets you request check in your code, for example:
	```C
	void f(int x) {
		int y;
		int z = x + y;
		VALGIND_CHECK_MEM_IS_DEFINED(&z, sizeof(z));
		printf("\d\n", z);
	}
	```
	[more client request macros](http://valgrind.org/docs/manual/mc-manual.html#mc-manual.clientreqs)
- other valgrind tools:
	+ [Helgrind](http://valgrind.org/docs/manual/hg-manual.html) - for multithread programming
	+ Callgrind - gives information about the performance characteristics of a program, based on Valgrind's simulation of hardware resources
	+ Massif - profiles the dynamic memory allocations in the heap, this giving info about how much memory is allocated at any given time and where in the code memory was allocated
	+ [overview if the tools](http://valgrind.org/info/tools.html), other tools may also have their headers for the client requests (just like memcheck.h)
