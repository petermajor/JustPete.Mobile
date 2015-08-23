using System;
using Foundation;
using GoogleCast;

namespace JustPete.Touch.Infrastructure
{
	public class DeviceScannerListener : NSObject, IGCKDeviceScannerListener
	{
		public Action<GCKDevice> OnDeviceDidComeOnline { get; set; }

		public Action<GCKDevice> OnDeviceDidGoOffline { get; set; }

		[Export ("deviceDidComeOnline:")]
		public void DeviceDidComeOnline (GCKDevice device)
		{
			Console.WriteLine ("Device found: {0}", device.FriendlyName);

			var onDeviceDidComeOnline = OnDeviceDidComeOnline;
			if (onDeviceDidComeOnline != null)
				onDeviceDidComeOnline (device);
		}

		[Export ("deviceDidGoOffline:")]
		public void DeviceDidGoOffline (GCKDevice device)
		{
			Console.WriteLine ("Device disappeared: {0}", device.FriendlyName);

			var onDeviceDidGoOffline = OnDeviceDidGoOffline;
			if (onDeviceDidGoOffline != null)
				onDeviceDidGoOffline (device);
		}
	}
}