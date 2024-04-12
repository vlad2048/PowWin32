using FastForms.LINQPad.Utils;
using LINQPad;
using PowRxVar;

namespace FastForms.LINQPad;

public static class Resetter
{
	internal static Disp D = null!;

	internal static DateTime StartTime;
	internal static TimeSpan Time => DateTime.Now - StartTime;

	public static void Reset()
	{
		D?.Dispose();
		D = new Disp("Resetter.OnStart");

		//WinLogger.Reset();
		CssVars.Reset();
		Sync.Reset();
		SetStyles();

		//ApplicationHelpers.SetupDefaultExceptionHandlers();

		StartTime = DateTime.Now;
	}

	private static void SetStyles()
	{
		Util.HtmlHead.AddStyles("""
			body {
				font-family: Consolas;
			}
			
			.winlist {
				font-size: 10px;
			}
			.winlist .typeheader {
				display: none;
			}
			
			
			.winstate-geom {
				display: grid;
				grid-template-columns: 1fr 1fr;
			}
			
			
			.winstate-state {
				display: flex;
				flex-direction: column;
				row-gap: 2px;
				color: #000;
			}
			.winstate-state-item {
				padding: 2px 5px;
			}
			.winstate-state-item-on {
				background-color: #5fba49;
			}
			.winstate-state-item-off {
				background-color: #783745;
			}
			.winstate-state-showcmd {
				color: white;
			}
		
		""");
	}
}