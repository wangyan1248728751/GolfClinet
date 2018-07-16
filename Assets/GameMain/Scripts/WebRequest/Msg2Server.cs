using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

namespace Golf
{
	/// <summary>
	/// 游客登录
	/// </summary>
	public class m2s_logintourist : IMsgBase
	{
		public m2s_logintourist()
		{
			msgName = "logintourist";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string touName;
		public string touPwd;
		public void MsgAnalysis(JsonData res, Action<object> action)
		{

			m2c_logintourist m2c = new m2c_logintourist();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();
			if (m2c.resultCode == "0")
			{
				m2c.name = res["user"]["userMap"][0]["name"].ToString();
				m2c.pwd = res["user"]["userMap"][0]["pwd"].ToString();
			}
			Debug.Log("游客登录");
			if (action != null)
			{
				action(m2c);
			}
		}
	}

	/// <summary>
	/// 登录服务器、获得游戏服务器列表
	/// </summary>
	public class m2s_login : IMsgBase
	{
		public m2s_login()
		{
			msgName = "login";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string gid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }

		public string name;
		public string pwd;
		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			Debug.Log("解析登录消息");
			//   Debug.Log(".........."+res.ToJson());
			m2c_login m2slogin = new m2c_login();
			m2slogin.smaps = new List<serverMap>();
			m2slogin.resultCode = res["respVo"]["resultCode"].ToString();
			m2slogin.resultDesc = res["respVo"]["resultDesc"].ToString();
			if (m2slogin.resultCode == "0")
			{
				m2slogin.ausession = res["ausession"].ToString();
				for (int i = 0; i < res["server"]["serverMap"].Count; i++)
				{
					serverMap smap = new serverMap();
					smap.load = res["server"]["serverMap"][i]["load"].ToString();
					smap.id = res["server"]["serverMap"][i]["id"].ToString();
					smap.ip = res["server"]["serverMap"][i]["ip"].ToString();
					smap.name = res["server"]["serverMap"][i]["name"].ToString();
					smap.port = res["server"]["serverMap"][i]["port"].ToString();
					smap.isnew = res["server"]["serverMap"][i]["isnew"].ToString();
					smap.recommend = res["server"]["serverMap"][i]["recommend"].ToString();
					m2slogin.smaps.Add(smap);

				}
				GameEntry.WebRequestToServerComponent.SetUserIdAndSession("", "", m2slogin.ausession, "");
			}
			if (action != null)
			{
				action(m2slogin);
			}
		}
	}

	/// <summary>
	/// 注册
	/// </summary>
	public class m2s_regist : IMsgBase
	{
		public m2s_regist()
		{
			msgName = "regist";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string gid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string name;
		public string pwd;
		public string message;
		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_regist m2c = new m2c_regist();
			m2c.smaps = new List<serverMap>();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();
			if (m2c.resultCode == "0")
			{
				m2c.ausession = res["ausession"].ToString();
				for (int i = 0; i < res["server"]["serverMap"].Count; i++)
				{
					serverMap smap = new serverMap();
					smap.load = res["server"]["serverMap"][i]["load"].ToString();
					smap.id = res["server"]["serverMap"][i]["id"].ToString();
					smap.ip = res["server"]["serverMap"][i]["ip"].ToString();
					smap.name = res["server"]["serverMap"][i]["name"].ToString();
					smap.port = res["server"]["serverMap"][i]["port"].ToString();
					m2c.smaps.Add(smap);

				}
				GameEntry.WebRequestToServerComponent.SetUserIdAndSession("", "", m2c.ausession, "");
			}
			if (action != null)
			{
				action(m2c);
			}
		}
	}

	/// <summary>
	/// 登录游戏服务器，获得session
	/// </summary>
	public class m2s_getsession : IMsgBase
	{
		public m2s_getsession()
		{
			msgName = "getsession";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string gid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string name;
		public string serverid;
		public string serverurl;
		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_getsession m2c = new m2c_getsession();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();
			if (m2c.resultCode == "0")
			{
				//Debug.Log(res.ToJson());
				if (res["user"]["userMap"].Count == 0)
				{
					GameEntry.UI.OpenUIForm(UIFormId.MessageForm);
					GameEntry.Event.Fire(1, new ShowMessageEventArgs("服务器异常！"));
					return;
				}
				m2c.id = res["user"]["userMap"][0]["name"].ToString();
				m2c.session = res["user"]["userMap"][0]["session"].ToString();
				m2c.auid = res["user"]["userMap"][0]["id"].ToString();
				GameEntry.WebRequestToServerComponent.SetUserIdAndSession(m2c.id, m2c.session, "", "");
				GameEntry.WebRequestToServerComponent.SetGameServerUrl(serverurl);
			}
			if (action != null)
			{
				action(m2c);
			}
		}
	}

	/// <summary>
	/// 获取用户信息
	/// </summary>
	public class m2s_getuserinfo : IMsgBase
	{
		public m2s_getuserinfo()
		{
			msgName = "getuserinfo";
		}

		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string auid;
		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_getuserinfo m2c = new m2c_getuserinfo();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();
			if (m2c.resultCode == "0")
			{
				m2c.id = res["userinfo"]["userInfoMap"][0]["uid"].ToString();
				m2c.nickname = res["userinfo"]["userInfoMap"][0]["nickname"].ToString();

			}
			if (action != null)
			{
				action(m2c);
			}
		}
	}

	/// <summary>
	/// 改名
	/// </summary>
	public class m2s_updatenickname : IMsgBase
	{
		public m2s_updatenickname()
		{
			msgName = "updatenickname";
		}

		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string nickname;
		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_updatenickname m2c = new m2c_updatenickname();
			m2c.resultCode = res["resultCode"].ToString();
			m2c.resultDesc = res["resultDesc"].ToString();
			if (m2c.resultCode == "0")
			{

			}
			if (action != null)
			{
				action(m2c);
			}
		}
	}



	/// <summary>
	/// 创建角色，设置角色性别、昵称
	/// </summary>
	public class m2s_registroleinfo : IMsgBase
	{
		public m2s_registroleinfo()
		{
			msgName = "registroleinfo";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }

		public int key;
		public string value;
		public string nickname;
		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_registroleinfo m2c = new m2c_registroleinfo();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();
			if (m2c.resultCode == "0")
			{
				m2c.uid = res["roleinfomap"][0]["uid"].ToString();
				m2c.rid = res["roleinfomap"][0]["id"].ToString();
				m2c.lastLogin = res["roleinfomap"][0]["lastLogin"].ToString();
				m2c.nickname = res["roleinfomap"][0]["nickname"].ToString();
				m2c.roleProMap = new List<rolepromap>();
				for (int i = 0; i < res["roleinfomap"][0]["rolepropertymap"]["rolepromap"].Count; i++)
				{
					rolepromap rm = new rolepromap();
					JsonData rolepro = res["roleinfomap"][0]["rolepropertymap"]["rolepromap"][i];
					rm.id = rolepro["id"].ToString();
					rm.name = rolepro["name"].ToString();
					rm.ptyid = rolepro["ptyid"].ToString();
					rm.ptyunit = rolepro["ptyunit"].ToString();
					rm.ptyvalue = rolepro["ptyvalue"].ToString();
					rm.rid = rolepro["rid"].ToString();
					rm.type = rolepro["type"].ToString();
					rm.belong = rolepro["belong"].ToString();
					m2c.roleProMap.Add(rm);
				}
				GameEntry.WebRequestToServerComponent.SetUserIdAndSession("", "", "", m2c.rid);
			}

			Debug.Log("解析服务器时间消息");
			if (action != null)
			{
				action(m2c);
			}
		}
	}

	public class m2s_addmessagesession : IMsgBase
	{
		public m2s_addmessagesession()
		{
			msgName = "addmessagesession";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string name { set; get; }
		public string messagetype { set; get; }
		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_addmessagesession m2c = new m2c_addmessagesession();
			m2c.resultCode = res["resultCode"].ToString();
			m2c.resultDesc = res["resultDesc"].ToString();
			if (action != null)
			{
				action(m2c);
			}
		}
	}

	/// <summary>
	/// 获取排行榜数据
	/// </summary>
	public class m2s_getranklistbyuidbtid : IMsgBase
	{
		public m2s_getranklistbyuidbtid()
		{
			msgName = "getranklistbyuidbtid";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string num { set; get; }

		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_getranklistbyuidbtid m2c = new m2c_getranklistbyuidbtid();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();

			if (m2c.resultCode == "0")
			{
				m2c.RankDatas = new List<RankData>();

				//for (int i = 0; i < res["roleinfomap"][0]["rolepropertymap"]["rolepromap"].Count; i++)
				for (int i = 0; i < res["currentMap"].Count; i++)
				{
					RankData rankData = new RankData();
					rolepromap rm = new rolepromap();
					JsonData currentMap = res["currentMap"][i];

					rankData.name = currentMap["nickname"].ToString();
					rankData.score = int.Parse(currentMap["score"].ToString());
					rankData.timeId = int.Parse(currentMap["id"].ToString());
					m2c.RankDatas.Add(rankData);
				}
				m2c.RankDatas.Sort(delegate (RankData x, RankData y)
				{
					int a = y.score.CompareTo(x.score);
					if (x.score == y.score)
					{
						a = x.timeId.CompareTo(y.timeId);
					}

					return a;
				});
				for (int m = 0; m < m2c.RankDatas.Count; m++)
					m2c.RankDatas[m].rank = m;
			}

			if (action != null)
			{
				action(m2c);
			}
		}
	}


	/// <summary>
	/// 上传排行榜数据
	/// </summary>
	public class m2s_updateranking : IMsgBase
	{
		public m2s_updateranking()
		{
			msgName = "updateranking";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string score { set; get; }

		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_updateranking m2c = new m2c_updateranking();
			m2c.resultCode = res["resultCode"].ToString();
			m2c.resultDesc = res["resultDesc"].ToString();

			if (m2c.resultCode == "0")
			{

			}

			if (action != null)
			{
				action(m2c);
			}
		}
	}

	/// <summary>
	/// 查询用户道具
	/// </summary>
	public class m2s_getacticlebyuid : IMsgBase
	{
		public m2s_getacticlebyuid()
		{
			msgName = "getacticlebyuid";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }

		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_getacticlebyuid m2c = new m2c_getacticlebyuid();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();

			if (m2c.resultCode == "0")
			{
				m2c.goodsList = new List<Goods>();
				for (int i = 0; i < res["acticleMap"].Count; i++)
				{
					Goods goods = new Goods();
					goods.id = int.Parse(res["acticleMap"][i]["id"].ToString());
					goods.sid = int.Parse(res["acticleMap"][i]["sid"].ToString());
					goods.num = int.Parse(res["acticleMap"][i]["num"].ToString());
					goods.url = res["acticleMap"][i]["url"].ToString();
					
					m2c.goodsList.Add(goods);
				}
					
			}

			if (action != null)
			{
				action(m2c);
			}
		}
	}

	/// <summary>
	/// 核销道具
	/// </summary>
	public class m2s_updateacticlenum : IMsgBase
	{
		public m2s_updateacticlenum()
		{
			msgName = "updateacticlenum";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public int id { set; get; }
		public Goods acticle { set; get; }

		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_updateacticlenum m2c = new m2c_updateacticlenum();
			m2c.resultCode = res["resultCode"].ToString();
			m2c.resultDesc = res["resultDesc"].ToString();

			if (m2c.resultCode == "0")
			{

			}

			if (action != null)
			{
				action(m2c);
			}
		}
	}

	/// <summary>
	/// 增加道具
	/// </summary>
	public class m2s_exchangeshop : IMsgBase
	{
		public m2s_exchangeshop()
		{
			msgName = "exchangeshop";
		}
		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public int shopid { set; get; }
		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_exchangeshop m2c = new m2c_exchangeshop();
			m2c.resultCode = res["resultCode"].ToString();
			m2c.resultDesc = res["resultDesc"].ToString();
			if (m2c.resultCode == "0")
			{

			}

			if (action != null)
			{
				action(m2c);
			}
		}
	}


		/// <summary>
		/// 获取地块表
		/// </summary>
		public class m2s_getgolfislands : IMsgBase
	{
		public m2s_getgolfislands()
		{
			msgName = "getgolfislands";
		}

		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string mid { set; get; }


		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_getgolfislands m2c = new m2c_getgolfislands();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();

			if (m2c.resultCode == "0")
			{
				Dictionary<int, DRRewardMap> RewardMap = new Dictionary<int, DRRewardMap>();
				List<Vector2> webPos = new List<Vector2>();
				for (int i = 0; i < res["golfislandsmap"].Count; i++)
				{
					Vector2 vec2 = new Vector2();
					vec2.x = float.Parse(res["golfislandsmap"][i]["topx"].ToString());
					vec2.y = float.Parse(res["golfislandsmap"][i]["topy"].ToString());
					webPos.Add(vec2);

					JsonData rolepro = res["golfislandsmap"][i]["golfislandawardsmap"]["golfislandawardsmap"];
					for (int j = 0; j < rolepro.Count; j++)
					{
						DRRewardMap map = new DRRewardMap();
						map.Id = int.Parse(rolepro[j]["ptseq"].ToString());
						map.Score = int.Parse(rolepro[j]["gpoint"].ToString());
						map.ScoreColor = rolepro[j]["gpcolor"].ToString();
						if (((IDictionary)rolepro[j]).Contains("url"))
							map.ImageUrl = rolepro[j]["url"].ToString();
						map.goodsId = int.Parse(rolepro[j]["sid"].ToString());

						RewardMap.Add(map.Id, map);
					}
				}
				GameEntry.GameData.webPos = webPos;
				m2c.webRewardMap = RewardMap;
			}

			if (action != null)
			{
				action(m2c);
			}
		}
	}
    public class m2s_forpayqrcode:IMsgBase
    {
        public m2s_forpayqrcode()
        {
            msgName = "jlxpay-web/qrcode.jpg?bizOrderId=4&amount=1";
        }
        public string uid { get; set; }
        public string rid { get; set; }
        public string session { get; set; }
        public string msgName{get;set;}
        public string odd;
        public string money;

       


        public void MsgAnalysis(JsonData res, Action<object> action)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 获取设备信息
    /// </summary>
    public class m2s_getgolfmachine : IMsgBase
	{
		public m2s_getgolfmachine()
		{
			msgName = "getgolfmachine";
		}

		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string mid { set; get; }
		public string name { set; get; }


		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_getgolfmachine m2c = new m2c_getgolfmachine();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();

			if (m2c.resultCode == "0")
			{
				if (res["golfmachinemap"].Count != 0)
				{
					m2c.gmid = int.Parse(res["golfmachinemap"][0]["id"].ToString());
					m2c.callback = res["golfmachinemap"][0]["callback"].ToString();
				}
			}
			if (action != null)
			{
				action(m2c);
			}
		}
	}

	/// <summary>
	/// 获取击球点
	/// </summary>
	public class m2s_getkickoffposition : IMsgBase
	{
		public m2s_getkickoffposition()
		{
			msgName = "getkickoffposition";
		}

		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public string mid { set; get; }
		public int gmid { set; get; }


		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_getkickoffposition m2c = new m2c_getkickoffposition();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();

			if (m2c.resultCode == "0")
			{
				m2c.teePosX = -100f;
				if (res["kickoffpositionmap"].Count != 0)
					m2c.teePosX = float.Parse(res["kickoffpositionmap"][0]["topxy"].ToString());
			}
			if (action != null)
			{
				action(m2c);
			}
		}
	}

	public class m2s_fortencentrequest : IMsgBase
	{
		public m2s_fortencentrequest()
		{
			msgName = "fortencentrequest";
		}

		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public int mid { set; get; }


		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_fortencentrequest m2c = new m2c_fortencentrequest();
			m2c.resultCode = res["respVo"]["resultCode"].ToString();
			m2c.resultDesc = res["respVo"]["resultDesc"].ToString();

			if (m2c.resultCode == "0")
			{
				m2c.uid = res["tencentrequestMap"][0]["openid"].ToString();
				m2c.auid = res["tencentrequestMap"][0]["uid"].ToString();
				m2c.session = res["tencentrequestMap"][0]["session"].ToString();
                m2c.WxName = res["tencentrequestMap"][0]["name"].ToString();
                m2c.WxAvator = res["tencentrequestMap"][0]["imag"].ToString();
            }
			if (action != null)
			{
				action(m2c);
			}
		}
	}

	public class m2s_deltencentrequest : IMsgBase
	{
		public m2s_deltencentrequest()
		{
			msgName = "deltencentrequest";
		}

		public string uid { set; get; }
		public string rid { set; get; }
		public string session { set; get; }
		public string msgName { set; get; }
		public int mid { set; get; }


		public void MsgAnalysis(JsonData res, Action<object> action)
		{
			m2c_deltencentrequest m2c = new m2c_deltencentrequest();
			//m2c.resultCode = res["respVo"]["resultCode"].ToString();
			//m2c.resultDesc = res["respVo"]["resultDesc"].ToString();

			if (m2c.resultCode == "0")
			{
				//m2c.uid = res["tencentrequestMap"][0]["openid"].ToString();
				//m2c.auid = res["tencentrequestMap"][0]["uid"].ToString();
				//m2c.session = res["tencentrequestMap"][0]["session"].ToString();
			}
			if (action != null)
			{
				action(m2c);
			}
		}
	}




}


