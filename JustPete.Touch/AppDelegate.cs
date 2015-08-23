using Foundation;
using UIKit;

namespace JustPete.Touch
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		UICastViewController viewController;
		UINavigationController nav;

		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			Window = new UIWindow (UIScreen.MainScreen.Bounds);
			viewController = new UICastViewController ();
			nav = new UINavigationController (viewController);
			Window.RootViewController = nav;
			Window.MakeKeyAndVisible ();
			return true;
		}
	}
}


