using FastForms.LINQPad.Hooking.Events;
using FastForms.LINQPad.MessageLogging;
using LINQPad;
using LINQPad.Controls;
using PowRxVar;
using Vanara.PInvoke;

namespace FastForms.LINQPad.WinLogging;

sealed class WinRow : IDisposable
{
	private readonly Disp d = new("WinRow");
	public void Dispose() => d.Dispose();

	public WinState State { get; }
	private readonly MsgTracker msgTracker;

	private readonly DumpContainer parentIdDC = new();
	private readonly DumpContainer ownerIdDC = new();
	private readonly DumpContainer captureDC = new();
	private readonly DumpContainer geomNfoDC = new();
	private readonly DumpContainer stateNfoDC = new();
	private readonly DumpContainer stylesNfoDC = new();
	private readonly DumpContainer msgDC = new();

	public object RenderRow => new {
		Id = State.Id.Render(),
		Parent = parentIdDC,
		Owner = ownerIdDC,
		Capture = captureDC,
		Geom = geomNfoDC,
		State = stateNfoDC,
		Styles = stylesNfoDC,
		Messages = msgDC,
	};

	public WinRow(HWND hwnd, WinLoggerOpt opt)
	{
		State = new WinState(hwnd, d);
		msgTracker = new MsgTracker(opt, d);

		State.ParentId.Subscribe(e => parentIdDC.UpdateContent(e.Render())).D(d);
		State.OwnerId.Subscribe(e => ownerIdDC.UpdateContent(e.Render())).D(d);
		State.Capture.Subscribe(e => captureDC.UpdateContent(e.Render())).D(d);
		State.GeomNfo.Subscribe(e => geomNfoDC.UpdateContent(e.Render())).D(d);
		State.StateNfo.Subscribe(e => stateNfoDC.UpdateContent(e.Render())).D(d);
		State.StylesNfo.Subscribe(e => stylesNfoDC.UpdateContent(e.Render())).D(d);

		msgTracker.WhenChanged.Subscribe(_ => msgDC.UpdateContent(msgTracker.Render())).D(d);
	}

	public void ProcessEvent(IHookEvt evt)
	{
		msgTracker.ProcessEvent(evt);
		State.ProcessEvent(evt);
	}
}



file static class RenderExt
{
	public static Literal Render(this WinId? id) => id switch
	{
		null => new Literal("_"),
		not null => new Literal(
			$$"""
			  <div>
			  	<div>{{id.Hwnd.fmtPtr()}}</div>
			  	<div>{{id.ClassName}}</div>
			  </div>
			  """)
	};

	public static Literal Render(this WinGeomNfo e) => new(
		$$"""
		  <div class='winstate-geom'>
		  	<span>WinR   </span><span>{{e.WindowRect}}</span>
		  	<span>ClientR</span><span>{{e.ClientRect}}</span>
		  </div>
		  """
	);

	public static Literal Render(this CapNfo e) => new(
		$$"""
		  <div class='winstate-state'>
		  	{{MkStateOnOffItem(e.IsCapture, "capture", "no capture")}}
		  	{{MkStateOnOffItem(e.IsActive, "active", "inactive")}}
		  	{{MkStateOnOffItem(e.IsFocus, "focus", "no focus")}}
		  	{{MkStateOnOffItem(e.IsMoveSize, "movesize", "no movesize")}}
		  </div>
		  """
	);

	public static Literal Render(this WinStateNfo e) => new(
		$$"""
		  <div class='winstate-state'>
		  	{{MkStateOnOffItem(e.IsVisible, "visible", "hidden")}}
		  	{{MkStateOnOffItem(e.IsEnabled, "enabled", "disabled")}}
		  	{{MkStateShowCmdItem(e.ShowCmd)}}
		  </div>
		  """
	);

	public static Literal Render(this WinStylesNfo e) => new(
		$$"""
		<div>
			<div>{{e.Styles:X}}</div>
			<div>{{e.ExStyles:X}}</div>
		</div>
		"""
	);


	private static string MkStateOnOffItem(bool val, string strOn, string strOff) => val switch
	{
		false => $"<div class='winstate-state-item winstate-state-item-off'>{strOff}</div>",
		true => $"<div class='winstate-state-item winstate-state-item-on'>{strOn}</div>",
	};

	private static string MkStateShowCmdItem(ShowWindowCommand showCmd)
	{
		var str = $"{showCmd}".Replace("SW_", "");
		return $$"""<div class='winstate-state-showcmd'>{{str}}</div>""";
	}


	private static string fmtPtr(this HWND v) => $"0x{v.DangerousGetHandle():X8}";
}