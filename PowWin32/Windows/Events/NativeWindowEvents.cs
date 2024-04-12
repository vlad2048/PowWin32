using PowRxVar;
using PowWin32.Windows.ReactiveLight;
using PowWin32.Windows.ReactiveLight.Interfaces;
using PowWin32.Windows.StructsPackets;

namespace PowWin32.Windows.Events;

public sealed class NativeWindowEvents
{
	private readonly Disp d;

	// @formatter:off
	private readonly ILiteSubject    <WindowMessage>          whenMessage;
	public           ILiteObservable <WindowMessage>          WhenMessage           => whenMessage;
	private readonly ILiteSubject    <WindowMessage>          whenMessagePost;
	public           ILiteObservable <WindowMessage>          WhenMessagePost       => whenMessagePost;
	// @formatter:on


	// [Auto Fields Start]
	// @formatter:off
	private readonly ILiteSubject    <ActivatePacket>       whenActivate;         
	public           ILiteObservable <ActivatePacket>       WhenActivate          => whenActivate;         
	
	private readonly ILiteSubject    <ActivateAppPacket>    whenActivateApp;      
	public           ILiteObservable <ActivateAppPacket>    WhenActivateApp       => whenActivateApp;      
	
	private readonly ILiteSubject    <AppCommandPacket>     whenAppCommand;       
	public           ILiteObservable <AppCommandPacket>     WhenAppCommand        => whenAppCommand;       
	
	private readonly ILiteSubject    <CaptureChangedPacket> whenCaptureChanged;   
	public           ILiteObservable <CaptureChangedPacket> WhenCaptureChanged    => whenCaptureChanged;   
	
	private readonly ILiteSubject    <CommandPacket>        whenCommand;          
	public           ILiteObservable <CommandPacket>        WhenCommand           => whenCommand;          
	
	private readonly ILiteSubject    <CreatePacket>         whenCreate;           
	public           ILiteObservable <CreatePacket>         WhenCreate            => whenCreate;           
	
	private readonly ILiteSubject    <DisplayChangePacket>  whenDisplayChange;    
	public           ILiteObservable <DisplayChangePacket>  WhenDisplayChange     => whenDisplayChange;    
	
	private readonly ILiteSubject    <EnablePacket>         whenEnable;           
	public           ILiteObservable <EnablePacket>         WhenEnable            => whenEnable;           
	
	private readonly ILiteSubject    <EraseBkgndPacket>     whenEraseBkgnd;       
	public           ILiteObservable <EraseBkgndPacket>     WhenEraseBkgnd        => whenEraseBkgnd;       
	
	private readonly ILiteSubject    <GetMinMaxInfoPacket>  whenGetMinMaxInfo;    
	public           ILiteObservable <GetMinMaxInfoPacket>  WhenGetMinMaxInfo     => whenGetMinMaxInfo;    
	
	private readonly ILiteSubject    <HotKeyPacket>         whenHotKey;           
	public           ILiteObservable <HotKeyPacket>         WhenHotKey            => whenHotKey;           
	
	private readonly ILiteSubject    <MenuCommandPacket>    whenMenuCommand;      
	public           ILiteObservable <MenuCommandPacket>    WhenMenuCommand       => whenMenuCommand;      
	
	private readonly ILiteSubject    <MouseActivatePacket>  whenMouseActivate;    
	public           ILiteObservable <MouseActivatePacket>  WhenMouseActivate     => whenMouseActivate;    
	
	private readonly ILiteSubject    <MovePacket>           whenMove;             
	public           ILiteObservable <MovePacket>           WhenMove              => whenMove;             
	
	private readonly ILiteSubject    <NcActivatePacket>     whenNcActivate;       
	public           ILiteObservable <NcActivatePacket>     WhenNcActivate        => whenNcActivate;       
	
	private readonly ILiteSubject    <NcCalcSizePacket>     whenNcCalcSize;       
	public           ILiteObservable <NcCalcSizePacket>     WhenNcCalcSize        => whenNcCalcSize;       
	
	private readonly ILiteSubject    <NcCreatePacket>       whenNcCreate;         
	public           ILiteObservable <NcCreatePacket>       WhenNcCreate          => whenNcCreate;         
	
	private readonly ILiteSubject    <NcHitTestPacket>      whenNcHitTest;        
	public           ILiteObservable <NcHitTestPacket>      WhenNcHitTest         => whenNcHitTest;        
	
	private readonly ILiteSubject    <NcMouseMovePacket>    whenNcMouseMove;      
	public           ILiteObservable <NcMouseMovePacket>    WhenNcMouseMove       => whenNcMouseMove;      
	
	private readonly ILiteSubject    <NcPaintPacket>        whenNcPaint;          
	public           ILiteObservable <NcPaintPacket>        WhenNcPaint           => whenNcPaint;          
	
	private readonly ILiteSubject    <PaintPacket>          whenPaint;            
	public           ILiteObservable <PaintPacket>          WhenPaint             => whenPaint;            
	
	private readonly ILiteSubject    <ParentNotifyPacket>   whenParentNotify;     
	public           ILiteObservable <ParentNotifyPacket>   WhenParentNotify      => whenParentNotify;     
	
	private readonly ILiteSubject    <QuitPacket>           whenQuit;             
	public           ILiteObservable <QuitPacket>           WhenQuit              => whenQuit;             
	
	private readonly ILiteSubject    <ShowWindowPacket>     whenShowWindow;       
	public           ILiteObservable <ShowWindowPacket>     WhenShowWindow        => whenShowWindow;       
	
	private readonly ILiteSubject    <SizePacket>           whenSize;             
	public           ILiteObservable <SizePacket>           WhenSize              => whenSize;             
	
	private readonly ILiteSubject    <SysCommandPacket>     whenSysCommand;       
	public           ILiteObservable <SysCommandPacket>     WhenSysCommand        => whenSysCommand;       
	
	private readonly ILiteSubject    <KeyPacket>            whenKey;              
	public           ILiteObservable <KeyPacket>            WhenKey               => whenKey;              
	
	private readonly ILiteSubject    <KeyCharPacket>        whenKeyChar;          
	public           ILiteObservable <KeyCharPacket>        WhenKeyChar           => whenKeyChar;          
	
	private readonly ILiteSubject    <MouseButtonPacket>    whenMouseButton;      
	public           ILiteObservable <MouseButtonPacket>    WhenMouseButton       => whenMouseButton;      
	
	private readonly ILiteSubject    <NcMouseButtonPacket>  whenNcMouseButton;    
	public           ILiteObservable <NcMouseButtonPacket>  WhenNcMouseButton     => whenNcMouseButton;    
	
	private readonly ILiteSubject    <MouseWheelPacket>     whenMouseWheel;       
	public           ILiteObservable <MouseWheelPacket>     WhenMouseWheel        => whenMouseWheel;       
	
	private readonly ILiteSubject    <WindowPosPacket>      whenWindowPosChanged; 
	public           ILiteObservable <WindowPosPacket>      WhenWindowPosChanged  => whenWindowPosChanged; 
	
	private readonly ILiteSubject    <WindowPosPacket>      whenWindowPosChanging;
	public           ILiteObservable <WindowPosPacket>      WhenWindowPosChanging => whenWindowPosChanging;
	
	private readonly ILiteSubject    <FocusPacket>          whenKillFocus;        
	public           ILiteObservable <FocusPacket>          WhenKillFocus         => whenKillFocus;        
	
	private readonly ILiteSubject    <FocusPacket>          whenSetFocus;         
	public           ILiteObservable <FocusPacket>          WhenSetFocus          => whenSetFocus;         
	
	private readonly ILiteSubject    <MousePacket>          whenMouseHover;       
	public           ILiteObservable <MousePacket>          WhenMouseHover        => whenMouseHover;       
	
	private readonly ILiteSubject    <MousePacket>          whenMouseMove;        
	public           ILiteObservable <MousePacket>          WhenMouseMove         => whenMouseMove;        
	
	private readonly ILiteSubject    <Packet>               whenClose;            
	public           ILiteObservable <Packet>               WhenClose             => whenClose;            
	
	private readonly ILiteSubject    <Packet>               whenDestroy;          
	public           ILiteObservable <Packet>               WhenDestroy           => whenDestroy;          
	
	private readonly ILiteSubject    <Packet>               whenEnterSizeMove;    
	public           ILiteObservable <Packet>               WhenEnterSizeMove     => whenEnterSizeMove;    
	
	private readonly ILiteSubject    <Packet>               whenExitSizeMove;     
	public           ILiteObservable <Packet>               WhenExitSizeMove      => whenExitSizeMove;     
	
	private readonly ILiteSubject    <Packet>               whenMouseLeave;       
	public           ILiteObservable <Packet>               WhenMouseLeave        => whenMouseLeave;       
	
	private readonly ILiteSubject    <Packet>               whenNcDestroy;        
	public           ILiteObservable <Packet>               WhenNcDestroy         => whenNcDestroy;        
	
	private readonly ILiteSubject    <Packet>               whenTimeChange;       
	public           ILiteObservable <Packet>               WhenTimeChange        => whenTimeChange;       
	// @formatter:on
	// [Auto Fields End]


	internal NativeWindowEvents(Disp d)
	{
		this.d = d;

		// @formatter:off
		whenMessage           = new LiteSubject<WindowMessage>       ().D(d);
		whenMessagePost       = new LiteSubject<WindowMessage>       ().D(d);
		// @formatter:on


		// [Auto Constructor Start]
		// @formatter:off
		whenActivate          = new LiteSubject<ActivatePacket>      ().D(d);
		whenActivateApp       = new LiteSubject<ActivateAppPacket>   ().D(d);
		whenAppCommand        = new LiteSubject<AppCommandPacket>    ().D(d);
		whenCaptureChanged    = new LiteSubject<CaptureChangedPacket>().D(d);
		whenCommand           = new LiteSubject<CommandPacket>       ().D(d);
		whenCreate            = new LiteSubject<CreatePacket>        ().D(d);
		whenDisplayChange     = new LiteSubject<DisplayChangePacket> ().D(d);
		whenEnable            = new LiteSubject<EnablePacket>        ().D(d);
		whenEraseBkgnd        = new LiteSubject<EraseBkgndPacket>    ().D(d);
		whenGetMinMaxInfo     = new LiteSubject<GetMinMaxInfoPacket> ().D(d);
		whenHotKey            = new LiteSubject<HotKeyPacket>        ().D(d);
		whenMenuCommand       = new LiteSubject<MenuCommandPacket>   ().D(d);
		whenMouseActivate     = new LiteSubject<MouseActivatePacket> ().D(d);
		whenMove              = new LiteSubject<MovePacket>          ().D(d);
		whenNcActivate        = new LiteSubject<NcActivatePacket>    ().D(d);
		whenNcCalcSize        = new LiteSubject<NcCalcSizePacket>    ().D(d);
		whenNcCreate          = new LiteSubject<NcCreatePacket>      ().D(d);
		whenNcHitTest         = new LiteSubject<NcHitTestPacket>     ().D(d);
		whenNcMouseMove       = new LiteSubject<NcMouseMovePacket>   ().D(d);
		whenNcPaint           = new LiteSubject<NcPaintPacket>       ().D(d);
		whenPaint             = new LiteSubject<PaintPacket>         ().D(d);
		whenParentNotify      = new LiteSubject<ParentNotifyPacket>  ().D(d);
		whenQuit              = new LiteSubject<QuitPacket>          ().D(d);
		whenShowWindow        = new LiteSubject<ShowWindowPacket>    ().D(d);
		whenSize              = new LiteSubject<SizePacket>          ().D(d);
		whenSysCommand        = new LiteSubject<SysCommandPacket>    ().D(d);
		whenKey               = new LiteSubject<KeyPacket>           ().D(d);
		whenKeyChar           = new LiteSubject<KeyCharPacket>       ().D(d);
		whenMouseButton       = new LiteSubject<MouseButtonPacket>   ().D(d);
		whenNcMouseButton     = new LiteSubject<NcMouseButtonPacket> ().D(d);
		whenMouseWheel        = new LiteSubject<MouseWheelPacket>    ().D(d);
		whenWindowPosChanged  = new LiteSubject<WindowPosPacket>     ().D(d);
		whenWindowPosChanging = new LiteSubject<WindowPosPacket>     ().D(d);
		whenKillFocus         = new LiteSubject<FocusPacket>         ().D(d);
		whenSetFocus          = new LiteSubject<FocusPacket>         ().D(d);
		whenMouseHover        = new LiteSubject<MousePacket>         ().D(d);
		whenMouseMove         = new LiteSubject<MousePacket>         ().D(d);
		whenClose             = new LiteSubject<Packet>              ().D(d);
		whenDestroy           = new LiteSubject<Packet>              ().D(d);
		whenEnterSizeMove     = new LiteSubject<Packet>              ().D(d);
		whenExitSizeMove      = new LiteSubject<Packet>              ().D(d);
		whenMouseLeave        = new LiteSubject<Packet>              ().D(d);
		whenNcDestroy         = new LiteSubject<Packet>              ().D(d);
		whenTimeChange        = new LiteSubject<Packet>              ().D(d);
		// @formatter:on
		// [Auto Constructor End]
	}


	internal void DispatchMessagePost(ref WindowMessage msg)
	{
		if (d.IsDisposed) return;
		whenMessagePost.OnNext(ref msg);
	}

	internal unsafe void DispatchMessage(ref WindowMessage msg)
	{
		whenMessage.OnNext(ref msg);

		switch (msg.Id)
		{
			// [Auto Switch Start]
			// @formatter:off
			case WM.WM_ACTIVATE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new ActivatePacket(ptr);
					whenActivate.OnNext(ref packet);
				}
				break;
	
			case WM.WM_ACTIVATEAPP:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new ActivateAppPacket(ptr);
					whenActivateApp.OnNext(ref packet);
				}
				break;
	
			case WM.WM_APPCOMMAND:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new AppCommandPacket(ptr);
					whenAppCommand.OnNext(ref packet);
				}
				break;
	
			case WM.WM_CAPTURECHANGED:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new CaptureChangedPacket(ptr);
					whenCaptureChanged.OnNext(ref packet);
				}
				break;
	
			case WM.WM_COMMAND:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new CommandPacket(ptr);
					whenCommand.OnNext(ref packet);
				}
				break;
	
			case WM.WM_CREATE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new CreatePacket(ptr);
					whenCreate.OnNext(ref packet);
				}
				break;
	
			case WM.WM_DISPLAYCHANGE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new DisplayChangePacket(ptr);
					whenDisplayChange.OnNext(ref packet);
				}
				break;
	
			case WM.WM_ENABLE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new EnablePacket(ptr);
					whenEnable.OnNext(ref packet);
				}
				break;
	
			case WM.WM_ERASEBKGND:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new EraseBkgndPacket(ptr);
					whenEraseBkgnd.OnNext(ref packet);
				}
				break;
	
			case WM.WM_GETMINMAXINFO:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new GetMinMaxInfoPacket(ptr);
					whenGetMinMaxInfo.OnNext(ref packet);
				}
				break;
	
			case WM.WM_HOTKEY:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new HotKeyPacket(ptr);
					whenHotKey.OnNext(ref packet);
				}
				break;
	
			case WM.WM_MENUCOMMAND:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new MenuCommandPacket(ptr);
					whenMenuCommand.OnNext(ref packet);
				}
				break;
	
			case WM.WM_MOUSEACTIVATE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new MouseActivatePacket(ptr);
					whenMouseActivate.OnNext(ref packet);
				}
				break;
	
			case WM.WM_MOVE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new MovePacket(ptr);
					whenMove.OnNext(ref packet);
				}
				break;
	
			case WM.WM_NCACTIVATE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new NcActivatePacket(ptr);
					whenNcActivate.OnNext(ref packet);
				}
				break;
	
			case WM.WM_NCCALCSIZE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new NcCalcSizePacket(ptr);
					whenNcCalcSize.OnNext(ref packet);
				}
				break;
	
			case WM.WM_NCCREATE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new NcCreatePacket(ptr);
					whenNcCreate.OnNext(ref packet);
				}
				break;
	
			case WM.WM_NCHITTEST:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new NcHitTestPacket(ptr);
					whenNcHitTest.OnNext(ref packet);
				}
				break;
	
			case WM.WM_NCMOUSEMOVE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new NcMouseMovePacket(ptr);
					whenNcMouseMove.OnNext(ref packet);
				}
				break;
	
			case WM.WM_NCPAINT:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new NcPaintPacket(ptr);
					whenNcPaint.OnNext(ref packet);
				}
				break;
	
			case WM.WM_PAINT:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new PaintPacket(ptr);
					whenPaint.OnNext(ref packet);
				}
				break;
	
			case WM.WM_PARENTNOTIFY:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new ParentNotifyPacket(ptr);
					whenParentNotify.OnNext(ref packet);
				}
				break;
	
			case WM.WM_QUIT:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new QuitPacket(ptr);
					whenQuit.OnNext(ref packet);
				}
				break;
	
			case WM.WM_SHOWWINDOW:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new ShowWindowPacket(ptr);
					whenShowWindow.OnNext(ref packet);
				}
				break;
	
			case WM.WM_SIZE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new SizePacket(ptr);
					whenSize.OnNext(ref packet);
				}
				break;
	
			case WM.WM_SYSCOMMAND:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new SysCommandPacket(ptr);
					whenSysCommand.OnNext(ref packet);
				}
				break;
	
			case WM.WM_KEYFIRST:
			case WM.WM_KEYUP:
			case WM.WM_SYSKEYDOWN:
			case WM.WM_SYSKEYUP:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new KeyPacket(ptr);
					whenKey.OnNext(ref packet);
				}
				break;
	
			case WM.WM_CHAR:
			case WM.WM_DEADCHAR:
			case WM.WM_SYSCHAR:
			case WM.WM_SYSDEADCHAR:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new KeyCharPacket(ptr);
					whenKeyChar.OnNext(ref packet);
				}
				break;
	
			case WM.WM_LBUTTONDOWN:
			case WM.WM_LBUTTONUP:
			case WM.WM_LBUTTONDBLCLK:
			case WM.WM_RBUTTONDOWN:
			case WM.WM_RBUTTONUP:
			case WM.WM_RBUTTONDBLCLK:
			case WM.WM_MBUTTONDOWN:
			case WM.WM_MBUTTONUP:
			case WM.WM_MBUTTONDBLCLK:
			case WM.WM_XBUTTONDOWN:
			case WM.WM_XBUTTONUP:
			case WM.WM_XBUTTONDBLCLK:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new MouseButtonPacket(ptr);
					whenMouseButton.OnNext(ref packet);
				}
				break;
	
			case WM.WM_NCLBUTTONDOWN:
			case WM.WM_NCLBUTTONUP:
			case WM.WM_NCLBUTTONDBLCLK:
			case WM.WM_NCRBUTTONDOWN:
			case WM.WM_NCRBUTTONUP:
			case WM.WM_NCRBUTTONDBLCLK:
			case WM.WM_NCMBUTTONDOWN:
			case WM.WM_NCMBUTTONUP:
			case WM.WM_NCMBUTTONDBLCLK:
			case WM.WM_NCXBUTTONDOWN:
			case WM.WM_NCXBUTTONUP:
			case WM.WM_NCXBUTTONDBLCLK:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new NcMouseButtonPacket(ptr);
					whenNcMouseButton.OnNext(ref packet);
				}
				break;
	
			case WM.WM_MOUSEWHEEL:
			case WM.WM_MOUSELAST:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new MouseWheelPacket(ptr);
					whenMouseWheel.OnNext(ref packet);
				}
				break;
	
			case WM.WM_WINDOWPOSCHANGED:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new WindowPosPacket(ptr);
					whenWindowPosChanged.OnNext(ref packet);
				}
				break;
	
			case WM.WM_WINDOWPOSCHANGING:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new WindowPosPacket(ptr);
					whenWindowPosChanging.OnNext(ref packet);
				}
				break;
	
			case WM.WM_KILLFOCUS:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new FocusPacket(ptr);
					whenKillFocus.OnNext(ref packet);
				}
				break;
	
			case WM.WM_SETFOCUS:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new FocusPacket(ptr);
					whenSetFocus.OnNext(ref packet);
				}
				break;
	
			case WM.WM_MOUSEHOVER:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new MousePacket(ptr);
					whenMouseHover.OnNext(ref packet);
				}
				break;
	
			case WM.WM_MOUSEMOVE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new MousePacket(ptr);
					whenMouseMove.OnNext(ref packet);
				}
				break;
	
			case WM.WM_CLOSE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new Packet(ptr);
					whenClose.OnNext(ref packet);
				}
				break;
	
			case WM.WM_DESTROY:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new Packet(ptr);
					whenDestroy.OnNext(ref packet);
				}
				break;
	
			case WM.WM_ENTERSIZEMOVE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new Packet(ptr);
					whenEnterSizeMove.OnNext(ref packet);
				}
				break;
	
			case WM.WM_EXITSIZEMOVE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new Packet(ptr);
					whenExitSizeMove.OnNext(ref packet);
				}
				break;
	
			case WM.WM_MOUSELEAVE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new Packet(ptr);
					whenMouseLeave.OnNext(ref packet);
				}
				break;
	
			case WM.WM_NCDESTROY:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new Packet(ptr);
					whenNcDestroy.OnNext(ref packet);
				}
				break;
	
			case WM.WM_TIMECHANGE:
				fixed (WindowMessage* ptr = &msg)
				{
					var packet = new Packet(ptr);
					whenTimeChange.OnNext(ref packet);
				}
				break;
			// @formatter:on
			// [Auto Switch End]
		}
	}
}
