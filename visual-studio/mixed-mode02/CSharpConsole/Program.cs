using System.Runtime.InteropServices;

[DllImport(@"..\..\..\..\x64\Debug\CppDll.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "addition")]
static extern int addition(int a, int b);

Console.WriteLine(addition(4, 8));
