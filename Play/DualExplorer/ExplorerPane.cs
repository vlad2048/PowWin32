using FastForms.Docking;
using PowWin32.Geom;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using Vanara.PInvoke;
using static Vanara.PInvoke.Shell32;

namespace DualExplorer;

sealed class ExplorerPane : ToolPane
{
	public ExplorerPane(string name) : base(name)
	{
		IExplorerBrowser browser = null!;

		Sys.Evt.WhenCreate.Subs((ref CreatePacket e) =>
		{
			browser = new IExplorerBrowser();
			browser.SetOptions(EXPLORER_BROWSER_OPTIONS.EBO_SHOWFRAMES);
			var hwnd = Sys.Handle.DangerousGetHandle();
			var clientR = Sys.ClientR.ToRect();

			var opts = new PFOLDERSETTINGS(
				FOLDERVIEWMODE.FVM_DETAILS,
				FOLDERFLAGS.FWF_NONE
			);
			browser.Initialize(hwnd, clientR, opts);

			SHParseDisplayName(@"D:\utils", 0, out var pidl, 0, out _);
			browser.BrowseToIDList(pidl, 0);
		});

		Sys.Evt.WhenSize.Subs((ref SizePacket e) =>
		{
			var clientR = Sys.ClientR.ToRect();
			browser.SetRect(0, clientR);
		});

		Sys.Evt.WhenDestroy.Subs((ref Packet e) =>
		{
			browser.Destroy();
		});
	}
}



static class ExplorerRunnerExt
{
	public static RECT ToRect(this R r) => new(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
}
