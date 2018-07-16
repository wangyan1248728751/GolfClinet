using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class CKitchenTimer
{
	private float timeToWaitFor;

	private float timerChecker;

	private bool doOnce = true;

	private CKitchenTimer.COUNTER_STATE _currentState;

	public CKitchenTimer.COUNTER_STATE CurrentState
	{
		get
		{
			return this._currentState;
		}
	}

	public bool isDone
	{
		get
		{
			return this.timerChecker > this.timeToWaitFor;
		}
	}

	public CKitchenTimer(float timeToWait)
	{
		this.timeToWaitFor = timeToWait;
	}

	public void DoOnce()
	{
		this.doOnce = false;
	}

	public void OverideTimerToWaitValue(float newValue)
	{
		this.timeToWaitFor = newValue;
	}

	public void Reset()
	{
		this.doOnce = true;
		this.timerChecker = 0f;
		this.UpdateState();
	}

	public void UpdateDeltaTime()
	{
		this.timerChecker += Time.deltaTime;
		this.UpdateState();
	}

	private void UpdateState()
	{
		CKitchenTimer.COUNTER_STATE cOUNTERSTATE = this._currentState;
		if (this.timerChecker > this.timeToWaitFor && this.doOnce)
		{
			this._currentState = CKitchenTimer.COUNTER_STATE.DONE;
		}
		else if (this.timerChecker > this.timeToWaitFor && !this.doOnce)
		{
			this._currentState = CKitchenTimer.COUNTER_STATE.DONOTHING;
		}
		else if (this.timerChecker <= 0f || this.timerChecker >= this.timeToWaitFor)
		{
			this._currentState = CKitchenTimer.COUNTER_STATE.READY;
		}
		else
		{
			this._currentState = CKitchenTimer.COUNTER_STATE.COUNTING;
		}
		if (this._currentState != cOUNTERSTATE && this.OnStateChanged != null)
		{
			this.OnStateChanged(this._currentState);
		}
	}

	public event Action<CKitchenTimer.COUNTER_STATE> OnStateChanged;

	public enum COUNTER_STATE
	{
		READY,
		COUNTING,
		DONE,
		DONOTHING
	}
}