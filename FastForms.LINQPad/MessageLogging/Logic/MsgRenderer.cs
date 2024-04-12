using System.Text;
using System.Windows.Forms;
using FastForms.LINQPad.MessageLogging.Structs;
using FastForms.LINQPad.Utils;
using LINQPad.Controls;
using PowWin32.Windows.StructsPInvoke;
using PowWin32.Windows.Utils;

namespace FastForms.LINQPad.MessageLogging.Logic;

static class MsgRenderer
{
	public static Literal Render(IEnumerable<IMsg> source, int fontSize)
	{
		var sb = new StringBuilder($"<div style='font-size:{fontSize}px'>");

		foreach (var msg in source)
		{
			sb.Append(RenderMsg(msg));
		}

		sb.Append("</div>");
		return new Literal(sb.ToString());
	}

	private static string RenderMsg(IMsg msg)
	{
		var sb = new StringBuilder("<div>");

		var timeStr = $"[{msg.Time.Seconds:00}:{msg.Time.Milliseconds:000}{msg.Time.Microseconds:000}] ";
		var timeColor = Cols.GetTimeColor(msg.Delta);
		sb.AddSpan(timeStr, timeColor);

		switch (msg)
		{
			case WinMsg e:
			{
				var idStr = $"{Pad(e.Nesting)}{e.Id}";
				var idColor = Cols.GetMsgColor(e);
				sb.AddSpan(idStr, idColor);
				break;
			}
		}

		sb.Append("</div>");
		return sb.ToString();
	}

	private static void AddSpan(this StringBuilder sb, string str, string color) => sb.Append($"""<span style='color:{color};white-space:pre'>{str}</span>""");
	private static string Pad(int lng) => new string(' ', lng);

	private static class Cols
	{
		public static string GetMsgColor(WinMsg msg) => msg.Id switch
		{
			WM.WM_ACTIVATE => (WindowActivateFlag)msg.WParam.ToSafeInt32().LowAsInt() switch
			{
				WindowActivateFlag.WA_INACTIVE => MsgOff.v(),
				_ => MsgOn.v(),
			},
			WM.WM_ACTIVATEAPP => msg.WParam.ToBool() switch
			{
				true => MsgOn.v(),
				false => MsgOff.v(),
			},
			WM.WM_NCACTIVATE => msg.WParam.ToBool() switch
			{
				true => MsgOn.v(),
				false => MsgOff.v(),
			},
			_ => GetMsgIdColor(msg.Id)
		};

		private static string GetMsgIdColor(WM id) => id switch {
			WM.WM_NCCREATE or WM.WM_NCACTIVATE or WM.WM_NCCALCSIZE or
				WM.WM_NCHITTEST or WM.WM_NCDESTROY or WM.WM_NCPAINT => NonClientMessages.v(),

			WM.WM_CREATE or WM.WM_DESTROY or WM.WM_CLOSE => LifecycleMessages.v(),

			WM.WM_PAINT or WM.WM_SHOWWINDOW => PaintMessages.v(),

			WM.WM_ACTIVATE or WM.WM_ACTIVATEAPP => ActivateMessages.v(),

			WM.WM_ENTERSIZEMOVE or WM.WM_EXITSIZEMOVE or WM.WM_WINDOWPOSCHANGING or WM.WM_WINDOWPOSCHANGED => PosMessages.v(),

			_ => DefaultColor.v()
		};

		public static string GetTimeColor(TimeSpan deltaT)
		{
			var ms = deltaT.TotalMilliseconds;
			if (ms < 005.0) return Time.Val0.v();
			if (ms < 010.0) return Time.Val1.v();
			if (ms < 020.0) return Time.Val2.v();
			if (ms < 040.0) return Time.Val3.v();
			if (ms < 080.0) return Time.Val4.v();
			if (ms < 150.0) return Time.Val5.v();
			if (ms < 300.0) return Time.Val6.v();
			return Time.Val6.v();
		}

		private const int DefaultColor = 0x8e929d;
		private const int NonClientMessages = 0xde1b21;
		private const int LifecycleMessages = 0xe016a4;
		private const int PaintMessages = 0x27e31e;
		private const int ActivateMessages = 0xe3e31b;
		private const int PosMessages = 0x2bb1d6;
		private const int MsgOn = 0x4bdb48;
		private const int MsgOff = 0xdb487c;
		
		private static class Time
		{
			public const int Val0 = 0x27781D;
			public const int Val1 = 0x57712B;
			public const int Val2 = 0x69702A;
			public const int Val3 = 0x6F632A;
			public const int Val4 = 0x6F5327;
			public const int Val5 = 0x683D20;
			public const int Val6 = 0x6B2C1E;
			public const int Val7 = 0x6F2121;
		}
	}
}