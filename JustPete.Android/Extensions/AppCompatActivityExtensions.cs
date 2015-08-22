using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace JustPete.Android.Extensions
{
	public static class AppCompatActivityExtensions
	{
		public static Toolbar InitToolbar(this AppCompatActivity activity)
		{
			var toolbar = activity.FindViewById<Toolbar> (Resource.Id.toolbar);
			activity.SetSupportActionBar(toolbar);
			return toolbar;
		}

		public static void EnableActionBarBackButton(this AppCompatActivity activity)
		{
			var actionBar = activity.SupportActionBar;
			if (actionBar != null)
				actionBar.SetDisplayHomeAsUpEnabled (true);
		}
	}
}