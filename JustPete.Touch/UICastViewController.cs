using GoogleCast;
using JustPete.Touch.Infrastructure;
using UIKit;

namespace JustPete.Touch
{
	public partial class UICastViewController : UIViewController
	{
		const string CAST_APP_ID = "A487EF70";

		GCKDeviceScanner _deviceScanner;

		GCKDeviceManager _deviceManager;

		GCKFilterCriteria _filterCriteria;

		UIBarButtonItem _googleCastButton;

		public UICastViewController () : base ("UICastViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = "Just Pete";

			NavigationItem.RightBarButtonItems = new UIBarButtonItem[0];

			CreateButtons ();

			StartScanning ();
		}

		void CreateButtons ()
		{
			_googleCastButton = new UIBarButtonItem (
				UIImage.FromFile ("icon-cast-identified.png"),
				UIBarButtonItemStyle.Plain, (s, e) =>  {
					System.Console.WriteLine ("button tapped");
			});
		}

		void UpdateButtonStates()
		{
			if (_deviceScanner != null && _deviceScanner.Devices.Length > 0)
			{
				// Show the Cast button.
				NavigationItem.RightBarButtonItems = new[] { _googleCastButton };
				if (_deviceManager != null && _deviceManager.ConnectionState == GCKConnectionState.Connected)
				{
					_googleCastButton.TintColor = UIColor.Blue;
				}
				else
				{
					_googleCastButton.TintColor = UIColor.Gray;
				}
			}
			else
			{
				NavigationItem.RightBarButtonItems = new UIBarButtonItem[0];
			}
		}

		void StartScanning ()
		{
			_filterCriteria = GCKFilterCriteria.FromAvailableApplication (CAST_APP_ID);

			_deviceScanner = new GCKDeviceScanner { FilterCriteria = _filterCriteria };

			var deviceScannerListener = new DeviceScannerListener
			{
				OnDeviceDidComeOnline = d => UpdateButtonStates (),
				OnDeviceDidGoOffline = d => UpdateButtonStates ()
			};

			_deviceScanner.AddListener(deviceScannerListener);

			_deviceScanner.StartScan ();
		}
	}
}

