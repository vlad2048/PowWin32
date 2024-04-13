using System.Reactive.Linq;
using FastForms.LINQPad.Hooking;
using FastForms.LINQPad.Hooking.Events;
using FastForms.LINQPad.Utils;
using FastForms.LINQPad.WinLogging;
using LINQPad;
using LINQPad.Controls;
using PowRxVar;
using Vanara.PInvoke;

namespace FastForms.LINQPad;

public static class WinLogger
{
	//private static HookDispatcher? dispatcher;
	//private static HookDispatcher Dispatcher => dispatcher ??= new HookDispatcher();
	//internal static void Reset() => dispatcher?.Dispose();

	private static HookDispatcher? dispatcher;

	public static Div GetPanel(Action<WinLoggerOpt>? optFun)
	{
		Sync.Chk(ThreadType.Win32, "You need to call WinLogger.GetPanel() from the Win32 thread");
		var opt = WinLoggerOpt.Build(optFun);

		var dc = new DumpContainer();
		var div = new Div(dc);
		div.CssClass = "winlist";

		dispatcher?.Dispose();
		dispatcher = new HookDispatcher(Resetter.D);

		var classCountMap = new Dictionary<string, int>();

		int GetClassCount(string className)
		{
			if (classCountMap.TryGetValue(className, out int cnt))
			{
				classCountMap[className]++;
				return cnt;
			}
			else
			{
				classCountMap[className] = 1;
				return 0;
			}
		}

		bool DoesHwndPassWinFilter(HWND hwnd)
		{
			if (opt.WinFilterClassName == null) return true;
			var className = hwnd.GetClassName();
			if (opt.WinFilterClassName != className) return false;
			if (opt.WinFilterClassInstance == null) return true;
			var classNameCnt = GetClassCount(className);
			return classNameCnt == opt.WinFilterClassInstance;
		}


		var winMap = new Dictionary<HWND, WinRow>().D(Resetter.D);
		var objLock = new object();
		WinRow CreateWin(HWND hwnd) => winMap[hwnd] = new WinRow(hwnd, opt);

		void RemoveWin(HWND hwnd)
		{
			lock (objLock)
			{
				var row = winMap[hwnd];
				row.Dispose();
				winMap.Remove(hwnd);
			}
		}

		WinRow GetOrCreateWin(HWND hwnd, ref bool needUpdate)
		{
			lock (objLock)
			{
				if (winMap.TryGetValue(hwnd, out var row))
					return row;
				needUpdate = true;
				return CreateWin(hwnd);
			}
		}

		Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(_ =>
		{
			WinRow[] rows;
			lock (objLock)
				rows = winMap.Values.ToArray();
			foreach (var row in rows)
				row.State.Update_Capture();
		}).D(Resetter.D);


		dispatcher.WhenEvt.Subscribe(evt =>
		{
			var hwnd = evt.Hwnd;
			if (hwnd == 0) return;

			if (!DoesHwndPassWinFilter(hwnd))
				return;
			var needUpdate = false;
			var win = GetOrCreateWin(hwnd, ref needUpdate);
			if (evt is WndProc_HookEvt { MsgId: WM.WM_NCDESTROY })
			{
				RemoveWin(hwnd);
				needUpdate = true;
			}
			else
			{
				win.ProcessEvent(evt);
			}
			if (needUpdate)
			{
				WinRow[] rows;
				lock (objLock)
					rows = winMap.Values.ToArray();
				if (opt.HideSysWins)
					rows = rows.Where(e => !IsSysWin(e.State)).ToArray();
				dc.UpdateContent(rows.Select(e => e.RenderRow));
			}
		}).D(Resetter.D);


		return div;
	}


	private static readonly HashSet<string> sysClasses = [
		"IME",
		"CicMarshalWndClass",
		"UserAdapterWindowClass",
		"MSCTFIME UI",
	];

	private static bool IsSysWin(WinState state) => sysClasses.Contains(state.Id.ClassName);
}