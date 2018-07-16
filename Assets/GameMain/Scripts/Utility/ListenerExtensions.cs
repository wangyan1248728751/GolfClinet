using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Golf
{
	public static class ListenerExtensions
	{
		static void TryTweenScale(GameObject go,EventListener listener)
		{
			TweenScale tweenscale = go.GetComponent<TweenScale>();
			if (tweenscale != null)
			{
				listener.onDown = (GameObject o) => { tweenscale.TweenScaleOnDown(); };
				listener.onUp = (GameObject o) => { tweenscale.TweenScaleOnUp(); };
			}
		}
		static EventListener GetListener(GameObject go)
		{
			EventListener listener = EventListener.Get(go);
			if (listener == null)
			{
				Debug.LogError("there is no UIEventListener on " + go.name);
				return null;
			}
			return listener;
		}
		public static void AddClick(this GameObject go, GameFrameworkAction action)
		{
			EventListener listener = GetListener(go);
			TryTweenScale(go, listener);
			listener.onClick = (GameObject o) => { action(); };
		}

		public static void AddClick(this GameObject go, GameFrameworkAction<int> action, int p)
		{
			EventListener listener = GetListener(go);
			TryTweenScale(go, listener);
			listener.onClick = (GameObject o) => { action(p); };
		}
		public static void AddClick(this GameObject go, GameFrameworkAction<string> action, string p)
		{
			EventListener listener = GetListener(go);
			TryTweenScale(go, listener);
			listener.onClick = (GameObject o) => { action(p); };
		}
		public static void AddClick(this GameObject go, GameFrameworkAction<UIFormId> action, UIFormId p)
		{
			EventListener listener = GetListener(go);
			TryTweenScale(go, listener);
			listener.onClick = (GameObject o) => { action(p); };
		}

		public static void AddClick(this GameObject go, GameFrameworkAction<float> action, float p)
		{
			EventListener listener = GetListener(go);
			TryTweenScale(go, listener);
			listener.onClick = (GameObject o) => { action(p); };
		}

		public static void AddClick(this GameObject go, GameFrameworkAction<bool> action, bool p)
		{
			EventListener listener = GetListener(go);
			TryTweenScale(go, listener);
			listener.onClick = (GameObject o) => { action(p); };
		}

		public static void AddClick(this GameObject go, GameFrameworkAction<GameObject> action)
		{
			EventListener listener = GetListener(go);
			TryTweenScale(go, listener);
			listener.onClick = (GameObject o) => { action(o); };
		}

		public static void AddHover(this GameObject go, GameFrameworkAction action)
		{
			EventListener listener = GetListener(go);
			listener.onEnter = (GameObject o) => { action(); };
		}
		public static void AddHover(this GameObject go, GameFrameworkAction<GameObject> action)
		{
			EventListener listener = GetListener(go);
			listener.onEnter = (GameObject o) => { action(go); };
		}
	}
}

