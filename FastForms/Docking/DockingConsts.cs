using FastForms.Docking.Enums;

namespace FastForms.Docking;

static class DockingConsts
{
	private const int MarginTool = 4;

	public const int MarginDoc = 6;


	public const int MinHolderSize = 10;

	public static int GetMargin(this TreeType type) =>
		type switch
		{
			TreeType.Tool or TreeType.Empty => MarginTool,
			TreeType.Doc or TreeType.Mixed => MarginDoc,
			_ => throw new ArgumentException()
		};

	public static class PropNames
	{
		public const string Docker = nameof(Docker);
	}
}