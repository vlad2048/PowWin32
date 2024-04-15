using FastForms;
using FastForms.Docking;
using FastForms.Docking.Enums;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows;

namespace DualExplorer;

static class Program
{
	private static readonly R AppR = new(-800, 70, 700, 550);

	
	[STAThread]
	static void Main()
	{
		var win = new AppWin(AppR, false);
		win.Docker.Dock([new ExplorerPane("PaneA"), new ExplorerPane("PaneB"), new ExplorerPane("PaneC"), new ExplorerPane("PaneD")]);
		win.Docker.Dock([new ExplorerPane("PaneE"), new ExplorerPane("PaneF"), new ExplorerPane("PaneG"), new ExplorerPane("PaneH")], Dock.Right);

		MsgPump.Run(win.Sys);

		DispDiag.CheckForUndisposedDisps();
	}
}
