﻿// This file has been autogenerated from a class added in the UI designer.

using System.Collections.Generic;
using CoreGraphics;

namespace AppKit
{
	public static class FrameExtension
	{
		public static CGRect Add(this CGRect sender, CGRect toAdd)
		{
			return new CGRect(sender.X + toAdd.X, sender.Y + toAdd.Y, sender.Width + toAdd.Width, sender.Height + toAdd.Height);
		}

		public static CGRect Add(this CGRect sender, CGPoint toAdd)
		{
			return new CGRect(sender.X + toAdd.X, sender.Y + toAdd.Y, sender.Width, sender.Height);
		}

		public static CGRect Add(this CGRect sender, CGSize toAdd)
		{
			return new CGRect(sender.X, sender.Y, sender.Width + toAdd.Width, sender.Height + toAdd.Height);
		}

		public static List<string> GenerateLog (this NSView view, string title) {
			return new List<string>() {
				string.Format("============={0}==========", title),
				string.Format("Type: {0}", view.GetType()),
				string.Format("Frame: {0}", view.Frame),
				string.Format("AccessibilityFrame: {0}", view.AccessibilityFrame),
				string.Format("Bounds: {0}", view.Bounds),
				string.Format("VisibleRect: {0}", view.VisibleRect()),
			};
		}
	}
}