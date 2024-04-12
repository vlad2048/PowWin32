using Vanara.PInvoke;

namespace PowWin32.Diag;

public static class Lgr
{
    public static void L(string s) => Console.WriteLine(s);

    public static string Fmt(this HWND hwnd) => $"0x{hwnd.DangerousGetHandle():X8}";
}