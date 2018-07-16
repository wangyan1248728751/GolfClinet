using System;
using System.Collections.Generic;
using UnityEngine;

public class BallTrajectory
{
	public List<BallTrajectory.Point> m_points;

	public float m_currentTime;

	public int m_indexHint;

	public int firstBounceFrame;

	public BallTrajectory()
	{
		this.m_currentTime = 0f;
		this.m_indexHint = 0;
		this.m_points = new List<BallTrajectory.Point>();
	}

	public void AddPoint(Vector3 pos, float deltaTime)
	{
		BallTrajectory.Point point = new BallTrajectory.Point();
		if (this.m_points.Count != 0)
		{
			Assert(deltaTime > 0f);
			BallTrajectory.Point item = this.m_points[this.m_points.Count - 1];
			point.time = item.time + deltaTime;
		}
		else
		{
			Assert(deltaTime == 0f);
			point.time = 0f;
		}
		point.pos = pos;
		this.m_points.Add(point);
	}

	public void AdvanceTime(float deltaTime)
	{
		this.m_currentTime += deltaTime;
		this.UpdateIndexHint();
	}

	public float GetCurrentFrame()
	{
		float item;
		int count = this.m_points.Count;
		Assert(count >= 1);
		Assert(this.m_indexHint <= count);
		if (this.m_indexHint != count)
		{
			item = (this.m_indexHint != 0 ? this.m_points[this.m_indexHint].time : this.m_points[0].time);
		}
		else
		{
			item = this.m_points[count - 1].time;
		}
		return item;
	}

	public Vector3 GetCurrentPosition()
	{
		Vector3 item;
		int count = this.m_points.Count;
		Assert(count >= 1);
		Assert(this.m_indexHint <= count);
		if (this.m_indexHint != count)
		{
			item = (this.m_indexHint != 0 ? this.m_points[this.m_indexHint].pos : this.m_points[0].pos);
		}
		else
		{
			item = this.m_points[count - 1].pos;
		}
		return item;
	}

	public Vector3 GetCurrentVelocity()
	{
		int num;
		int count = this.m_points.Count;
		Assert(count >= 1);
		Assert(this.m_indexHint <= count);
		if (count == 1)
		{
			return Vector3.zero;
		}
		num = (this.m_indexHint != count ? this.m_indexHint : count - 1);
		Assert(num > 0);
		Vector3 item = this.m_points[num - 1].pos;
		float single = this.m_points[num - 1].time;
		Vector3 vector3 = this.m_points[num].pos;
		float item1 = this.m_points[num].time;
		Assert(item1 > single);
		return (vector3 - item) * (1f / (item1 - single));
	}

	public Vector3 GetRestingPosition()
	{
		BallTrajectory.Point item = this.m_points[this.m_points.Count - 1];
		return item.pos;
	}

	public bool HasBounced()
	{
		return this.m_indexHint >= this.firstBounceFrame;
	}

	public bool IsAtEnd()
	{
		Assert(this.m_indexHint <= this.m_points.Count);
		//return this.m_indexHint == this.m_points.Count;
		if (m_indexHint < firstBounceFrame)
			return false;
		else
			return true;
		//return m_indexHint == firstBounceFrame;
	}

	public bool IsMidFlight(float beforeLanding)
	{
		Mathf.Clamp(beforeLanding, 0f, 1f);
		if ((float)this.m_indexHint >= (float)this.firstBounceFrame * beforeLanding)
		{
			return true;
		}
		return false;
	}

	public bool IsMidFlight()
	{
		return this.IsMidFlight(0.15f);
	}

	public void ResetTime()
	{
		this.m_currentTime = 0f;
		this.m_indexHint = 0;
	}

	public void UpdateIndexHint()
	{
		int count = this.m_points.Count;
		while (this.m_indexHint < count && this.m_points[this.m_indexHint].time <= this.m_currentTime)
		{
			this.m_indexHint++;
		}
		while (this.m_indexHint > 0 && this.m_points[this.m_indexHint - 1].time > this.m_currentTime)
		{
			this.m_indexHint--;
		}
	}

	public struct Point
	{
		public Vector3 pos;

		public float time;
	}

	public static void Assert(bool condition)
	{
		if (!condition)
		{
			Debug.LogError("Assert fail.");
			Debug.Break();
		}
	}
}