namespace FastForms.LINQPad.MessageLogging.Structs;

interface IMsg
{
    TimeSpan Time { get; }
    TimeSpan Delta { get; }
}
sealed record WinMsg(
	TimeSpan Time,
	TimeSpan Delta,
	int Nesting,
	WM Id,
	nint WParam,
	nint LParam
) : IMsg;
