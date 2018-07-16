using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Utilities
{
	internal static class ThreadingUtilities
	{
		public static Thread Run(Action action, Action onFinish, Action<Exception> onError)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (onError == null)
			{
				throw new ArgumentNullException("onError");
			}
			return ThreadingUtilities.Run<object>(() => {
				action();
				return null;
			}, (object o) => {
				if (onFinish != null)
				{
					onFinish();
				}
			}, onError);
		}

		public static Thread Run<T>(Func<T> func, Action<T> onFinish, Action<Exception> onError)
		{
			if (func == null)
			{
				throw new ArgumentNullException("func");
			}
			if (onFinish == null)
			{
				throw new ArgumentNullException("onFinish");
			}
			if (onError == null)
			{
				throw new ArgumentNullException("onError");
			}
			Thread thread = new Thread(() => {
				try
				{
					onFinish(func());
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (onError != null)
					{
						onError(exception);
					}
				}
			});
			thread.Start();
			return thread;
		}

		public static Thread[] WaitAll(Action[] actions, Action onFinish, Action<Exception> onError)
		{
			if (((IEnumerable<Action>)actions).Any<Action>((Action a) => a == null))
			{
				throw new ArgumentNullException("actions", "Some of actions are null");
			}
			if (onFinish == null)
			{
				throw new ArgumentNullException("onFinish");
			}
			if (onError == null)
			{
				throw new ArgumentNullException("onError");
			}
			int length = (int)actions.Length;
			object obj1 = new object();
			List<Exception> exceptions = new List<Exception>();
			Action action = () => {
				object obj = obj1;
				Monitor.Enter(obj);
				try
				{
					int num = length - 1;
					int num1 = num;
					length = num;
					if (num1 == 0)
					{
						if (exceptions.Count != 0)
						{
							onError(new AggregateException(exceptions));
						}
						else
						{
							try
							{
								onFinish();
							}
							catch (Exception exception)
							{
								exceptions.Add(exception);
								onError(new AggregateException(exceptions));
							}
						}
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			};
			Action<Exception> action1 = (Exception ex) => {
				object obj = obj1;
				Monitor.Enter(obj);
				try
				{
					exceptions.Add(ex);
					int num = length - 1;
					int num1 = num;
					length = num;
					if (num1 == 0)
					{
						onError(new AggregateException(exceptions));
					}
				}
				finally
				{
					Monitor.Exit(obj);
				}
			};
			Thread[] threadArray = new Thread[(int)actions.Length];
			for (int i = 0; i != (int)actions.Length; i++)
			{
				threadArray[i] = ThreadingUtilities.Run(actions[i], action, action1);
			}
			return threadArray;
		}
	}
}