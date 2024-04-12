namespace PowWin32.Diag;

public sealed class TimeLogger
{
	private DateTime lastTime = DateTime.Now;
	private int idx;

	public void Tick(string? msg = null)
	{
		var now = DateTime.Now;
		var delta = now - lastTime;
		lastTime = now;
		Console.WriteLine($"t {idx}: {delta}" + (msg != null ? $"  msg:{msg}" : ""));
		idx++;
	}
}