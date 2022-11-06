#pragma once

#include "hello_export.h"

HELLO_EXPORT void HelloFunc();
HELLO_EXPORT int add(int a, int b);
HELLO_EXPORT void print(int *p, int len);

typedef struct vector3
{
    float x, y, z;
} vector;
HELLO_EXPORT vector mul(vector *rls, vector *rhs);
HELLO_EXPORT void callBackTest(void (*p)(int), int value);
