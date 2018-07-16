using System;
using System.Collections.Generic;
using UnityEngine;

public class TracerManager : MonoBehaviour
{
	public Color currentTracer;

	public Color newestPreviousTracerColor;

	public Color oldestPreviousTracerColor;

	private bool _includeBallsOnTeeOrFlight = true;

	private float endWidthAtOneMeter = 0.006f;

	private float startWidthAtOneMeter = 0.006f;

	private Transform gCameraTransform;

	//private NumericDisplays _numericDisplays;

	//private NumericDisplayStatic _numericDisplayStatic;

	private TracerManager.TRACER_AMOUNT tracerAmount = TracerManager.TRACER_AMOUNT.ONE;

	private IList<CBall> golfBalls;

	private IList<Vector3> startPoints;

	public bool includeBallsOnTeeOrFlight
	{
		get
		{
			return this._includeBallsOnTeeOrFlight;
		}
	}

	public int TracersAmount
	{
		get
		{
			return (int)this.tracerAmount;
		}
	}

	public TracerManager()
	{
	}

	public void AddBallWithTracer(CBall ballWithTracer)
	{
		this.golfBalls.Insert(0, ballWithTracer);
		this.startPoints.Insert(0, ballWithTracer.gameObject.transform.position);
		this.ChangeTracerDisplay(this.tracerAmount);
	}

	private void Awake()
	{
		this.golfBalls = new List<CBall>();
		this.startPoints = new List<Vector3>();
		//this.gCameraTransform = CGameManager.instance.UiHolder.GolfCamera.transform;
		//this._numericDisplays = CGameManager.instance.UiHolder.NumericDisplays;
		//this._numericDisplayStatic = CGameManager.instance.UiHolder.NumericDisplaysStatic;
	}

	public void ChangeTracerDisplay(TracerManager.TRACER_AMOUNT amount)
	{
		this.tracerAmount = amount;
		this.DisableAll();
		int num = 0;
		int num1 = 0;
		while (num1 < this.golfBalls.Count)
		{
			if (num != (int)amount)
			{
				if (this._includeBallsOnTeeOrFlight || this.golfBalls[num1].isFlightComplete)
				{
					this.golfBalls[num1].SetTrailActive(true);
					num++;
				}
				else
				{
					this.golfBalls[num1].SetTrailActive(false);
				}
				num1++;
			}
			else
			{
				break;
			}
		}
		//if (this._numericDisplays != null)
		//{
		//	this._numericDisplays.DrawLines();
		//}
		//if (this._numericDisplayStatic != null)
		//{
		//	this._numericDisplayStatic.DrawLines();
		//}
	}

	public void DeleteAllTracers()
	{
		for (int i = 0; i < this.golfBalls.Count; i++)
		{
			this.golfBalls[i].DestroyTracerFromBall();
			this.golfBalls[i].gameObject.SetActive(false);
		}
		this.golfBalls.Clear();
	}

	public void DestroyLastAmount(int count)
	{
		int num = (count > this.golfBalls.Count ? this.golfBalls.Count : count);
		for (int i = 0; i < num; i++)
		{
			this.golfBalls[i].DestroyTracerFromBall();
		}
	}

	private void DisableAll()
	{
		foreach (CBall golfBall in this.golfBalls)
		{
			golfBall.SetTrailActive(false);
		}
	}

	public void RemoveBallWithTracer(CBall ballWithTracer)
	{
		int num = 0;
		while (num < this.golfBalls.Count)
		{
			if (this.golfBalls[num].gameObject != ballWithTracer.gameObject)
			{
				num++;
			}
			else
			{
				this.golfBalls[num].SetTrailActive(false);
				this.golfBalls.RemoveAt(num);
				break;
			}
		}
		this.ChangeTracerDisplay(this.tracerAmount);
	}

	public void SetIncludeBallsOnTeeOrFlight(bool include)
	{
		this._includeBallsOnTeeOrFlight = include;
	}

	private void Update()
	{
		if (this.golfBalls.Count > 50)
		{
			for (int i = this.golfBalls.Count - 1; i > 49; i--)
			{
				this.golfBalls[i].DestroyTracerFromBall();
			}
		}
		Vector3 vector3 = this.gCameraTransform.position;
		for (int j = 0; j < this.golfBalls.Count; j++)
		{
			float item = (vector3 - this.startPoints[j]).magnitude;
			Vector3 item1 = vector3 - this.golfBalls[j].transform.position;
			float single = item1.magnitude;
			this.golfBalls[j].SetTrailScales(single * this.startWidthAtOneMeter, item * this.endWidthAtOneMeter);
		}
	}

	public enum TRACER_AMOUNT
	{
		NONE = 0,
		ONE = 1,
		FIVE = 5,
		FIFTY = 50,
		ALL = 2147483647
	}
}