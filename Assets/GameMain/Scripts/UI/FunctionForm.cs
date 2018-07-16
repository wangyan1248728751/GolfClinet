using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using GameFramework;
using ZenFulcrum.EmbeddedBrowser;
using System;
using System.Net;
using Newtonsoft.Json.Linq;
using LitJson;

namespace Golf
{
    public class FunctionForm : UGuiForm
    {
        [SerializeField]
        GameObject threeD;
        [SerializeField]
        GameObject start;
        [SerializeField]
        GameObject closebtn;
        [SerializeField]
        GameObject info;
        [SerializeField]
        Text id;
        [SerializeField]
        Text username;
        //[SerializeField]
        //GameObject headImage;
        [SerializeField]
        GameObject quit;
        [SerializeField]
        Image image=null;
        [SerializeField]
        GameObject QRCodeimg;
        [SerializeField]
        Browser browser;
        [SerializeField]
        GameObject closeqr;
        [SerializeField]
        Text hour;
        [SerializeField]
        GameObject sub;
        [SerializeField]
        GameObject plus;
        [SerializeField]
        GameObject yesBtn;
        [SerializeField]
        Text confirmORchange;
        [SerializeField]
        GameObject gamemode;
        [SerializeField]
        GameObject friendMode;
        [SerializeField]
        GameObject onlineMode;
        [SerializeField]
        GameObject backF;

        Button Cstart;
        float passTime = 0;
        ProcedureFunction m_procedureLogin = null;

        int hours = 8;
        private void Awake()
        {
            Cstart = start.GetComponent<Button>();
            Cstart.onClick.AddListener(() => { begin(); });
        }
        
        protected internal override void OnOpen(object userData)
        {
            hours = 1;
            base.OnOpen(userData);
            Application.runInBackground = true;
            m_procedureLogin = (ProcedureFunction)userData;

            id.text= GameEntry.GameData.userData.id;
            username.text = GameEntry.GameData.WxName;
            threeD.AddClick(openQR);
            start.AddClick(begin);
            quit.AddClick(close);
            closeqr.AddClick(closeQR);
            plus.AddClick(addHours);
            sub.AddClick(remHours);
            yesBtn.AddClick(confirmNchange);
            friendMode.AddClick(battle);
            onlineMode.AddClick(startgame);
            backF.AddClick(backf);
        }
        bool isOpen=false;
        float time = 10;
        bool PaySucess=false;
        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            hour.text = hours.ToString();
            timer();
            passTime += elapseSeconds;
            if (passTime > 1)
            {
                passTime = 0;
                if (odd!=null)
                {
                    StartCoroutine(WeChatPay());
                    if (server=="PAID")
                    {
                        odd = null;
                        server = null;
                        openProcess();
                        isOpen = true;
                        closeQR();
                        //timeHour.Clear();
                    };
                }
            }
        }
        void timer()
        {
            if (isOpen)
            {

                if (time >= 1)
                {
                    time -= Time.deltaTime;
                }
                else
                {
                    KillProcess("CG3D");
                    isOpen = false;
                }

            }
        }
        void confirmNchange()
        {
            if (browser.gameObject.activeSelf==false)
            {
                browser.gameObject.SetActive(true);
                confirmORchange.text = "修改时间";
                plus.AddClick(donothing);
                sub.AddClick(donothing);
            }
            else
            {
                browser.gameObject.SetActive(false);
                confirmORchange.text = "确定";
                plus.AddClick(addHours);
                sub.AddClick(remHours);
            }
        }
        void donothing()
        {

        }
        void addHours()
        {

            if (hours<8)
            {
                hours++;
                LoadQRImage();
            }
        }
        void remHours()
        {
            if (hours>1)
            {
                hours--;
                LoadQRImage();
            }
        }
        IEnumerator LoadImage(string path)
        {
            WWW www = new WWW(path);
            yield return www;
            Texture2D texture = www.texture;
            Sprite sprites = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprites;
        }

        protected internal override void OnClose(object userData)
        {
            m_procedureLogin = null;
            gamemode.SetActive(false);
            base.OnClose(userData);
        }
        void begin()
        {
            gamemode.SetActive(true);
            //m_procedureLogin.Loginsucess = true;
        }
        void startgame()
        {
            GameEntry.GameData.onLine = true;
            m_procedureLogin.Loginsucess = true;
        }
        void battle()
        {
            GameEntry.GameData.onLine = false;
            m_procedureLogin.Loginsucess = true;
        }
        void backf()
        {
            gamemode.SetActive(false);
        }
        void close()
        {
            m_procedureLogin.quit = true;
        }
        string odd;
        string path;
        Dictionary<string, int> timeHour = new Dictionary<string, int>();
        string saveOdd;
        float saveHour=0;
        bool isClick=false;
        void LoadQRImage()
        {
            TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
            long t = (long)cha.TotalMilliseconds;
            odd = GameEntry.GameData.boxName + GameEntry.GameData.ShopName + t + hours;
            timeHour.Add(odd, hours);
            time = hours * 10;
            path = "http://47.100.175.248:8080/jlxpay-web/qrcode.jpg?bizOrderId=" + odd + "&amount="+hours;
            browser.Url = path;
        }

        void openQR()
        {
            QRCodeimg.gameObject.SetActive(true);
            LoadQRImage();
        }
        void closeQR()
        {
            QRCodeimg.gameObject.SetActive(false);
            browser.gameObject.SetActive(false);
            confirmORchange.text = "确定";
            plus.AddClick(addHours);
            sub.AddClick(remHours);
            hours = 1;
        }
        void openProcess()
        {
            Process.Start("C:\\ProgramData\\Creative Golf 3D for SkyTrak\\bin\\CG3D.exe") ;
        }
        void closeProcess()
        {

        }

        void KillProcess(string processName)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        if (process.ProcessName == processName)
                        {
                            process.Kill();
                            UnityEngine.Debug.Log("已杀死进程");
                        }
                    }
                }
                catch (System.InvalidOperationException)
                {
                    //UnityEngine.Debug.Log("Holy batman we've got an exception!");
                }
            }
        }

        string http;
        string server;
        //public IEnumerator RequestQuickLogin()
        //{
        //    if (timeHour.Keys != null)
        //    {
        //        foreach (var item in timeHour)
        //        {
        //            if (isClick)
        //            {
        //                http = "http://47.100.175.248:8080/jlxpay-web/wxPayment.json?bizOrderId=" + item.Key;
        //                WWW ret = new WWW(http);
        //                yield return ret;
        //                if (string.IsNullOrEmpty(ret.text))
        //                {
        //                    yield break;
        //                }
        //                JsonData jsdArray = JsonMapper.ToObject(ret.text);
        //                string msg = jsdArray["msg"].ToString();
        //                if (msg == "success")
        //                {
        //                    server = jsdArray["data"]["paymentStatus"].ToString();
        //                    if (server == "PAID")
        //                    {
        //                        time = item.Value * 10;
        //                        PaySucess = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        public IEnumerator WeChatPay()
        {
            http = "http://47.100.175.248:8080/jlxpay-web/wxPayment.json?bizOrderId=" + odd;
            WWW ret = new WWW(http);
            yield return ret;
            if (string.IsNullOrEmpty(ret.text))
            {
                yield break;
            }
            JsonData jsdArray = JsonMapper.ToObject(ret.text);
            string msg = jsdArray["msg"].ToString();
            if (msg == "success"&&odd!=null)
            {
                server = jsdArray["data"]["paymentStatus"].ToString();
            }
        }
    }
}
