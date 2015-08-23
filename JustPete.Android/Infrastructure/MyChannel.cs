using System;
using Android.Gms.Cast;
using JustPete.Core.Messages;
using Android.Gms.Common.Apis;
using JustPete.Core;
 
namespace JustPete.Android.Infrastructure
{
	public class MyChannel : Java.Lang.Object, CastClass.IMessageReceivedCallback
	{
		public const string CastNamespace = "urn:x-cast:com.petermajor.justpete";

		public Action<CastDevice, string, string> MessageReceived { get; set; }

		readonly IGoogleApiClient _apiClient;

		public MyChannel(IGoogleApiClient apiClient)
		{
			_apiClient = apiClient;
		}

		public void OnMessageReceived (CastDevice castDevice, string nameSpace, string message)
		{
			var messageReceived = MessageReceived;
			if (messageReceived != null)
				messageReceived (castDevice, nameSpace, message);
		}


		public void Join(string name)
		{
			Console.WriteLine ("Join as '{0}'", name);

			var message = new JoinMessage { Name = name };

			SendMessage (message.ToJson());
		}

		void SendMessage(string message)
		{
			CastClass.CastApi.SendMessage(_apiClient, CastNamespace, message);
		}
	}
}