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

		#region Properties

		bool IsNextResponderOverlayVisible {
			get {
				return debugNextOverlayWindow?.Visible ?? false;
			}
			set {
				if (debugNextOverlayWindow == null)
					return;
				debugNextOverlayWindow.Visible = value;
			}
		}

		bool IsPreviousResponderOverlayVisible {
			get {
				return debugPreviousOverlayWindow?.Visible ?? false;
			}
			set {
				if (debugPreviousOverlayWindow == null)
					return;
				debugPreviousOverlayWindow.Visible = value;
			}
		}

		bool IsFirstResponderOverlayVisible {
			get {
				return debugOverlayWindow?.Visible ?? false;
			}
			set {
				if (debugOverlayWindow == null)
					return;
				debugOverlayWindow.Visible = value;
			}
		}

		public DebugWindow (IntPtr handle) : base (handle)
		{

		}

		public void SetMenu (NSMenu menu)
		{
			this.menu = menu ?? throw new NullReferenceException ("Menu cannot be null");
			int menuCount = 0;
			var submenu = this.menu.ItemAt (0).Submenu;

			submenu.InsertItem (new NSMenuItem ("Show KeyViewLoop Debug Window", ShowHideDetailDebuggerWindow), menuCount++);
			submenu.InsertItem (new NSMenuItem ("Show First Responder Overlay", ShowFirstResponderOverlayHandler), menuCount++);
			submenu.InsertItem (new NSMenuItem ("Show Next Responder Overlay", ShowNextResponderOverlayHandler), menuCount++);
			submenu.InsertItem (new NSMenuItem ("Show Previous Responder Overlay", ShowPreviousResponderOverlayHandler), menuCount++);

			submenu.InsertItem (NSMenuItem.SeparatorItem, menuCount++);
		}

		void ShowFirstResponderOverlayHandler (object sender, EventArgs e)
		{
			IsFirstResponderOverlayVisible = !IsFirstResponderOverlayVisible;
			RefreshDebugData (FirstResponder);

			var menuItem = (NSMenuItem)sender;
			menuItem.Title = string.Format ("{0} First Responder Overlay", ToMenuAction (IsFirstResponderOverlayVisible));
		}

		void ShowPreviousResponderOverlayHandler (object sender, EventArgs e)
		{
			IsPreviousResponderOverlayVisible = !IsPreviousResponderOverlayVisible;
			RefreshDebugData (FirstResponder);

			var menuItem = (NSMenuItem)sender;
			menuItem.Title = string.Format ("{0} Previous Responder Overlay", ToMenuAction (IsPreviousResponderOverlayVisible));
		}

		void ShowNextResponderOverlayHandler (object sender, EventArgs e)
		{
			IsNextResponderOverlayVisible = !IsNextResponderOverlayVisible;
			RefreshDebugData (FirstResponder);

			var menuItem = (NSMenuItem)sender;
			menuItem.Title = string.Format ("{0} Next Responder Overlay", ToMenuAction (IsNextResponderOverlayVisible));
		}

		void ShowHideDetailDebuggerWindow (object sender, EventArgs e)
		{
			debugStatusWindow.IsVisible = !debugStatusWindow.IsVisible;
		}

		string ToMenuAction (bool value)
		{
			return value ? "Show" : "Hide";
		}

		void RefreshDebugData (NSResponder firstResponder)
		{
			var view = firstResponder as NSView;
			if (view != null) {

				if (debugOverlayWindow == null) {
					debugOverlayWindow = new BorderedWindow (CGRect.Empty, NSColor.Blue);
					AddChildWindow (debugOverlayWindow, NSWindowOrderingMode.Above);
				}

				debugOverlayWindow.AlignWith (view);

				var nextKeyView = view.NextValidKeyView as NSView;
				if (nextKeyView != null) {
					if (debugNextOverlayWindow == null) {
						debugNextOverlayWindow = new BorderedWindow (CGRect.Empty, NSColor.Yellow);
						AddChildWindow (debugNextOverlayWindow, NSWindowOrderingMode.Above);
					}
					debugNextOverlayWindow.AlignWith (nextKeyView);
				}

				var previousKeyView = view.PreviousValidKeyView as NSView;
				if (previousKeyView != null) {
					if (debugPreviousOverlayWindow == null) {
						debugPreviousOverlayWindow = new BorderedWindow (CGRect.Empty, NSColor.Red);
						AddChildWindow (debugPreviousOverlayWindow, NSWindowOrderingMode.Above);
					}
					debugPreviousOverlayWindow.AlignWith (previousKeyView);
				}

				if (debugStatusWindow == null) {
					debugStatusWindow = new StatusWindow (new CGRect (10, 10, 300, 500));
					AddChildWindow (debugStatusWindow, NSWindowOrderingMode.Above);
				}

				debugStatusWindow.AlignWith (Frame);

				var elements = view.GenerateLog ("Current");
				if (nextKeyView != null) {
					elements.AddRange (nextKeyView.GenerateLog ("Next"));
				};
				if (previousKeyView != null) {
					elements.AddRange (previousKeyView.GenerateLog ("Previous"));
				};

				debugStatusWindow.GenerateStatusView (elements);
			}
		}

		public override void BecomeMainWindow ()
		{
			base.BecomeMainWindow ();
		}

		public override bool MakeFirstResponder (NSResponder aResponder)
		{
			RefreshDebugData (aResponder);
			return base.MakeFirstResponder (aResponder);
		}
	}
}
