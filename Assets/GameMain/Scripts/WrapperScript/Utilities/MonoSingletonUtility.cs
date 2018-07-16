using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

public static class MonoSingletonUtility
{
	public static void ClearSingletons()
	{
		if (MonoSingletonUtility.ClearCallback != null)
		{
			MonoSingletonUtility.ClearCallback();
		}
	}

	public static event EventDelegate.Callback ClearCallback;
}