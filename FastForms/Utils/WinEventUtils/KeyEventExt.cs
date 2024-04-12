using System.Reactive;
using System.Reactive.Linq;
using PowRxVar;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPInvoke;

namespace FastForms.Utils.WinEventUtils;

public static class KeyEventExt
{
	public static IObservable<Unit> WhenKey(this SysWin sys, VirtualKey key) => sys.Evt.WhenKey.ToObs().Where(e => e.IsKeyDown && e.VirtualKey == key).ToUnit();
}