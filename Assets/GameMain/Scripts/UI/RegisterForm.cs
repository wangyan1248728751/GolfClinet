using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using System;

namespace Golf

{
	public class RegisterForm : UGuiForm
	{

		[SerializeField]
		private InputField m_userNameField = null;
		[SerializeField]
		private InputField m_authCodeField = null;
		[SerializeField]
		private InputField m_passWordField = null;
		[SerializeField]
		private Text m_sendAuthText = null;
		[SerializeField]
		private GameObject m_AuthCodeRect = null;
		[SerializeField]
		private Text m_userNameText = null;
		private float m_currentTimerTime = 0f;
		private float m_allTimerTime = 60f;
		[SerializeField]
		private GameObject m_sendAuthCodeBtn = null;
		[SerializeField]
		private GameObject m_registerBtn = null;
		[SerializeField]
		private GameObject m_quitBtn = null;
		protected internal override void OnOpen(object userData)
		{
			base.OnOpen(userData);

			GameEntry.UI.OpenUIForm(UIFormId.MessageForm);

			m_userNameField.text = String.Empty;
			m_passWordField.text = String.Empty;
			#region Btn注册
			m_registerBtn.AddClick(OnRegisterButtonClick);
			m_quitBtn.AddClick(Close, false);
			m_sendAuthCodeBtn.AddClick(OnSendAuthCodeClick);
			#endregion
		}
		/// <summary>
		/// 发送手机验证码
		/// </summary>
		public void OnSendAuthCodeClick()
		{
#if UNITY_EDITOR
			if (m_currentTimerTime == 0)
			{
				m2s_addmessagesession msg = new m2s_addmessagesession();
				msg.name = m_userNameField.text.Trim();
				msg.messagetype = "1";
				M2SInfo m2sInfo = new M2SInfo(msg, MsgSuccessAuthCode, MsgFailure);
				GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
			}
#else
            if (m_currentTimerTime == 0)
           {
               if (m_userNameField.text.Trim().Length != 11)return;
               m2s_addmessagesession msg = new m2s_addmessagesession();
               msg.name = m_userNameField.text.Trim();
               msg.messagetype = "1";
               M2SInfo m2sInfo = new M2SInfo(msg, MsgSuccessAuthCode, MsgFailure);
               GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
           }
#endif
		}
		/// <summary>
		/// 获取验证码失败
		/// </summary>
		/// <param name="obj"></param>
		private void MsgFailure(object obj)
		{

		}
		/// <summary>
		/// 获取验证码成功
		/// </summary>
		/// <param name="obj"></param>
		private void MsgSuccessAuthCode(object obj)
		{
			m2c_addmessagesession msg = (m2c_addmessagesession)obj;
			if (msg.resultCode == "0")
			{
				Debug.Log("获取验证码成功！");
				StartCoroutine(Timer(m_allTimerTime));
			}
			else if (msg.resultCode == "-1")
			{
				GameEntry.UI.OpenDialog(new DialogParams()
				{
					Mode = 1,
					//Message = GameEntry.Localization.GetString("Register.SendFailure"),
					Message = msg.resultDesc,
				});
				Debug.Log(msg.resultDesc);
			}
		}

		/// <summary>
		/// 注册事件
		/// </summary>
		public void OnRegisterButtonClick()
		{

			if (m_userNameField == null || m_authCodeField == null || m_passWordField == null)
				return;
			if (m_userNameField.text == "" || m_authCodeField.text == "" || m_passWordField.text == "")
			{
				string concent = GameEntry.Localization.GetString("ToolTip.InputNoNull");
				
				GameEntry.Event.Fire(this, new ShowMessageEventArgs(concent));
				return;
			}
			if (m_userNameField.text.Trim().Length != 11)
			{
				string concent = GameEntry.Localization.GetString("ToolTip.UserError");
				GameEntry.Event.Fire(this, new ShowMessageEventArgs(concent));
				return;
			}
			if (m_passWordField.text.Trim().Length < 6)
			{
				string concent = GameEntry.Localization.GetString("ToolTip.PwdNum");
				GameEntry.Event.Fire(this, new ShowMessageEventArgs(concent));
				return;
			}
			///此处将注册的信息发送到服务器
			SendRegesitMessage();
		}
		/// <summary>
		/// 向服务器发送注册信息
		/// </summary>
		public void SendRegesitMessage()
		{
			m2s_regist msg = new m2s_regist();
			msg.name = m_userNameField.text.Trim();
			msg.pwd = m_passWordField.text.Trim();
			msg.message = m_authCodeField.text.Trim();

			msg.gid = "1";
			M2SInfo m2sInfo = new M2SInfo(msg, MsgSuccessLogin, MsgFailureLogin);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}
		/// <summary>
		/// 向服务器发送注册信息成功事件
		/// </summary>
		/// <param name="obj"></param>
		public void MsgSuccessLogin(object obj)
		{
			m2c_regist msg = (m2c_regist)obj;
			if (msg.resultCode == "0")
			{
				GameEntry.Event.Fire(this, new ShowMessageEventArgs("注册成功！"));
				Close();
			}
			if (msg.resultCode == "-1")
			{
				GameEntry.Event.Fire(this, new ShowMessageEventArgs(msg.resultDesc));
			}
		}
		/// <summary>
		/// 向服务器发送注册信息失败事件
		/// </summary>
		/// <param name="obj"></param>
		public void MsgFailureLogin(object obj)
		{
			Debug.Log("注册失败");
		}
		/// <summary>
		/// 计时器
		/// </summary>
		/// <param name="time">倒计时总时间</param>
		/// <returns></returns>
		private IEnumerator Timer(float time)
		{
			while (true)
			{
				yield return new WaitForSeconds(1f);
				time -= 1;
				if (time == 0)
				{
					m_currentTimerTime = 0;
					m_sendAuthText.text = GameEntry.Localization.GetString("ToolTip.SendAuthCode");
					break;
				}
				m_currentTimerTime = time;
				m_sendAuthText.text = m_currentTimerTime.ToString() + "s";
			}
		}

	}
}