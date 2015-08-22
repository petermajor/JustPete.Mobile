using Android.App;
using Android.OS;
using Android.Support.V7.App;
using JustPete.Android.Extensions;
using Android.Support.V7.Media;
using Android.Gms.Cast;
using Android.Views;
using Android.Support.V4.View;
using MediaRouteActionProvider = Android.Support.V7.App.MediaRouteActionProvider;
using Android.Runtime;
 
namespace JustPete.Android
{
	[Activity (Label = "@string/app_name", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		const string CAST_APPLICATION_ID = "";

		MediaRouter _mediaRouter;

		MediaRouteSelector _mediaRouteSelector;

		MyCallback _mediaRouterCallback;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.activity_main);

			this.InitToolbar();

			_mediaRouter = MediaRouter.GetInstance(ApplicationContext);

			_mediaRouteSelector = new MediaRouteSelector.Builder()
				.AddControlCategory(CastMediaControlIntent.CategoryForCast("A487EF70"))
				.Build();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			base.OnCreateOptionsMenu (menu);

			MenuInflater.Inflate(Resource.Menu.menu_main, menu);
			var mediaRouteMenuItem = menu.FindItem(Resource.Id.media_route_menu_item);
			var mediaRouteActionProvider = MenuItemCompat.GetActionProvider (mediaRouteMenuItem).JavaCast<MediaRouteActionProvider>();
			//var mediaRouteActionProvider = (MediaRouteActionProvider) MenuItemCompat.GetActionProvider(mediaRouteMenuItem);
			mediaRouteActionProvider.RouteSelector = _mediaRouteSelector;
			return true;
		}

		protected override void OnStart ()
		{
			base.OnStart ();

			_mediaRouterCallback = new MyCallback ();
			_mediaRouter.AddCallback (_mediaRouteSelector, _mediaRouterCallback, MediaRouter.CallbackFlagRequestDiscovery);
		}

		protected override void OnStop ()
		{
			_mediaRouter.RemoveCallback (_mediaRouterCallback);
			_mediaRouterCallback = null;
			base.OnStop ();
		}
	}

	public class MyCallback : MediaRouter.Callback
	{
	}
}


