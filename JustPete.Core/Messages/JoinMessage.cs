using System;

namespace JustPete.Core.Messages
{
	public class JoinMessage
	{
		public string Command { get { return "join"; } }

		public string Name { get; set; }
	}
}