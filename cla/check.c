#include <stdio.h>
    #include <stdlib.h>
    int main(void) {
        float *t = 1 + (float *) malloc(sizeof(float) * sizeof(float));
        t--;
        *t = 8.0;
        t[1] = *t / 4.0;
        t++;
        t[-1] = *t / 2.0;
        printf("%f\n",*t);
        free(--t);
        return 0;
    }
