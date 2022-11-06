# C#-C++

Примеры совместного использования managed и unmanaged кода.
Короче говоря, использование в приложениях .NET кода из библиотек на C++.

# Visual Studio 2022 example 01

Пример от [Microsoft](https://learn.microsoft.com/en-us/visualstudio/debugger/how-to-debug-managed-and-native-code?view=vs-2022)
Консольное .NET Framework приложение обращается к функции из библиотеки на C++.
Сам пример [здеся](visual-studio/mixed-mode01)

# Visual Studio 2022 example 02

В предыдущем случае консольное .NET Framework приложение обращается к функции из "кастрированной" библиотеки на C++, созданной из пустого шаблона без обычной точки входа. В этом примере используем .NET 6.0 консольное приложение и полноценный проект библиотеки.
Сам пример [здеся](visual-studio/mixed-mode02)
