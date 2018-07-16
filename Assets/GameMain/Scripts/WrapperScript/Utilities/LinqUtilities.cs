using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Utilities
{
	public static class LinqUtilities
	{
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (T t in collection)
			{
				action(t);
			}
		}
	}
}