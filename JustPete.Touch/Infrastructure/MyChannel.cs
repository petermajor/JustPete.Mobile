using System;
using GoogleCast;

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
	}
}

