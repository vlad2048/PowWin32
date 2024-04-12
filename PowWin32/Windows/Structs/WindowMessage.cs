using Vanara.PInvoke;

namespace PowWin32.Windows.Structs;

public struct WindowMessage
{
    public HWND Hwnd;
    public WM Id;
    public nint WParam;
    public nint LParam;
    public nint Result;
    public bool Handled;

    public WindowMessage(HWND hwnd, WM id, nint wParam, nint lParam)
    {
        Hwnd = hwnd;
        Id = id;
        WParam = wParam;
        LParam = lParam;
        Result = nint.Zero;
        Handled = false;
    }
}