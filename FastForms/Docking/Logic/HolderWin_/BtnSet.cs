using System.Drawing.Imaging;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FastForms.Docking.Utils.Btns_;
using FastForms.Utils;
using FastForms.Utils.GdiUtils;
using PowBasics.CollectionsExt;
using PowRxVar;
using PowWin32.Geom;
using PowWin32.Windows;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.StructsPackets;
using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace FastForms.Docking.Logic.HolderWin_;







/*
enum BtnMouseState
{
    Normal,
    Hover,
    Pressed,
}

sealed record BtnDrawRes(
    Bitmap Bmp,
    Brush? BackBrush,
    ImageAttributes? Attrs
);

static class GfxBtnExt
{
    private static readonly ImageAttributes emptyAttrs = new();

    public static void PaintBtn(this Graphics gfx, BtnDrawRes res, Pt pos)
    {
        var r = new R(pos.X, pos.Y, res.Bmp.Width, res.Bmp.Height);
        if (res.BackBrush != null) gfx.FillRectangle(res.BackBrush, r.ToR());
        gfx.DrawImage(res.Bmp, r.ToR(), 0, 0, r.Width, r.Height, GraphicsUnit.Pixel, res.Attrs ?? emptyAttrs);
    }
}


sealed record BtnLayout(int Btn)
{
    public bool Visible { get; set; }
    public R R { get; set; }
}

readonly record struct BtnSetLoc(Pt Pos, int Width);


sealed class BtnSet<E> where E : struct, Enum
{
    private interface ISt;
    private sealed record NoneSt : ISt;
    private sealed record HoverSt(int Btn) : ISt;
    private sealed record PressSt(int Btn, bool IsOver) : ISt;



    private readonly SysWin win;
    private readonly BtnSetStyle style;
    private readonly Bitmap[] bmps;
    private readonly Func<R, BtnSetLoc> locFun;
    private readonly int gutter;

    private readonly ISubject<E> whenClicked;

    private ISt st = new NoneSt();
    private ISt St
    {
        get => st;
        set
        {
            if (value.Equals(st)) return;
            st = value;

            if (User32.IsWindow(win.Handle))
            {
	            // ReSharper disable once UnusedVariable
	            var invalidR = layout.Where(e => e.Visible).Select(e => e.R).Union();
                win.Invalidate();
            }
        }
    }

    private readonly BtnLayout[] layout = Enum.GetValues(typeof(E)).Cast<E>().SelectToArray(e => new BtnLayout(e.ToInt()));

    public IObservable<E> WhenClicked => whenClicked.AsObservable();
    public int Count => layout.Count(e => e.Visible);

    public bool IsScreenPointOver(Pt ptScr)
    {
	    var ptCli = win.Screen2Client(ptScr);
	    var res = layout.Any(e => e.R.Contains(ptCli));
	    return res;
    }


    public BtnSet(
	    SysWin win,
        Func<R, BtnSetLoc> locFun,
        int gutter,
        Bitmap[] bmps,
        Func<Bitmap, BtnMouseState, bool, BtnDrawRes> resFun
    )
    {
        this.win = win;
        whenClicked = new Subject<E>().D(win.D);
        this.bmps = bmps;
        this.locFun = locFun;
        this.gutter = gutter;
        for (var i = 0; i < bmps.Length; i++)
        {
            foreach (var mouseState in Enum.GetValues<BtnMouseState>())
                foreach (var active in BtnSetFileExt.Bools)
                    resMap[(i, mouseState, active)] = resFun(bmps[i], mouseState, active);
        }

		win.Evt.WhenMouseButton.Where(e => e is { IsDown: true, Button: MouseButton.Left }).Subs((ref MouseButtonPacket _) => MouseDown());
        win.Evt.WhenMouseButton.Where(e => e is { IsDown: false, Button: MouseButton.Left }).Subs((ref MouseButtonPacket _) => MouseUp());
        win.Evt.WhenMouseMove.Subs((ref MousePacket e) => MouseMove(e.Point));
        win.Evt.WhenWindowPosChanged.Subs((ref WindowPosPacket _) => ComputeLayout());
    }



	public void SetVisible(params E[] es)
    {
        if (es.IsSame(layout)) return;
        var indices = es.Select(e => e.ToInt()).ToHashSet();
        for (var i = 0; i < layout.Length; i++)
            layout[i].Visible = indices.Contains(i);

        ComputeLayout();

        St = new NoneSt();
    }



    public void Paint(Graphics gfx, bool active)
    {
        foreach (var btnLayout in layout)
        {
            if (!btnLayout.Visible) continue;

            var (btnId, btnR) = (btnLayout.Btn, btnLayout.R);
            var state = St switch
            {
                HoverSt { Btn: var btn } when btn == btnId => BtnMouseState.Hover,
                PressSt { Btn: var btn, IsOver: true } when btn == btnId => BtnMouseState.Pressed,
                PressSt { Btn: var btn, IsOver: false } when btn == btnId => BtnMouseState.Hover,
                _ => BtnMouseState.Normal,
            };
            var res = resMap[(btnId, state, active)];
            gfx.PaintBtn(res, btnR.Pos);
        }
    }


    private void MouseMove(Pt mousePos)
    {
        var mouseBtn = layout.GetMouseBtn(mousePos);

        switch (St)
        {
            case NoneSt when mouseBtn.HasValue:
                St = new HoverSt(mouseBtn.Value);
                break;
            case HoverSt:
                St = mouseBtn switch
                {
                    null => new NoneSt(),
                    not null => new HoverSt(mouseBtn.Value)
                };
                break;
            case PressSt { Btn: var btn, IsOver: true } when mouseBtn != btn:
                St = new PressSt(btn, false);
                break;
            case PressSt { Btn: var btn, IsOver: false } when mouseBtn == btn:
                St = new PressSt(btn, true);
                break;
        }
    }

    private void MouseDown()
    {
        switch (St)
        {
            case HoverSt { Btn: var btn }:
                St = new PressSt(btn, true);
                break;
        }
    }

    private void MouseUp()
    {
        switch (St)
        {
            case PressSt { Btn: var btn, IsOver: var isOver }:
                if (isOver)
                    whenClicked.OnNext(btn.ToEnum<E>());
                St = new NoneSt();
                break;
        }
    }

    private void ComputeLayout()
    {
        if (!User32.IsWindow(win.Handle)) return;

        var clientR = win.ClientR;
        var loc = locFun(clientR);
        var x = loc.Pos.X + loc.Width;
        var y = loc.Pos.Y;
        for (var i = layout.Length - 1; i >= 0; i--)
        {
            if (!layout[i].Visible)
            {
                layout[i].R = R.Empty;
                continue;
            }
            var bmp = bmps[i];
            x -= bmp.Width;
            layout[i].R = new R(x, y, bmp.Width, bmp.Height);
            x -= gutter;
        }
    }
}




file static class BtnSetFileExt
{
    public static readonly bool[] Bools = [false, true];

    public static bool IsSame<E>(this E[] es, BtnLayout[] layout) where E : struct, Enum =>
        layout.Length == es.Length &&
        layout.Zip(es, (l, e) => (btn: l.Btn, e: e.ToInt())).All(t => t.btn == t.e);

    public static int? GetMouseBtn(this BtnLayout[] layout, Pt mousePos)
    {
        foreach (var btn in layout)
            if (btn.R.Contains(mousePos)) return btn.Btn;
        return null;
    }
}
*/