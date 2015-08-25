using System;

namespace JustPete.Core.Messages
{
	public class GuessMessage
	{
		public string Command { get { return "guess"; } }

		public int Value { get; set; }
	}
}