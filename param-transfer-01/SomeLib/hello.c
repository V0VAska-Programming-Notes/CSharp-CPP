#include <stdio.h>

#include "hello.h"

void HelloFunc()
{
    printf("Hello World\n");
}

int add(int a, int b)
{
    return a + b;
}

void print(int *p, int len)
{
    for (int i = 0; i < len; i++)
    {
        printf("%d ", p[i]);
        p[i] *= 2;
    }
    printf("\n");
}

vector mul(vector *rls, vector *rhs)
{
    vector v;
    v.x = rls->x * rhs->x;
    v.y = rls->y * rhs->y;
    v.z = rls->z * rhs->z;
    return v;
}

void callBackTest(void (*p)(int), int value)
{
    p(value);
}
