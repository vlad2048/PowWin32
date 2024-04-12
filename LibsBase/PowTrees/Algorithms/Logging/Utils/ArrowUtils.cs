using PowBasics.CollectionsExt;
using PowWin32.Geom;

// ReSharper disable once CheckNamespace
namespace PowTrees.Algorithms;

static class ArrowUtils
{
    public static void DrawArrows<T>(TNod<T> rootOrig, Dictionary<TNod<T>, R> layout, Action<Pt, string> print)
    {
	    var root = rootOrig.MapN(e => layout[e]);
        //┌─┬─┐    ╭───╮
        //│ │ │    │   │
        //├─┼─┤    ╰───╯
        //└─┴─┘    

        var chHoriz = "─";
        var chVert = "│";
        var chCornerTop = "┌";
        var chCornerBottom = "└";
        var chCross = "┼";
        var chTUp = "┴";
        var chTDown = "┬";
        var chTLeft = "┤";
        var chTRight = "├";
        var chArrow = "►";

        root
            .Where(e => e.Kids.Count == 1).ForEach(n =>
            {
                var d = n.Kids[0].V.X - (n.V.X + n.V.Width);
                print(n.V.OnTheRight(), $"{new string(chHoriz[0], d - 1)}{chArrow}");
            });

        root
            .Where(e => e.Kids.Count > 1)
            .ForEach(n =>
            {
                var rp = n.V;
                var rcs = n.Kids.SelectToArray(e => e.V);
                var xMid = (rp.X + rp.Width + rcs[0].X - 1) / 2;
                var yMid = rp.YMid();
                var yMin = rcs.First().YMid();
                var yMax = rcs.Last().YMid();

                print(n.V.OnTheRight(), new string(chHoriz[0], xMid - (rp.X + rp.Width)));

                for (var i = yMin; i <= yMax; i++)
                    print(new Pt(xMid, i), chVert);

                print(new Pt(xMid, rp.YMid()), chTLeft);

                for (var i = 0; i < rcs.Length; i++)
                {
                    var r = rcs[i];
                    var y = r.YMid();

                    var ch = (i == 0, y == yMid, i == rcs.Length - 1) switch
                    {
                        (true, true, true) => chHoriz,
                        (true, true, false) => chTDown,
                        (true, false, false) => chCornerTop,
                        (false, true, false) => chCross,
                        (false, false, true) => chCornerBottom,
                        (false, true, true) => chTUp,
                        (false, false, false) => chTRight,
                        _ => "V"
                    };
                    print(new Pt(xMid, y), $"{ch}{new string(chHoriz[0], r.X - xMid - 2)}{chArrow}");
                }
            });
    }


    private static Pt OnTheRight(this R r) => new(r.Right, r.YMid());

    private static int YMid(this R r) => r.Y + r.Height / 2;
}