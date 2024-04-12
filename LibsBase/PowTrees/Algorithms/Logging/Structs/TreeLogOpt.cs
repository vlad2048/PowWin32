using PowWin32.Geom;

// ReSharper disable once CheckNamespace
namespace PowTrees.Algorithms;

public sealed class TreeLogOpt<T>
{
    public Func<T, string> FmtFun { get; set; } = e => $"{e}";
    public Sz GutterSz { get; set; } = new(3, 1);
    public bool AlignLevels { get; set; } = true;

    private TreeLogOpt() { }

    internal static TreeLogOpt<T> Make(Action<TreeLogOpt<T>>? optFun)
    {
        var opt = new TreeLogOpt<T>();
        optFun?.Invoke(opt);
        return opt;
    }
}