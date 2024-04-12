using System.Drawing;
using PowBasics.Algorithms;
using PowBasics.CollectionsExt;
using PowBasics.ColorCode;
using PowBasics.ColorCode.Structs;
using PowWin32.Geom;

// ReSharper disable once CheckNamespace
namespace PowTrees.Algorithms;


public static class Algo_LoggingColored
{
	public static Txt LogColored<T>(
		this TNod<T> root,
		Action<T, ITxtWriter> writeFun,
		Action<TreeLogColoredOpt>? optFun = null
	) where T : notnull
	{
		var txtMap = root.ToDictionary(
			e => e.V,
			e =>
			{
				var writer = new TxtWriter();
				writeFun(e.V, writer);
				return writer.Txt;
			}
		);

		var opt = TreeLogColoredOpt.Make(optFun);

		var layout = root.Layout(
			e => txtMap[e].GetSize().ToSz(),
			layoutOpt =>
			{
				layoutOpt.GutterSz = opt.GutterSz;
				layoutOpt.AlignLevels = opt.AlignLevels;
			});
		var treeSz = layout.Values.Union().Size;
		var buffer = new Buffer(treeSz);

		foreach (var (n, r) in layout)
		{
			var txt = txtMap[n.V];
			buffer.Print(r.Pos, txt);
		}

		ArrowUtils.DrawArrows(root, layout, (pos, str) => buffer.Print(pos, Txt.FromChunk(str, opt.ArrowColor)));

		return buffer.GetTxt();
	}


	private sealed class Buffer
	{
		private sealed record Ch(char Char, Color Color)
		{
			public static readonly Ch Empty = new(' ', Color.White);
		}

		private readonly R r;
		private Sz sz => r.Size;
		private readonly Ch[,] arr;

		public Buffer(Sz sz)
		{
			r = new R(Pt.Empty, sz);
			arr = new Ch[sz.Width, sz.Height];
			for (var x = 0; x < sz.Width; x++)
			for (var y = 0; y < sz.Height; y++)
				arr[x, y] = Ch.Empty;
		}

		public void Print(Pt pos, Txt txt)
		{
			var txtSize = txt.GetSize().ToSz();
			var txtR = new R(pos, txtSize);
			var unionR = new[] { r, txtR }.Union();
			if (unionR != r) throw new ArgumentException("Trying to write outside of the bounds");

			var y = pos.Y;
			foreach (var line in txt.Lines)
			{
				var x = pos.X;
				foreach (var chunk in line)
				{
					var (text, color) = chunk;
					foreach (var c in text)
					{
						arr[x, y] = new Ch(c, color);
						x++;
					}
				}
				y++;
			}
		}


		public Txt GetTxt()
		{
			var writer = new TxtWriter();
			for (var y = 0; y < sz.Height; y++)
			{
				var bufLine = new Ch[sz.Width];
				for (var x = 0; x < sz.Width; x++)
					bufLine[x] = arr[x, y];

				var charChunks = ConsecutiveSplitter.Split(bufLine, (a, b) => a.Color != b.Color);
				foreach (var charChunk in charChunks)
				{
					var chArr = charChunk.SelectToArray(e => e.Char);
					var color = charChunk.First().Color;
					var text = new string(chArr);
					writer.Write(text, color);
				}
				writer.WriteLine();
			}
			return writer.Txt;
		}
	}
}



file static class Algo_LoggingColored_Ext
{
	public static Sz ToSz(this (int, int) t) => new(t.Item1, t.Item2);
}