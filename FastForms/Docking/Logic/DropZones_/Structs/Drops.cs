using FastForms.Docking.Logic.Layout_.Enums;
using FastForms.Docking.Logic.Layout_.Nodes;
using PowWin32.Geom;

namespace FastForms.Docking.Logic.DropZones_.Structs;

// @formatter:off
interface IDrop;

// Add Pane to existing Holder
// ===========================
interface		IAddDrop							: IDrop { HolderNode Holder { get; } }
sealed record	Holder_Over_Drop(HolderNode Holder) : IAddDrop;

// Add Pane to new Holder
// ======================
interface		INewDrop																			: IDrop;
interface		ISideNewDrop																		: INewDrop { SDir SDir { get; } }
sealed record	Holder_Side_Drop				(HolderNode Holder, NodeType SrcType, SDir SDir)	: ISideNewDrop { public static IDrop[] MakeAll(HolderNode holder, NodeType srcType) => [.. Enum.GetValues<SDir>().Select(sdir => new Holder_Side_Drop(holder, srcType, sdir))]; }
sealed record	Holder_Side_CreateDocRoot_Drop	(ToolHolderNode Holder, SDir SDir)					: ISideNewDrop { public static IDrop[] MakeAll(ToolHolderNode holder) => [.. Enum.GetValues<SDir>().Select(sdir => new Holder_Side_CreateDocRoot_Drop(holder, sdir))]; }
sealed record	ToolRoot_Side_Drop				(SDir SDir)											: ISideNewDrop;
sealed record	DocRoot_Side_Drop				(SDir SDir)											: ISideNewDrop { public static readonly IDrop[] All = [.. Enum.GetValues<SDir>().Select(sdir => new DocRoot_Side_Drop(sdir))]; }

sealed record	ToolRoot_Init_Drop	: INewDrop;
sealed record	DocRoot_Init_Drop	: INewDrop;
// @formatter:on
