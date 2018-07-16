using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.Video;
using UnityEngine.UI;

namespace Golf
{
    public class oldLoginForm : UGuiForm
    {
        ProcedureLogin _ProcedureLogin = null;
        [SerializeField]
        GameObject RegisterDlg;
        [SerializeField]
        GameObject LoginDlg;

		private ProcedureLogin m_ProcedureLogin = null;

		protected internal override void OnOpen(object userData)
		{
			base.OnOpen(userData);

			GameEntry.UI.OpenUIForm(UIFormId.MessageForm);

			m_ProcedureLogin = (ProcedureLogin)userData;
			if (m_ProcedureLogin == null)
			{
				Log.Warning("ProcedureMenu is invalid when open LoginFrom.");
				return;
			}

		}



		/// <summary>
		/// 注册
		/// </summary>
		public void Register()
        {
            TestLogin();
        }

        /// <summary>
        /// 账号登陆
        /// </summary>
        public void Login()
        {
            TestLogin();
        }

        /// <summary>
        ///游客登录
        /// </summary>
        public void VisitorLogin()
        {
			m2s_logintourist msg = new m2s_logintourist();
			M2SInfo info = new M2SInfo(msg, success, fail);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(info);


		}

		public void success(object obj)
		{
			//m2c_logintourist data = (m2c_logintourist)obj;
			Debug.Log("success");
		}

		public void fail(object obj)
		{
			//m2c_logintourist data = (m2c_logintourist)obj;
			Debug.Log("fail");
		}











		public void TestLogin()
        {
			//m_ProcedureLogin.LoginSuccess = true;
			Close(true);
        }

        public void OpenDlg(GameObject _go)
        {
            _go.SetActive(true);
            GameEntry.PromotionalVideo.RemoveTimer();
        }

        public void CloseDlg(GameObject _go)
        {
            _go.SetActive(false);
            GameEntry.PromotionalVideo.StartTimer();
        }
    }

}
