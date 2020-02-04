#include <stdio.h>

int	main(void)
{
	//char *str = "WTF just happened";

	//printf("%c\n", str[2]);
	//printf("%c\n", 2[str]);
	
	struct
	{
		int a;
		int b;
	} Data1 = {0, 0};

	union
	{
		int a;
		int b;
		int c;
		int d;
		int e;
	} Data2 = {0};

	Data1.a++;
	Data2.a++;
	printf("%d %d\n", Data1.b, Data2.b);
	printf("%lu %lu\n", sizeof(Data1), sizeof(Data2));
	printf("wow - %lu\n", sizeof(int));
	return (0);
}
