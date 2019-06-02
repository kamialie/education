    #include <stdio.h>
    int main(void) {
        int X = 1;
    #define	X 1
        int Y = X - 2;
    #undef X
        printf("%d",Y+X);
        return 0;
    }
