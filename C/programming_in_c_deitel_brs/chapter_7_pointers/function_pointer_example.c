#include <stdio.h>

void fun_square(int i)
{
    printf("square is %d\n", i * i);
}

void fun_sum(int i)
{
    printf("sum is %d\n", i + i);
}

void organizer(void (*fun) (int), int i)
{
    (*fun)(i);
    // also works as following
    // fun(i);
}

int main(void)
{
    organizer(fun_square, 3);
    organizer(fun_sum, 3);
    return (0);
}