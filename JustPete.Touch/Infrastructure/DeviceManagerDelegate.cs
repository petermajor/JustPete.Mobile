using System;
using Foundation;
using GoogleCast;

namespace JustPete.Touch.Infrastructure
{
	public class DeviceManagerDelegate : NSObject, IGCKDeviceManagerDelegate
	{
		public Action<GCKDeviceManager> OnDidConnect { get; set; }

		public Action<GCKDeviceManager, GCKApplicationMetadata, string, bool> OnDidConnectToCastApplication { get; set; }

		[Export ("deviceManagerDidConnect:")]
		public void DidConnect (GCKDeviceManager deviceManager)
		{
			Console.WriteLine ("Connected");

			var onDidConnect = OnDidConnect;
			if (onDidConnect != null)
				onDidConnect (deviceManager);
		}

		[Export ("deviceManager:didConnectToCastApplication:sessionID:launchedApplication:")]
		public void DidConnectToCastApplication (GCKDeviceManager deviceManager, GCKApplicationMetadata applicationMetadata, string sessionId, bool launchedApplication)
		{
			Console.WriteLine ("Application has launched");

			var onDidConnectToCastApplication = OnDidConnectToCastApplication;
			if (onDidConnectToCastApplication != null)
				onDidConnectToCastApplication (deviceManager, applicationMetadata, sessionId, launchedApplication);
		}
	}
}