namespace PowTrees.Geom;

readonly record struct VecR(VecPt Min, VecPt Max)
{
	public double Width => Max.X - Min.X;
	public double Height => Max.Y - Min.Y;
}