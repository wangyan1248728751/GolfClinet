using System;

public interface IGameInterface
{
	string CurrentClubName
	{
		get;
	}

	bool IsGameOver
	{
		get;
	}

	bool IsNewGame
	{
		get;
	}

	bool IsPlayAgain
	{
		get;
	}

	bool IsReadyForGameStart
	{
		get;
	}

	bool IsReplay
	{
		get;
	}

	bool IsReplayAvailable
	{
		get;
	}

	bool IsShutDown
	{
		get;
	}

	bool IsSinglePlayer
	{
		get;
	}

	void ChangeCameraMode(CGolfCamera.CAMERA_BEHAVIOR camMode, bool isIgnoreBallLand);

	void DoReplay(TFlightData data);

	bool GameOver();

	int GetClubColorIndex(string clubName);

	bool MoveTee();

	bool PostShot();

	bool PrepareTee();

	void RemovePrevBall(CBall ball);

	void RequestExitGame();

	void ResetGame();

	bool SetClub(string clubName);

	bool SetNextTurn();

	void SetUp();

	void ShutDownCleanUp();

	void ShutDownRequest();

	void StartActivity();

	bool TakeShot();

	void Update();

	bool WatchShot();
}