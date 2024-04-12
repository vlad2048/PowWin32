using FastForms;
using FastForms.Docking;
using FastForms.Docking.Enums;
using PowWin32.Geom;
using PowWin32.Windows;
using FastForms.Docking.Logic.DockerWin_;
using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Utils;
using PowWin32.Windows.StructsPInvoke;
using FastForms.Utils.WinEventUtils;
using PlayFast.Demos;
using PowRxVar;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PlayFast;

static class Program
{
	private static Pane PaneA() => new SimplePane("Bookmarks", 0xF5F5F5); //0x49a5de);
	private static Pane PaneB() => new SimplePane("Output", 0xF5F5F5); //0xdb4bcf);
	private static Pane PaneC() => new SimplePane("Error List", 0xd4a526);
	private static Pane PaneD() => new SimplePane("Solution Explorer", 0xa0e823);
	private static Pane PaneE() => new SimplePane("Docker", 0xe37d42);
	private static Pane PaneF() => new SimplePane("Watch", 0x983dd9);


	private static void Main()
	{
		//ConsoleMemChecker.Init(win.Sys);
		//TypicalWin32App.Run();

		RunMerge();

		DispDiag.CheckForUndisposedDisps();
	}



	private const int OfsX = -600;
	private static readonly R AppR = new(50 + OfsX, 70, 500, 450);
	private static readonly R DockerR = new(20 + OfsX, 600, 250, 150);
	//private static readonly R DockerR2 = new(20 + OfsX, 800, 250, 150);

	private static void RunMerge()
	{
		var win = new AppWin(AppR, true); win.Docker.AddPanes([PaneA(), PaneB()]);
		var docker = Docker.MakeExtra(N.RootTool(N.Holder(NodeType.Tool, PaneE())), win.Sys, DockerR);

		win.Docker.Name = "Main";
		docker.Name = "Docker_1";

		MsgPump.Run(win.Sys);
	}


	/*
	private static void RunDockerWinSingle()
	{
		var winR = new R(-500, 130, 400, 200);

		var dockerRoot = N.RootTool(
			N.Holder(NodeType.Tool, PaneA())
		);

		var docker = new Docker(dockerRoot, null, winR);

		docker.Sys.WhenKey(VirtualKey.SPACE).Subscribe(_ =>
		{
		}).D(docker.D);


		MsgPump.Run(docker.Sys);
	}


	private static void RunDockerWinSplit()
	{
		var winR = new R(-500, 130, 400, 200);

		var dockerRoot = N.RootTool(
			N.Split(NodeType.Tool, Dir.Horz,
				N.Holder(NodeType.Tool, PaneA(), PaneB()),
				N.Holder(NodeType.Tool, PaneC(), PaneD())
			)
			//N.Holder(NodeType.Tool, paneA, paneB)
		);

		//var win = new DockerWin(winR, dockerRoot);
		var docker = new Docker(dockerRoot, null, winR);

		docker.Sys.WhenKey(VirtualKey.SPACE).Subscribe(_ =>
		{
		}).D(docker.Sys.D);

		MsgPump.Run(docker.Sys);
	}


	private static void MainRun()
	{
		var win = new AppWin(new R(50, 100, 300, 250), false);

		win.Docker.AddPanes([PaneA(), PaneB(), PaneC(), PaneD()]);

		MsgPump.Run(win.Sys);
	}
	*/
}
