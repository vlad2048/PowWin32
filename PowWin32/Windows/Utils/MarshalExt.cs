using System.Runtime.InteropServices;

namespace PowWin32.Windows.Utils;

static class MarshalExt
{
	public static nint ToPtr<TDelegate>(this TDelegate del) where TDelegate : notnull => Marshal.GetFunctionPointerForDelegate(del);

	public static TDelegate FromPtr<TDelegate>(this nint ptr) where TDelegate : Delegate => Marshal.GetDelegateForFunctionPointer<TDelegate>(ptr);

	public static nint ToPtr(this string str) => Marshal.StringToHGlobalAuto(str);
}