using System.Runtime.CompilerServices;
using LINQPad;
using LINQPad.Controls;
using PowRxVar;

namespace FastForms.LINQPad.RxLogging;

public interface IPanel
{
	void Set(string str);
}

public static class VarLoggerExt
{
	public static IPanel RxDump<T>(this IRoVar<T> rxVar, [CallerArgumentExpression(nameof(rxVar))] string varName = "n/a")
	{
		var dcInner = new DumpContainer();
		var panel = new Panel();

		var dc = new DumpContainer();
		dc.AppendContent(dcInner);
		dc.AppendContent(panel.DC);
		dc.Dump();


		rxVar.Subscribe(e => dcInner.UpdateContent(new Literal($"""
			<div>
				<span style='font-weight:bold'>{varName}: </span>
				<span>{e}</span>
			</div>
		""")));

		return panel;
	}

	private sealed class Panel : IPanel
	{
		public DumpContainer DC { get; } = new();
		public void Set(string str) => DC.UpdateContent(str);
	}
}