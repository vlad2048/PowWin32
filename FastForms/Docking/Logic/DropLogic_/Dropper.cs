using System.Reactive.Linq;
using DynamicData;
using FastForms.Docking.Logic.DropLogic_.Structs;
using FastForms.Docking.Logic.DropLogic_.Wins;
using FastForms.Docking.Logic.Layout_.Nodes;
using FastForms.Utils.Win32;
using PowMaybe;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows.ReactiveLight;
using Vanara.PInvoke;

namespace FastForms.Docking.Logic.DropLogic_;



static class Dropper
{
	public static void Setup(
		Docker dockerSrc
	)
	{
		var sysSrc = dockerSrc.Sys;
		var d = dockerSrc.D;
		var dropWin = new DropWin().D(d);
		var drop = Var.Make(May.None<Drop>(), d);


		// Track mouse position while the window is being moved
		// ====================================================
		var mouse =
			Obs.Merge(
				sysSrc.Evt.WhenEnterSizeMove.Select(_ => true),
				sysSrc.Evt.WhenExitSizeMove.Select(_ => false)
			)
			.Select(isMoving_ => isMoving_ switch
			{
				false => Obs.Return(May.None<Pt>()),
				true => sysSrc.Evt.WhenWindowPosChanged.Select(_ => May.Some(MouseTracking.GetCursorPos()))
			})
			.Switch();


		// Detect docking (early subscription needed to avoid seeing drop=None)
		// ==============
		sysSrc.Evt.WhenExitSizeMove.ToObs()
			.WithLatestFrom(drop, (_, drop_) => drop_)
			.WhenSome()
			.Subscribe(drop_ =>
			{
				L($"DROP => {drop_}");
				var (dockerDst, target) = (drop_.Docker, drop_.Target);
				Pane[] panes =
				[
					..
					from node in dockerSrc.Root
					where node.V is HolderNode
					let holder = (HolderNode)node.V
					from pane in holder.State.GetAndRemovePanes()
					select pane
				];
				dockerDst.ExecDrop(target, panes);
				dockerSrc.Sys.Destroy();
			}).D(d);


		// Find the zones to display
		// =========================
		var zones = mouse
			.Select2(mouse_ => GetDockerUnderMouse(mouse_, sysSrc.Handle))
			.SelectMayArray(t => t.Item2.QueryZones(dockerSrc.TreeType.V).Where(zone => zone.ZoneR.Contains(t.Item1)).ToArray())
			.ToCache(e => e.Id, (a, b) => a.Id == b.Id, d);


		// Track the active drop (mouse hover)
		// ===================================
		zones
			.TransformMany(zone => zone.Drops, drop_ => drop_.Id)
			.ToCollection()
			.CombineLatest(mouse, (ds, mayMouse) => mayMouse.IsSome(out var mouse_) switch
			{
				false => May.None<Drop>(),
				true => ds.FirstOrMaybe(e => e.R.Contains(mouse_))
			})
			.Subscribe(mayDrop => drop.V = mayDrop).D(d);


		// Display the zones and drop buttons in popup windows
		// ===================================================
		zones
			.Transform(zone => new ZoneWin(zone, drop))
			.DisposeMany()
			.Subscribe().D(d);


		// Display the pane drop hint for the active drop
		// ==============================================
		drop.Subscribe(mayDrop => dropWin.Set(mayDrop)).D(d);
	}


	private static Maybe<Docker> GetDockerUnderMouse(Pt mouse, HWND exclude) => WindowFinder.GetWindowAt<Docker>(mouse, DockingConsts.PropNames.Docker, exclude).ToMaybe();


	private static IObservable<IChangeSet<V, K>> ToCache<V, K>(
		this IObservable<V[]> source,
		Func<V, K> keyFun,
		Func<V, V, bool> cmpFun,
		Disp d
	) where V : notnull where K : notnull
	{
		var srcCache = new SourceCache<V, K>(keyFun).D(d);
		source.Subscribe(xs => srcCache.EditDiff(xs, cmpFun)).D(d);
		return srcCache.Connect();
	}




	private static IObservable<Maybe<(T, U)>> Select2<T, U>(this IObservable<Maybe<T>> source, Func<T, Maybe<U>> fun) =>
		source
			.Select(mayVal =>
				from val in mayVal
				from res in fun(val)
				select (val, res)
			);

	private static IObservable<U[]> SelectMayArray<T, U>(this IObservable<Maybe<T>> source, Func<T, U[]> fun) =>
		source
			.Select(mayVal => mayVal.IsSome(out var val) switch
			{
				true => fun(val),
				false => []
			});


	private static IObservable<T> WhenSome<T>(this IObservable<Maybe<T>> source) =>
		source
			.Where(e => e.IsSome())
			.Select(e => e.Ensure());





	//private static IRoVar<Maybe<T>> ToMayVar<T>(this IObservable<Maybe<T>> source, Disp d) => Var.Make(May.None<T>(), source, d);


	//private static IObservable<Zone[]> GetActiveZones(this IObservable<Maybe<Pt>> mouse, SysWin sys);


	/*
	private static IObservable<Maybe<Docker>> GetDockerUnderMouse(IObservable<bool> isMoving, SysWin sysSrc) =>
		isMoving
			.Select(moving => moving switch
			{
				false => Obs.Return(May.None<Docker>()),
				true => sysSrc.Evt.WhenWindowPosChanged
					.Select(_ => GetDockerUnderMouse(sysSrc.Handle))
			})
			.Switch();


	private static Maybe<Docker> GetDockerUnderMouse(HWND exclude) => WindowFinder.GetWindowUnderMouse<Docker>(DockingConsts.PropNames.Docker, exclude).ToMaybe();

	private static IObservable<U[]> SelectMayArray<T, U>(this IObservable<Maybe<T>> source, Func<T, U[]> fun) =>
		source
			.Select(mayVal => mayVal.IsSome(out var val) switch
			{
				true => fun(val),
				false => []
			});
	*/

	//private static IRoVar<Maybe<T>> ToMayVar<T>(this IObservable<Maybe<T>> source, Disp d) => Var.Make(May.None<T>(), source, d);
}



file static class RSetFileExt
{
	public static bool Contains(this RSet r, Pt p) => r.Rs.Any(e => e.Contains(p));
}