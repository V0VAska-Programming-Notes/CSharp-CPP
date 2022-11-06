# Debug C# and C++ in the same debugging session

Visual Studio lets you enable more than one debugger type in a debugging session, which is called mixed-mode debugging. In this tutorial, you learn to debug both managed and native code in a single debugging session.

This tutorial shows how to debug native code from a managed app, but you can also [debug managed code from a native app](https://learn.microsoft.com/en-us/visualstudio/debugger/how-to-debug-in-mixed-mode?view=vs-2022). The debugger also supports other types of mixed-mode debugging, such as debugging [Python and native code](https://learn.microsoft.com/en-us/visualstudio/python/debugging-mixed-mode-c-cpp-python-in-visual-studio?view=vs-2022), and using the script debugger in app types such as ASP.NET.

## Create a simple native DLL

### Create the files for the DLL project

1. Create Empty Project for C++.

2. Add to the project Mixed_Mode.cpp file:

```c++
#include "Mixed_Mode.h"
```

3. Next add Mixed_Mode.h:

```c++
#ifndef MIXED_MODE_MULTIPLY_HPP
#define MIXED_MODE_MULTIPLY_HPP

extern "C"
{
	__declspec(dllexport) int __stdcall mixed_mode_multiply(int a, int b) {
		return a * b;
	}
}
#endif
```

### Configure and build the DLL project

1. In the Visual Studio toolbar, select Debug configuration and x86 or x64 platform. If your calling app will be .NET Core, which always runs in 64-bit mode, select x64 as the platform.

2. In Solution Explorer, select the Mixed_Mode_Debugging project node and select the Properties icon, or right-click the project node and select Properties.

3. At the top of the Properties pane, make sure the Configuration is set to Active(Debug) and the Platform is the same as what you set in the toolbar: x64, or Win32 for x86 platform.
> **Important**. If you switch platform from x86 to x64 or vice versa, you must reconfigure the properties for the new platform.

4. Under Configuration Properties in the left pane, select Linker > Advanced, and in the dropdown next to No Entry Point, select No. If you had to change it to No, select Apply.

5. Under Configuration Properties, select General, and in the dropdown next to Configuration Type, select Dynamic Library (.dll). Select Apply, and then select OK.

6. Select the project in Solution Explorer and then select Build > Build Solution, press F7, or right-click the project and select Build.

## Create a simple managed app to call the DLL

1. Create 'Console App for .NET Core' or 'Console App (.NET Framework) for C#'. Then, type a name like Mixed_Mode_Calling_App and click Next or Create, whichever option is available.
For .NET Core, choose either the recommended target framework or .NET 6, and then choose Create.
> **Note**. You could also add the new managed project to your existing C++ solution. We are creating the project in a new solution to make the mixed-mode debugging task more difficult.

2. Replace all the code in Program.cs with the following code:

```c#
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
```

3. In the new code, replace the file path in [DllImport] with your file path to the dll you just created. See the code comment for hints. Make sure to replace the username placeholder.

## Configure mixed-mode debugging

1. In Solution Explorer, select the Mixed_Mode_Calling_App project node and select the Properties icon, or right-click the project node and select Properties.

2. Enable native code debugging in the properties.
Select Debug in the left pane, select Open debug launch profiles UI, then select the Enable native code debugging check box, and then close the properties page to save the changes.

3. If you are targeting an x64 DLL from a .NET Framework app, change the platform target from Any CPU to x64. To do this, you may need to select Configuration Manager from the Debug toolbar's Solution Platform drop-down. Then, if you can't switch to x64 directly, create a New Configuration that targets x64.

