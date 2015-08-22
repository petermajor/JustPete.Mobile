using System;
using Android.App;
using Android.Gms.Cast;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Media;
using Android.Views;
using JustPete.Android.Extensions;
using JustPete.Android.Infrastructure;
using MediaRouteActionProvider = Android.Support.V7.App.MediaRouteActionProvider;
using ResultCallback = JustPete.Android.Infrastructure.ResultCallback<Android.Gms.Cast.CastClass.IApplicationConnectionResult>;
 
namespace JustPete.Android
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity : AppCompatActivity, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener
	{
		public const int CONNECTION_FAILURE_RESOLUTION_REQUEST = 9000;

		MediaRouter _mediaRouter;

		MediaRouteSelector _mediaRouteSelector;

		CastDevice _selectedDevice;

		MyMediaRouterCallback _mediaRouterCallback;

		IGoogleApiClient _apiClient;

		MyChannel _channel;

		bool _applicationStarted;

		string _sessionId;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.activity_main);

			this.InitToolbar();

			_mediaRouter = MediaRouter.GetInstance(ApplicationContext);

			_mediaRouteSelector = new MediaRouteSelector.Builder()
				.AddControlCategory(CastMediaControlIntent.CategoryForCast(GetAppId()))
				.Build();
			
			_mediaRouterCallback = new MyMediaRouterCallback
			{
				RouteSelected = OnRouteSelected,
				RouteUnselected = OnRouteUnselected
			};
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);

			MenuInflater.Inflate(Resource.Menu.menu_main, menu);
			var mediaRouteMenuItem = menu.FindItem(Resource.Id.media_route_menu_item);
			var mediaRouteActionProvider = MenuItemCompat.GetActionProvider (mediaRouteMenuItem).JavaCast<MediaRouteActionProvider>();
			mediaRouteActionProvider.RouteSelector = _mediaRouteSelector;
			return true;
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			_mediaRouter.AddCallback (_mediaRouteSelector, _mediaRouterCallback, MediaRouter.CallbackFlagRequestDiscovery);
		}

		protected override void OnStop ()
		{
			_mediaRouter.RemoveCallback (_mediaRouterCallback);

			base.OnStop ();
		}

		void OnRouteSelected (MediaRouter router, MediaRouter.RouteInfo route)
		{
			Console.WriteLine ("OnRouteSelected");

			_selectedDevice = CastDevice.GetFromBundle (route.Extras);

			LaunchReceiver ();
		}

		void OnRouteUnselected (MediaRouter router, MediaRouter.RouteInfo route)
		{
			Console.WriteLine ("OnRouteUnselected");

			Teardown (false);

			_selectedDevice = null;
		}

		void LaunchReceiver()
		{
			try
			{
				var _castListener = new MyCastListener
				{
					ApplicationDisconnected = delegate (int statusCode)
					{
						Console.WriteLine ("Application disconnected - code {0}", statusCode);
						Teardown(true);
					}
				};

				var apiOptionsBuilder = new CastClass.CastOptions.Builder(_selectedDevice, _castListener)
					.Build();

				_apiClient = new GoogleApiClientBuilder(this, this, this)
					.AddApi (CastClass.API, apiOptionsBuilder)
					.Build();

				_apiClient.Connect();
			}
			catch (Exception e)
			{
				Console.WriteLine ("Failed LaunchReceiver - {0}", e);
			}
		}

		void Teardown(bool selectDefaultRoute)
		{
			Console.WriteLine ("Teardown");

			if (_apiClient != null)
			{
				if (_applicationStarted)
				{
					if (_apiClient.IsConnected || _apiClient.IsConnecting)
					{
						try
						{
							CastClass.CastApi.StopApplication(_apiClient, _sessionId);
							if (_channel != null)
							{
								CastClass.CastApi.RemoveMessageReceivedCallbacks(
									_apiClient,
									GetChannelNamespace());
								_channel = null;
							}
						}
						catch (Exception e)
						{
							Console.WriteLine("Exception while removing channel - {0}", e);
						}
						_apiClient.Disconnect();
					}
					_applicationStarted = false;
				}
				_apiClient = null;
			}

			if (selectDefaultRoute)
			{
				_mediaRouter.SelectRoute(_mediaRouter.DefaultRoute);
			}
			_selectedDevice = null;
			_sessionId = null;
		}

		public void OnConnected (Bundle connectionHint)
		{
			if (_apiClient == null)
				return;

			var pendingResult = CastClass.CastApi
				.LaunchApplication (_apiClient, GetAppId(), new LaunchOptions { RelaunchIfRunning = false } );

			pendingResult.SetResultCallback (new ResultCallback(OnApplicationConnectionResult));
		}

		void OnApplicationConnectionResult(CastClass.IApplicationConnectionResult result)
		{
			Console.WriteLine("ApplicationConnectionResultCallback - {0}", result.Status.StatusCode);
			if (result.Status.IsSuccess)
			{
				_sessionId = result.SessionId;

				Console.WriteLine("application name: {0}, status: {1}, sessionId: {2}, wasLaunched: {3}",
					result.ApplicationMetadata.Name,
					result.ApplicationStatus,
					_sessionId,
					result.WasLaunched);
				
				_applicationStarted = true;

				// Create the custom message
				// channel
				_channel = new MyChannel { MessageReceived = OnMessageReceived };
					
				try {
					CastClass.CastApi.SetMessageReceivedCallbacks(
						_apiClient,
						GetChannelNamespace(),
						_channel);
				}
				catch (Exception e)
				{
					Console.WriteLine ("Exception while creating channel - {0}", e);
				}

				// set the initial instructions
				// on the receiver
				SendMessage("Test Test");
			}
			else
			{
				Console.WriteLine ("application could not launch");
				Teardown(true);
			}
		}

		void SendMessage(string message)
		{
			if (_apiClient != null && _channel != null)
			{
				try
				{
					var pendingResult = CastClass.CastApi.SendMessage(
						_apiClient,
						GetChannelNamespace(),
						message);
						
//					.setResultCallback(
//							new ResultCallback<Status>() {
//								@Override
//								public void onResult(Status result) {
//									if (!result.isSuccess()) {
//										Log.e(TAG, "Sending message failed");
//									}
//								}
//							});
				}
				catch (Exception e)
				{
					Console.WriteLine("Exception while sending message - {0}", e);
				}
			}
		}

		void OnMessageReceived(CastDevice castDevice, string nameSpace, string message)
		{
			Console.WriteLine ("OnMessageReceived: {0}", message);
		}

		public void OnConnectionSuspended (int cause)
		{
			Console.WriteLine ("OnConnectionSuspended");
		}

		public void OnConnectionFailed (global::Android.Gms.Common.ConnectionResult result)
		{
			Console.WriteLine ("OnConnectionFailed");
			Teardown (false);
		}

		string GetAppId()
		{
			return Resources.GetString (Resource.String.cast_app_id);
		}

		string GetChannelNamespace()
		{
			return Resources.GetString (Resource.String.cast_namespace);
		}
	}
}