using FastForms;
using FastForms.Docking;
using FastForms.Docking.Enums;
using PowWin32.Geom;
using PowWin32.Windows;
using FastForms.Docking.Logic.Layout_.Enums;
using PowRxVar;
using PowWin32.Diag;

namespace PlayFast;

static class Program
{
	private static Pane PaneA() => new SimplePane("Bookmarks", 0xF5F5F5);
	private static Pane PaneB() => new SimplePane("Output", 0xdb4bcf);
	private static Pane PaneC() => new SimplePane("Error List", 0xd4a526);
	private static Pane PaneD() => new SimplePane("Solution Explorer", 0xa0e823);
	private static Pane PaneE() => new SimplePane("Docker", 0xe37d42);
	private static Pane PaneF() => new SimplePane("Watch", 0x983dd9);


	private static void Main()
	{
		RunMerge();

		DispDiag.CheckForUndisposedDisps();
	}



	private const int OfsX = -800;
	private static readonly R AppR = new(50 + OfsX, 70, 700, 550);
	private static readonly R DockerR = new(20 + OfsX, 650, 350, 250);

	private static void RunMerge()
	{
		var win = new AppWin(AppR, false);
		//ConsoleMemChecker.Init(win.Sys);
		win.Docker.Name = "Main";
		win.Docker.Dock([PaneA()]);
		win.Docker.Dock([PaneB()], Dock.Right);



		//var dockerRoot = N.RootTool(N.Holder(NodeType.Tool, PaneE(), PaneF()));
		//var docker = Docker.MakeExtra(dockerRoot, win.Sys, DockerR);
		//docker.Name = "Docker_1";


		MsgPump.Run(win.Sys);
	}
}
