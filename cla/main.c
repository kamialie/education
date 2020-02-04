  #include <stdio.h> 
    int main(void) { 
        char s[20]; 
        FILE *f = fopen("data","w"); 
        int i = fputs("1248",f); 
        fclose(f); 
        f = fopen("data","r"); 
        fgets(s + 2,4,f); 
        putchar(s[4]); 
        fclose(f); 
        return 0; 
    }
