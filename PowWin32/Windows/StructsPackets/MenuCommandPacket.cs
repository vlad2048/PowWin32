﻿using PowWin32.Windows.Utils;
using Vanara.PInvoke;

namespace PowWin32.Windows.StructsPackets;

// @formatter:off
public readonly unsafe struct MenuCommandPacket(WindowMessage* Message) : IPacket
{
	public WM MsgId => Message->Id;
	public HWND Hwnd => Message->Hwnd;
	public bool Handled { get => Message->Handled; set => Message->Handled = value; }

	public int MenuIndex => Message->WParam.ToSafeInt32();
	public nint MenuHandle => Message->LParam;
}