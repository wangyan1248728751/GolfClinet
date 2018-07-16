using GameFramework;
using GameFramework.Event;
using LitJson;
using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Golf
{
	public class WebRequestToServerComponent : GameFrameworkComponent
    {
        private string GameServerUrl = "http://192.168.1.248:6072/";
        [SerializeField]
        private string GameLoginServerUrl = "http://192.168.1.248:6066/";
        [SerializeField]
        private string GameLoginServerIOSUrl = "http://192.168.1.248:6066/";
        [SerializeField]
        public string UpLoadHeadImageUrl = "http://192.168.1.248:8080/dlltest/FileServlet?dir=";
        [SerializeField]
        public string UpLoadHeadImageIOSUrl = "http://192.168.1.248:8080/dlltest/FileServlet?dir=";
        public string userid = "";
		public string auid = "";
		public string session = "";
        private string ausession = "";
        private string roleid = "";

        public bool closeConnectFormFailure;
        private bool gameOver = false;//游戏结束了，不发消息了。

		/// <summary>
		/// 设置游戏服务器
		/// </summary>
		/// <param name="url">服务器地址</param>
		public void SetGameServerUrl(string url)
        {
            GameServerUrl = url;
        }

        /// <summary>
        /// 设置用户ID和Session
        /// </summary>
        /// <param name="uid">用户ＩＤ</param>
        /// <param name="gamesessionid">游戏服务器标识码Session</param>
        /// <param name="loginsessionid">鉴权服务器识别码Session</param>
        /// <param name="rid">角色ID</param>
        public void SetUserIdAndSession(string uid,string gamesessionid,string loginsessionid, string rid)
        {
            if (uid!="")
            {
                userid = uid;
            }
            if (gamesessionid != "")
            {
                session = gamesessionid;
            }
            if (loginsessionid != "")
            {
                ausession = loginsessionid;
            }
            if (rid!="")
            {
                roleid = rid;
            }
            
        }
        /// <summary>
        /// 清除登录游戏服务的相关信息
        /// </summary>
        public void ClearLoginMess()
        {
            roleid = "";
            userid = "";
        }


        public void InitWebRequestToServerComponent()
        {
            GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
        }

        public string GetUpLoadHeadImageUrl()
        {
#if UNITY_IOS
            return UpLoadHeadImageIOSUrl.Trim();
#endif
#if UNITY_ANDROID
            return UpLoadHeadImageUrl.Trim();
#endif
            return UpLoadHeadImageUrl.Trim();
        }
        public string GetGameLoginServerUrl()
        {
#if UNITY_IOS
            return GameLoginServerIOSUrl.Trim();
#endif
#if UNITY_ANDROID
            return GameLoginServerUrl.Trim();
#endif
            return GameLoginServerUrl.Trim();
        }

        /// <summary>
        /// 发送消息到服务端
        /// </summary>
        /// <param name="m2s"></param>
        public void SendJsonMsg(M2SInfo m2s)
        {
            if (gameOver)
                return;
            m2s.msg.uid = userid;
            m2s.msg.rid = roleid;
            string url = GameServerUrl + m2s.msg.msgName;
            if (userid == "")//鉴权服务器
            {
                m2s.msg.session = ausession;
                url = GetGameLoginServerUrl() + m2s.msg.msgName;
            }
            else//游戏服务器
            {
                m2s.msg.session = session;
                url = GameServerUrl + m2s.msg.msgName;
            }

            //if (!m2s.ignoreConnectStatue)
            //{
            //    GameEntry.UI.OpenUIForm(UIFormId.ConnectForm);
            //}
           

            byte[] jsonDataPost = Utility.Json.ToJsonData(m2s.msg);
            GameEntry.WebRequest.AddWebRequest(url, jsonDataPost, m2s);
        }


        /// <summary>
        /// 接收服务端的返回消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            M2SInfo msgBase = ne.UserData as M2SInfo;
            if (msgBase == null)
            {
                return;
            }
            //if (!msgBase.ignoreConnectStatue)
            //{
            //    if (connectForm != null)
            //    {
            //        if (connectForm.isActiveAndEnabled)
            //        {
            //            connectForm.CloseConnectForm(true);
            //        }
            //        closeConnectFormFailure = false;
            //    }
            //    else
            //    {
            //        closeConnectFormFailure = true;
            //    }
            //}
           

            string responseJson = Utility.Converter.GetString(ne.GetWebResponseBytes());
			Debug.Log("服务器返回消息：\n" + responseJson);
			//JsonData data = JsonMapper.ToObject(responseJson);
			if (ne.UserData != null)
            {
                
                JsonData res = JsonMapper.ToObject(responseJson);
                //string resultCode = "0";
                //if (res.Inst_Object.Keys.Contains("respVo"))
                //    resultCode = res["respVo"]["resultCode"].ToString();
                //else if(res.Inst_Object.Keys.Contains("resultCode"))
                //    resultCode = res["resultCode"].ToString();
                //if (resultCode == "-8")
                //{
                //    Debug.Log("session error,please restart game!");
                //    gameOver = true;
                //    GameEntry.UI.OpenDialog(new DialogParams()
                //    {
                //        Mode = 1,
                //        Title = GameEntry.Localization.GetString("WebRequestToServer.SessionTip"),
                //        Message = GameEntry.Localization.GetString("WebRequestToServer.SessionError"),
                //        OnClickConfirm = delegate (object userData) { UnityGameFramework.Runtime.GameEntry.Shutdown(UnityGameFramework.Runtime.ShutdownType.Quit); },
                //    });
                //}
                msgBase.msg.MsgAnalysis(res, msgBase.SuccessAction);
                return;
            }
        }

        /// <summary>
        /// 响应失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (ne.UserData as M2SInfo == null)
            {
                return;
            }
            //if (!((M2SInfo)ne.UserData).ignoreConnectStatue)
            //{
            //    if (connectForm != null)
            //    {
            //        if (connectForm.isActiveAndEnabled)
            //        {
            //            if (!connectForm.CloseConnectForm(false, (M2SInfo)ne.UserData))
            //            {
            //                GameEntry.WebRequestToServerComponent.SendJsonMsg((M2SInfo)ne.UserData);
            //            }
            //        }
            //    }
            //}
            //GameEntry.UI.CloseUIForm(GameEntry.UI.GetUIForm(UIFormId.ConnectForm));
                 

            if (ne.UserData != null)
            {
                if (((M2SInfo)ne.UserData).FailureAction != null)
                {
                    ((M2SInfo)ne.UserData).FailureAction(ne.UserData);
                }
                return;
            }
        }

	}
}