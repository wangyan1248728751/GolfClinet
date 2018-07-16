using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utilities;

public abstract class BaseGameBehaviour : IGameInterface
{
	private const int MaxBallsOnTheField = 50;

	protected const int topThree = 3;

	protected BaseGameBehaviour.GAME_TYPE gamePlayersType = BaseGameBehaviour.GAME_TYPE.SINGLE_PLAYER;

	protected IList<CBall> previousBalls;

	protected CBall currentBall;

	protected GameObject tee;

	protected GameObject pin;

	protected CGolfCamera golfCameraManager;

	protected GameObject uiDialsHUD;

	protected GameObject uiClubSelect;

	//protected GameExitPanelView _gameExitPanel;

	//protected PreAndPostShotPillView uiPillPanel;

	protected ClubPopUpManager clubPopUpManager;

	//protected NumericDisplays numericDisplays;

	//protected NumericDisplayStatic numericDisplayStatic;

	protected TracerManager tracerManager;

	protected GameSettings gameSettings;

	//protected PanelFade panelFade;

	protected CKitchenTimer ktWatchWinnerScreen;

	protected CKitchenTimer ktWatchBall;

	protected CKitchenTimer KTWatchNextUp;

	protected CGolfCamera.CAMERA_BEHAVIOR _cameraBehaviour;

	protected bool isReplay;

	protected bool isReplayAvailable;

	protected bool ballHasLanded;

	protected bool ignoreBallLand;

	protected List<PLAYER> players;

	protected uint iPlayerTurn;

	protected uint iCurrentRound;

	protected bool allRoundsComplete;

	protected uint totalRoundsToPlay;

	protected uint turnsPerRound;

	protected uint prevPlayerTurn;

	protected Activity activity;

	protected float timeWatchBall;

	private CBall originalBall;

	private CBall replayBall;

	private bool _isGameOver;

	private bool _isShutdown;

	private string _currentClubName;

	private bool _shotValuesWasSaved;

	public string CurrentClubName
	{
		get
		{
			return this._currentClubName.Replace('\u00B0', '\u00B0');
		}
	}

	protected virtual CGolfCamera.CAMERA_BEHAVIOR DefaultCameraBehavior
	{
		get
		{
			return CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE;
		}
	}

	public bool IsGameOver
	{
		get
		{
			return this._isGameOver;
		}
	}

	public virtual bool IsNewGame
	{
		get
		{
			return false;
		}
		protected set
		{
		}
	}

	public virtual bool IsPlayAgain
	{
		get
		{
			return false;
		}
		protected set
		{
		}
	}

	public virtual bool IsReadyForGameStart
	{
		get
		{
			return true;
		}
	}

	public bool IsReplay
	{
		get
		{
			return this.isReplay;
		}
	}

	public bool IsReplayAvailable
	{
		get
		{
			return this.isReplayAvailable;
		}
	}

	public bool IsShutDown
	{
		get
		{
			return this._isShutdown;
		}
	}

	public virtual bool IsSinglePlayer
	{
		get
		{
			return true;
		}
	}

	protected BaseGameBehaviour()
	{
		this.iPlayerTurn = 1;
		this.iCurrentRound = 1;
		this.allRoundsComplete = false;
		this.players = new List<PLAYER>();
		this.previousBalls = new List<CBall>(50);
		this.tee = GameObject.FindGameObjectWithTag("Tee");
		this.pin = GameObject.FindGameObjectWithTag("Pin");
		this.uiDialsHUD = CGameManager.instance.UiHolder.UiDialsParent;
		this.golfCameraManager = CGameManager.instance.UiHolder.GolfCamera;
		//this._gameExitPanel = CGameManager.instance.UiHolder.GameExitPanel;
		//this.uiPillPanel = CGameManager.instance.UiHolder.PrePostPill;
		//this.clubPopUpManager = CGameManager.instance.UiHolder.ClubPopUpManager;
		this.uiClubSelect = this.clubPopUpManager.gameObject;
		//this.numericDisplays = CGameManager.instance.UiHolder.NumericDisplays;
		//this.numericDisplayStatic = CGameManager.instance.UiHolder.NumericDisplaysStatic;
		//this.numericDisplays.SetPanels();
		this.tracerManager = CGameManager.instance.UiHolder.TracerManager;
		this.gameSettings = CGameManager.instance.UiHolder.GameSettings;
		//this.panelFade = CGameManager.instance.UiHolder.PanelFade;
		this.ktWatchBall = new CKitchenTimer(2f);
		this.ktWatchWinnerScreen = new CKitchenTimer(2f);
		this.KTWatchNextUp = new CKitchenTimer(2f);
		this._isShutdown = false;
	}

	protected void AddCurrentBallToPrevBalls()
	{
		if (this.isReplay)
		{
			return;
		}
		if (this.currentBall != null && !this.previousBalls.Contains(this.currentBall))
		{
			if (this.previousBalls.Count == 50)
			{
				CSimulationManager.instance.DestroyBall(this.previousBalls[0]);
			}
			this.previousBalls.Add(this.currentBall);
		}
	}

	protected void AddPlayerToGame(string esnUserID, string firstName, bool isLefty)
	{
		PLAYER pLAYER = new PLAYER(esnUserID, firstName, false, this.totalRoundsToPlay, this.turnsPerRound, isLefty);
		this.players.Add(pLAYER);
	}

	public virtual void ChangeCameraMode(CGolfCamera.CAMERA_BEHAVIOR camMode, bool isIgnoreBallLand)
	{
	}

	protected void CleanUpReplay()
	{
		this.isReplay = false;
		CSimulationManager.instance.DestroyBall(this.replayBall);
		this.originalBall.gameObject.SetActive(true);
		//CGameManager.instance.UiHolder.NumericDisplays.ResetDials(true);
		//CGameManager.instance.UiHolder.NumericDisplaysStatic.ResetDials(true);
	}

	protected void ClearAllBalls()
	{
		while (this.previousBalls.Count > 0)
		{
			if (this.previousBalls[0] == null)
			{
				this.previousBalls.RemoveAt(0);
			}
			else
			{
				CSimulationManager.instance.DestroyBall(this.previousBalls[0]);
			}
		}
		CSimulationManager.instance.DestroyBall(this.currentBall);
		this.currentBall = null;
		this.previousBalls.Clear();
	}

	protected void CreateNewBall()
	{
		this.currentBall = CSimulationManager.instance.GetNewBall();
		this.ballHasLanded = false;
		this.golfCameraManager.SetTarget(this.currentBall.gameObject);
	}

	public void DoReplay(TFlightData data)
	{
		if (data.isValid)
		{
			this.isReplay = true;
			this.originalBall = data.ballTransform.GetComponent<CBall>();
			this.originalBall.gameObject.SetActive(false);
			this.replayBall = this.currentBall;
			this.replayBall.name = "Prop_Ball::Replay";
			//this.replayBall.SetNewTracerMaterial(ApplicationDataManager.instance.GetTracerMaterialByColor(this.originalBall.tracerColor));
			//CSimulationManager.instance.ReplayShot(data);
			//this.numericDisplays.ResetDials();
			//this.numericDisplayStatic.ResetDials();
		}
	}

	protected void FinishActivity()
	{
		if (this.activity != null)
		{
			SessionAndActivityManager.Instance.FinishMainActivity();
			this.activity = null;
		}
	}

	public virtual bool GameOver()
	{
		return true;
	}

	public virtual int GetClubColorIndex(string clubName)
	{
		int num = 0;
		IList<TFlightData> flights = CFlightDataStorage.instance.GetFlights();
		if (flights.Count == 0)
		{
			return num;
		}
		List<string> list = (
			from data in flights
			select data.clubName).Distinct<string>().ToList<string>();
		if (!list.Contains(clubName))
		{
			return list.Count;
		}
		return list.IndexOf(clubName);
	}

	protected Vector2 GetRelativeBallPos(Vector3 ballPos, Vector3 teePos)
	{
		Vector3 vector3 = ballPos - teePos;
		float single = vector3.x * 1.09361f;
		return new Vector2(single, vector3.z * 1.09361f);
	}

	public virtual bool MoveTee()
	{
		return true;
	}

	public virtual bool PostShot()
	{
		return true;
	}

	public virtual bool PrepareTee()
	{
		return true;
	}

	protected virtual void ProcessWatchShot_BallHasLanded()
	{
		this.ballHasLanded = true;
	}

	protected virtual void ProcessWatchShot_BallIsMidFlight()
	{
		this.golfCameraManager.SetCameraBehavior(this._cameraBehaviour);
	}

	protected virtual void ProcessWatchShot_Common()
	{
	}

	protected virtual bool ProcessWatchShot_IsSimulationComplete()
	{
		//this.panelFade.FadeIn();
		this.ktWatchBall.UpdateDeltaTime();
		if (!this.ktWatchBall.isDone)
		{
			return false;
		}
		this.ktWatchBall.Reset();
		return true;
	}

	public void RemovePrevBall(CBall ball)
	{
		if (ball == null)
		{
			return;
		}
		if (!this.previousBalls.Contains(ball))
		{
			AppLog.LogError(string.Concat("Previous balls dont contain ball ", ball.gameObject.name), true);
		}
		else
		{
			this.previousBalls.Remove(ball);
		}
	}

	public virtual void RequestExitGame()
	{
		this.ShutDownRequest();
	}

	public virtual void ResetGame()
	{
		this.ResetGameOver();
		this.ktWatchBall.Reset();
		this.ktWatchWinnerScreen.Reset();
		this.KTWatchNextUp.Reset();
		this.IsNewGame = false;
		this.IsPlayAgain = false;
		this.ClearAllBalls();
	}

	protected void ResetGameOver()
	{
		this._isGameOver = false;
	}

	protected void SetActivitiesAndShotsReadyToBeSynchronized()
	{
		if (this.activity == null || !LoginManager.IsUserLoggedIn || !this.IsSinglePlayer)
		{
			return;
		}
		(
			from a in (IEnumerable<Activity>)this.activity.GetSubactivitiesRecursively(true)
			where !a.IsReadyToBeSynchronized
			select a).ForEach<Activity>((Activity a) =>
			{
				a.IsReadyToBeSynchronized = true;
				//DatabaseManager.InsertOrUpdate<Activity>(a);
			});
		((IEnumerable<Activity>)this.activity.GetSubactivitiesRecursively(true)).SelectMany<Activity, Shot>((Activity a) => a.Shots).Where<Shot>((Shot sh) => !sh.IsReadyToBeSynchronized).ForEach<Shot>((Shot sh) =>
		{
			sh.IsReadyToBeSynchronized = true;
			//DatabaseManager.InsertOrUpdate<Shot>(sh);
		});
	}

	public bool SetClub(string clubName)
	{
		string str = clubName.Trim();
		str = str.Replace('\u00B0', '\u00B0');
		if (str.Equals(this._currentClubName))
		{
			return false;
		}
		this._currentClubName = str;
		string str1 = string.Copy(this._currentClubName);
		for (int i = 0; i < str1.Length; i++)
		{
			if (str1[i] == '#')
			{
				str1 = str1.Substring(0, str1.Length - (str1.Length - i)).Trim();
			}
		}
		if (!string.IsNullOrEmpty(Club.GetClubIDFromName(str1.ToUpper())))
		{
			return true;
		}
		Debug.Log(string.Concat("ERROR: ", this._currentClubName.ToUpper()));
		Debug.LogError("Invalid club type given. Will not save to the sever due to server rejection due to incorrect data.");
		return false;
	}

	protected void SetGameOver()
	{
		this._isGameOver = true;
		this.SetActivitiesAndShotsReadyToBeSynchronized();
	}

	public virtual bool SetNextTurn()
	{
		return true;
	}

	public virtual void SetUp()
	{
		this.gamePlayersType = BaseGameBehaviour.GAME_TYPE.SINGLE_PLAYER;
		if (!LoginManager.IsUserLoggedIn)
		{
			this.AddPlayerToGame("ID1234", "Player1", false);
		}
		else
		{
			string str = string.Concat(LoginManager.UserData.FirstName, " ", LoginManager.UserData.LastName);
			str = (string.IsNullOrEmpty(str) || str.Trim() == string.Empty ? "Player1" : str.ToUpper());
			int id = LoginManager.UserData.Id;
			this.AddPlayerToGame(id.ToString(), str, CGameManager.instance.UiHolder.GameSettings.IsHandnessLefty);
			if (this.activity == null)
			{
				this.StartActivity();
			}
		}
	}

	protected virtual void ShowBallMarker()
	{
	}

	public virtual void ShutDownCleanUp()
	{
		Debug.Log("Start ShutDown");
		Debug.Log("Shutting down system");
		this.FinishActivity();
		CFlightDataStorage.instance.ClearAllData();
		this.gameSettings.OnGameShutDown();
		Debug.Log("End ShutDown");
	}

	public void ShutDownRequest()
	{
		this._isShutdown = true;
	}

	public virtual void StartActivity()
	{
		this.activity = SessionAndActivityManager.Instance.CreateActivity();
	}

	public abstract bool TakeShot();

	public virtual void Update()
	{
	}

	public virtual bool WatchShot()
	{
		if (!this.isReplay && CSimulationManager.instance.isSimulationComplete && !this._shotValuesWasSaved)
		{
			TFlightData mostRecentFlight = CFlightDataStorage.instance.GetMostRecentFlight();
			Vector2 relativeBallPos = this.GetRelativeBallPos(mostRecentFlight.carryPoint.location, this.tee.transform.position);
			Vector2 vector2 = this.GetRelativeBallPos(this.currentBall.transform.position, this.tee.transform.position);
			mostRecentFlight.carryPosX = relativeBallPos.x;
			mostRecentFlight.carryPosY = relativeBallPos.y;
			mostRecentFlight.totalPosX = vector2.x;
			mostRecentFlight.totalPosY = vector2.y;
			if (LoginManager.IsUserLoggedIn)
			{
				Shot lastShotFromCurrentActivity = SessionAndActivityManager.Instance.GetLastShotFromCurrentActivity();
				lastShotFromCurrentActivity.CarryPosX = relativeBallPos.x;
				lastShotFromCurrentActivity.CarryPosY = relativeBallPos.y;
				lastShotFromCurrentActivity.TotalPosX = vector2.x;
				lastShotFromCurrentActivity.TotalPosY = vector2.y;
				lastShotFromCurrentActivity.IsDirty = true;
				//DatabaseManager.InsertOrUpdate<Shot>(lastShotFromCurrentActivity);
			}
			this._shotValuesWasSaved = true;
		}
		if (CSimulationManager.instance.isSimulationComplete)
		{
			if (this.ProcessWatchShot_IsSimulationComplete())
			{
				this._shotValuesWasSaved = false;
				return true;
			}
		}
		else if (this.currentBall.hasBallLanded && !this.ballHasLanded && !this.ignoreBallLand)
		{
			this.ProcessWatchShot_BallHasLanded();
		}
		else if (!this.currentBall.GetTrajectory().IsMidFlight())
		{
			this.ProcessWatchShot_Common();
		}
		else
		{
			this.ProcessWatchShot_BallIsMidFlight();
		}
		return false;
	}

	protected enum GAME_TYPE
	{
		SINGLE_PLAYER = 1,
		MULTI_PLAYER = 2
	}
}