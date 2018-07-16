//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class PracticeGameType : BaseGameBehaviour
//{
//	private IPracticeGreensController _targetPracticeController;

//	private readonly List<ShotsDispersionData> _dispersionData = new List<ShotsDispersionData>();

//	private GameFieldEllipses _gameFieldEllipses;

//	private DispersionCirclesClubView _dispersionCirclesClubView;

//	private DispersionCirclesOverheadView _offlinePanelEllipses;

//	private bool _isHitResultDisplayed;

//	private bool _isDispersionEllipseDrawed;

//	private TracerManager _tracerMgr
//	{
//		get
//		{
//			return CGameManager.instance.UiHolder.TracerManager;
//		}
//	}

//	public PracticeGameType(float timeWatchBall)
//	{
//		this._targetPracticeController = new PracticeGreensController();
//		this.timeWatchBall = timeWatchBall;
//		this.ktWatchBall.OverideTimerToWaitValue(timeWatchBall);
//		this.uiDialsHUD.SetActive(true);
//		this.uiClubSelect.SetActive(true);
//		PinManager component = this.pin.GetComponent<PinManager>();
//		if (component != null)
//		{
//			component.pinAndFlag.SetActive(false);
//			component.theGreen.SetActive(false);
//			component.targetRings.SetActive(false);
//			component.yardMarkers.SetActive(true);
//			component.SetToggleToMarkers();
//		}
//		CSimulationManager.instance.PreBallLaunchDelegate = new CSimulationManager.BallLaunchedDelegate(this.BallLaunchedSubscriber);
//		CSimulationManager.instance.SetBallTracerHasColor(true);
//		this._targetPracticeController.OnNewSubActivityStartedEvent += new Action(this.OnNewSubActivityStarted);
//	}

//	private void BallLaunchedSubscriber()
//	{
//		this._tracerMgr.ChangeTracerDisplay(TracerManager.TRACER_AMOUNT.ONE);
//	}

//	public override void ChangeCameraMode(CGolfCamera.CAMERA_BEHAVIOR camMode, bool isIgnoreBallLand)
//	{
//		this.ignoreBallLand = isIgnoreBallLand;
//		this._cameraBehaviour = camMode;
//		if (camMode == CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE || camMode == CGolfCamera.CAMERA_BEHAVIOR.CAMERA_45_DEGREES)
//		{
//			this.ktWatchBall.OverideTimerToWaitValue(1.5f);
//		}
//		else
//		{
//			this.ktWatchBall.OverideTimerToWaitValue(this.timeWatchBall);
//		}
//	}

//	private void DrawDispersionEllipse()
//	{
//		int clubColorIndex = this.GetClubColorIndex(base.CurrentClubName);
//		if (clubColorIndex + 1 > this._dispersionData.Count)
//		{
//			ShotsDispersionData shotsDispersionDatum = new ShotsDispersionData();
//			this._dispersionData.Add(shotsDispersionDatum);
//			this._gameFieldEllipses.AddNewEllipse(shotsDispersionDatum);
//			this._offlinePanelEllipses.AddData(shotsDispersionDatum);
//		}
//		TFlightData mostRecentFlight = CFlightDataStorage.instance.GetMostRecentFlight();
//		this._dispersionData[clubColorIndex].AddShot(mostRecentFlight);
//		this._gameFieldEllipses.DrawDispersionEllipse(clubColorIndex);
//		this._offlinePanelEllipses.DrawDispersionEllipse(clubColorIndex);
//		this.UpdateDispersionValue();
//	}

//	private void OnDispersionDistanceSettingChanged()
//	{
//		for (int i = 0; i < this._dispersionData.Count; i++)
//		{
//			this._gameFieldEllipses.DrawDispersionEllipse(i);
//			this._offlinePanelEllipses.DrawDispersionEllipse(i);
//		}
//		this.UpdateDispersionValue();
//	}

//	private void OnNewSubActivityStarted()
//	{
//		this.isReplayAvailable = false;
//	}

//	private void OnShotWasDeleted(TFlightData flightData)
//	{
//		int clubColorIndex = this.GetClubColorIndex(flightData.clubName);
//		this._dispersionData[clubColorIndex].DeleteShot(flightData);
//		this._gameFieldEllipses.DrawDispersionEllipse(clubColorIndex);
//		this._offlinePanelEllipses.DrawDispersionEllipse(clubColorIndex);
//		this.UpdateDispersionValue();
//	}

//	public override bool PostShot()
//	{
//		this.isReplayAvailable = true;
//		this._isHitResultDisplayed = false;
//		this._isDispersionEllipseDrawed = false;
//		base.SetActivitiesAndShotsReadyToBeSynchronized();
//		return true;
//	}

//	public override bool PrepareTee()
//	{
//		this.currentBall = CSimulationManager.instance.GetNewBall();
//		this.ballHasLanded = false;
//		this.golfCameraManager.SetTarget(this.currentBall.gameObject);
//		this._tracerMgr.ChangeTracerDisplay(this.gameSettings.TracerAmount);
//		this.panelFade.FadeIn();
//		this.uiClubSelect.SetActive(true);
//		if (this.isReplay)
//		{
//			base.CleanUpReplay();
//		}
//		CSimulationManager.instance.UpdateHMPlayerHandedness(CGameManager.instance.UiHolder.GameSettings.IsHandnessLefty);
//		return true;
//	}

//	public override void ResetGame()
//	{
//		base.ResetGame();
//		this._dispersionData.Clear();
//		this._gameFieldEllipses.Reset();
//		this._offlinePanelEllipses.Reset();
//		this.UpdateDispersionValue();
//	}

//	public override bool SetNextTurn()
//	{
//		if (this.currentBall != null)
//		{
//			base.AddCurrentBallToPrevBalls();
//		}
//		this.golfCameraManager.SetCameraBehavior(this.DefaultCameraBehavior);
//		CSimulationManager.instance.ResetScene();
//		return true;
//	}

//	public override void SetUp()
//	{
//		this.clubPopUpManager.SetClub("UNDEFINED");
//		base.SetUp();
//		this._targetPracticeController.Setup();
//		this._dispersionCirclesClubView = CGameManager.instance.UiHolder.DispersionCirclesClubView;
//		this._offlinePanelEllipses = CGameManager.instance.UiHolder.DispCircleOffline;
//		this._gameFieldEllipses = CGameManager.instance.UiHolder.GameFieldEllipses;
//		if (this.gameSettings != null)
//		{
//			this.gameSettings.OnDispersionDistanceSettingChanged = new Action(this.OnDispersionDistanceSettingChanged);
//			this.OnDispersionDistanceSettingChanged();
//		}
//		this.clubPopUpManager.OnClubWasChanged += new Action(this.UpdateDispersionValue);
//		CFlightDataStorage.instance.OnFlightWasDeleted += new Action<TFlightData>(this.OnShotWasDeleted);
//	}

//	private void ShowHideHUDPanels(bool show)
//	{
//		Transform transforms = this.uiDialsHUD.transform;
//	}

//	public override void ShutDownCleanUp()
//	{
//		this._targetPracticeController.ShutDown();
//		this.clubPopUpManager.OnClubWasChanged -= new Action(this.UpdateDispersionValue);
//		CFlightDataStorage.instance.OnFlightWasDeleted -= new Action<TFlightData>(this.OnShotWasDeleted);
//		base.ShutDownCleanUp();
//	}

//	public override bool TakeShot()
//	{
//		if (!CSimulationManager.instance.ballLaunched)
//		{
//			return false;
//		}
//		this.isReplayAvailable = false;
//		this.panelFade.FadeOut();
//		this.uiClubSelect.SetActive(false);
//		this.ShowHideHUDPanels(false);
//		return true;
//	}

//	private void UpdateDispersionValue()
//	{
//		string str;
//		int clubColorIndex = this.GetClubColorIndex(base.CurrentClubName);
//		if (clubColorIndex >= this._dispersionData.Count)
//		{
//			this._dispersionCirclesClubView.UpdateDispersionValue("--", clubColorIndex);
//			return;
//		}
//		float single = (!this.gameSettings.IsDispersionDistanceTotal ? this._dispersionData[clubColorIndex].ShotCarryDispersion : this._dispersionData[clubColorIndex].ShotTotalDispersion);
//		str = (!this._dispersionData[clubColorIndex].IsDispersionAvailable ? "--" : string.Format("{0:F0} {1}", UnitsConverter.Instance.GetDistance(single), UnitsConverter.CurrentDistanceUnitsShort));
//		this._dispersionCirclesClubView.UpdateDispersionValue(str, clubColorIndex);
//	}

//	public override bool WatchShot()
//	{
//		if (CSimulationManager.instance.isSimulationComplete && this._targetPracticeController.IsTargetGreenTurnedOn && !this._isHitResultDisplayed)
//		{
//			this._targetPracticeController.BallEndedFlight(this.currentBall.transform);
//			this._isHitResultDisplayed = true;
//		}
//		if (base.WatchShot())
//		{
//			return true;
//		}
//		if (!this.isReplay && CSimulationManager.instance.isSimulationComplete && !this._isDispersionEllipseDrawed)
//		{
//			this.DrawDispersionEllipse();
//			this._isDispersionEllipseDrawed = true;
//		}
//		if (CSimulationManager.instance.isSimulationComplete)
//		{
//			this.ShowHideHUDPanels(true);
//		}
//		return false;
//	}
//}