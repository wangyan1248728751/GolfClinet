using Security;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CGameManager : MonoBehaviour
{
	protected CKitchenTimer SceneTimer;

	private static CGameManager _instance;

	public GameUIHolder UiHolder;

	private CGameManager.GAMESTATE _gameState;

	private CGameManager.GAME_SUBSTATE _gameSubState;

	private bool playerReadyForShot;

	private bool replayShotRequested;

	private CGameManager.GAME_RULES_TYPE _currentGameType;

	private IGameInterface _gameInterface;

	private bool _restartGameInited;

	private bool _shutdownComplete;

	public CGameManager.GAME_RULES_TYPE currentGameType
	{
		get
		{
			return this._currentGameType;
		}
		set
		{
			this._currentGameType = value;
			AppLog.Log(string.Concat("GameType ", this._currentGameType), true);
		}
	}

	private CGameManager.GAMESTATE gameState
	{
		get
		{
			return this._gameState;
		}
		set
		{
			this._gameState = value;
			AppLog.Log(string.Concat("GameState ", this._gameState), true);
		}
	}

	private CGameManager.GAME_SUBSTATE gameSubState
	{
		get
		{
			return this._gameSubState;
		}
		set
		{
			this._gameSubState = value;
			AppLog.Log(string.Concat("GameSubState ", this._gameSubState), true);
		}
	}

	public static CGameManager instance
	{
		get
		{
			return CGameManager._instance;
		}
	}

	public bool IsGameOver
	{
		get
		{
			return this._gameInterface.IsGameOver;
		}
	}

	public bool IsReplay
	{
		get
		{
			return (this._gameInterface == null ? false : this._gameInterface.IsReplay);
		}
	}

	public bool isReplayReady
	{
		get
		{
			if (this._gameInterface == null)
			{
				return false;
			}
			return this._gameInterface.IsReplayAvailable;
		}
	}

	public bool isSinglePlayer
	{
		get
		{
			if (this._gameInterface == null)
			{
				return true;
			}
			return this._gameInterface.IsSinglePlayer;
		}
	}

	public bool shutdownComplete
	{
		get
		{
			return this._shutdownComplete;
		}
	}

	private void Awake()
	{
		if (CGameManager._instance == null)
		{
			CGameManager._instance = this;
		}
	}

	//	//public static DatabaseManager.QueryAggregateFunc ConvertGameTypeToQueryAggregateFunc(CGameManager.GAME_RULES_TYPE gameType)
	//	//{
	//	//	switch (gameType)
	//	//	{
	//	//		case CGameManager.GAME_RULES_TYPE.LongDrive:
	//	//			{
	//	//				return DatabaseManager.QueryAggregateFunc.MAX;
	//	//			}
	//	//		case CGameManager.GAME_RULES_TYPE.ClosestToPin:
	//	//			{
	//	//				return DatabaseManager.QueryAggregateFunc.MIN;
	//	//			}
	//	//		case CGameManager.GAME_RULES_TYPE.Bullseye:
	//	//			{
	//	//				return DatabaseManager.QueryAggregateFunc.SUM;
	//	//			}
	//	//	}
	//	//	throw new Exception("Unknown challenge type in ConvertGameTypeToQueryAggregateFunc");
	//	//}

	private void FixedUpdate()
	{
		if (this._restartGameInited)
		{
			return;
		}
		switch (this.gameState)
		{
			case CGameManager.GAMESTATE.INITIALIZE:
				{
					this.ProcessInitializeState();
					break;
				}
			case CGameManager.GAMESTATE.OPTIONS:
				{
					this.ProcessOptionsState();
					break;
				}
			case CGameManager.GAMESTATE.SETUP:
				{
					this.ProcessSetupState();
					break;
				}
			case CGameManager.GAMESTATE.RUNNING:
				{
					this.ProcessRunningState();
					break;
				}
			case CGameManager.GAMESTATE.SHUTDOWN:
				{
					this.ProcessShutdownState();
					break;
				}
		}
	}

	public int GetClubColorIndex(string clubName)
	{
		return this._gameInterface.GetClubColorIndex(clubName);
	}

	public int GetCurrentClubColorIndex()
	{
		return this._gameInterface.GetClubColorIndex(this._gameInterface.CurrentClubName);
	}

	public string GetCurrentClubName()
	{
		return this._gameInterface.CurrentClubName;
	}

	private BaseGameBehaviour InitializeGameMode()
	{
		BaseGameBehaviour practiceGameType = null;
		if (CGameSelectManager.GameSelected != CGameManager.GAME_RULES_TYPE.None)
		{
			this.currentGameType = CGameSelectManager.GameSelected;
		}
		switch (this.currentGameType)
		{
			case CGameManager.GAME_RULES_TYPE.None:
				{
					break;
				}
			case CGameManager.GAME_RULES_TYPE.Practice:
				{
					//practiceGameType = new PracticeGameType(4f);
					break;
				}
			case CGameManager.GAME_RULES_TYPE.LongDrive:
				{
					//practiceGameType = new LongDriveGameType(4f, 3f);
					break;
				}
			case CGameManager.GAME_RULES_TYPE.ClosestToPin:
				{
					//practiceGameType = new ClosestToPinGameType(8f, 3f);
					break;
				}
			case CGameManager.GAME_RULES_TYPE.Bullseye:
				{
					//practiceGameType = new TargetPracticeGameType(8f, 3f);
					break;
				}
			case CGameManager.GAME_RULES_TYPE.SkillsAssessment:
				{
					//practiceGameType = new SkillsAssessmentGameType(4f);
					break;
				}
			case CGameManager.GAME_RULES_TYPE.BagMapping:
				{
					//practiceGameType = new BagMappingGameType(4f);
					break;
				}
			default:
				{
					throw new Exception("No handler to initialize current game type");
				}
		}
		return practiceGameType;
	}

	private void ProcessInitializeState()
	{
		this._gameInterface = this.InitializeGameMode();
		this.UiHolder.GameSettings.Initialize();
		SecurityWrapperService.Instance.ArmBox();
		switch (this.currentGameType)
		{
			case CGameManager.GAME_RULES_TYPE.Practice:
				{
					this.gameState = CGameManager.GAMESTATE.SETUP;
					break;
				}
			case CGameManager.GAME_RULES_TYPE.LongDrive:
			case CGameManager.GAME_RULES_TYPE.ClosestToPin:
			case CGameManager.GAME_RULES_TYPE.Bullseye:
				{
					this.gameState = CGameManager.GAMESTATE.OPTIONS;
					break;
				}
			case CGameManager.GAME_RULES_TYPE.SkillsAssessment:
			case CGameManager.GAME_RULES_TYPE.BagMapping:
				{
					this.gameState = CGameManager.GAMESTATE.OPTIONS;
					break;
				}
		}
	}

	private void ProcessOptionsState()
	{
		if (this._gameInterface.IsShutDown)
		{
			this.gameState = CGameManager.GAMESTATE.SHUTDOWN;
			return;
		}
		if (this._gameInterface.IsReadyForGameStart)
		{
			this.gameState = CGameManager.GAMESTATE.SETUP;
		}
	}

	private void ProcessRunningState()
	{
		if (this._restartGameInited)
		{
			return;
		}
		if (this._gameInterface.IsShutDown)
		{
			this.gameState = CGameManager.GAMESTATE.SHUTDOWN;
			return;
		}
		if (this._gameInterface.IsNewGame || this._gameInterface.IsPlayAgain)
		{
			this.gameSubState = CGameManager.GAME_SUBSTATE.GAME_OVER;
		}
		switch (this.gameSubState)
		{
			case CGameManager.GAME_SUBSTATE.PREP_TEE:
				{
					if (this._gameInterface.PrepareTee())
					{
						this.gameSubState = CGameManager.GAME_SUBSTATE.MOVE_TEE;
					}
					break;
				}
			case CGameManager.GAME_SUBSTATE.MOVE_TEE:
				{
					if (this._gameInterface.MoveTee())
					{
						this.gameSubState = CGameManager.GAME_SUBSTATE.TAKE_SHOT;
						SecurityWrapperService.Instance.ArmBox();
					}
					break;
				}
			case CGameManager.GAME_SUBSTATE.TAKE_SHOT:
				{
					if (this.replayShotRequested)
					{
						TFlightData mostRecentFlight = CFlightDataStorage.instance.GetMostRecentFlight();
						if (mostRecentFlight != null)
						{
							this._gameInterface.DoReplay(mostRecentFlight);
						}
						this.replayShotRequested = false;
					}
					if (this._gameInterface.TakeShot())
					{
						this.gameSubState = CGameManager.GAME_SUBSTATE.WATCH_SHOT;
					}
					break;
				}
			case CGameManager.GAME_SUBSTATE.WATCH_SHOT:
				{
					if (this._gameInterface.WatchShot())
					{
						this.gameSubState = CGameManager.GAME_SUBSTATE.POST_SHOT;
					}
					break;
				}
			case CGameManager.GAME_SUBSTATE.POST_SHOT:
				{
					if (this._gameInterface.PostShot())
					{
						if (!this._gameInterface.IsGameOver)
						{
							this.gameSubState = CGameManager.GAME_SUBSTATE.SET_NEXT_TURN;
						}
						else
						{
							this.gameSubState = CGameManager.GAME_SUBSTATE.GAME_OVER;
						}
					}
					break;
				}
			case CGameManager.GAME_SUBSTATE.SET_NEXT_TURN:
				{
					if (this._gameInterface.SetNextTurn())
					{
						this.gameSubState = CGameManager.GAME_SUBSTATE.PREP_TEE;
					}
					break;
				}
			case CGameManager.GAME_SUBSTATE.GAME_OVER:
				{
					if (this._gameInterface.GameOver())
					{
						this.gameSubState = CGameManager.GAME_SUBSTATE.SET_NEXT_TURN;
						if (this._gameInterface.IsPlayAgain)
						{
							this._gameInterface.ResetGame();
							this.gameSubState = CGameManager.GAME_SUBSTATE.SET_NEXT_TURN;
						}
						if (this._gameInterface.IsNewGame)
						{
							this._restartGameInited = true;
							this._gameInterface.ResetGame();
							this._gameInterface.ShutDownCleanUp();
							base.Invoke("ReloadGameScene", 2f);
						}
					}
					break;
				}
		}
	}

	private void ProcessSetupState()
	{
		if (this._gameInterface.IsShutDown)
		{
			this.gameState = CGameManager.GAMESTATE.SHUTDOWN;
			return;
		}
		this._gameInterface.SetUp();
		this.gameState = CGameManager.GAMESTATE.RUNNING;
		this.gameSubState = CGameManager.GAME_SUBSTATE.PREP_TEE;
	}

	private void ProcessShutdownState()
	{
		if (!this._shutdownComplete)
		{
			this._gameInterface.ShutDownCleanUp();
			this._shutdownComplete = true;
			//this.UiHolder.CleanAll();
			SecurityWrapperService.Instance.QueueUpdateMMS();
			//AsyncSceneLoadingView.LoadScene("MainNew", 3f);
		}
	}

	private void ReloadGameScene()
	{
		//this.UiHolder.CleanAll();
		SceneManager.LoadScene("OLD_DrivingRange_Mountain_new");
	}

	public void RequestDeleteBall(CBall ball)
	{
		this._gameInterface.RemovePrevBall(ball);
	}

	//public void RequestExitGame(GameExitPanelView exitPopupView)
	//{
	//	this._gameInterface.RequestExitGame();
	//}

	public void RequestMainActivityStart()
	{
		this._gameInterface.StartActivity();
	}

	public void RequestShotReplay()
	{
		this.replayShotRequested = true;
	}

	public void RequestShutdown(bool isSoftShutdown = false)
	{
		if (this._gameInterface != null)
		{
			this._gameInterface.ShutDownRequest();
		}
	}

	public void ResetGame()
	{
		this._gameInterface.ResetGame();
		this.gameSubState = CGameManager.GAME_SUBSTATE.POST_SHOT;
	}

	public bool SetClub(string clubName)
	{
		return this._gameInterface.SetClub(clubName);
	}

	public void SetupPlayCamera(CGolfCamera.CAMERA_BEHAVIOR CamMode, bool IsIgnoreBallLand)
	{
		this._gameInterface.ChangeCameraMode(CamMode, IsIgnoreBallLand);
	}

	private void Start()
	{
		this._shutdownComplete = false;
		this.gameState = CGameManager.GAMESTATE.INITIALIZE;
		this.gameSubState = CGameManager.GAME_SUBSTATE.PREP_TEE;
		this.SceneTimer = new CKitchenTimer(2f);
		//NextPlayerPopUp.Create();
	}

	private void Update()
	{
		if (this._gameInterface != null)
		{
			this._gameInterface.Update();
		}
	}

	public enum GAME_RULES_TYPE
	{
		None,
		Practice,
		LongDrive,
		ClosestToPin,
		Bullseye,
		SkillsAssessment,
		BagMapping
	}

	public enum GAME_SUBSTATE
	{
		PREP_TEE,
		MOVE_TEE,
		TAKE_SHOT,
		WATCH_SHOT,
		POST_SHOT,
		SET_NEXT_TURN,
		GAME_OVER
	}

	public enum GAMESTATE
	{
		INITIALIZE,
		OPTIONS,
		SETUP,
		RUNNING,
		SHUTDOWN
	}
}