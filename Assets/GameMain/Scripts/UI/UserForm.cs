using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.DataTable;
using TMPro;
using System.Collections;
using DG.Tweening;

namespace Golf
{
    public class UserForm : UGuiForm
    {
        [SerializeField]
        GameObject userAvatar;
        [SerializeField]
        GameObject bagParent;

        [SerializeField]
        GameObject pathBtn;
        [SerializeField]
        Text pathBtnText;

		[SerializeField]
		Text ballCount;

        [SerializeField]
        Text GetScoreText;

        [SerializeField]
        GameObject RestartGameBtn;

        [SerializeField]
		List<GameObject> ballScoreObjs;
        [SerializeField]
        List<GameObject> redballObjs;
        [SerializeField]
        List<GameObject> blueballObjs;
		[SerializeField]
		Text totalScoreText;
		[SerializeField]
		GameObject MiniScoreBtn;

		[SerializeField]
		GameObject closeGameBtn;

		[SerializeField]
		GameObject testHitBtn;

		[SerializeField]
		GameObject redPoint;

		[SerializeField]
		GameObject showResultFormBtn;
        [SerializeField]
        GameObject gameBallResult;

		[SerializeField]
		GameObject DeviceShotStateView;
		[SerializeField]
		Text DeviceShotStateText;
        [SerializeField]
        Text goodsCount;
        [SerializeField]
        Text timer;
        [SerializeField]
        Button drop;
        [SerializeField]
        Image rota;
        [SerializeField]
        GameObject RestartCam;
        [SerializeField]
        Text playerPrepare;
        [SerializeField]
        GameObject colorballbg;
        Button restartbtn;
        Button Cdrop;
		internal protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
		}

        protected internal override void OnOpen(object userData)
		{
			base.OnOpen(userData);
			GameEntry.Event.Subscribe(HitBallResultEventArgs.EventId, HitBallResult);
			GameEntry.Event.Subscribe(HitBallEndEventArgs.EventId, HitBallEnd);
			GameEntry.Event.Subscribe(GameStartEventArgs.EventId, GameStartEvent);
			GameEntry.Event.Subscribe(DeviceShotStateEventArgs.EventId, DeviceShotStateRefresh);
            
            RefreshUI();
            drop.onClick.AddListener(() =>
            {
                StartCoroutine(moveTo());
            });
			userAvatar.AddClick(OpenBag);
			closeGameBtn.AddClick(CloseGame);
			redPoint.SetActive(false);
            add = false;
			testHitBtn.AddClick(test.Instance.testClick);
			showResultFormBtn.AddClick(ShowResultFormBtnOnclick); 
		}

		internal protected override void OnClose(object userData)
        {
            GameEntry.Event.Unsubscribe(HitBallResultEventArgs.EventId, HitBallResult);
			GameEntry.Event.Unsubscribe(HitBallEndEventArgs.EventId, HitBallEnd);
			GameEntry.Event.Unsubscribe(GameStartEventArgs.EventId, GameStartEvent);
			redPoint.SetActive(false);
            add = false;
			base.OnClose(userData);
        }
        private void OnEnable()
        {
            if (GameEntry.GameData.onLine)
            {
                gameBallResult.SetActive(true);
                userAvatar.SetActive(true);
                MiniScoreBtn.SetActive(true);
                colorballbg.SetActive(false);
            }
            else
            {
                gameBallResult.SetActive(false);
                userAvatar.SetActive(false);
                MiniScoreBtn.SetActive(false);
                colorballbg.SetActive(true);
            }
        }
        protected internal override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            RefreshUI();
        }
        bool isMove=false;
        public IEnumerator prepare()
        {
            if (!GameEntry.GameData.onLine)
            {
                yield return new WaitForSeconds(0.5f);
                if (GameEntry.GameData.currGolfball.Count%2==1&& (GameEntry.HitBall.redisland.Count != 8 && GameEntry.HitBall.blueisland.Count != 8))
                {
                    playerPrepare.text = "二号玩家开始击球";
                }
                else
                {
                    playerPrepare.text = "一号玩家开始击球";
                }
                yield return new WaitForSeconds(2);
                playerPrepare.text = null;

            }
        }
        IEnumerator moveTo()
        {
            if (rota.transform.rotation!=Quaternion.Euler(new Vector3(0,0,-90)))
            {
                rota.transform.Rotate(new Vector3(0, 0, -180));
                drop.transform.Translate(new Vector3(170, 0, 0));
                isMove = false;
            }
            else
            {
                rota.transform.Rotate(new Vector3(0, 0, 180));
                drop.transform.Translate(new Vector3(-170, 0, 0));
                isMove = true;
                yield return new WaitForSeconds(2);
                if (isMove)
                {
                rota.transform.Rotate(new Vector3(0, 0, -180));
                drop.transform.Translate(new Vector3(170, 0, 0));
                }
            }
        }
        void CloseGame()
        {
            rota.transform.Rotate(new Vector3(0, 0, -180));
            drop.transform.Translate(new Vector3(170, 0, 0));
            GameEntry.GameCore.LeaveGame();
		}
        public void clickRestart()
        {
            isSub = false;
            m_time = 8;
            timer.text = null;
        }

        public void ResetGameCameraPos()
        {
            GameEntry.Map._gameCameraManager.ResetPos();
            StartCoroutine(prepare());
        }

        public void OpenBag()
        {
            redPoint.SetActive(false);
            add = false;
            count = 0;
            if (GameEntry.GameData.onLine)
            {
                if (!GameEntry.UI.IsLoadingUIForm(AssetUtility.GetUIFormAsset("UserInfoForm")))
                    GameEntry.UI.OpenUIForm(UIFormId.UserInfoForm);
            }
            else
                GameEntry.Event.Fire(this, new ShowMessageEventArgs("游客模式 背包不可用!"));
        }

        public void CloseBag()
        {
            bagParent.SetActive(false);
        }

        public void PathBtnOnClick()
        {
            if (GameEntry.GameData.pathBtnState < pathBtnEnum.more)
                GameEntry.GameData.pathBtnState++;
            else
                GameEntry.GameData.pathBtnState = pathBtnEnum.one;
            pathBtnText.text = GameEntry.Localization.GetString("pathBtnEnum." + GameEntry.GameData.pathBtnState.ToString());
            GameEntry.HitBall.ShowFlightPath(GameEntry.GameData.pathBtnState);
        }


        void GameStartEvent(object sender, GameEventArgs e)
        {
            RefreshUI();
            StartCoroutine(prepare());

        }

        public void HitBallResult(object sender, GameEventArgs e)
        {
            //HitBallResultEventArgs ne = (HitBallResultEventArgs)e;

            //刷新击球数
            ballCount.text = "X " + (GameEntry.GameData.MaxGolfballCount - GameEntry.GameData.currGolfball.Count);
            MiniScoreState = MiniScoreEnum.none;
            SetMiniScore();
            isSub = true;
        }

        public void HitBallEnd(object sender, GameEventArgs e)
        {
            //HitBallEndEventArgs ne = (HitBallEndEventArgs)e;

            GameEntry.HitBall.ShowFlightPath(GameEntry.GameData.pathBtnState);

            if (GameEntry.GameData.onLine)
            {
                golfballHitData currData = GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1];
                if (currData.rewardMapId != 0)
                {
                    DRRewardMap data = GameEntry.GameData.webRewardMap[currData.rewardMapId];
                    AddGoods(data.goodsId);
                }
            }

            //得分相关处理
            //ScoreFly();
            ScoreShine();
            Ballshine();
            if (GameEntry.GameData.currGolfball.Count >= GameEntry.GameData.MaxGolfballCount && GameEntry.GameData.onLine)
            {
                StartCoroutine(showResultForm());
            }
            else if ((GameEntry.HitBall.redisland.Count == 8 || GameEntry.HitBall.blueisland.Count == 8) && !GameEntry.GameData.onLine)
            {
                StartCoroutine(showResultForm());
            }
        }

        void DeviceShotStateRefresh(object sender, GameEventArgs e)
        {
            DeviceShotStateEventArgs eventArgs = e as DeviceShotStateEventArgs;

            switch (eventArgs.State)
            {
                case DeviceShotStateEventArgs.DeviceShotState.Start:
                    DeviceShotStateView.SetActive(true);
                    DeviceShotStateText.text = "数据计算中";
                    break;
                case DeviceShotStateEventArgs.DeviceShotState.End:
                    DeviceShotStateView.SetActive(false);
                    break;
                case DeviceShotStateEventArgs.DeviceShotState.Error:
                    StartCoroutine(ShotError());
                    break;
            }
        }

        IEnumerator ShotError()
        {
            DeviceShotStateText.text = "数据异常";
            yield return new WaitForSeconds(2);
            DeviceShotStateView.SetActive(false);
        }


        IEnumerator showResultForm()
        {
            showResultFormBtn.SetActive(true);
            yield return new WaitForSeconds(10);
            showResultFormBtn.SetActive(false);
        }

        void ShowResultFormBtnOnclick()
        {
            if (GameEntry.GameData.needShowResultForm)
                GameEntry.UI.OpenUIForm(UIFormId.ResultForm);
            else
                showResultFormBtn.SetActive(false);
        }

        public float gapTime=0.6f;
        float temp;
        bool IsDispaly;
        public bool add;
        int count=0;
        public float m_time=8;
        bool isSub=false;

        public void Effect()
        {
        temp+= Time.deltaTime; 
        if(temp>= gapTime)
        {
                //if(count>=5)
                if (add)
                {
                    if (IsDispaly)
                    {
                        redPoint.gameObject.SetActive(false);
                        IsDispaly = false;
                        temp = 0;
                    }
                    else
                    {
                        redPoint.gameObject.SetActive(true);
                        IsDispaly = true;
                        temp = 0;
                    }
                }
        } 
        }
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            Effect();
            goodsCount.text = count.ToString();
            if (isSub)
            {
                timer.text = m_time.ToString();
                m_time -= Time.deltaTime;
                if (m_time<=0)
                {
                    StartCoroutine(prepare());
                    ResetGameCameraPos();
                    clickRestart();
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetGameCameraPos();
                clickRestart();
            }else if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                test.Instance.testClick();
            }
        }


        void AddGoods(int goodsId)
		{
			if (goodsId == 0)
				return;

			redPoint.SetActive(true);
            add = true;
            count++;
			m2s_exchangeshop msg = new m2s_exchangeshop();
			msg.shopid = goodsId;
			M2SInfo m2sInfo = new M2SInfo(msg, AddGoodsSuccess, WebRequestFail);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		void AddGoodsSuccess(object obj)
		{

		}

		private void WebRequestFail(object obj)
		{
			GameEntry.Event.Fire(this, new ShowMessageEventArgs("网络异常!"));
		}

		private void ScoreFly()
		{
			GameObject obj = GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1].rewardMap;
			if (obj == null)
				return;

			Vector3 screenPos = GameEntry.Map._gameCameraManager._camera.WorldToScreenPoint(obj.transform.position);
			Debug.Log("pos:" + screenPos);
			GetScoreText.transform.parent.localPosition = screenPos;

			GetScoreText.gameObject.SetActive(true);
			GetScoreText.text = GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1].score.ToString();
			GetScoreText.GetComponent<Animation>().Stop();
			GetScoreText.GetComponent<Animation>().Play();
		}
        private void Ballshine()
        {
            for (int i = 0; i < redballObjs.Count; i++)
            {
                if (i<GameEntry.HitBall.redisland.Count)
                {
                    if (redballObjs[i].activeSelf==false)
                    {
                        redballObjs[i].SetActive(true);
                    }
                }
                else
                {
                    redballObjs[i].SetActive(false);
                }
            }
            for (int i = 0; i < blueballObjs.Count; i++)
            {
                if (i < GameEntry.HitBall.blueisland.Count)
                {
                    if (blueballObjs[i].activeSelf == false)
                    {
                        blueballObjs[i].SetActive(true);
                    }
                }
                else
                {
                    blueballObjs[i].SetActive(false);
                }
            }
        }
        private void ScoreShine()
        {
			for (int i = 0; i < ballScoreObjs.Count; i++)
			{
				if (i < GameEntry.GameData.currGolfball.Count)
				{
					if (ballScoreObjs[i].activeSelf == false)
					{
						ballScoreObjs[i].SetActive(true);
						ballScoreObjs[i].GetComponentInChildren<Text>().text = GameEntry.GameData.currGolfball[i].score.ToString();
					}
				}
				else
					ballScoreObjs[i].SetActive(false);
			}
			totalScoreText.text = GameEntry.GameData.totalScore.ToString();
		}

		public void RefreshUI()
		{
			ScoreShine();
            Ballshine();
            ballCount.text = "X " + (GameEntry.GameData.MaxGolfballCount - GameEntry.GameData.currGolfball.Count);
			MiniScoreState = MiniScoreEnum.all;
			SetMiniScore();
		}

		public void MiniScoreOnclick()
		{
			if (MiniScoreState != MiniScoreEnum.all)
				MiniScoreState++;
			else
				MiniScoreState = MiniScoreEnum.none;

			SetMiniScore();
		}


		MiniScoreEnum MiniScoreState = MiniScoreEnum.none;
		void SetMiniScore()
		{
			MiniScoreBtn.GetComponentInChildren<Text>().text = GameEntry.Localization.GetString("MiniScoreEnum." + MiniScoreState.ToString());

			if (GameEntry.Map._islandManager != null)
				switch (MiniScoreState)
				{
					case MiniScoreEnum.none:
						foreach (IslandScoreBase items in GameEntry.Map._islandManager.islandScoreList)
						{
							foreach (Renderer renderer in items._renderer)
							{
								renderer.enabled = false;
							}

							foreach (TextMeshPro text in items._text)
							{
								text.enabled = false;
							}
						}
						break;

					case MiniScoreEnum.score:
						foreach (IslandScoreBase items in GameEntry.Map._islandManager.islandScoreList)
						{
							foreach (Renderer renderer in items._renderer)
							{
								renderer.enabled = false;
							}

							foreach (TextMeshPro text in items._text)
							{
								text.enabled = true;
							}
						}
						break;

					case MiniScoreEnum.pic:

						foreach (IslandScoreBase items in GameEntry.Map._islandManager.islandScoreList)
						{
							foreach (Renderer renderer in items._renderer)
							{
								renderer.enabled = true;
							}

							foreach (TextMeshPro text in items._text)
							{
								text.enabled = false;
							}
						}
						break;

					case MiniScoreEnum.all:
						foreach (IslandScoreBase items in GameEntry.Map._islandManager.islandScoreList)
						{
							foreach (Renderer renderer in items._renderer)
							{
								renderer.enabled = true;
							}

							foreach (TextMeshPro text in items._text)
							{
								text.enabled = true;
							}
						}
						break;
				}
		}

		/// <summary>
		/// 小地图分数图片显示状态 烂名字 凑合用
		/// </summary>
		enum MiniScoreEnum
		{
			/// <summary>
			/// 全不显示
			/// </summary>
			none,
			/// <summary>
			/// 只显示分数
			/// </summary>
			score,
			/// <summary>
			/// 只显示图片
			/// </summary>
			pic,
			/// <summary>
			/// 全显示
			/// </summary>
			all,

		}

	}


}
