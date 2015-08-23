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
		UIKit.UIButton buttonJoin { get; set; }

		[Outlet]
		UIKit.UITextField textName { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
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
