﻿using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct ActivateAppPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public bool IsActive => Message->WParam.ToBool();
	public uint ConverseThreadId => Message->LParam.ToSafeUInt32();
}