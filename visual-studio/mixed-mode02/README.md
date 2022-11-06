# Нативная DLL и .NET 6.0 приложение

1. Создаём 'Dynamic-Link Library (DLL)', вбиваем CppDll как имя проекта и mixed-mode02 как solution.

2. Добавим к проекту 'MathFunctions.h':

```c++
#pragma once

#ifdef CPPDLL_EXPORTS
#define CPPDLL_API __declspec(dllexport)
#else
#define CPPDLL_API __declspec(dllimport)
#endif // CPPDLL_EXPORTS

extern "C" CPPDLL_API int __cdecl addition(int, int);
```

3. Добавим и сырец 'MathFunctions.cpp':

```c++
#include "pch.h"
#include "MathFunctions.h"

int addition(int a, int b)
{
	return a + b;
}
```

4. В свойствах проекта проверяем установки Linker->Advanced, в поле 'No Entry Point' должно быть No.

5. Строим библиотеку под x64.

6. К solution теперь добавим новый проект 'Console App' для C#. Обзываем CSharpConsole под .Net 6.0. Используем "новомодный" подход, не ставя галочку под 'Do not use top-level statements' (ежели поставим, то код, ессно, нуна будет слегонца изменить).

7. В свойствах проекта идём в Debug, открываем линк в General и ставим галочку 'Enable native code debugging'.

8. Разворачиваем в свойствах Build->General и выбираем Platform Target под x64 (должен совпадать с выбором для DLL).

9. Содержимое Program.cs:

```c#
using System.Runtime.InteropServices;

[DllImport(@"..\..\..\..\x64\Debug\CppDll.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "addition")]
static extern int addition(int a, int b);

Console.WriteLine(addition(4, 8));
```

10. При создании приложения хоть и упоминается об Any CPU, но в действительности строится под x64. Можно для красоты поковыряться в Configuration Manager, чтобы повыкидывать все 'Any CPU'.

В итоге у нас всё фурыкает и дебужится с заходом внутрь dll.
