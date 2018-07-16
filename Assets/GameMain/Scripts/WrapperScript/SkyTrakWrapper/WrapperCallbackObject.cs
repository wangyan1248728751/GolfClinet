using System;
using UnityEngine;

public class WrapperCallbackObject : MonoBehaviour
{
    public Action<string> CallbackAction;

    private static WrapperCallbackObject _instance;

    private void Awake()
    {
        if (WrapperCallbackObject._instance != null)
        {
            Destroy(base.gameObject);
            return;
        }
        DontDestroyOnLoad(base.gameObject);
        WrapperCallbackObject._instance = this;
    }

    public void CallbackMethod(string eventString)
    {
		AppLog.Log(string.Concat("!!!!!!!!!!!!! CallbackMethod:", eventString), true);
		if (this.CallbackAction != null)
		{
			this.CallbackAction(eventString);
		}
	}
}