using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

namespace Golf
{
    public class QRCodeLoginForm : UGuiForm, iLogin
    {
        [SerializeField]
        Image QRImage;
        [SerializeField]
        UnityWebView webView;
        [SerializeField]
        GameObject exitBtn;
        [SerializeField]
        GameObject tips;
        [SerializeField]
        Browser browser;

        private ProcedureLogin m_ProcedureLogin = null;

        string QRUrl;
        int MachineId;
        float passTime = 0;
        bool needLogout = true;
        bool login = false;

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_ProcedureLogin = (ProcedureLogin)userData;
            QRUrl = null;
            MachineId = 0;
            passTime = 0;
            login = false;
            needLogout = true;
            //webView.gameObject.SetActive(true);
            browser.gameObject.SetActive(true);
            tips.SetActive(false);

            exitBtn.AddClick(exitBtnOnClick);

            NetFsnState = 0;
            NetFSN();
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            //轮询查询二维码登陆情况
            passTime += elapseSeconds;
            if (passTime > 1 && !login)
            {
                passTime = 0;
                CheckQRCodeState();
            }
        }

        void exitBtnOnClick()
        {
            m_ProcedureLogin.LoginExit = true;
        }

        public void CloseForm()
        {
            Close(true);
        }


        public int NetFsnState = 0;
        public void NetFSN()
        {
            switch (NetFsnState)
            {
                case 0:
                    //根据设备号 请求二维码
                    GetQRCodeUrl();
                    break;
                case 1:
                    //显示二维码
                    LoadQRImage(QRUrl);
                    break;
                case 2:
                    //获取玩家信息
                    GetUserInfo();
                    break;
                case 3:
                    //获取高尔夫信息
                    GetServerRewardMap();
                    break;
                case 4:
                    //设备信息
                    GetGolfMachine();
                    break;
                case 5:
                    //在线模式进入游戏
                    GameEntry.GameData.onLine = true;
                    m_ProcedureLogin.enter = true;
                    break;
                case 6:
                    //离线模式进入游戏
                    GameEntry.GameData.onLine = false;
                    m_ProcedureLogin.enter = true;
                    break;
            }
        }

        void Logout(object obj)
        {
            m2c_fortencentrequest m2c = (m2c_fortencentrequest)obj;

            m2s_deltencentrequest msg = new m2s_deltencentrequest();
            if (MachineId == 0)
                return;
            msg.mid = MachineId;
            M2SInfo m2sInfo = new M2SInfo(msg, LogoutSuccess, MsgFailureLogin);
            GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
        }

        void LogoutSuccess(object obj)
        {
            Debug.Log("注销成功");
            needLogout = false;
        }

        void GetQRCodeUrl()
        {
            m2s_getgolfmachine msg = new m2s_getgolfmachine();
            msg.mid = GameEntry.GameData.ShopName.ToString();
            msg.name = GameEntry.GameData.boxName;
            M2SInfo m2sInfo = new M2SInfo(msg, GetQRCodeUrlSuccess, MsgFailureLogin);
            
            byte[] jsonDataPost = Utility.Json.ToJsonData(m2sInfo.msg);
            GameEntry.WebRequest.AddWebRequest("http://47.100.175.248:6077/" + msg.msgName, jsonDataPost,m2sInfo);

        }
        
        
        void GetQRCodeUrlSuccess(object obj)
        {
            m2c_getgolfmachine msg = (m2c_getgolfmachine)obj;
            if (!string.IsNullOrEmpty(msg.callback))
            {
                MachineId = msg.gmid;
                QRUrl = string.Format("https://open.weixin.qq.com/connect/qrconnect?appid=wxc2290f256563ff64&redirect_uri=http%3A%2F%2Fwww.golfgalaxytech.cn%3A8080%2Fwsx.tools%2F{0}&response_type=code&scope=snsapi_login&state=STATE#wechat_redirect", msg.callback);
                Debug.Log("请求地址成功");
                NetFsnState++;
                NetFSN();
            }
            else
            {
                Debug.Log("fail");
                GameEntry.Event.Fire(this, new ShowMessageEventArgs("无法获取设备号对应二维码!"));
                //webView.gameObject.SetActive(false);
                browser.gameObject.SetActive(false);
                tips.SetActive(true);
                //NetFsnState = 0;
                //NetFSN();
            }
        }

        void CheckQRCodeState()
        {
            m2s_fortencentrequest msg = new m2s_fortencentrequest();
            if (MachineId == 0) return;
            msg.mid = MachineId;
            M2SInfo m2sInfo;
            if (needLogout)
            {
                m2sInfo = new M2SInfo(msg, Logout, MsgFailureLogin);
            }
            else
                m2sInfo = new M2SInfo(msg, QRCodeLoginSuccess, MsgFailureLogin);

            GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
        }

        void QRCodeLoginSuccess(object obj)
        {
            m2c_fortencentrequest msg = (m2c_fortencentrequest)obj;
            if (msg.resultCode == "0")
            {
                GameEntry.WebRequestToServerComponent.SetUserIdAndSession(msg.uid, msg.session, "", "");
                GameEntry.WebRequestToServerComponent.auid = msg.auid;
                GameEntry.WebRequestToServerComponent.SetGameServerUrl("http://47.100.175.248:6077/");
                GameEntry.GameData.WxName = msg.WxName;
                GameEntry.GameData.WxAvator = msg.WxAvator;

                login = true;
                NetFsnState++;
                NetFSN();
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="auid"></param>
        void GetUserInfo()
        {
            m2s_getuserinfo msg = new m2s_getuserinfo();
            msg.auid = GameEntry.WebRequestToServerComponent.auid;
            msg.session = GameEntry.WebRequestToServerComponent.session;
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
            GameEntry.Event.Fire(this, new ShowMessageEventArgs("网络异常"));
        }


        void LoadQRImage(string path)
        {
            Debug.Log("path:" + path);
            //webView.gameObject.SetActive(true);
            browser.gameObject.SetActive(true);
            tips.SetActive(false);
            //webView.LoadURL(path);
            browser.Url = path;
            //Application.OpenURL(path);
        }
    }
}