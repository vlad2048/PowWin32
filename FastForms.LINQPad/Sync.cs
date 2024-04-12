using System.Reactive.Concurrency;
using LINQPad;
using LINQPad.Controls;
using Vanara.PInvoke;

namespace FastForms.LINQPad;

public enum ThreadType { LinqPad, Win32, Other };

public static class Sync
{
	public static uint Win32ThreadId { get; private set; }
	public static DumpContainer? VerboseDC { get; set; }

	public static IScheduler Sched_LinqPad = null!;
	public static IScheduler Sched_Win32 = null!;

	public static void Start_Win32(Action action)
	{
		Util.CreateSynchronizationContext();
		Sched_LinqPad = Init(ThreadType.LinqPad);

		Task.Run(() =>
		{
			SynchronizationContext.SetSynchronizationContext(new System.Windows.Forms.WindowsFormsSynchronizationContext());
			Sched_Win32 = Init(ThreadType.Win32);
			Win32ThreadId = Kernel32.GetCurrentThreadId();

			action();
		});
	}

	public static Button Btn_Win32(string name, Action action) => new(name, _ => Sched_Win32.Schedule(() =>
	{
		Chk(ThreadType.Win32);
		action();
	}));

	public static void Chk(ThreadType type, string? str = null)
	{
		var expNfo = list.FirstOrDefault(e => e.Type == type);
		if (expNfo == null) throw new ArgumentException($"Thread type: {type} has not been inited @ {str}");
		var expTId = expNfo.Id;
		var actTId = Thread.CurrentThread.ManagedThreadId;
		if (actTId != expTId)
		{
			var actNfo = list.FirstOrDefault(e => e.Id == actTId);
			var actDescr = actNfo switch
			{
				not null => $"{actNfo.Type}",
				null => $"[{Thread.CurrentThread.Name}({actTId})]",
			};
			throw new ArgumentException($"Wrong thread for type: {type} @ {str}\nExpected:{type} vs Actual:{actDescr}".Dump());
		}
		if (VerboseDC != null && str != null) VerboseDC.AppendContent($"Thread {type} confirmed @ {str}");
	}


	internal static void Reset()
	{
		list.Clear();
	}


	private sealed record ThreadNfo(
		ThreadType Type,
		int Id,
		string Name,
		SynchronizationContext Ctx
	);

	private static readonly List<ThreadNfo> list = new();

	private static IScheduler Init(ThreadType type)
	{
		if (list.Any(e => e.Type == type)) throw new ArgumentException($"Thread[{type}] is already inited");
		var ctx = SynchronizationContext.Current;
		if (ctx == null) throw new ArgumentException("Thread has no SynchronizationContext");
		if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = $"{type}";
		list.Add(new ThreadNfo(type, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name, ctx));
		if (VerboseDC != null) VerboseDC.AppendContent($"Inited {$"{type}".PadRight(7)} with a ctx of type: {ctx.GetType().Name}");
		return new SynchronizationContextScheduler(ctx);
	}
}