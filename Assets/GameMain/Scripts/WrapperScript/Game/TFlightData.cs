using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TFlightData
{
	public string ESN;

	public Color32 flightColor;

	public string timeOfHit;

	public int esnShotId;

	public DateTime dateOfHit;

	public List<TFlightPoint> trajectory;

	public TFlightPoint carryPoint;

	public float carry;

	public float side;

	public float maxHeight;

	public float travelTime;

	public float clubSpeed;

	public float smashFactor;

	public float attackAngle;

	public float launchAngle;

	public float descentAngle;

	public float backSpin;

	public float horizontalAngle;

	public float sideSpin;

	public float totalSpeedMPH;

	public float totalSpeedMPS;

	public float offline;

	public float roll;

	public float spinAxis;

	public float totalSpin;

	public float carryPosX;

	public float carryPosY;

	public float totalPosX;

	public float totalPosY;

	public int courseConditionId;

	public bool isLefty;

	public string clubTypeID;

	public string clubName;

	private bool dataIsNotDeleted;

	public Transform ballTransform;

	private bool isMoving;

	private Vector3 prev = Vector3.zero;

	public bool isValid
	{
		get
		{
			return this.dataIsNotDeleted;
		}
	}

	public TFlightData()
	{
		this.trajectory = new List<TFlightPoint>();
		this.carryPoint = new TFlightPoint();
		this.carry = 0f;
	}

	public void AddBallCurrentPoint()
	{
		if (this.ballTransform != null)
		{
			Vector3 vector3 = this.ballTransform.position - this.prev;
			this.isMoving = vector3.sqrMagnitude > 0.0001f;
			this.prev = this.ballTransform.position;
			if (this.isMoving)
			{
				float single = Time.time - float.Parse(this.timeOfHit);
				TFlightPoint tFlightPoint = new TFlightPoint()
				{
					time = single,
					location = this.ballTransform.position
				};
				this.AddPoint(this.ballTransform.position, single);
			}
		}
	}

	public void AddPoint(Vector3 point, float time)
	{
		TFlightPoint tFlightPoint = new TFlightPoint()
		{
			location = point,
			time = time
		};
		this.trajectory.Add(tFlightPoint);
	}

	public void AddPointOffset(Vector3 point, float time)
	{
		TFlightPoint tFlightPoint = new TFlightPoint()
		{
			location = point + new Vector3(118.54f, 0.02f, 0f),
			time = time
		};
		this.trajectory.Add(tFlightPoint);
	}

	public static TFlightTrajectory CalculateAverageTrajectory(IList<TFlightData> shotList, int maxPoints = 100)
	{
		List<Vector3> vector3s;
		TFlightPoint tFlightPoint;
		List<List<TFlightPoint>> lists = new List<List<TFlightPoint>>();
		foreach (TFlightData tFlightDatum in shotList)
		{
			lists.Add(TFlightData.GenerateMissingTrajectory(tFlightDatum));
		}
		List<List<TFlightPoint>> lists1 = new List<List<TFlightPoint>>();
		List<List<TFlightPoint>> lists2 = new List<List<TFlightPoint>>();
		for (int i = 0; i < lists.Count; i++)
		{
			int num = lists[i].IndexOf(shotList[i].carryPoint);
			lists1.Add(lists[i].GetRange(0, num + 2));
			lists2.Add(lists[i].GetRange(num + 2, lists[i].Count - (num + 2)));
		}
		int num1 = lists1.Min<List<TFlightPoint>>((List<TFlightPoint> list) => list.Count);
		if (num1 > maxPoints)
		{
			num1 = maxPoints;
		}
		int num2 = lists2.Min<List<TFlightPoint>>((List<TFlightPoint> list) => list.Count);
		if (num2 > maxPoints)
		{
			num2 = maxPoints;
		}
		List<Vector3> vector3s1 = new List<Vector3>(new Vector3[num1]);
		List<Vector3> vector3s2 = new List<Vector3>(new Vector3[num2]);
		for (int j = 0; j < num1; j++)
		{
			for (int k = 0; k < lists1.Count; k++)
			{
				float count = (float)lists1[k].Count / (float)num1;
				int num3 = (int)((float)j * count);
				List<Vector3> item = vector3s1;
				vector3s = item;
				int num4 = j;
				int num5 = num4;
				item[num4] = vector3s[num5] + lists1[k][num3].location;
			}
		}
		for (int l = 0; l < num2; l++)
		{
			for (int m = 0; m < lists2.Count; m++)
			{
				float single = (float)lists2[m].Count / (float)num2;
				int num6 = (int)((float)l * single);
				List<Vector3> item1 = vector3s2;
				vector3s = item1;
				int num7 = l;
				int num8 = num7;
				item1[num7] = vector3s[num8] + lists2[m][num6].location;
			}
		}
		List<TFlightPoint> tFlightPoints = new List<TFlightPoint>();
		for (int n = 0; n < vector3s1.Count; n++)
		{
			tFlightPoint = new TFlightPoint()
			{
				location = vector3s1[n] / (float)shotList.Count
			};
			tFlightPoints.Add(tFlightPoint);
		}
		List<TFlightPoint> tFlightPoints1 = new List<TFlightPoint>();
		for (int o = 0; o < vector3s2.Count; o++)
		{
			tFlightPoint = new TFlightPoint()
			{
				location = vector3s2[o] / (float)shotList.Count
			};
			tFlightPoints1.Add(tFlightPoint);
		}
		return new TFlightTrajectory(tFlightPoints, tFlightPoints1);
	}

	public static List<TFlightPoint> GenerateMissingTrajectory(TFlightData data)
	{
		if (data == null)
		{
			Debug.LogError("Data is null: Empty trajectory generated");
			return new List<TFlightPoint>();
		}
		CBallFlightManager instance = CBallFlightManager.GetInstance();
		if (instance == null)
		{
			Debug.LogError("Flight Manager is null: Empty trajectory generated");
			return new List<TFlightPoint>();
		}
		if (WeatherManager.instance != null)
		{
			instance.SetWeather(WeatherManager.instance.altitude, WeatherManager.instance.temperature, WeatherManager.instance.humindity, WeatherManager.barometer, WeatherManager.instance.windSpeed, (double)WeatherManager.instance.windDirection);
		}
		else
		{
			instance.SetWeather(0, 70, 50, WeatherManager.barometer, 0, 90);
		}
		//GrassManager.instance.SetGrassType((GrassManager.GrassType)data.courseConditionId);
		instance.CalculateCollision((double)data.totalSpeedMPH, (double)data.launchAngle, (double)data.horizontalAngle, (double)data.backSpin, (double)data.sideSpin);
		Vector3 vector3 = new Vector3(118.5399f, 0.06167692f, 0f);
		BallTrajectory ballTrajectory = instance.CalculateFlightTrajectory(vector3, (double)data.totalSpeedMPH, (double)data.launchAngle, (double)data.backSpin, (double)(-1f * data.sideSpin), (double)data.side, data.isLefty, null);
		List<TFlightPoint> tFlightPoints = new List<TFlightPoint>();
		for (int i = 0; i < ballTrajectory.m_points.Count; i++)
		{
			TFlightPoint tFlightPoint = new TFlightPoint()
			{
				location = ballTrajectory.m_points[i].pos,
				time = ballTrajectory.m_points[i].time
			};
			tFlightPoints.Add(tFlightPoint);
			if (ballTrajectory.firstBounceFrame == i)
			{
				data.carryPoint = tFlightPoint;
			}
		}
		return tFlightPoints;
	}

	public List<TFlightPoint> GetFlightData()
	{
		if (this.trajectory == null || this.trajectory.Count == 0)
		{
			this.trajectory = TFlightData.GenerateMissingTrajectory(this);
		}
		return this.trajectory;
	}

	public void MarkDataForDelete()
	{
		this.PrepNodeForDelete();
	}

	private void PrepNodeForDelete()
	{
		this.trajectory.Clear();
		this.timeOfHit = string.Empty;
		this.ESN = string.Empty;
		this.carryPoint = null;
		this.clubTypeID = string.Empty;
		this.carry = 0f;
		this.side = 0f;
		this.maxHeight = 0f;
		this.travelTime = 0f;
		this.clubSpeed = 0f;
		this.smashFactor = 0f;
		this.attackAngle = 0f;
		this.launchAngle = 0f;
		this.descentAngle = 0f;
		this.backSpin = 0f;
		this.horizontalAngle = 0f;
		this.sideSpin = 0f;
		this.totalSpeedMPH = 0f;
		this.offline = 0f;
		this.roll = 0f;
		this.spinAxis = 0f;
		this.totalSpin = 0f;
		this.carryPosX = 0f;
		this.carryPosY = 0f;
		this.totalPosX = 0f;
		this.totalPosY = 0f;
		this.courseConditionId = 0;
		this.clubName = string.Empty;
		this.dataIsNotDeleted = false;
	}

	private int PullMaxHeight()
	{
		if (this.trajectory.Count > 0)
		{
			return this.PullMaxHeight(ref this.trajectory);
		}
		List<TFlightPoint> tFlightPoints = TFlightData.GenerateMissingTrajectory(this);
		return this.PullMaxHeight(ref tFlightPoints);
	}

	private int PullMaxHeight(ref List<TFlightPoint> flightPointArray)
	{
		float single = 0f;
		foreach (TFlightPoint tFlightPoint in flightPointArray)
		{
			if (tFlightPoint.location.y <= single)
			{
				continue;
			}
			single = tFlightPoint.location.y;
		}
		return (int)(single * 1.09361f * 3f);
	}

	public void SetFlagDataIsGood()
	{
		this.dataIsNotDeleted = true;
	}

	public static void SortTFlightDataListByESN(ref List<TFlightData> list)
	{
		TFlightData tFlightDatum = new TFlightData();
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = i; j < list.Count; j++)
			{
				if (list[j].esnShotId < list[i].esnShotId)
				{
					tFlightDatum = list[i];
					list[i] = list[j];
					list[j] = tFlightDatum;
				}
			}
		}
	}
}

public class TFlightPoint
{
	public Vector3 location;

	public float time;
}