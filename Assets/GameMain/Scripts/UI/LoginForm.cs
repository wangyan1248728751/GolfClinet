using GameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Golf
{
	public class LoginForm : UGuiForm,iLogin
	{
		[SerializeField]
		private InputField m_userNameField = null;
		[SerializeField]
		private InputField m_passWordField = null;
		[SerializeField]
		private Toggle m_automaticLogon_Icon = null;
		[SerializeField]
		private Text m_toolTip = null;
		private ProcedureLogin m_ProcedureLogin = null;
		[SerializeField]
		private GameObject m_loginConcent = null;

		[SerializeField]
		private GameObject m_loginBtn = null;
		[SerializeField]
		private GameObject m_visitorBtn = null;
		[SerializeField]
		private GameObject m_testLoginBtn = null;
		[SerializeField]
		private GameObject m_registerBtn = null;
		[SerializeField]
		private GameObject m_forgotPwdBtn = null;
		[SerializeField]
		private GameObject m_removeToolTipBtn = null;
		[SerializeField]
		private Text VersionNumber;


		private string currentUserName, currentPsd;
		protected internal override void OnInit(object userData)
		{

			base.OnInit(userData);
			#region Btn注册
			m_loginBtn.AddClick(OnStartButtonClick);
			m_visitorBtn.AddClick(OnVistorLogonClick);
			m_registerBtn.AddClick(OnRegistrationButtonClick);
			m_forgotPwdBtn.AddClick(OnPasswordRetrievalButtonClick);
			m_testLoginBtn.AddClick(TestLoginBtnOnclick);
            #endregion

        }
       
		protected internal override void OnOpen(object userData)
		{
			base.OnOpen(userData);

			GameEntry.UI.OpenUIForm(UIFormId.MessageForm);

			m_loginConcent.SetActive(false);
			m_userNameField.text = "";
			m_passWordField.text = "";

			m_ProcedureLogin = (ProcedureLogin)userData;
			if (m_ProcedureLogin == null)
			{
				Log.Warning("ProcedureMenu is invalid when open LoginFrom.");
				return;
			}
			//GameEntry.UI.GetUIForm(UIFormId.ConnectForm).Close();

		}

		public void CloseForm()
		{
			Close(true);
		}

		/// <summary>
		/// 游客登录事件
		/// </summary>
		public void OnVistorLogonClick()
		{
			//m2s_logintourist msg = new m2s_logintourist();
			//M2SInfo m2sInfo = new M2SInfo(msg, Msglogintourist, MsgFailurelogintourist);
			//GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);

			NetFsnState = 5;
			NetFSN();
		}

		/// <summary>
		/// 游客登录成功事件
		/// </summary>
		/// <param name="obj"></param>
		public void Msglogintourist(object obj)
		{
			m2c_logintourist msg = (m2c_logintourist)obj;
			if (msg.resultCode == "0")
			{
				string setUserName = msg.name;
				string setPsd = msg.pwd;
				GameEntry.Setting.SetString("VisitorUserName", setUserName);
				GameEntry.Setting.SetString("VisitorPsd", setPsd);
				SendLoginMessage(setUserName, setPsd);
				//Debug.Log(" sdd " + setPsd);
			}
			if (msg.resultCode == "-1")
			{

			}
		}
		/// <summary>
		/// 游客登录失败事件
		/// </summary>
		/// <param name="obj"></param>
		public void MsgFailurelogintourist(object obj)
		{

		}
		/// <summary>
		/// 注册事件
		/// </summary>
		public void OnRegistrationButtonClick()
		{
			GameEntry.UI.OpenUIForm(UIFormId.RegisterForm);
		}
		/// <summary>
		/// 找回密码事件
		/// </summary>
		public void OnPasswordRetrievalButtonClick()
		{
			//GameEntry.UI.OpenUIForm(UIFormId.PasswordRetrievalForm);
		}
		/// <summary>
		/// 登录触发
		/// </summary>
		public void OnStartButtonClick()
		{
			if (!LoginJudgment(m_userNameField, m_passWordField))
				return;
			if (m_userNameField.text.Trim().Length != 11)
			{
				string concent = GameEntry.Localization.GetString("ToolTip.NoExist");

				GameEntry.Event.Fire(this, new ShowMessageEventArgs(concent));
				return;
			}
			NetFsnState = 0;
			NetFSN();
		}

		public void TestLoginBtnOnclick()
		{
			m_userNameField.text = "17397959946";
			m_passWordField.text = "123456";

			NetFsnState = 0;
			NetFSN();
		}


		public int NetFsnState = 0;
		public void NetFSN()
		{
			switch (NetFsnState)
			{
				case 0:
					//登陆鉴权
					SendLoginMessage(m_userNameField.text.Trim(), m_passWordField.text.Trim());
					break;
				case 1:
					//登陆游戏服务器 && 更新用户信息
					SendChooseServerMessage();
					break;

				case 2:
					//获取高尔夫信息

					GetServerRewardMap();

					break;
				case 3:
					//设备信息
					GetGolfMachine();

					break;
				//case 4:
				//	//击球点

				//	break;
				case 4:
					//在线模式进入游戏
					GameEntry.GameData.onLine = true;
					m_loginConcent.SetActive(false);
                    m_ProcedureLogin.enter = true;
                    //GameEntry.UI.OpenUIForm(UIFormId.FunctionForm);
                    break;
				case 5:
					//离线模式进入游戏
					GameEntry.GameData.onLine = false;
					m_loginConcent.SetActive(false);
                    m_ProcedureLogin.enter = true;
                    //GameEntry.UI.OpenUIForm(UIFormId.FunctionForm);
                    break;
			}
		}





















		/// <summary>
		/// 登录判断
		/// </summary>
		public bool LoginJudgment(InputField userNameField, InputField passWordField)
		{
			if (userNameField == null || passWordField == null)
				return false;
			if (userNameField.text == "" || passWordField.text == "")
			{
				string concent = GameEntry.Localization.GetString("ToolTip.NoNull");
				GameEntry.UI.OpenUIForm(UIFormId.MessageForm);
				GameEntry.Event.Fire(1, new ShowMessageEventArgs(concent));
				return false;
			}
			return true;
		}
		/// <summary>
		/// 向服务器发送登录信息
		/// </summary>
		public void SendLoginMessage(string userName, string passWord)
		{
			currentUserName = userName;
			currentPsd = passWord;
			m_loginConcent.SetActive(true);
			m2s_login msg = new m2s_login();
			msg.name = userName;
			msg.pwd = passWord;
			msg.gid = "4";
			M2SInfo m2sInfo = new M2SInfo(msg, MsgSuccessLogin, MsgFailureLogin);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}
		/// <summary>
		/// 登录响应成功事件
		/// </summary>
		/// <param name="obj"></param>
		public void MsgSuccessLogin(object obj)
		{
			m2c_login msg = (m2c_login)obj;
			if (msg.resultCode == "0")
			{
				ServerListInfo.ServerList = msg.smaps;
				Dictionary<string, serverMap> serverListDic = new Dictionary<string, serverMap>();
				foreach (serverMap item in msg.smaps)
				{
					serverListDic.Add(item.id, item);
				}
				ServerListInfo.ServerDic = serverListDic;

				NetFsnState++;
				NetFSN();
			}
			if (msg.resultCode == "-1")
			{
				m_loginConcent.SetActive(false);
				GameEntry.Event.Fire(this, new ShowMessageEventArgs(msg.resultDesc));
			}
		}

		/// <summary>
		/// 连接游戏服务器
		/// </summary>
		void SendChooseServerMessage()
		{
			serverMap m_serverMap = ServerListInfo.ServerDic["7"];
			if (m_serverMap.load == "-1")
			{
				GameEntry.Event.Fire(this, new ShowMessageEventArgs("服务器维护中..."));
				return;
			}

			m2s_getsession msg = new m2s_getsession();
			msg.name = currentUserName;
			msg.serverid = m_serverMap.id;
			msg.serverurl = "http://" + m_serverMap.ip + ":" + m_serverMap.port + "/";
			msg.gid = "4";
			M2SInfo m2sInfo = new M2SInfo(msg, ChooseServerSuccess, MsgFailureLogin);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		void ChooseServerSuccess(object obj)
		{
			//Debug.Log("游戏服务器登陆成功");
			m2c_getsession m2c = (m2c_getsession)obj;
			GetUserInfo(m2c);

		}

		/// <summary>
		/// 获取用户信息
		/// </summary>
		/// <param name="auid"></param>
		void GetUserInfo(m2c_getsession data)
		{
			m2s_getuserinfo msg = new m2s_getuserinfo();
			msg.auid = data.auid;
			msg.session = data.session;
			M2SInfo m2sInfo = new M2SInfo(msg, GetUserInfoSuccess, MsgFailureLogin);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		void GetUserInfoSuccess(object obj)
		{
			m2c_getuserinfo info = (m2c_getuserinfo)obj;
			UserData data = new UserData();
			data.id = info.id;
			data.name = info.nickname;
			GameEntry.GameData.userData = data;

			NetFsnState++;
			NetFSN();
		}


		//获取高尔夫信息
		void GetServerRewardMap()
		{
			m2s_getgolfislands msg = new m2s_getgolfislands();
			msg.mid = GameEntry.GameData.ShopName.ToString();
			M2SInfo m2sInfo = new M2SInfo(msg, GetServerRewardMapSuccess, MsgFailureLogin);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		void GetServerRewardMapSuccess(object obj)
		{
			m2c_getgolfislands msg = (m2c_getgolfislands)obj;
			GameEntry.GameData.webRewardMap = msg.webRewardMap;

			NetFsnState++;
			NetFSN();
		}

		//设备信息
		void GetGolfMachine()
		{
			m2s_getgolfmachine msg = new m2s_getgolfmachine();
			msg.mid = GameEntry.GameData.ShopName.ToString();
			msg.name = GameEntry.GameData.boxName;
			M2SInfo m2sInfo = new M2SInfo(msg, GetGolfMachineSuccess, MsgFailureLogin);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		void GetGolfMachineSuccess(object obj)
		{
			m2c_getgolfmachine msg = (m2c_getgolfmachine)obj;
			if (msg.gmid != -100)
				GetKickoffPosition(msg.gmid);
			else
			{
				GameEntry.Event.Fire(this, new ShowMessageEventArgs("未获取到设备对应的击球点 将使用默认设置"));
				NetFsnState++;
				NetFSN();
			}
			//NetFsnState ++;
			//NetFSN();
		}

		//击球点
		void GetKickoffPosition(int gmid)
		{
			m2s_getkickoffposition msg = new m2s_getkickoffposition();
			msg.mid = GameEntry.GameData.ShopName.ToString();
			msg.gmid = gmid;
			M2SInfo m2sInfo = new M2SInfo(msg, GetKickoffPositionSuccess, MsgFailureLogin);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		void GetKickoffPositionSuccess(object obj)
		{
			m2c_getkickoffposition msg = (m2c_getkickoffposition)obj;
			if (msg.teePosX < -10)
				GameEntry.Event.Fire(this, new ShowMessageEventArgs("未获取到设备对应的击球点 将使用默认设置"));
			else
				GameEntry.GameData.teePosX = msg.teePosX;

			NetFsnState++;
			NetFSN();
		}





		/// <summary>
		/// 登录响应失败事件
		/// </summary>
		/// <param name="obj"></param>
		public void MsgFailureLogin(object obj)
		{
			//IMsgToClient msg = (IMsgToClient)obj;
			m_loginConcent.SetActive(false);
			GameEntry.Event.Fire(this, new ShowMessageEventArgs("网络异常"));
		}


		protected internal override void OnClose(object userData)
		{
			m_ProcedureLogin = null;

			base.OnClose(userData);
		}

	}
}