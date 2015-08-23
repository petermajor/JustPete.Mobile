using System;
using GoogleCast;
using JustPete.Core;
using JustPete.Core.Messages;

namespace JustPete.Touch.Infrastructure
{
	public class MyChannel : GCKCastChannel
	{
		const string CastNamespace = "urn:x-cast:com.petermajor.justpete";

		public Action<string> OnDidReceiveTextMessage { get; set; }

		public MyChannel () : base(CastNamespace)
		{
		}

		public override void DidReceiveTextMessage (string message)
		{
			var onDidReceiveTextMessage = OnDidReceiveTextMessage;
			if (onDidReceiveTextMessage != null)
				onDidReceiveTextMessage (message);
		}
//
//		public final void join(GoogleApiClient apiClient, String name) {
//			try {
//				Log.d(TAG, "join: " + name);
//				JSONObject payload = new JSONObject();
//				payload.put(KEY_COMMAND, KEY_JOIN);
//				payload.put(KEY_NAME, name);
//				sendMessage(apiClient, payload.toString());
//			} catch (JSONException e) {
//				Log.e(TAG, "Cannot create object to join a game", e);
//			}
//		}

		public void Join(string name)
		{
			Console.WriteLine ("Join as '{0}'", name);

			var message = new JoinMessage { Name = name };

			SendTextMessage (message.ToJson());
		}
	}
}

