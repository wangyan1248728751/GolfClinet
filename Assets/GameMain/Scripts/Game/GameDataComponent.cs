using GameFramework.DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Golf
{
	public class GameDataComponent : GameFrameworkComponent
	{
        public string WxName;
        public string WxAvator;
        public bool isResetName=true;
		/// <summary>
		/// 高尔夫球上限数
		/// </summary>
		public readonly int MaxGolfballCount = 10;

		/// <summary>
		/// 高尔夫球击球数据
		/// </summary>
		public List<golfballHitData> currGolfball = new List<golfballHitData>();

		public float teePosX = 0.5f;

		public bool needShowResultForm = false;

		/// <summary>
		/// 当前总分
		/// </summary>
		public int totalScore
		{
			get
			{
				int score = 0;
				foreach (golfballHitData data in currGolfball)
					score += data.score;
				return score;
			}
		}

		/// <summary>
		/// 显示球飞行轨迹的状态
		/// </summary>
		public pathBtnEnum pathBtnState = pathBtnEnum.one;

		/// <summary>
		/// 游戏中X分钟未击球返回主界面的时间(s)
		/// </summary>
		public int LeaveGameTime = 300;

		public Camera MapCamera;
		//账户信息
		public UserData userData;

		//online
		public bool onLine = false;

		/// <summary>
		/// 设备ID
		/// </summary>
		public string boxName
		{
			get
			{
				return PlayerPrefs.GetString("boxName");
				//return "SKYTRAK_C47F51027DCD";
			}
			set
			{
				PlayerPrefs.SetString("boxName", value);
			}
		}

		/// <summary>
		/// 商家ID
		/// </summary>
		public int ShopName
		{
			get
			{
				return PlayerPrefs.GetInt("ShopName");
				//return 13;
			}
			set
			{
				PlayerPrefs.SetInt("ShopName", value);
			}
		}

		//背包信息

		//地形岛屿数据

		public IDataTable<DRRewardMap> dtRewardMap {
			get
			{
				return GameEntry.DataTable.GetDataTable<DRRewardMap>();
			}
		}

		public Dictionary<int,DRRewardMap> webRewardMap = null;
		public List<Vector2> webPos = null;

		public void RefreshRewardMap()
		{
			//TODO:判断网络
			//dtRewardMap = GameEntry.DataTable.GetDataTable<DRRewardMap>();
			
		}


		/// <summary>
		/// 排行榜数据
		/// </summary>
		public List<RankData> RankDatas;

		/// <summary>
		/// 处理ballHitData
		/// </summary>
		public void SetScoreData(int rewardMapId, out int score)
		{
			if (dtRewardMap == null)
				RefreshRewardMap();

			DRRewardMap rewardMapData = new DRRewardMap();
			if (rewardMapId != 0)
			{
				if (onLine)
					rewardMapData = webRewardMap[rewardMapId];
				else
					rewardMapData = dtRewardMap.GetDataRow(rewardMapId);
			}
			score = rewardMapData.Score;

		}

	}

	public struct golfballHitData
	{
		/// <summary>
		/// 落点
		/// </summary>
		public Vector3 endPoint;

		/// <summary>
		/// 飞行时间
		/// </summary>
		public float time;
		/// <summary>
		/// 最大高度
		/// </summary>
		public float maxHeight;
		/// <summary>
		/// 速度
		/// </summary>
		public float speed;
		/// <summary>
		/// 仰角
		/// </summary>
		public float launchAngle;
		/// <summary>
		/// 水平角
		/// </summary>
		public float horizontalAngle;

		/// <summary>
		/// 倒旋
		/// </summary>
		public float backSpin;

		/// <summary>
		/// 侧旋
		/// </summary>
		public float sideSpin;

		private int _rewardMapId;
		/// <summary>
		/// 得分地块
		/// </summary>
		public int rewardMapId
		{
			set
			{
				_rewardMapId = value;
				GameEntry.GameData.SetScoreData(value, out score);
			}
			get { return _rewardMapId; }
		}

		public GameObject rewardMap;

		/// <summary>
		/// 得分
		/// </summary>
		public int score;
		/// <summary>
		/// 球
		/// </summary>
		public CBall ball;
	}

	public class RankData
	{
		public int rank;
		public string name;
		public int score;
		public int timeId;
	}

	public class UserData
	{
		public string id;
		public string name;


	}

}