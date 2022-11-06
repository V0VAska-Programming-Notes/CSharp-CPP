using System;
using System.Runtime.InteropServices;

namespace Mixed_Mode_Calling_App
{
    public class Program
    {
        // Replace the file path shown here with the
        // file path on your computer. For .NET Core, the typical (default) path
        // for a 64-bit DLL might look like this:
        // C:\Users\username\source\repos\Mixed_Mode_Debugging\x64\Debug\Mixed_Mode_Debugging.dll
        // Here, we show a typical path for a DLL targeting the **x86** option.
        [DllImport(@"..\..\..\..\..\MixedModeDll\x64\Debug\MixedModeDll.dll", EntryPoint =
        "mixed_mode_multiply", CallingConvention = CallingConvention.StdCall)]
        public static extern int Multiply(int x, int y);
        public static void Main(string[] args)
        {
            int result = Multiply(7, 7);
            Console.WriteLine("The answer is {0}", result);
            Console.ReadKey();
        }
    }
}
