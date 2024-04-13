using System.Reactive.Linq;

namespace PowRxVar;

public static class VarOps
{
	public static IRoVar<U> SelectVar<T, U>(this IRoVar<T> rx, Func<T, U> fun, Disp d) =>
		Var.Make(
			fun(rx.V),
			rx.Select(fun),
			d
		);

	public static IRoVar<U> Switch<T, U>(this IRoVar<T> rx, Func<T, IRoVar<U>> sel, Disp d) =>
		Var.Make(
			sel(rx.V).V,
			rx.Select(sel).Switch(),
			d
		);
}