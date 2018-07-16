using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Golf
{
    public class GameCoreComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 游戏当前状态
        /// </summary>
        GameRunningState _gameRunningState;

        int LeaveGameTimerEventId;

        public void GameInit()
        {
            GameEntry.Map.Init();
            GameEntry.HitBall.Init();
        }

        public void GameStart()
        {
			//清理物体 

			//岛屿位置 相机位置 分数重置
			GameEntry.Map.ResetMap();

			GameEntry.Event.Fire(this, new GameStartEventArgs());

			GameEntry.HitBall.PrepareHit();
            //未击球倒计时
            //LeaveGameTimerEventId = TimerManager.CreateTimerEvent(LeaveGame, GameEntry.GameData.LeaveGameTime);

        }

        private void Update()
        {
            if (_gameRunningState != GameRunningState.running)
                return;
            if ((GameEntry.GameData.currGolfball.Count >= GameEntry.GameData.MaxGolfballCount) && GameEntry.GameData.onLine)
            { GameOver();            }
            if ((GameEntry.HitBall.redisland.Count==8||GameEntry.HitBall.blueisland.Count==8)&&!GameEntry.GameData.onLine)
            {
                GameOver();
            }
        }

        public void GameOver()
        {
            _gameRunningState = GameRunningState.stop;
            TimerManager.StopTimerEvent(LeaveGameTimerEventId);

			GameEntry.GameData.needShowResultForm = true;
			//显示结算界面
			StartCoroutine(OpenResultForm());
			//GameEntry.UI.OpenUIForm(UIFormId.ResultForm);
			//排行榜
			if (GameEntry.GameData.onLine)
				GameEntry.UI.OpenUIForm(UIFormId.LeaderboardForm);
		}

        public void GameRestart()
        {
			GameEntry.HitBall.hidePath();
			if (GameEntry.UI.HasUIForm(UIFormId.LeaderboardForm))
				GameEntry.UI.GetUIForm(UIFormId.LeaderboardForm).Close(true);
            if (GameEntry.UI.HasUIForm(UIFormId.BallInfoForm))
                GameEntry.UI.GetUIForm(UIFormId.BallInfoForm).Close(true);
            GameEntry.Map._islandManager.ShineEnd();
			foreach (golfballHitData data in GameEntry.GameData.currGolfball)
			{
				if (data.ball != null)
					GameObject.Destroy(data.ball.gameObject);
			}
			GameEntry.GameData.currGolfball.Clear();
            
			GameStart();
        }
        public void HitGolfBall(golfballHitData data)
        {
            if (GameEntry.UI.HasUIForm(UIFormId.BallInfoForm))
                GameEntry.UI.GetUIForm(UIFormId.BallInfoForm).Close(true);
            GameCameraManager.Instance.ResetPos();
            GameEntry.HitBall.HitGolfBall(ref data);
			GameEntry.GameData.currGolfball.Add(data);

			GameEntry.Event.Fire(this, new HitBallResultEventArgs(data.rewardMapId));
            //重置未击球倒计时
            //TimerManager.ModifyTimerEvent(LeaveGameTimerEventId, GameEntry.GameData.LeaveGameTime);
            GameEntry.UI.OpenUIForm(UIFormId.BallInfoForm);
        }

		public void BallEnd(CBall ball)
		{
			GameEntry.HitBall.BallEnd(ball);

			GameEntry.Map._islandManager.Shine();
			GameEntry.UI.OpenUIForm(UIFormId.BallInfoForm);

			GameEntry.Event.Fire(this, new HitBallEndEventArgs());

			if ((GameEntry.GameData.currGolfball.Count < GameEntry.GameData.MaxGolfballCount)||(GameEntry.HitBall.redisland.Count!=8&&GameEntry.HitBall.blueisland.Count!=8&&!GameEntry.GameData.onLine))
				return;

			GameOver();
		}


		public void LeaveGame()
        {
			GameEntry.UI.CloseAllLoadedUIForms();
			GameEntry.UI.CloseAllLoadingUIForms();

			//清理
			foreach (golfballHitData data in GameEntry.GameData.currGolfball)
			{
				if (data.ball != null)
					GameObject.Destroy(data.ball.gameObject);
			}
			GameEntry.GameData.currGolfball.Clear();
			GameEntry.HitBall.hidePath();
            //清理结束
            //ProcedureFunction fProcedure = GameEntry.Procedure.CurrentProcedure as ProcedureFunction;
            //if (fProcedure!=null)
            //{
            //    fProcedure.closeGame = true;
            //}
			ProcedureMain currProcedure = GameEntry.Procedure.CurrentProcedure as ProcedureMain;
			if(currProcedure != null)
			{
				//GameEntry.GameData.onLine = false;
				currProcedure.closeGame = true;
			}
        }

		IEnumerator OpenResultForm()
		{
            if (GameEntry.GameData.onLine)
			yield return new WaitForSeconds(10);
			if (GameEntry.GameData.needShowResultForm)
				GameEntry.UI.OpenUIForm(UIFormId.ResultForm);
		}


	}


    enum GameRunningState
    {
        stop,
        running,
        pause,
    }
}