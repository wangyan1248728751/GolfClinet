using Data;
using Security;
using SkyTrakWrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Golf;

public class CSimulationManager : MonoBehaviour
{
	public static CSimulationManager instance;

	public GameObject m_ballTemplate;

	public GameObject m_terrain;

	public bool WaitOnShot;

	public bool waitToShoot = true;

	public bool m_resetCamera = true;

	public const float YARD_TO_METER = 0.9144f;

	public const float METER_TO_YARD = 1.09361f;

	public const float MPS_TO_MPH = 2.2369f;

	public const float MPH_TO_MPS = 0.447047263f;

	public const int YARDS_TO_FEET = 3;

	public const int FEET_TO_INCHES = 12;

	public const int YARDS_TO_INCHES = 36;

	public CSimulationManager.BallLaunchedDelegate PreBallLaunchDelegate;

	private bool m_ballNotLaunched = true;

	private bool m_ballHasBounced;

	private Vector3 m_launchPosition;

	private Vector3 m_landedPosition;

	private Vector3 m_restingPosition;

	private Transform m_ballTransform;

	private CBall m_ball;

	private bool ballTracerHasColor;

	private int nextTracerColorIndex;

	private int currentSimulatedShotIndex;

	private bool isStoringTFlightData;

	private float m_timeSinceShotEnded;

	private bool simulationComplete;

	//private NumericDisplays _numericDisplays
	//{
	//	get
	//	{
	//		return CGameManager.instance.UiHolder.NumericDisplays;
	//	}
	//}

	//private NumericDisplayStatic _numericDisplayStatic
	//{
	//	get
	//	{
	//		return CGameManager.instance.UiHolder.NumericDisplaysStatic;
	//	}
	//}

	public bool ballHasLanded
	{
		get
		{
			return this.m_ballHasBounced;
		}
	}

	public bool ballLaunched
	{
		get
		{
			return !this.m_ballNotLaunched;
		}
	}

	public bool isSimulationComplete
	{
		get
		{
			return this.simulationComplete;
		}
	}

	public CSimulationManager()
	{
	}

	private void _OnToggleFullscreen()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}

	private void AddPointToFlightData()
	{
		if (this.isStoringTFlightData)
		{
			CFlightDataStorage.instance.GetMostRecentFlight().AddPoint(this.m_ball.transform.position, this.m_timeSinceShotEnded);
		}
	}

	private void Awake()
	{
		CSimulationManager.instance = this;
	}

	public float CalculateDistancePosNegYards(Vector3 A, Vector3 B)
	{
		if (A.z > B.z)
		{
			return (A - B).magnitude * 1.09361f;
		}
		return -(A - B).magnitude * 1.09361f;
	}

	public float CalculateDistanceYards(Vector3 A, Vector3 B)
	{
		return (A - B).magnitude * 1.09361f;
	}

	public bool CheckClubUsed(string clubName)
	{
		bool flag = false;
		using (IEnumerator<TFlightData> enumerator = CFlightDataStorage.instance.GetFlights().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.clubName != clubName)
				{
					continue;
				}
				flag = true;
				return flag;
			}
			return false;
		}
		return flag;
	}

	public void DestroyBall(CBall ball)
	{
		ball.DestroySelf();
	}

	private void FillSimulationGreens<T>(T[] colliders, List<SimulationGreen> simGreens, float greenScale, Vector3 ballPosition)
	where T : Collider
	{
		T[] tArray = colliders;
		for (int i = 0; i < (int)tArray.Length; i++)
		{
			T t = tArray[i];
			float single = 1f;
			if ((object)t is SphereCollider)
			{
				single = ((object)t as SphereCollider).radius;
			}
			if ((object)t is CapsuleCollider)
			{
				single = ((object)t as CapsuleCollider).radius;
			}
			Vector3 vector3 = t.transform.position;
			float single1 = single * greenScale;
			Vector3 vector31 = new Vector3((vector3.z - ballPosition.z) / 0.3048f, 0f, (vector3.x - ballPosition.x) / 0.3048f);
			simGreens.Add(new SimulationGreen(vector31, single1 / 0.3048f));
		}
	}

	private void FixedUpdate()
	{
		if (this.m_ballNotLaunched || this.simulationComplete)
		{
			return;
		}
		TFlightData mostRecentFlight = CFlightDataStorage.instance.GetMostRecentFlight();
		if (this.m_ball == null || mostRecentFlight == null)
		{
			return;
		}
		this.m_timeSinceShotEnded += Time.deltaTime;
		this.m_restingPosition = this.m_ball.transform.position;
		if (this.m_ball.isFlightComplete)
		{
			TFlightData offline = CFlightDataStorage.instance.GetMostRecentFlight();
			offline.offline = this.GetOffline();
			if (this.isStoringTFlightData)
			{
				Messenger<TFlightData>.Broadcast(MESSAGE_EVENTS.SHOT_COMPLETE, offline);
			}
			this.m_ball.name = "Prop_Ball::Old";
			this.m_ball.PurgeTrajectoryBuffer();
			this.simulationComplete = true;

			GameEntry.GameCore.BallEnd(m_ball);
			return;
		}
		if (!this.m_ballHasBounced)
		{
			this.m_landedPosition = this.m_ball.transform.position;
			mostRecentFlight.carry = this.GetFlightDistance();
			mostRecentFlight.carryPoint = new TFlightPoint()
			{
				location = this.m_landedPosition
			};
			if (this.m_ball.hasBallLanded)
			{
				this.m_ballHasBounced = true;
			}
		}
		mostRecentFlight.roll = this.GetRollDistance();
		this.AddPointToFlightData();
	}

	//public List<Simulationgreen> generategreensdatafromscene(vector3 ballposition)
	//{
	//	list<simulationgreen> simulationgreens = new list<simulationgreen>();
	//	unityengine.object[] objarray = unityengine.object.findobjectsoftype(typeof(greenscenetarget));
	//	for (int i = 0; i < (int)objarray.length; i++)
	//	{
	//		greenscenetarget greenscenetarget = (greenscenetarget)objarray[i];
	//		spherecollider[] componentsinchildren = greenscenetarget.getcomponentsinchildren<spherecollider>(true);
	//		vector3 vector3 = greenscenetarget.gameobject.transform.localscale;
	//		this.fillsimulationgreens<spherecollider>(componentsinchildren, simulationgreens, vector3.x, ballposition);
	//		capsulecollider[] capsulecolliderarray = greenscenetarget.getcomponentsinchildren<capsulecollider>(true);
	//		vector3 vector31 = greenscenetarget.gameobject.transform.localscale;
	//		this.fillsimulationgreens<capsulecollider>(capsulecolliderarray, simulationgreens, vector31.x, ballposition);
	//	}
	//	return simulationgreens;
	//}

	public float GetFlightDistance()
	{
		return this.CalculateDistanceYards(this.m_landedPosition, this.m_launchPosition);
	}

	public CBall GetNewBall()
	{
		GameObject gameObject = this.InstantiateBall();
		gameObject.name = "Prop_Ball::New";
		this.m_ballTransform = gameObject.transform;
		this.m_ball = gameObject.GetComponent<CBall>();
		TrailRenderer component = gameObject.GetComponent<TrailRenderer>();
		component.sortingLayerName = "BallTracer";
		component.sortingOrder = 2;
		return this.m_ball;
	}

	public float GetOffline()
	{
		return (this.m_restingPosition.x - this.m_launchPosition.x) * 1.09361f;
	}

	public float GetRollDistance()
	{
		return this.CalculateDistancePosNegYards(this.m_restingPosition, this.m_landedPosition);
	}

	public float GetTotalDistanceWholeYards()
	{
		return this.CalculateDistanceYards(this.m_launchPosition, this.m_restingPosition);
	}

	public float GetTotalDistanceWithFraction()
	{
		return this.CalculateDistanceYards(this.m_launchPosition, this.m_restingPosition);
	}

	private void HardwareDelegateOnLaunchBall(RIPESpeedParamsType ripeSpeedParamsType, RIPESpinParamsType ripeSpinParamsType, RIPEFlightParamsType ripeFlightParams)
	{
		UnityEngine.Debug.Log("CCimualtionManager HardwareDelegate called");
		if (this.m_ballNotLaunched)
		{
			float single = 0f;
			float single1 = 0f;
			float single2 = 0f;
			//if (!this.RIPESpeedParamsEquals(FlightData.Instance.GetOldSpeedParams(), ripeSpeedParamsType) || !this.RIPESpinParamsEquals(FlightData.Instance.GetOldSpinParams(), ripeSpinParamsType) || !this.RIPEFlightParamsEquals(FlightData.Instance.GetOldFlightParams(), ripeFlightParams))
			//{
			LaunchBall(ripeSpeedParamsType.launchAngle, ripeSpeedParamsType.horizontalAngle, ripeSpeedParamsType.totalSpeed, single, single1, single2, ripeSpeedParamsType.horizontalAngle, "ClubName", ripeFlightParams.flightDuration, ripeFlightParams.maxHeight, -ripeSpinParamsType.backSpin, ripeSpinParamsType.sideSpin, ripeSpinParamsType.spinAxis, ripeSpinParamsType.totalSpin, true);
			FlightData.Instance.SetOldSpeedParams(ripeSpeedParamsType);
			FlightData.Instance.SetOldSpinParams(ripeSpinParamsType);
			FlightData.Instance.SetOldFlightParams(ripeFlightParams);
			//}
			//this._numericDisplays.ResetDials();
		}
	}

	private GameObject InstantiateBall()
	{
		GameObject gameObject = null;
		if (this.m_ballTemplate != null)
		{
			gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_ballTemplate);
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	public void LaunchBall(float launchAngle, float horizontalAngle, float totalSpeed_MPS, float attackAngle, float clubSpeed, float smashFactor, float side, string clubName, 
		float flightDuration, float maxHeight, float backSpin, float sideSpin, float spinAxis, float totalSpin, bool isNotReplay)
	{
		bool isHandnessLefty = false;
		int currentGrassType;
		if (launchAngle < 0f)
		{
			launchAngle = 0f;
		}
		if (this.PreBallLaunchDelegate != null)
		{
			this.PreBallLaunchDelegate();
		}
		if (this.m_ballTransform != null)
		{
			float totalSpeedMPS = totalSpeed_MPS * 2.2369f;
			this.m_launchPosition = this.m_ball.transform.position;
			Vector3 mLaunchPosition = this.m_launchPosition;
			Vector3 vector3 = mLaunchPosition;
			this.m_restingPosition = mLaunchPosition;
			this.m_landedPosition = vector3;
			CBallFlightManager.GetInstance().CalculateCollision((double)totalSpeedMPS, (double)launchAngle, (double)horizontalAngle, (double)backSpin, (double)sideSpin);
			if (CGameManager.instance == null)
			{
				isHandnessLefty = (!LoginManager.IsUserLoggedIn ? false : LoginManager.UserData.IsLefty);
			}
			else
			{
				//isHandnessLefty = CGameManager.instance.UiHolder.GameSettings.IsHandnessLefty;
			}
			bool flag = isHandnessLefty;
			//List<SimulationGreen> simulationGreens = this.GenerateGreensDataFromScene(this.m_launchPosition);
			BallTrajectory ballTrajectory = CBallFlightManager.GetInstance().CalculateFlightTrajectory(this.m_launchPosition, (double)(totalSpeed_MPS * 2.2369f), (double)launchAngle, (double)backSpin, (double)(sideSpin * -1f), (double)horizontalAngle, flag, null);

			
			BallTrajectory.Point endPoint = ballTrajectory.m_points[ballTrajectory.firstBounceFrame];
			
			foreach(BallTrajectory.Point point in ballTrajectory.m_points)
			{
				if (point.pos.y > maxHeight)
					maxHeight = point.pos.y;
			}

			golfballHitData data = new golfballHitData();
			data.endPoint = endPoint.pos;
			data.time = endPoint.time;
			data.maxHeight = maxHeight;
			data.speed = totalSpeedMPS;
			data.launchAngle = launchAngle;
			data.horizontalAngle = horizontalAngle;
			data.backSpin = backSpin;
			data.sideSpin = sideSpin;
			data.ball = m_ball;

			GameEntry.GameCore.HitGolfBall(data);
			UnityEngine.Debug.Log(string.Format("最大高度:{0},飞行时间{1},速度{2},发射角度{3},偏转角度{4}", maxHeight, endPoint.time,totalSpeedMPS,launchAngle,horizontalAngle));


			this.SetBallTracerColor();
			TFlightData tFlightDatum = new TFlightData();

			tFlightDatum.ESN = (string.IsNullOrEmpty(ApplicationDataManager.instance.ESN) ? "NOESN" : string.Copy(ApplicationDataManager.instance.ESN));
			tFlightDatum.esnShotId = ApplicationDataManager.instance.GenerateLocalEsnID();
			tFlightDatum.timeOfHit = Time.time.ToString();
			tFlightDatum.dateOfHit = DateTime.Now;
			tFlightDatum.ballTransform = this.m_ballTransform;
			tFlightDatum.attackAngle = attackAngle;
			tFlightDatum.clubSpeed = (float)CBallFlightManager.GetInstance().SpeedMPH;
			tFlightDatum.descentAngle = CBallFlightManager.GetInstance().DescentAngle;
			tFlightDatum.smashFactor = totalSpeedMPS / tFlightDatum.clubSpeed;
			tFlightDatum.backSpin = backSpin;
			tFlightDatum.totalSpeedMPH = totalSpeedMPS;
			tFlightDatum.totalSpeedMPS = totalSpeed_MPS;
			tFlightDatum.launchAngle = launchAngle;
			tFlightDatum.horizontalAngle = horizontalAngle;

			//数据处理
			tFlightDatum.travelTime = flightDuration;
			tFlightDatum.maxHeight = maxHeight;

			tFlightDatum.side = side;
			tFlightDatum.sideSpin = sideSpin;
			tFlightDatum.isLefty = flag;
			tFlightDatum.spinAxis = spinAxis;
			tFlightDatum.totalSpin = totalSpin;

			TFlightData tFlightDatum1 = tFlightDatum;
			if (GrassManager.instance == null)
			{
				currentGrassType = 0;
			}
			else
			{
				currentGrassType = (int)GrassManager.instance.CurrentGrassType;
			}
			tFlightDatum1.courseConditionId = currentGrassType;
			TFlightPoint tFlightPoint = new TFlightPoint()
			{
				location = new Vector3(0f, 0f, 0f),
				time = 0f
			};
			//tFlightDatum.clubName = CGameManager.instance.GetCurrentClubName();
			//tFlightDatum.clubTypeID = Club.GetClubIDFromName(tFlightDatum.clubName);
			tFlightDatum.clubName = "ClubName";
			tFlightDatum.clubTypeID = "clubTypeID";

			tFlightDatum.flightColor = this.m_ball.tracerColor;
			if (!isNotReplay)
			{
				CFlightDataStorage.instance.ReplayShotData = tFlightDatum;
			}
			else
			{
				CFlightDataStorage.instance.AddFlight(tFlightDatum);
			}
			this.isStoringTFlightData = isNotReplay;
			this.m_ballNotLaunched = false;
			if (this.OnBallLaunched != null)
			{
				this.OnBallLaunched();
			}
			this.m_ball.SetTrajectory(ballTrajectory);
			//CGameManager.instance.UiHolder.TracerManager.AddBallWithTracer(this.m_ball);
		}
	}

	private void OnDestroy()
	{
		SecurityWrapperService.Instance.OnBallLaunched -= new Action<RIPESpeedParamsType, RIPESpinParamsType, RIPEFlightParamsType>(this.HardwareDelegateOnLaunchBall);
		UnityEngine.Debug.Log("CSimulationManager was destroyed.");
	}

	//public void ReplayShot(TFlightData data)
	//{
	//	float single = data.sideSpin;
	//	this.LaunchBall(data.launchAngle, data.horizontalAngle, data.totalSpeedMPS, data.attackAngle, data.clubSpeed, data.smashFactor, data.side, data.clubName, data.travelTime, data.maxHeight, data.backSpin, single, data.spinAxis, data.totalSpin, false);
	//}

	public void ResetScene()
	{
		this.simulationComplete = false;
		this.m_ballNotLaunched = true;
		CFlightDataStorage.instance.hasLanded = false;
		this.m_ballHasBounced = false;
		this.m_launchPosition = new Vector3();
		this.m_landedPosition = new Vector3();
		this.m_restingPosition = new Vector3();
		this.m_timeSinceShotEnded = 0f;
	}

	private bool RIPEFlightParamsEquals(RIPEFlightParamsType old, RIPEFlightParamsType newer)
	{
		bool flag = false;
		bool flag1 = false;
		flag = (Mathf.Abs(old.carry - newer.carry) >= 0.001f ? false : true);
		flag1 = (Mathf.Abs(old.maxHeight - newer.maxHeight) >= 0.001f ? false : true);
		return (!flag ? false : flag1);
	}

	private bool RIPESpeedParamsEquals(RIPESpeedParamsType old, RIPESpeedParamsType newer)
	{
		bool flag = false;
		bool flag1 = false;
		bool flag2 = false;
		if (Mathf.Abs(old.launchAngle - newer.launchAngle) >= 0.001f)
		{
			UnityEngine.Debug.Log(string.Concat(new object[] { "Launch Angle False | Old : ", old.launchAngle, "  New : ", newer.launchAngle }));
			flag = false;
		}
		else
		{
			UnityEngine.Debug.Log(string.Concat(new object[] { "Launch Angle True | Old : ", old.launchAngle, "  New : ", newer.launchAngle }));
			flag = true;
		}
		if (Mathf.Abs(old.horizontalAngle - newer.horizontalAngle) >= 0.001f)
		{
			UnityEngine.Debug.Log(string.Concat(new object[] { "Horizontal Angle False | Old : ", old.horizontalAngle, "  New : ", newer.horizontalAngle }));
			flag1 = false;
		}
		else
		{
			UnityEngine.Debug.Log(string.Concat(new object[] { "Horizontal Angle True | Old : ", old.horizontalAngle, "  New : ", newer.horizontalAngle }));
			flag1 = true;
		}
		if (Mathf.Abs(old.totalSpeed - newer.totalSpeed) >= 0.001f)
		{
			UnityEngine.Debug.Log(string.Concat(new object[] { "Total Speed True | Old : ", old.totalSpeed, "  New : ", newer.totalSpeed }));
			flag2 = false;
		}
		else
		{
			UnityEngine.Debug.Log(string.Concat(new object[] { "Total Speed True | Old : ", old.totalSpeed, "  New : ", newer.totalSpeed }));
			flag2 = true;
		}
		return (!flag || !flag1 ? false : flag2);
	}

	private bool RIPESpinParamsEquals(RIPESpinParamsType old, RIPESpinParamsType newer)
	{
		bool flag = false;
		bool flag1 = false;
		flag = (Mathf.Abs(old.backSpin - newer.backSpin) >= 0.001f ? false : true);
		flag1 = (Mathf.Abs(old.sideSpin - newer.sideSpin) >= 0.001f ? false : true);
		return (!flag ? false : flag1);
	}

	private void SetBallTracerColor()
	{
		//if (this.ballTracerHasColor)
		//{
		//	this.nextTracerColorIndex = CGameManager.instance.GetCurrentClubColorIndex();
		//	this.m_ball.SetNewTracerMaterial(ApplicationDataManager.instance.GetTracerMaterialByIndex(this.nextTracerColorIndex));
		//}
	}

	private void SetBallTracerColor(int colorIndex)
	{
		//this.m_ball.SetNewTracerMaterial(ApplicationDataManager.instance.GetTracerMaterialByIndex(colorIndex));
	}

	public void SetBallTracerHasColor(bool usesColor)
	{
		this.ballTracerHasColor = usesColor;
	}

	private void Start()
	{
		this.m_ballHasBounced = false;
		this.m_launchPosition = new Vector3();
		this.m_landedPosition = new Vector3();
		this.m_restingPosition = new Vector3();
		this.simulationComplete = false;
		this.m_ballNotLaunched = true;
		this.m_ballTemplate.SetActive(false);
		SecurityWrapperService.Instance.OnBallLaunched += new Action<RIPESpeedParamsType, RIPESpinParamsType, RIPEFlightParamsType>(this.HardwareDelegateOnLaunchBall);
		this.m_timeSinceShotEnded = 0f;
	}

	public void UpdateHMPlayerHandedness(bool isLefty)
	{
		SecurityWrapperService.Instance.SetHandedness(isLefty);
	}

	public event Action OnBallLaunched;

	public delegate void BallLaunchedDelegate();
}