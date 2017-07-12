﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using CoreGraphics;
using AppKit;

namespace MonoDevelop.Mac.Debug
{
	public class DebugWindow : NSWindow
	{
		NSMenu menu;

		BorderedWindow debugOverlayWindow;
		BorderedWindow debugNextOverlayWindow;
		BorderedWindow debugPreviousOverlayWindow;

		StatusWindow debugStatusWindow;

		bool ShowNextResponderOverlay
		{
			get
			{
				return debugNextOverlayWindow.IsVisible;
			}
			set
			{
				debugNextOverlayWindow.IsVisible = value;
			}
		}
		bool ShowPreviousResponderOverlay
		{
			get
			{
				return debugPreviousOverlayWindow.IsVisible;
			}
			set
			{
				debugPreviousOverlayWindow.IsVisible = value;
			}
		}
		bool ShowFirstResponderOverlay { 
			get {
				return debugOverlayWindow.IsVisible;
			}
			set {
				debugOverlayWindow.IsVisible = value;
			}
		}

		public DebugWindow (IntPtr handle) : base (handle)
		{
			
		}

		public void SetMenu (NSMenu menu) 
		{
			this.menu = menu ?? throw new NullReferenceException("Menu cannot be null");

			var submenu = this.menu.ItemAt(0).Submenu;
			submenu.InsertItem(new NSMenuItem("Debug Key View Loop", DebugKeyViewLoopHandler), 0);

			submenu.InsertItem(new NSMenuItem("Show/Hide First Responder Overlay", ShowFirstResponderOverlayHandler), 1);
			submenu.InsertItem(new NSMenuItem("Show/Hide First Responder Overlay", ShowFirstResponderOverlayHandler), 1);

			submenu.InsertItem(new NSMenuItem("Debug Key View Loop", ShowHideDetailDebuggerWindow), 2);
			submenu.InsertItem(NSMenuItem.SeparatorItem, 3);
		}

		void ShowFirstResponderOverlayHandler(object sender, EventArgs e)
		{
			ShowFirstResponderOverlay = !ShowFirstResponderOverlay;
			RefreshDebugData(FirstResponder);
		}

		void ShowHideDetailDebuggerWindow(object sender, EventArgs e)
		{
			debugStatusWindow.IsVisible = !debugStatusWindow.IsVisible;
		}

		void DebugKeyViewLoopHandler(object sender, EventArgs e)
		{
			
		}

		void RefreshDebugData (NSResponder firstResponder) 
		{
			var view = firstResponder as NSView;
			if (view != null) {

				if (debugOverlayWindow == null)
				{
					debugOverlayWindow = new BorderedWindow (CGRect.Empty, NSColor.Blue);
					AddChildWindow(debugOverlayWindow, NSWindowOrderingMode.Above);
				}

				debugOverlayWindow.AlignWith(view);

				var nextKeyView = view.NextValidKeyView as NSView;
				if (nextKeyView != null) {
					if (debugNextOverlayWindow == null)
					{
						debugNextOverlayWindow = new BorderedWindow(CGRect.Empty, NSColor.Yellow);
						AddChildWindow(debugNextOverlayWindow, NSWindowOrderingMode.Above);
					}
					debugNextOverlayWindow.AlignWith(nextKeyView);
				}

				var previousKeyView = view.PreviousValidKeyView as NSView;
				if (previousKeyView != null)
				{
					if (debugPreviousOverlayWindow == null)
					{
						debugPreviousOverlayWindow = new BorderedWindow(CGRect.Empty, NSColor.Red);
						AddChildWindow(debugPreviousOverlayWindow, NSWindowOrderingMode.Above);
					}
					debugPreviousOverlayWindow.AlignWith(previousKeyView);
				}

				if (debugStatusWindow == null)
				{
					debugStatusWindow = new StatusWindow(new CGRect(10, 10, 300, 500));
					AddChildWindow(debugStatusWindow, NSWindowOrderingMode.Above);
				}

				debugStatusWindow.AlignWith(Frame);

				var elements = view.GenerateLog("Current"); 
				if (nextKeyView != null) {
					elements.AddRange(nextKeyView.GenerateLog("Next"));
				};
				if (previousKeyView != null)
				{
					elements.AddRange(previousKeyView.GenerateLog("Previous"));
				};

				debugStatusWindow.GenerateStatusView(elements);
			}
		}

		public override void BecomeMainWindow()
		{
			base.BecomeMainWindow();
		}

		public override bool MakeFirstResponder(NSResponder aResponder)
		{
			RefreshDebugData(aResponder);
			return base.MakeFirstResponder(aResponder);
		}
	}
}
