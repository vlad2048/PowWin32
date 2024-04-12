namespace FastForms.LINQPad;

public sealed class WinLoggerOpt
{
	public bool HideSysWins { get; set; } = true;
    public int? MsgQueueLength { get; set; } = 10;
    public int MsgFontSize { get; set; } = 10;
    public HashSet<WM>? MsgsToInclude { get; set; }
    public string? WinFilterClassName { get; set; }
    public int? WinFilterClassInstance { get; set; }

    public WinLoggerOpt() { }

    internal static WinLoggerOpt Build(Action<WinLoggerOpt>? optFun)
    {
        var opt = new WinLoggerOpt();
        optFun?.Invoke(opt);
        return opt;
    }
}


static class WinLoggerOptExt
{
	public static bool AcceptMsg(this WinLoggerOpt opt, WM id) => opt.MsgsToInclude switch
	{
		null => true,
		not null => opt.MsgsToInclude.Contains(id)
	};
}