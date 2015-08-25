// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace JustPete.Touch
{
	[Register ("UICastViewController")]
	partial class UICastViewController
	{
		[Outlet]
		UIKit.UIButton buttonGuess { get; set; }

		[Outlet]
		UIKit.UIButton buttonJoin { get; set; }

		[Outlet]
		UIKit.UILabel promptCast { get; set; }

		[Outlet]
		UIKit.UILabel promptGuess { get; set; }

		[Outlet]
		UIKit.UILabel promptJoin { get; set; }

		[Outlet]
		UIKit.UITextField textGuess { get; set; }

		[Outlet]
		UIKit.UITextField textName { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (promptCast != null) {
				promptCast.Dispose ();
				promptCast = null;
			}

			if (promptJoin != null) {
				promptJoin.Dispose ();
				promptJoin = null;
			}

			if (textGuess != null) {
				textGuess.Dispose ();
				textGuess = null;
			}

			if (promptGuess != null) {
				promptGuess.Dispose ();
				promptGuess = null;
			}

			if (buttonGuess != null) {
				buttonGuess.Dispose ();
				buttonGuess = null;
			}

			if (textName != null) {
				textName.Dispose ();
				textName = null;
			}

			if (buttonJoin != null) {
				buttonJoin.Dispose ();
				buttonJoin = null;
			}
		}
	}
}
