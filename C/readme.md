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
