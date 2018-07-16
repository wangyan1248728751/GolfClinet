using System;
using System.Collections.Generic;

public static class Messenger<T>
{
	private static Dictionary<MESSAGE_EVENTS, Action<T>> eventTable;

	static Messenger()
	{
		Messenger<T>.eventTable = new Dictionary<MESSAGE_EVENTS, Action<T>>();
	}

	public static void AddListener(MESSAGE_EVENTS eventType, Action<T> handler)
	{
		if (!Messenger<T>.eventTable.ContainsKey(eventType))
		{
			Messenger<T>.eventTable.Add(eventType, null);
		}
		Messenger<T>.eventTable[eventType] = (Action<T>)Delegate.Combine(Messenger<T>.eventTable[eventType], handler);
	}

	public static void Broadcast(MESSAGE_EVENTS eventType, T arg1)
	{
		Action<T> @delegate;
		if (Messenger<T>.eventTable.TryGetValue(eventType, out @delegate) && @delegate != null)
		{
			@delegate(arg1);
		}
	}

	public static void RemoveListener(MESSAGE_EVENTS eventType, Action<T> handler)
	{
		Messenger<T>.eventTable[eventType] = (Action<T>)Delegate.Remove(Messenger<T>.eventTable[eventType], handler);
	}
}

public static class Messenger<T, U>
{
	private static Dictionary<MESSAGE_EVENTS, Action<T, U>> eventTable;

	static Messenger()
	{
		Messenger<T, U>.eventTable = new Dictionary<MESSAGE_EVENTS, Action<T, U>>();
	}

	public static void AddListener(MESSAGE_EVENTS eventType, Action<T, U> handler)
	{
		if (!Messenger<T, U>.eventTable.ContainsKey(eventType))
		{
			Messenger<T, U>.eventTable.Add(eventType, null);
		}
		Messenger<T, U>.eventTable[eventType] = (Action<T, U>)Delegate.Combine(Messenger<T, U>.eventTable[eventType], handler);
	}

	public static void Broadcast(MESSAGE_EVENTS eventType, T arg1, U arg2)
	{
		Action<T, U> @delegate;
		if (Messenger<T, U>.eventTable.TryGetValue(eventType, out @delegate) && @delegate != null)
		{
			@delegate(arg1, arg2);
		}
	}

	public static void RemoveListener(MESSAGE_EVENTS eventType, Action<T, U> handler)
	{
		Messenger<T, U>.eventTable[eventType] = (Action<T, U>)Delegate.Remove(Messenger<T, U>.eventTable[eventType], handler);
	}
}

public static class Messenger<T, U, V>
{
	private static Dictionary<MESSAGE_EVENTS, Action<T, U, V>> eventTable;

	static Messenger()
	{
		Messenger<T, U, V>.eventTable = new Dictionary<MESSAGE_EVENTS, Action<T, U, V>>();
	}

	public static void AddListener(MESSAGE_EVENTS eventType, Action<T, U, V> handler)
	{
		if (!Messenger<T, U, V>.eventTable.ContainsKey(eventType))
		{
			Messenger<T, U, V>.eventTable.Add(eventType, null);
		}
		Messenger<T, U, V>.eventTable[eventType] = (Action<T, U, V>)Delegate.Combine(Messenger<T, U, V>.eventTable[eventType], handler);
	}

	public static void Broadcast(MESSAGE_EVENTS eventType, T arg1, U arg2, V arg3)
	{
		Action<T, U, V> @delegate;
		if (Messenger<T, U, V>.eventTable.TryGetValue(eventType, out @delegate) && @delegate != null)
		{
			@delegate(arg1, arg2, arg3);
		}
	}

	public static void RemoveListener(MESSAGE_EVENTS eventType, Action<T, U, V> handler)
	{
		Messenger<T, U, V>.eventTable[eventType] = (Action<T, U, V>)Delegate.Remove(Messenger<T, U, V>.eventTable[eventType], handler);
	}
}