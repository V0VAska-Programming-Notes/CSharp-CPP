# Передача различных параметров между C/C++ и C#

Не помню, где нашукал пример, но автору спасибо. Исходники в целом не мои, лишь только убрал шероховатости. Например, оперделение __declspec() через #define лишает возможности использовать статическое связывание под форточками. Ну и ышо чё по-мелочи...

Чтобы всё работало с пол-пинка и под форточками, и под линуксом, подразумевается использование входящего в репозитарий CMakePresets.txt с выбранным пресетом под ниндзю.

## Пока чистый Си
Структура каталогов нашей тестовой поляны:

```
.
├── CMakeLists.txt
└── SomeLib
    ├── CMakeLists.txt
    ├── hello.c
    └── hello.h
```

Используем VSCode.

SomeLib/hello.h:

```c++
#pragma once

#include "hello_export.h"

HELLO_EXPORT void HelloFunc();
HELLO_EXPORT int add(int a, int b);
```

SomeLib/hello.c:

```c++
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
```

SomeLib/CMakeLists.txt

```cmake
include(GenerateExportHeader)

set(LIBHELLO_SRC hello.c)

add_library(hello SHARED ${LIBHELLO_SRC})

generate_export_header(hello)

target_include_directories(hello
    PUBLIC
        "$<BUILD_INTERFACE:${CMAKE_CURRENT_BINARY_DIR}>"
)
```

CMakeLists.txt:

```cmake
cmake_minimum_required(VERSION 3.21)

project(PInvoke)

add_subdirectory(SomeLib)
```

Переоткроем папку в VSCode. Выберем x64-ninja-debug как пресет... В итоге на выходе должны получить готовую библиотеку.

Из корневого каталога нашей "структуры" каталогов запустим терминал и вдолбим:

```
dotnet new console --framework "net6.0" -o testnet
```

В каталоге testnet создастся зародыш нашего тестового приложения.

Изменим содержимое Program.cs на:

```c#
using System.Runtime.InteropServices;

[DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello", EntryPoint = "HelloFunc")]
extern static void HelloFunc();

[DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
extern static int add(int a, int b);

int t = add(2, 3);
Console.WriteLine(t);

HelloFunc();

Console.ReadKey();
```

Ессно, что следует проверить пути, зависящие от пресета, вернее от ОС, на которой мутим... Пресет и там и там "ниндзявский дебуг".
В линухе путь будет `../../../../out/build/linux-ninja-debug/SomeLib/libhello`

Команда dotnet build, запущенная из директории testnet, пересобирает C# приложение. Также из командной строки запускаем построенный testnet(.exe), должно работать.
Вообще, и в линухе тоже из VSCode (при наличии C# расширения) можно править .cs-файл и запускать .NET приложение. Только не забыть в launch.json прописать `"console": "integratedTerminal"`, чтобы во встроенном терминале мона было кнопиську для ReadKey() жмякнуть. Прям в директории param-transfer-01 запущаем Code и правим/билдим в одном флаконе и библиотеку и .NET приложение. Можно вообще попробовать и в CMake всё объединённо замутить, тогда можно будет и с путями не париться...

## Работа с указателями

Для C/C++ никаких изменений, но вот в C# надоть использовать unsafe block. Сперва следует разрешить использование данной фичи в файле проекта .csproj, добавив <AllowUnsafeBlocks>true</AllowUnsafeBlocks> (в VSCode) или в project/build/general свойствах проекта (VS).

В hello.h добавим:

```c++
HELLO_EXPORT void print(int *p, int len);
```

В hello.c:

```c++
void print(int *p, int len)
{
    for (int i = 0; i < len; i++)
    {
        printf("%d ", p[i]);
        p[i] *= 2;
    }
    printf("\n");
}
```

Program.cs:

```c#
using System.Runtime.InteropServices;

[DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello", EntryPoint = "HelloFunc")]
extern static void HelloFunc();

[DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
extern static int add(int a, int b);

unsafe
{
    [DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
    extern static void print(int *p, int len);

    int[] a = new int[10];
    for (int i = 0; i < 10; i++)
    {
        a[i] = add(i, i);
    }

    fixed(int *p = &(a[0]))
    {
        print(p, 10);
    }

    for (int i = 0; i < 10; i++)
    {
        Console.Write(a[i] + ", ");
    }
    Console.Write("\n");
}

int t = add(2, 3);
Console.WriteLine(t);

HelloFunc();

Console.ReadKey();
```

В терминале должны получить вывод:

```
0 2 4 6 8 10 12 14 16 18
0, 4, 8, 12, 16, 20, 24, 28, 32, 36,
5
Hello World
```

## Структуры

Касательно C/C++, так никаких изменений. А вот в C# нужно:
- чётко определить "построение" структуры, чтобы все члены были ранжированы по порядку
- все члены структуры должны быть заявлены как public
Само содержание структуры в C/C++ и C# должно быть одинаковым.

В hello.h добавим

```c++
typedef struct vector3
{
    float x, y, z;
} vector;

HELLO_EXPORT vector mul(vector *rls, vector *rhs);
```

В hello.c:

```c++
vector mul(vector *rls, vector *rhs)
{
    vector v;
    v.x = rls->x * rhs->x;
    v.y = rls->y * rhs->y;
    v.z = rls->z * rhs->z;
    return v;
}
```

<details><summary>Program.cs</summary>

```c#
using System.Runtime.InteropServices;

[DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello", EntryPoint = "HelloFunc")]
extern static void HelloFunc();

[DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
extern static int add(int a, int b);

unsafe
{
    [DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
    extern static void print(int *p, int len);

    [DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
    extern static vector3 mul(vector3* lhs, vector3* rhs);

    int[] a = new int[10];
    for (int i = 0; i < 10; i++)
    {
        a[i] = add(i, i);
    }

    fixed(int *p = &(a[0]))
    {
        print(p, 10);
    }

    for (int i = 0; i < 10; i++)
    {
        Console.Write(a[i] + ", ");
    }
    Console.Write("\n");

    vector3 l, r;
    l.x = 1;
    l.y = 2;
    l.z = 3;
    r.x = 2;
    r.y = 3;
    r.z = 4;

    vector3 v = mul(&l, &r);
    Console.WriteLine($"x: {v.x}, y: {v.y}, z: {v.z}");
}

int t = add(2, 3);
Console.WriteLine(t);

HelloFunc();

Console.ReadKey();

[StructLayout(LayoutKind.Sequential)]
struct vector3{
    public float x, y, z;
}
```

</details>

## Как насчёт callback?

hello.h:

```c++
HELLO_EXPORT void callBackTest(void (*p)(int), int value);
```

hello.c:

```c++
void callBackTest(void (*p)(int), int value)
{
    p(value);
}
```

В Program.cs следует добавить нечто вроде:

```c#
[DllImport(@"libhello")]
extern static void callBackTest(printFromCSharp cSharp, int value);

printFromCSharp callback = printFromCSharpImpl;
callBackTest(callback, 10);

static void printFromCSharpImpl(int value){
    Console.WriteLine("This is called from CSharp: " + value);
}

delegate void printFromCSharp(int value);
```

<details><summary>Полный листинг Program.cs</summary>

```c#
using System.Runtime.InteropServices;

[DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello", EntryPoint = "HelloFunc")]
extern static void HelloFunc();

[DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
extern static int add(int a, int b);

unsafe
{
    [DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
    extern static void print(int *p, int len);

    [DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
    extern static vector3 mul(vector3* lhs, vector3* rhs);

    int[] a = new int[10];
    for (int i = 0; i < 10; i++)
    {
        a[i] = add(i, i);
    }

    fixed(int *p = &(a[0]))
    {
        print(p, 10);
    }

    for (int i = 0; i < 10; i++)
    {
        Console.Write(a[i] + ", ");
    }
    Console.Write("\n");

    vector3 l, r;
    l.x = 1;
    l.y = 2;
    l.z = 3;
    r.x = 2;
    r.y = 3;
    r.z = 4;

    vector3 v = mul(&l, &r);
    Console.WriteLine($"x: {v.x}, y: {v.y}, z: {v.z}");
}

int t = add(2, 3);
Console.WriteLine(t);

HelloFunc();

[DllImport(@"../../../../out/build/win-ninja-x64-debug/SomeLib/hello")]
extern static void callBackTest(printFromCSharp cSharp, int value);

printFromCSharp callback = printFromCSharpImpl;
callBackTest(callback, 10);

static void printFromCSharpImpl(int value)
{
    Console.WriteLine("This is called from CSharp: " + value);
}

Console.ReadKey();

[StructLayout(LayoutKind.Sequential)]
struct vector3{
    public float x, y, z;
}

delegate void printFromCSharp(int value);
```

</details>

# C++

Обошлись в примере чистым Си... Но ежели придётся использовать C++, то прототипы функций в заголовочном файле .h следует прописывать с указанием `extern "C"`.

Также возможно указание конкретного соглашения о вызове, скажем, __cdecl. Со специфичным для форточек __stdcall могут быть и портачки, поскольку и с `extern "C"` имена функций могут быть "покалечены" (mangled). Помимо __fastcall можно поэксперементировать и с опять-таки специфичной винде (точнее мелкомягким) __thiscall.

Ежели функции прописаны в заголовках C/C++ с указанием соглашения о вызове, например,

```c++
extern "C"
{
    ...
    __declspec(dllexport) int __cdecl foo();
    ...
}
```

то в C# указываем такой же тип соглашения:

```c#
[DllImport(@"<path>", CallingConvention = CallingConvention.Cdecl)]
```
