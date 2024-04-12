using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FastForms.LINQPad.Hooking.Events;
using FastForms.LINQPad.MessageLogging.Logic;
using FastForms.LINQPad.MessageLogging.Structs;
using LINQPad;
using LINQPad.Controls;
using PowRxVar;

namespace FastForms.LINQPad.MessageLogging;

public sealed class MsgTracker(WinLoggerOpt opt, Disp d)
{
	private readonly ConcurrentQueue<IMsg> MsgQueue = new();
    private readonly Subject<Unit> whenChanged = new Subject<Unit>().D(d);
    private int nesting;
    private TimeSpan lastTime = TimeSpan.Zero;

    private TimeSpan DeltaT
    {
	    get
	    {
		    var now = Resetter.Time;
			var delta = now - lastTime;
			lastTime = now;
			return delta;
	    }
    }

    public IObservable<Unit> WhenChanged => whenChanged.AsObservable();
    public DumpContainer MsgDC { get; } = new();

    public void ProcessEvent(IHookEvt evt)
    {
	    if (!opt.MsgQueueLength.HasValue) return;
	    var display = false;
	    switch (evt)
	    {
		    case WndProc_HookEvt e when opt.AcceptMsg(e.MsgId):
		    {
			    var msg = new WinMsg(Resetter.Time, DeltaT, nesting, e.MsgId, e.WParam, e.LParam);
			    MsgQueue.Enqueue(msg);
			    nesting++;
			    display = true;
				break;
		    }
		    case WndProcRet_HookEvt e when opt.AcceptMsg(e.MsgId):
		    {
			    nesting--;
			    break;
		    }
	    }

	    if (display)
		    whenChanged.OnNext(Unit.Default);
    }

    public Literal Render()
    {
	    if (!opt.MsgQueueLength.HasValue) return new Literal("_");
		while (MsgQueue.Count > opt.MsgQueueLength.Value)
		    MsgQueue.TryDequeue(out _);
		return MsgRenderer.Render(MsgQueue, opt.MsgFontSize);
    }

    private void DisplayQueue()
    {
	    if (!opt.MsgQueueLength.HasValue) return;
	    while (MsgQueue.Count > opt.MsgQueueLength.Value)
		    MsgQueue.TryDequeue(out _);

	    var content = MsgRenderer.Render(MsgQueue, opt.MsgFontSize);
	    MsgDC.UpdateContent(content);
    }
}