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

		public void Join(string name)
		{
			Console.WriteLine ("Join as '{0}'", name);

			var message = new JoinMessage { Name = name };

			SendTextMessage (message.ToJson());
		}

		public void Guess(int value)
		{
			Console.WriteLine ("Guess number '{0}'", value);

			var message = new GuessMessage { Value = value };

			SendTextMessage (message.ToJson());
		}
	}
}

