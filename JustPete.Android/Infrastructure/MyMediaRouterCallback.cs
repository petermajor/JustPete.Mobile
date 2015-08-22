using System;
using Android.Support.V7.Media;
 
namespace JustPete.Android.Infrastructure
{

	public class MyMediaRouterCallback : MediaRouter.Callback
	{
		public Action<MediaRouter, MediaRouter.RouteInfo> RouteSelected { get; set; }

		public Action<MediaRouter, MediaRouter.RouteInfo> RouteUnselected { get; set; }

		public override void OnRouteSelected (MediaRouter router, MediaRouter.RouteInfo route)
		{
			var routeSelected = RouteSelected;
			if (routeSelected != null)
				routeSelected (router, route);
		}

		public override void OnRouteUnselected (MediaRouter router, MediaRouter.RouteInfo route)
		{
			var routeUnselected = RouteUnselected;
			if (routeUnselected != null)
				routeUnselected (router, route);
		}
	}

}