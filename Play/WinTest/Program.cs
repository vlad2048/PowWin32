using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Structs;
using PowWin32.Windows.StructsPackets;
using Vanara.PInvoke;
using WinTest.Wins;
using WinTest.Wins.ToolWinLogic;

namespace WinTest;

static class Program
{
	private static readonly R MainR = new(30, 50, 400, 350);
	private const bool StartDocked = false;
	private static readonly R DockR = StartDocked switch
	{
		true => new(120, 30, 180, 100),
		false => new(150, 160, 250, 200)
	};


	static void Main()
	{
		Console.Clear();
		var mainWin = new AppWin(MainR);
		var toolWin = new ToolWin(DockR, mainWin.Sys.Handle, StartDocked);

		mainWin.Sys.Evt.WhenMouseButton.Subs((ref MouseButtonPacket e) =>
		{
			if (!e.IsDown) return;
			if (toolWin.IsDocked)
				toolWin.Undock();
			else
				toolWin.Dock();
		});

		toolWin.Sys.Evt.WhenNcCalcSize.Subs((ref NcCalcSizePacket e) =>
		{
			Console.WriteLine("NcCalcSize");
		});

		MsgPump.Run(mainWin.Sys);
	}
}