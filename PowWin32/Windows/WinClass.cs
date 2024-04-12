using System.Runtime.InteropServices;
using PowWin32.Diag;
using PowWin32.Geom;
using PowWin32.Windows.Structs;
using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.StructsPInvokeWM;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace PowWin32.Windows;

public class WinClass
{
	private static readonly uint cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX));
	private static readonly HashSet<string> registeredClassNames = new();
	private static SafeHICON? defaultIcon;
	private static SafeHCURSOR? defaultCursor;

	private static HICON DefaultIcon => defaultIcon ??= LoadIcon(default, IDI_APPLICATION);
	private static HCURSOR DefaultCursor => defaultCursor ??= LoadCursor(default, IDC_ARROW);
	private static HBRUSH DefaultBrush => SystemColorIndex.COLOR_WINDOW + 1;

	// ReSharper disable once CollectionNeverQueried.Local
	private static readonly List<WindowProcWM> trampolineWndProcs = new();

	private readonly string className;
	private readonly HINSTANCE hInstance;


	internal static void Reset()
	{
		foreach (var className in registeredClassNames)
			UnregisterClass(className, Kernel32.GetModuleHandle());
	}

	public WinClass(
		string? className = null,
		HINSTANCE hInstance = default,
		WindowProcWM? wndProc = null,
		WindowClassStyles styles = 0,
		HICON? hIcon = default,
		HICON hIconSm = default,
		HCURSOR? hCursor = default,
		HBRUSH? hBrush = default,
		string? menuName = null,
		int extraClsBytes = 0,
		int extraWndBytes = 0
	)
	{
		this.className = className ?? Guid.NewGuid().ToString("N");
		this.hInstance = hInstance.IsNull ? Kernel32.GetModuleHandle() : hInstance;

		var wc = new WNDCLASSEXWM
		{
			cbSize = cbSize,
			lpfnWndProc = MakeWndProc(wndProc ?? User32WM.DefWindowProcWM),
			hInstance = this.hInstance,
			lpszClassName = this.className,
			style = styles,
			hIcon = hIcon ?? DefaultIcon,
			hIconSm = hIconSm,
			hCursor = hCursor ?? DefaultCursor,
			hbrBackground = hBrush ?? DefaultBrush,
			lpszMenuName = menuName!,
			cbClsExtra = extraClsBytes,
			cbWndExtra = extraWndBytes,
		};
		User32WM.RegisterClassExWM(wc).Check();
		registeredClassNames.Add(wc.lpszClassName);
	}



	public void CreateWindow(
		SysWin win,
		WinStylesDef stylesDef,
		R? r = null,
		HWND hwndParent = default,
		string? text = null
	)
	{
		ThreadUtils.EnsureUIThread();

		var winGCHandle = GCHandle.Alloc(win);
		HWND hwnd = 0;

		try
		{
			hwnd = CreateWindowEx(
				stylesDef.StylesEx,
				className,
				text,
				stylesDef.Styles,
				(r?.X).OrDef(),
				(r?.Y).OrDef(),
				(r?.Width).OrDef(),
				(r?.Height).OrDef(),
				hwndParent,
				0,
				hInstance,
				GCHandle.ToIntPtr(winGCHandle)
			);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Exception calling User32Methods.CreateWindowEx");
			Console.Error.WriteLine(ex);
		}
		finally
		{
			winGCHandle.Free();
			if (hwnd == 0)
			{
				win.Destroy();
				//win.Dispose();
				throw new Exception("Failed to create window");
			}
		}
	}




	private static WindowProcWM MakeWndProc(WindowProcWM classWndProc)
	{
		var classWndProcPtr = classWndProc.ToPtr();

		WindowProcWM wndProc = (hwnd, msg, wParam, lParam) =>
		{
			switch (msg)
			{
				case WM.WM_NCCREATE:
					// Windows gives us the lpParam parameter we gave when calling CreateWindowEx here.
					// We use it to transmit the C# class instance to connect.
					var win = lParam.ExtractNativeWindowFromMessage();
					win.Attach(hwnd, classWndProcPtr);
					return win.WndProc(hwnd, msg, wParam, lParam);

				default:
					return classWndProc(hwnd, msg, wParam, lParam);
			}
		};

		trampolineWndProcs.Add(wndProc);

		return wndProc;
	}
}



file static class WinClassFileExt
{
	public static unsafe SysWin ExtractNativeWindowFromMessage(this nint lParam)
	{
		var createStruct = *(CreateStruct*)lParam.ToPointer();
		var nativeWindowPtr = createStruct.CreateParams;
		if (nativeWindowPtr == 0) throw new ArgumentException("CreateParams == 0");
		var gcHandle = GCHandle.FromIntPtr(nativeWindowPtr);
		if (gcHandle.Target is not SysWin sysWin) throw new ArgumentException("CreateParams is not a SysWin");
		return sysWin;
	}

	public static int OrDef(this int? v) => v switch
	{
		not null => v.Value,
		null => (int)CreateWindowFlags.CW_USEDEFAULT
	};
}
