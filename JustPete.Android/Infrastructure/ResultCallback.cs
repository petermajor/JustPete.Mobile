using System;
using Android.Gms.Common.Apis;
using Android.Runtime;

namespace JustPete.Android.Infrastructure
{
	public class ResultCallback<T> : Java.Lang.Object, IResultCallback where T : class, IJavaObject
	{
		public Action<T> OnResultAction { get; set; }

		public ResultCallback()
		{
		}

		public ResultCallback(Action<T> onResultAction)
		{
			OnResultAction = onResultAction;
		}

		public void OnResult (Java.Lang.Object result)
		{
			if (OnResultAction != null)
				OnResultAction (result.JavaCast<T>());
		}
	}
}