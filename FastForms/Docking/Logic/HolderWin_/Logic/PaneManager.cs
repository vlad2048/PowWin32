using System.Reactive.Linq;
using FastForms.Docking.Logic.HolderWin_.Painting;
using PowBasics.CollectionsExt;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.Utils;

namespace FastForms.Docking.Logic.HolderWin_.Logic;

static class PaneManager
{
	public static void Setup(SysWin sys, HolderState state)
	{
		// When adding Panes -> Assign Pane.Parent
		// =======================================
		state.WhenPanesAdded
			.Subscribe(panes =>
			{
				var paneR = ComputePaneR(sys, state);
				panes.ForEach(pane => pane.SetParent(sys, paneR));
			}).D(sys.D);


		// When Panes, ActivePane -> Set Panes Visibility
		// ==============================================
		Obs.Merge(
				state.Panes.WhenChanged,
				state.JerkLay.ToUnit()
			)
			.Subscribe(_ =>
			{
				for (var i = 0; i < state.Panes.Arr.V.Length; i++)
					state.Panes.Arr.V[i].SetVisibility(i == state.Panes.Idx.V);
				sys.Invalidate();
			}).D(sys.D);


		// Recompute the Panes layout when needed
		// ======================================
		Obs.Merge(
				sys.Evt.WhenSize.ToUnit(),
				state.Panes.Arr.Select(e => e.Length > 1).DistinctUntilChanged().ToUnit(),
				state.JerkLay.ToUnit()
			)
			.Subscribe(_ =>
			{
				var paneR = ComputePaneR(sys, state);
				foreach (var pane in state.Panes.Arr.V)
					pane.SetR(paneR);
			}).D(sys.D);
	}


	private static R ComputePaneR(SysWin sys, HolderState state) => HolderLayout.GetPaneR(sys.ClientR, state.Panes.Arr.V.Length, state.JerkLay.V);
}