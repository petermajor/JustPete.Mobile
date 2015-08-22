using System;
using Android.Gms.Cast;
 
namespace JustPete.Android.Infrastructure
{
	public class MyChannel : Java.Lang.Object, CastClass.IMessageReceivedCallback
	{
		public Action<CastDevice, string, string> MessageReceived { get; set; }

		public void OnMessageReceived (CastDevice castDevice, string nameSpace, string message)
		{
			var messageReceived = MessageReceived;
			if (messageReceived != null)
				messageReceived (castDevice, nameSpace, message);
		}
	}
}