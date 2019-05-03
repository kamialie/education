#include <stdio.h>
# define SIZE 15

void print_row(int array[], int low, int mid, int high)
{
    for (size_t i = 0; i <= SIZE - 1; i++)
    {
        if (i < low || i > high)
            printf("    ");
        else if (i == mid)
            printf("%3d*", array[i]);
        else
            printf("%3d ", array[i]);
    }
    printf("\n");
    
}
int binary_search(int array[], int key, int low, int high)
{
    int middle;

    while (low <= high)
    {
        middle = (high + low ) / 2;

        print_row(array, low, middle, high);

        if (key == array[middle])
            return (middle);
        else if (key < array[middle])
            high = middle - 1;
        else
            low = middle + 1;
    }
    return (-1);
}

void    print_header()
{
    int i;
    printf("\nSubscripts:\n");

    for (i = 0; i <= SIZE - 1; i++)
        printf("%3d", i);
    
    printf("\n");

    for (i = 0; i <= 4 * SIZE; i++)
        printf("-");

    printf("\n");
}

int main(int argc, char const *argv[])
{
    int array[SIZE];
    int i;
    int key;
    int result;

    for (i = 0; i <= SIZE; i++)
        array[i] = 2 * i;
    
    printf("Enter a number between 0 and 28: ");
    scanf("%d", &key);

    print_header();
    result = binary_search(array, key, 0, SIZE - 1);

    if (result != -1)
        printf("\n%d foind in array element %d\n", key, result);
    else
        printf("\n%d not found\n", key);

    return (0);
}
