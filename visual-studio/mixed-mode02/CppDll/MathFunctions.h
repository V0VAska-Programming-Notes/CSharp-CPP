#pragma once

#ifdef CPPDLL_EXPORTS
#define CPPDLL_API __declspec(dllexport)
#else
#define CPPDLL_API __declspec(dllimport)
#endif // CPPDLL_EXPORTS

extern "C" CPPDLL_API int __cdecl addition(int, int);
