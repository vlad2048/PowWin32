using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPInvokeWM;

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
public delegate nint WindowProcWM([In] HWND hwnd, [In] WM uMsg, [In] nint wParam, [In] nint lParam);
