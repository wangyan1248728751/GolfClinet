using System;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
	private static Scheduler schedulerInstance;

	private static List<Scheduler.DELEGATE_OBJECT> all;

	private bool allStop;

	public static Scheduler instance
	{
		get
		{
			return Scheduler.schedulerInstance;
		}
	}

	public int AddDelegate(Scheduler.delegateMethod method, int intervalSeconds)
	{
		Scheduler.DELEGATE_OBJECT dELEGATEOBJECT = new Scheduler.DELEGATE_OBJECT(intervalSeconds)
		{
			ourDelegate = method
		};
		Scheduler.all.Add(dELEGATEOBJECT);
		return Scheduler.all.Count - 1;
	}

	private void Awake()
	{
		if (Scheduler.schedulerInstance != null)
		{
			Destroy(base.gameObject);
			return;
		}
		Scheduler.schedulerInstance = this;
		if (Scheduler.all == null)
		{
			Scheduler.all = new List<Scheduler.DELEGATE_OBJECT>();
		}
		DontDestroyOnLoad(this);
		NetworkInternetVerification.instance.VerifyInternetAvailability();
	}

	public void SetSchedulerIndexToImmediate(int index)
	{
		if (index < 0 || index > Scheduler.all.Count)
		{
			return;
		}
		Scheduler.all[index].immediate = true;
	}

	public void SoftStart()
	{
		this.allStop = true;
	}

	public void SoftStop()
	{
		this.allStop = false;
	}

	public void StartAll()
	{
		this.allStop = false;
		foreach (Scheduler.DELEGATE_OBJECT dELEGATEOBJECT in Scheduler.all)
		{
			dELEGATEOBJECT.localStop = false;
		}
	}

	public void StartIndex(int index)
	{
		if (index < 0 || index >= Scheduler.all.Count)
		{
			return;
		}
		Scheduler.all[index].localStop = false;
	}

	public void Stop(int index)
	{
		if (index < 0 || index >= Scheduler.all.Count)
		{
			return;
		}
		Scheduler.all[index].localStop = true;
	}

	public void StopAll()
	{
		this.allStop = true;
		foreach (Scheduler.DELEGATE_OBJECT dELEGATEOBJECT in Scheduler.all)
		{
			dELEGATEOBJECT.localStop = true;
		}
	}

	private void Update()
	{
		if (!this.allStop)
		{
			for (int i = 0; i < Scheduler.all.Count; i++)
			{
				Scheduler.DELEGATE_OBJECT item = Scheduler.all[i];
				if (!item.localStop)
				{
					item.timer.UpdateDeltaTime();
					if (item.immediate)
					{
						item.immediate = false;
						item.skipNext = true;
						item.ourDelegate();
						item.timer.Reset();
					}
					else if (item.timer.isDone)
					{
						item.timer.Reset();
						if (!item.skipNext)
						{
							item.ourDelegate();
						}
						else
						{
							item.skipNext = false;
						}
					}
				}
			}
		}
	}

	public class DELEGATE_OBJECT
	{
		public bool localStop;

		public bool immediate;

		public bool skipNext;

		public int intervalSeconds;

		public CKitchenTimer timer;

		public Scheduler.delegateMethod ourDelegate;

		public DELEGATE_OBJECT(int intervalInSeconds)
		{
			this.intervalSeconds = intervalInSeconds;
			this.timer = new CKitchenTimer((float)this.intervalSeconds);
		}
	}

	public delegate void delegateMethod();
}