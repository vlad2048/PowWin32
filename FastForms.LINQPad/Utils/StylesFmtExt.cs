using System.Text;
using LINQPad.Controls;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace FastForms.LINQPad.Utils;

public static class StylesFmtExt
{
	public static Literal Fmt(this User32.WindowStyles v)
	{
		var sb = new StringBuilder();
		void write(string str, string col) => sb.Append($"<span style='color:{col}'>{str}</span>");
		void writeFlag(User32.WindowStyles flag, string str, string col)
		{
			if (v.HasFlag(flag))
				write(str, col);
			else
				write(str, Cols.Missing.v());
		}



		sb.Append("<div>");
		var typStr = (v.HasFlag(User32.WindowStyles.WS_POPUP), v.HasFlag(User32.WindowStyles.WS_CHILD)) switch
		{
			(false, false) => "_ ",
			(false, true) => "k ",
			(true, false) => "p ",
			(true, true) => "p/k ",
		};
		write(typStr, Cols.Type.v());
		writeFlag(User32.WindowStyles.WS_SYSMENU, "s", Cols.Box.v());
		writeFlag(User32.WindowStyles.WS_MINIMIZEBOX, "m", Cols.Box.v());
		writeFlag(User32.WindowStyles.WS_MAXIMIZEBOX, "M", Cols.Box.v());
		writeFlag(User32.WindowStyles.WS_SIZEBOX, "z ", Cols.Box.v());
		writeFlag(User32.WindowStyles.WS_DLGFRAME, "d", Cols.Frame.v());
		writeFlag(User32.WindowStyles.WS_BORDER, "b", Cols.Frame.v());
		sb.Append("</div>");



		sb.Append("<div>");
		if (!v.HasFlag(User32.WindowStyles.WS_VISIBLE)) write("v", Cols.Off.v());
		if (v.HasFlag(User32.WindowStyles.WS_DISABLED)) write("d", Cols.Off.v());
		var str = (v.HasFlag(User32.WindowStyles.WS_MAXIMIZE), v.HasFlag(User32.WindowStyles.WS_MINIMIZE)) switch
		{
			(false, false) => " ",
			(false, true) => "m ",
			(true, false) => "M ",
			(true, true) => "m/M ",
		};
		write(str, Cols.State.v());
		if (v.HasFlag(User32.WindowStyles.WS_HSCROLL) || v.HasFlag(User32.WindowStyles.WS_VSCROLL))
		{
			str = (v.HasFlag(User32.WindowStyles.WS_HSCROLL), v.HasFlag(User32.WindowStyles.WS_VSCROLL)) switch
			{
				(false, false) => throw new ArgumentException(),
				(false, true) => "sV ",
				(true, false) => "sH ",
				(true, true) => "sHV ",
			};
			write(str, Cols.ScrollClip.v());
		}
		if (v.HasFlag(User32.WindowStyles.WS_CLIPCHILDREN) || v.HasFlag(User32.WindowStyles.WS_CLIPSIBLINGS))
		{
			str = (v.HasFlag(User32.WindowStyles.WS_CLIPCHILDREN), v.HasFlag(User32.WindowStyles.WS_CLIPSIBLINGS)) switch
			{
				(false, false) => throw new ArgumentException(),
				(false, true) => "cS ",
				(true, false) => "cK ",
				(true, true) => "cKS ",
			};
			write(str, Cols.ScrollClip.v());
		}
		sb.Append("</div>");




		sb.Append("<div>");
		var valStr = $"{v:X}";
		write($"0x{valStr[..4]}", Cols.ValMain.v());
		write($" {valStr[4..]}  ", Cols.ValCtrl.v());
		sb.Append("</div>");


		return new Literal(sb.ToString());
	}


	public static Literal Fmt(this WindowStylesEx v)
	{
		var sb = new StringBuilder();
		sb.Append("<div>");
		void Add(params WindowStylesEx[] masks)
		{
			foreach (var mask in masks)
			{
				if (v.HasFlag(mask))
				{
					var str = $"{mask}";
					if (str.StartsWith("WS_EX_")) str = str[6..];
					sb.Append($"<div>{str}</div>");
				}
			}
		}
		Add([
			WindowStylesEx.WS_EX_DLGMODALFRAME,
			WindowStylesEx.WS_EX_NOPARENTNOTIFY,
			WindowStylesEx.WS_EX_TOPMOST,
			WindowStylesEx.WS_EX_ACCEPTFILES,
			WindowStylesEx.WS_EX_TRANSPARENT,
			WindowStylesEx.WS_EX_MDICHILD,
			WindowStylesEx.WS_EX_TOOLWINDOW,
			WindowStylesEx.WS_EX_WINDOWEDGE,
			WindowStylesEx.WS_EX_CLIENTEDGE,
			WindowStylesEx.WS_EX_CONTEXTHELP,
			WindowStylesEx.WS_EX_RIGHT,
			WindowStylesEx.WS_EX_RTLREADING,
			WindowStylesEx.WS_EX_LEFTSCROLLBAR,
			WindowStylesEx.WS_EX_CONTROLPARENT,
			WindowStylesEx.WS_EX_STATICEDGE,
			WindowStylesEx.WS_EX_APPWINDOW,
			WindowStylesEx.WS_EX_LAYERED,
			WindowStylesEx.WS_EX_NOINHERITLAYOUT,
			WindowStylesEx.WS_EX_NOREDIRECTIONBITMAP,
			WindowStylesEx.WS_EX_LAYOUTRTL,
			WindowStylesEx.WS_EX_COMPOSITED,
			WindowStylesEx.WS_EX_NOACTIVATE,
		]);

		sb.Append("<div>");
		var valStr = $"{v:X}";
		sb.Append($"0x{valStr[..4]}");
		sb.Append($" {valStr[4..]}");
		sb.Append("</div>");

		sb.Append("</div>");
		return new Literal(sb.ToString());
	}



	private static class Cols
	{
		public const string ValMain = "#d6d6d6";
		public const string ValCtrl = "#636363";
		public const string Missing = "#444";
		public const string Type = "#d92998";
		public const string Box = "#2f64d6";
		public const string Frame = "#9fcc2d";

		public const string On = "#5e7a56";
		public const string Off = "#cf134b";
		public const string State = "#9beb5b";
		public const string ScrollClip = "#878787";
	}
}