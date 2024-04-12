using System.Text.Json.Serialization;
using Vanara.PInvoke;

namespace PowWin32.Geom;

public readonly record struct Pt(int X, int Y)
{
	public static readonly Pt Empty = new(0, 0);
	public override string ToString() => $"{X},{Y}";
	public bool Equals(Pt other) => X == other.X && Y == other.Y;
	public override int GetHashCode() => HashCode.Combine(X, Y);
	public static Pt operator +(Pt a, Pt b) => new(a.X + b.X, a.Y + b.Y);
	public static Pt operator -(Pt a, Pt b) => new(a.X - b.X, a.Y - b.Y);
	public static Pt operator -(Pt a) => new(-a.X, -a.Y);
	public static Pt operator +(Pt a, Marg m) => new(a.X + m.Left, a.Y + m.Up);
	public static Pt operator -(Pt a, Marg m) => new(a.X - m.Left, a.Y - m.Up);

	[JsonIgnore]
	public double Length => Math.Sqrt(X * X + Y * Y);


	// Conversion operators
	// ====================
	public static implicit operator Pt(System.Drawing.Point e) => new(e.X, e.Y);
	public static implicit operator System.Drawing.Point(Pt e) => new(e.X, e.Y);
	public static implicit operator System.Drawing.PointF(Pt e) => new(e.X, e.Y);

	public static implicit operator Pt(POINT e) => new(e.X, e.Y);
	public static implicit operator POINT(Pt e) => new(e.X, e.Y);

	//public static implicit operator Pt(NetCoreEx.Geometry.Point e) => new(e.X, e.Y);
	//public static implicit operator NetCoreEx.Geometry.Point(Pt e) => new(e.X, e.Y);
	//public static implicit operator NetCoreEx.Geometry.PointF(Pt e) => new(e.X, e.Y);

	//public static implicit operator Pt(SKPointI e) => new(e.X, e.Y);
	//public static implicit operator SKPointI(Pt e) => new(e.X, e.Y);
	//public static implicit operator SKPoint(Pt e) => new(e.X, e.Y);
}