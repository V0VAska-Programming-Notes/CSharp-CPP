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
