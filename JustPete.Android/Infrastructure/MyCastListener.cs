using System;
using Android.Gms.Cast;
 
namespace JustPete.Android.Infrastructure
{
	public class MyCastListener : CastClass.Listener
	{
		public Action<int> ApplicationDisconnected { get; set; }

		public override void OnApplicationDisconnected (int statusCode)
		{
			var applicationDisconnected = ApplicationDisconnected;
			if (applicationDisconnected != null)
				applicationDisconnected (statusCode);
		}
	}
}