using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.Video;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;
using System;
using System.Collections;
using LitJson;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Golf
{
    public class MenuForm : UGuiForm
    {
        //[SerializeField]
        //GameObject image;
        //[SerializeField]
        //GameObject friendMode;
        //[SerializeField]
        //GameObject onlineMode;
        //[SerializeField]
        //private GameObject m_QuitButton = null;

        private ProcedureMenu m_ProcedureMenu = null;

        public void OnStartButtonClick()
        {
           m_ProcedureMenu.StartGame();
        }
        public void startGame()
        {
            m_ProcedureMenu.StartGame();
        }
        public void battle()
        {
            GameEntry.GameData.onLine = false;
        }
        public void OnSettingButtonClick()
        {
            GameEntry.UI.OpenUIForm(UIFormId.SettingForm);
        }

        public void OnAboutButtonClick()
        {
            GameEntry.UI.OpenUIForm(UIFormId.AboutForm);
        }

        public void OnQuitButtonClick()
        {
            GameEntry.UI.OpenDialog(new DialogParams()
            {
                Mode = 2,
                Title = GameEntry.Localization.GetString("AskQuitGame.Title"),
                Message = GameEntry.Localization.GetString("AskQuitGame.Message"),
                OnClickConfirm = delegate (object userData) { UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
            });
        }
        
        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureMenu = (ProcedureMenu)userData;
            if (m_ProcedureMenu == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
            GameEntry.PromotionalVideo.StartTimer();
            //onlineMode.AddClick(startGame);
            //friendMode.AddClick(battle);
        }
       
        protected internal override void OnClose(object userData)
        {
            m_ProcedureMenu = null;
            GameEntry.PromotionalVideo.RemoveTimer();
            //image.SetActive(false);
            base.OnClose(userData);
        }

        public static implicit operator MenuForm(FunctionForm v)
        {
            throw new NotImplementedException();
        }
    }
}
