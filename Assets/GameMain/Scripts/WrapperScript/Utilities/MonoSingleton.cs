using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
where T : MonoSingleton<T>
{
	private static T MInstance;

	protected bool mDontDestroy;

	public bool DontDestroy
	{
		get
		{
			return this.mDontDestroy;
		}
	}

	public static bool IsSingletonInitialized
	{
		get;
		private set;
	}

	public static T Singleton
	{
		get
		{
			if (MonoSingleton<T>.MInstance == null)
			{
				MonoSingleton<T>.FindSingleton<T>();
				if (MonoSingleton<T>.MInstance == null)
				{
					MonoSingleton<T>.MInstance = (new GameObject(string.Concat("Temp Instance of ", typeof(T)), new Type[] { typeof(T) })).GetComponent<T>();
				}
				MonoSingleton<T>.IsSingletonInitialized = true;
			}
			return MonoSingleton<T>.MInstance;
		}
	}

	public virtual void Awake()
	{
		if (MonoSingleton<T>.IsSingletonInitialized)
		{
			return;
		}
		MonoSingleton<T>.IsSingletonInitialized = true;
		MonoSingleton<T>.MInstance = (T)(this as T);
		this.Initialize();
		MonoSingleton<T>.SetName();
		MonoSingletonUtility.ClearCallback += new EventDelegate.Callback(this.ClearSingleton);
	}

	public void ClearSingleton()
	{
		MonoSingleton<T>.MInstance = (T)null;
		MonoSingleton<T>.IsSingletonInitialized = false;
		MonoSingletonUtility.ClearCallback -= new EventDelegate.Callback(this.ClearSingleton);
	}

	public static void CreateSingleton<TP>()
	{
		MonoSingleton<T>.FindOrCreateSingleton<TP>();
	}

	public static void FindOrCreateSingleton<TP>()
	{
		if (MonoSingleton<T>.MInstance == null)
		{
			MonoSingleton<T>.FindSingleton<TP>();
			if (MonoSingleton<T>.MInstance == null)
			{
				MonoSingleton<T>.MInstance = (new GameObject(string.Concat("Temp Instance of ", typeof(TP)), new Type[] { typeof(TP) })).GetComponent<T>();
			}
			MonoSingleton<T>.MInstance.Initialize();
			MonoSingleton<T>.IsSingletonInitialized = true;
		}
	}

	public static void FindSingleton<TP>()
	{
		MonoSingleton<T>.MInstance = (T)(FindObjectOfType(typeof(TP)) as T);
	}

	public static Type GetActualSingletonType()
	{
		if (MonoSingleton<T>.MInstance == null)
		{
			return typeof(T);
		}
		return MonoSingleton<T>.MInstance.GetType();
	}

	public static TP GetInheritedSingleton<TP>(bool createNewInstance = true)
	{
		if (createNewInstance && (MonoSingleton<T>.MInstance == null || !((object)MonoSingleton<T>.MInstance is TP)))
		{
			MonoSingleton<T>.FindOrCreateSingleton<TP>();
		}
		else if (!createNewInstance && (MonoSingleton<T>.MInstance == null || !((object)MonoSingleton<T>.MInstance is TP)))
		{
			MonoSingleton<T>.FindSingleton<TP>();
		}
		if (MonoSingleton<T>.MInstance == null || !((object)MonoSingleton<T>.MInstance is TP))
		{
			return default(TP);
		}
		MonoSingleton<T>.IsSingletonInitialized = true;
		return (TP)(object)MonoSingleton<T>.MInstance;
	}

	public static T GetSingleton()
	{
		return MonoSingleton<T>.MInstance;
	}

	public virtual void Initialize()
	{
	}

	public static bool IsSingleReallyInitialized<TP>()
	{
		if (MonoSingleton<T>.MInstance == null)
		{
			return false;
		}
		return (object)MonoSingleton<T>.MInstance is TP;
	}

	public virtual void OnApplicationQuit()
	{
		MonoSingleton<T>.MInstance = (T)null;
		MonoSingleton<T>.IsSingletonInitialized = false;
	}

	public virtual void OnDestroy()
	{
		MonoSingleton<T>.MInstance = (T)null;
		MonoSingleton<T>.IsSingletonInitialized = false;
	}

	private static void SetName()
	{
		if (MonoSingleton<T>.MInstance == null)
		{
			return;
		}
		if (!MonoSingleton<T>.MInstance.name.Contains("Singleton"))
		{
			T mInstance = MonoSingleton<T>.MInstance;
			mInstance.name = string.Concat(mInstance.name, " Singleton");
		}
		if (MonoSingleton<T>.MInstance.mDontDestroy)
		{
			T tPointer = MonoSingleton<T>.MInstance;
			tPointer.name = string.Concat(tPointer.name, " DontDestroy");
		}
	}

	public static void SetSingleton(Type type)
	{
		if (MonoSingleton<T>.IsSingletonInitialized)
		{
			return;
		}
		T component = (T)((new GameObject(string.Concat("Temp instance of ", type.Name), new Type[] { type })).GetComponent(type) as T);
		MonoSingleton<T>.MInstance = component;
	}
}