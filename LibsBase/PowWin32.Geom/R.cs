using System.Text.Json.Serialization;
using Vanara.PInvoke;

namespace PowWin32.Geom;

public readonly record struct R(int X, int Y, int Width, int Height)
{
	/// <summary>
	/// Represents the pixel after the right of the rectangle such that Right = X + Width
	/// </summary>
	[JsonIgnore]
	public int Right => X + Width;

	/// <summary>
	/// Represents the pixel after the bottom of the rectangle such that Bottom = Y + Height
	/// </summary>
	[JsonIgnore]
	public int Bottom => Y + Height;

	[JsonIgnore]
	public Pt BottomRight => new(Right, Bottom);

	[JsonIgnore]
	public Pt Pos => new(X, Y);
	[JsonIgnore]
	public Sz Size => new(Width, Height);
	public static readonly R Empty = new(0, 0, 0, 0);

	public R(Pt pos, Sz size) : this(pos.X, pos.Y, size.Width, size.Height)
	{
	}

	public R(Sz size) : this(0, 0, size.Width, size.Height)
	{
	}
	public override string ToString() => $"{X},{Y} {Size}";


	public static R operator +(R a, Pt b) => a == Empty ? Empty : new R(a.Pos + b, a.Size);
	public static R operator +(Pt b, R a) => a == Empty ? Empty : new R(a.Pos + b, a.Size);
	public static R operator -(R a, Pt b) => a == Empty ? Empty : new R(a.Pos - b, a.Size);
	public static R operator -(Pt b, R a) => a == Empty ? Empty : new R(a.Pos - b, a.Size);

	public static R operator -(R r, Marg m)
	{
		if (m.Dir(Dir.Horz) >= r.Width || m.Dir(Dir.Vert) >= r.Height)
			return Empty;

		return new R(
			r.X + m.Left,
			r.Y + m.Up,
			r.Width - (m.Left + m.Right),
			r.Height - (m.Up + m.Bottom)
		);
	}

	public static R operator +(R r, Marg m) => new(r.X - m.Left, r.Y - m.Up, r.Width + m.Dir(Dir.Horz), r.Height + m.Dir(Dir.Vert));

	[JsonIgnore]
	public Pt Center => new(X + Width / 2, Y + Height / 2);

	public static R FromCenterAndSize(Pt center, Sz size) => new(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);


	// Conversion operators
	// ====================
	public static implicit operator R(System.Drawing.Rectangle e) => new(e.X, e.Y, e.Width, e.Height);
	public static implicit operator System.Drawing.Rectangle(R e) => new(e.X, e.Y, e.Width, e.Height);
	public static implicit operator System.Drawing.RectangleF(R e) => new(e.X, e.Y, e.Width, e.Height);

	public static implicit operator R(RECT e) => new(e.Left, e.Top, e.Width, e.Height);
	public static implicit operator RECT(R e) => new(e.X, e.Y, e.Right, e.Bottom);

	//public static implicit operator R(NetCoreEx.Geometry.Rectangle e) => new(e.Left, e.Top, e.Width, e.Height);
	//public static implicit operator NetCoreEx.Geometry.Rectangle(R e) => new(e.X, e.Y, e.Right, e.Bottom);
	//public static implicit operator NetCoreEx.Geometry.RectangleF(R e) => new(e.X, e.Y, e.Right, e.Bottom);

	//public static implicit operator R(SKRectI e) => new(e.Left, e.Top, e.Width, e.Height);
	//public static implicit operator SKRectI(R e) => new(e.X, e.Y, e.Right, e.Bottom);
	//public static implicit operator SKRect(R e) => new(e.X, e.Y, e.Right, e.Bottom);
}