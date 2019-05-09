#include <stdio.h>

void    fun(char str[])
{
    str[1] = 'D';
}

void    fun_2(char *str)
{
    *(str + 1) = 'D';
}

int main(void)
{
    char str[] = "Hello";
    char *s = "Hello";

    printf("before - %s\n", str);
    fun(str);
    printf("after - %s\n", str);
    printf("before - %s\n", s);
    fun_2(s);
    printf("after - %s\n", s);
    return (0);
}