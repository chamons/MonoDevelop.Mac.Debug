﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using CoreGraphics;
using AppKit;
using System.Collections.Generic;

namespace MonoDevelop.Mac.Debug
{
	class ViewDebugDelegate : IDisposable
	{
		readonly NSWindow window;
		NSView view, nextKeyView, previousKeyView;

		readonly BorderedWindow debugOverlayWindow;
		readonly BorderedWindow debugNextOverlayWindow;
		readonly BorderedWindow debugPreviousOverlayWindow;
		readonly StatusWindow debugStatusWindow;

		NSFirstResponderWatcher watcher;
		readonly List<NSMenuItem> menuItems;

		#region Properties

		bool IsNextResponderOverlayVisible {
			get {
				return debugNextOverlayWindow.Visible;
			}
			set {
				debugNextOverlayWindow.Visible = value;
			}
		}

		bool IsPreviousResponderOverlayVisible {
			get {
				return debugPreviousOverlayWindow.Visible;
			}
			set {
				debugPreviousOverlayWindow.Visible = value;
			}
		}

		bool IsFirstResponderOverlayVisible {
			get {
				return debugOverlayWindow.Visible;
			}
			set {
				debugOverlayWindow.Visible = value;
			}
		}

		bool IsStatusWindowVisible {
			get {
				return debugStatusWindow.ParentWindow != null;
			}
			set {
				ShowStatusWindow (value);
			}
		}

		NSMenu Submenu {
			get {
				return NSApplication.SharedApplication.Menu?.ItemAt (0)?.Submenu;
			}
		}

		#endregion

		public ViewDebugDelegate (NSWindow window)
		{
			this.window = window;

			if (debugOverlayWindow == null) {
				debugOverlayWindow = new BorderedWindow (CGRect.Empty, NSColor.Blue);
				this.window.AddChildWindow (debugOverlayWindow, NSWindowOrderingMode.Above);
			}
			if (debugNextOverlayWindow == null) {
				debugNextOverlayWindow = new BorderedWindow (CGRect.Empty, NSColor.Yellow);
				this.window.AddChildWindow (debugNextOverlayWindow, NSWindowOrderingMode.Above);
			}

			if (debugPreviousOverlayWindow == null) {
				debugPreviousOverlayWindow = new BorderedWindow (CGRect.Empty, NSColor.Red);
				this.window.AddChildWindow (debugPreviousOverlayWindow, NSWindowOrderingMode.Above);
			}

			if (debugStatusWindow == null) {
				debugStatusWindow = new StatusWindow (new CGRect (10, 10, 300, 500));
			}

			menuItems = new List<NSMenuItem> ();

			PopulateSubmenu ();
		}

		void ShowStatusWindow (bool value)
		{
			if (value) {
				if (!IsStatusWindowVisible) {
					window.AddChildWindow (debugStatusWindow, NSWindowOrderingMode.Above);
					RefreshStatusWindow ();
				}
			}
			else {
				debugStatusWindow?.Close ();
			}
		}

		void RefreshStatusWindow ()
		{
			debugStatusWindow.AlignWith (window.Frame);

			var anyFocusedView = view != null;
			if (!anyFocusedView)
				return;

			var elements = view.GenerateLog ("Current");
			if (nextKeyView != null) {
				elements.AddRange (nextKeyView.GenerateLog ("Next"));
			};
			if (previousKeyView != null) {
				elements.AddRange (previousKeyView.GenerateLog ("Previous"));
			};

			debugStatusWindow.GenerateStatusView (elements);

			watcher = new NSFirstResponderWatcher (window);
			watcher.Changed += (sender, e) => {
				if (e != null)
					RefreshDebugData (e);
			};
			watcher.Start ();
		}

		void PopulateSubmenu ()
		{
			var submenu = Submenu;
			if (submenu == null)
				throw new NullReferenceException ("Menu cannot be null");

			int menuCount = 0;
			submenu.AutoEnablesItems = false;

			ClearSubmenuItems (submenu);

			menuItems.Clear ();
			menuItems.AddRange (GetDefaultMenuItems ());

			foreach (var item in menuItems) {
				submenu.InsertItem (item, menuCount++);
			}
		}

		void ClearSubmenuItems (NSMenu submenu)
		{
			foreach (var item in menuItems) {
				submenu.RemoveItem (item);
			}
		}

		List<NSMenuItem> GetDefaultMenuItems ()
		{
			return new List<NSMenuItem> {
				new NSMenuItem (string.Format ("KeyViewLoop Debugger v{0}", GetAssemblyVersion ()), ShowHideDetailDebuggerWindow) { Enabled = false },
				NSMenuItem.SeparatorItem,
				new NSMenuItem ("Show KeyViewLoop Debug Window", ShowHideDetailDebuggerWindow),
				new NSMenuItem ("Show First Responder Overlay", ShowFirstResponderOverlayHandler),
				new NSMenuItem ("Show Next Responder Overlay", ShowNextResponderOverlayHandler),
				new NSMenuItem ("Show Previous Responder Overlay", ShowPreviousResponderOverlayHandler),
				NSMenuItem.SeparatorItem
			};
		}

		void ShowFirstResponderOverlayHandler (object sender, EventArgs e)
		{
			IsFirstResponderOverlayVisible = !IsFirstResponderOverlayVisible;
			RefreshDebugData (window.FirstResponder);

			var menuItem = (NSMenuItem)sender;
			menuItem.Title = string.Format ("{0} First Responder Overlay", ToMenuAction (!IsFirstResponderOverlayVisible));
		}

		void ShowPreviousResponderOverlayHandler (object sender, EventArgs e)
		{
			IsPreviousResponderOverlayVisible = !IsPreviousResponderOverlayVisible;
			RefreshDebugData (window.FirstResponder);

			var menuItem = (NSMenuItem)sender;
			menuItem.Title = string.Format ("{0} Previous Responder Overlay", ToMenuAction (!IsPreviousResponderOverlayVisible));
		}

		void ShowNextResponderOverlayHandler (object sender, EventArgs e)
		{
			IsNextResponderOverlayVisible = !IsNextResponderOverlayVisible;
			RefreshDebugData (window.FirstResponder);

			var menuItem = (NSMenuItem)sender;
			menuItem.Title = string.Format ("{0} Next Responder Overlay", ToMenuAction (!IsNextResponderOverlayVisible));
		}

		void ShowHideDetailDebuggerWindow (object sender, EventArgs e)
		{
			IsStatusWindowVisible = !IsStatusWindowVisible;

			var menuItem = (NSMenuItem)sender;
			menuItem.Title = string.Format ("{0} KeyViewLoop Debug Window", ToMenuAction (!IsStatusWindowVisible));
		}

		string ToMenuAction (bool value)
		{
			return value ? "Show" : "Hide";
		}

		void RefreshDebugData (NSResponder firstResponder)
		{
			view = firstResponder as NSView;
			if (view != null) {
				debugOverlayWindow.AlignWith (view);
			}

			nextKeyView = view?.NextValidKeyView as NSView;
			if (nextKeyView != null) {
				debugNextOverlayWindow.AlignWith (nextKeyView);
			}

			previousKeyView = view?.PreviousValidKeyView as NSView;
			if (previousKeyView != null) {
				debugPreviousOverlayWindow.AlignWith (previousKeyView);
			}

			RefreshStatusWindow ();
		}

		string GetAssemblyVersion ()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo (assembly.Location);
			return fileVersionInfo.ProductVersion;
		}

		public void Dispose ()
		{
			ClearSubmenuItems (Submenu);
			debugOverlayWindow?.Close ();
			debugNextOverlayWindow?.Close ();
			debugPreviousOverlayWindow?.Close ();
			debugStatusWindow?.Close ();
			watcher.Dispose ();
		}
	}
}
