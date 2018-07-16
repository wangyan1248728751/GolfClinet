using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.DataTable;

namespace Golf
{
	public class LeaderboardForm : UGuiForm
	{
		[SerializeField]
		List<RankListItem> RankListItems;

		[SerializeField]
		GameObject RankListView;
		[SerializeField]
		GameObject UploadView;

		[SerializeField]
		InputField uploadName;
		[SerializeField]
		GameObject uploadRankBtn;


		protected internal override void OnOpen(object userData)
		{
			base.OnOpen(userData);

			uploadRankBtn.AddClick(ChangeUserName);

			UploadView.SetActive(false);
			RankListView.SetActive(false);

			fsnKey = 0;
			RefreshFSN();

		}


		public void CheckScoreAndName()
		{
            //判断要不要上传分数
            if (GameEntry.GameData.RankDatas.Count == 0 || GameEntry.GameData.RankDatas[GameEntry.GameData.RankDatas.Count - 1].score < GameEntry.GameData.totalScore || GameEntry.GameData.RankDatas.Count < 10)
			{
				fsnKey = 2;
			}
			else
				fsnKey = 4;
			RefreshFSN();
		}

		int fsnKey = 0;
		//每个网络连接后刷新一次
		void RefreshFSN()
		{
			switch (fsnKey)
			{
				//获取分数
				case 0:
					{
						UpdateRankData();
						break;
					}
				//根据分数判断要不要上传
				case 1:
					{
						CheckScoreAndName();
						break;
					}
				//改名
				case 2:
					{
                        ChangeUserName();
						break;
					}
				//上传分数
				case 3:
					{
						UploadRankData();
						break;
					}
				//更新排行榜
				case 4:
					{
						UploadView.SetActive(false);
						UpdateRankData();
						break;
					}
				case 5:
					{
						showRankListUI();
						break;
					}
				default:
					break;
			}
		}

		public void showRankListUI()
		{
			for (int i = 0; i < RankListItems.Count; i++)
			{
				if (GameEntry.GameData.RankDatas.Count > i)
					RankListItems[i].SetData(GameEntry.GameData.RankDatas[i]);
				else
					RankListItems[i].gameObject.SetActive(false);
			}
			RankListView.SetActive(true);
		}

		void OpenUploadView()
		{
			UploadView.SetActive(true);
            uploadName.text = GameEntry.GameData.userData.name;
		}

		/// <summary>
		/// 改名
		/// </summary>
		void ChangeUserName()
		{
			m2s_updatenickname msg = new m2s_updatenickname();
			msg.nickname = GameEntry.GameData.WxName;
			M2SInfo m2sInfo = new M2SInfo(msg, ChangeUserNameSuccess, WebRequestFail);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}


		/// <summary>
		/// 上传分数
		/// </summary>
		public void UploadRankData()
		{
			m2s_updateranking msg = new m2s_updateranking();
			msg.score = GameEntry.GameData.totalScore.ToString();
			M2SInfo m2sInfo = new M2SInfo(msg, UploadRankDataSuccess, WebRequestFail);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		/// <summary>
		/// 更新分数
		/// </summary>
		public void UpdateRankData()
		{
			m2s_getranklistbyuidbtid msg = new m2s_getranklistbyuidbtid();
			msg.num = "10";
			M2SInfo m2sInfo = new M2SInfo(msg, UpdateRankDataSuccess, WebRequestFail);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}
        
		void ChangeUserNameSuccess(object obj)
		{
			fsnKey++;
			RefreshFSN();
		}

		private void UpdateRankDataSuccess(object obj)
		{
			m2c_getranklistbyuidbtid data = (m2c_getranklistbyuidbtid)obj;
			GameEntry.GameData.RankDatas = data.RankDatas;
			fsnKey++;
			RefreshFSN();
		}

		private void UploadRankDataSuccess(object obj)
		{
			Debug.Log("上传分数成功");
			fsnKey++;
			RefreshFSN();
		}


		private void WebRequestFail(object obj)
		{
			Debug.Log("Refresh RankListDatas Fail");
			Close();
		}

	}
}