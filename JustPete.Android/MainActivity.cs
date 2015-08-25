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
using Android.Widget;
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

		TextView _promptCast;
		TextView _promptJoin;
		TextView _promptGuess;
		EditText _textName;
		EditText _textGuess;
		Button _buttonJoin;
		Button _buttonGuess;

		MediaRouter _mediaRouter;
		MediaRouteSelector _mediaRouteSelector;
		CastDevice _selectedDevice;
		MyMediaRouterCallback _mediaRouterCallback;
		IGoogleApiClient _apiClient;
		MyChannel _channel;

		bool _applicationStarted;
		bool _hasJoined;

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

			Setup ();
		}

		void SubmitJoin()
		{
			if (string.IsNullOrWhiteSpace(_textName.Text))
				return;

			_channel.Join(_textName.Text.Trim());
			_hasJoined = true;
			UpdateEnabledStates();
			_textGuess.RequestFocus();
		}

		void SubmitGuess()
		{
			if (string.IsNullOrWhiteSpace(_textGuess.Text))
				return;

			int val;
			if (int.TryParse(_textGuess.Text.Trim(), out val))
				_channel.Guess(val);

			_textGuess.SelectAll();
		}

		void UpdateEnabledStates()
		{
			_promptCast.Enabled = !_applicationStarted;

			_promptJoin.Enabled = _applicationStarted && !_hasJoined;
			_textName.Enabled = _promptJoin.Enabled;
			_textName.Hint = _textName.Enabled ? "Name" : string.Empty;
			_buttonJoin.Enabled = _promptJoin.Enabled;

			_promptGuess.Enabled = _applicationStarted && _hasJoined;
			_textGuess.Enabled = _promptGuess.Enabled;
			_textGuess.Hint = _textGuess.Enabled ? "Guess" : string.Empty;
			_buttonGuess.Enabled = _promptGuess.Enabled;
		}

		void Setup()
		{
			_promptCast = FindViewById<TextView> (Resource.Id.promptCast);
			_promptJoin = FindViewById<TextView> (Resource.Id.promptJoin);
			_promptGuess = FindViewById<TextView> (Resource.Id.promptGuess);

			_textName = FindViewById<EditText> (Resource.Id.textName);
			_textName.EditorAction += (sender, e) => SubmitJoin();

			_textGuess = FindViewById<EditText> (Resource.Id.textGuess);
			_textGuess.EditorAction += (sender, e) => SubmitGuess();

			_buttonJoin = FindViewById<Button> (Resource.Id.buttonJoin);
			_buttonJoin.Click += (sender, e) => SubmitJoin();

			_buttonGuess = FindViewById<Button> (Resource.Id.buttonGuess);
			_buttonGuess.Click += (sender, e) => SubmitGuess();

			UpdateEnabledStates ();
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
					_applicationStarted = false;
					_hasJoined = false;
					if (_apiClient.IsConnected || _apiClient.IsConnecting)
					{
						try
						{
							CastClass.CastApi.LeaveApplication(_apiClient);
							if (_channel != null)
							{
								CastClass.CastApi.RemoveMessageReceivedCallbacks(
									_apiClient,
									MyChannel.CastNamespace);
								_channel = null;
							}
						}
						catch (Exception e)
						{
							Console.WriteLine("Exception while removing channel - {0}", e);
						}
						_apiClient.Disconnect();
					}
				}
				_apiClient = null;
			}

			if (selectDefaultRoute)
			{
				_mediaRouter.SelectRoute(_mediaRouter.DefaultRoute);
			}
			_selectedDevice = null;

			_textGuess.Text = string.Empty;
			UpdateEnabledStates ();
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
				Console.WriteLine("application name: {0}, status: {1}, sessionId: {2}, wasLaunched: {3}",
					result.ApplicationMetadata.Name,
					result.ApplicationStatus,
					result.SessionId,
					result.WasLaunched);
				
				_applicationStarted = true;

				// Create the custom message
				// channel
				_channel = new MyChannel(_apiClient) { MessageReceived = OnMessageReceived };
					
				try {
					CastClass.CastApi.SetMessageReceivedCallbacks(
						_apiClient,
						MyChannel.CastNamespace,
						_channel);
				}
				catch (Exception e)
				{
					Console.WriteLine ("Exception while creating channel - {0}", e);
				}

				UpdateEnabledStates();
				_textName.RequestFocus ();
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
						MyChannel.CastNamespace,
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
	}
}