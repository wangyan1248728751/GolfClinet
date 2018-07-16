using Security;
using SkyTrakWrapper;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AboutScreenView : MonoBehaviour
{
	private const string NotConnectedText = "NOT CONNECTED";

	private const string NotRegisteredText = "NOT REGISTERED";

	[SerializeField]
	private Text softwareVersionLabel;

	[SerializeField]
	private Text unitSerialNumberLabel;

	[SerializeField]
	private Text firmwareVersionLabel;

	[SerializeField]
	private Text registeredToLabel;

	[SerializeField]
	private Button registerDeviceButton;

	//[SerializeField]
	//private DeviceRegistrationView _deviceRegistrationView;

	[SerializeField]
	private Animator _animator;


	private void OnDeviceConnected()
	{
		this.UpdateValues();
	}

	private void OnDisable()
	{
		SecurityWrapperService.Instance.OnConnected -= new Action(this.OnDeviceConnected);
		SecurityWrapperService.Instance.OnMMSUpdated -= new Action(this.UpdateValues);
	}

	private void OnEnable()
	{
		this.UpdateValues();
		SecurityWrapperService.Instance.OnConnected += new Action(this.OnDeviceConnected);
		SecurityWrapperService.Instance.OnMMSUpdated += new Action(this.UpdateValues);
	}

	public void OpenDeviceRegistration()
	{
		//this._deviceRegistrationView.Show(true);
	}

	public void SendCurrentLogs()
	{
		AppLog.SendToEmail(true);
	}

	public void SendPreviousLogs()
	{
		AppLog.SendToEmail(false);
	}

	public void Show(bool show)
	{
		if (!show)
		{
			this._animator.Play("Hide");
		}
		else
		{
			this._animator.Play("Show");
			this.UpdateValues();
		}
	}

	private void ShowRegisterButton(bool show)
	{
		//this.registerDeviceButton.gameObject.SetActive(show);
	}

	private void UpdateValues()
	{
		if (SecurityWrapperService.Instance == null)
		{
			return;
		}
		this.softwareVersionLabel.text = string.Format("{0} ({1})", SoftwareVersion.ShortVersion, SoftwareVersion.BuildNumber);
		this.unitSerialNumberLabel.text = (!SecurityWrapperService.Instance.IsConnected ? "NOT CONNECTED" : SecurityWrapperService.Instance.ESN);
		this.firmwareVersionLabel.text = (!SecurityWrapperService.Instance.IsConnected ? "NOT CONNECTED" : SecurityWrapperService.Instance.FWVersion);
		string str = "NOT CONNECTED";
		if (!SecurityWrapperService.Instance.IsConnected)
		{
			this.ShowRegisterButton(false);
		}
		else
		{
			if (!SecurityWrapperService.Instance.MMSData.Registered)
			{
				str = "NOT REGISTERED";
			}
			else
			{
				AppLog.Log(string.Format("First name {0}", SecurityWrapperService.Instance.MMSData.MemberFirstName), true);
				AppLog.Log(string.Format("Last name {0}", SecurityWrapperService.Instance.MMSData.MemberLastName), true);
				string str1 = string.Format("{0} {1}", SecurityWrapperService.Instance.MMSData.MemberFirstName, SecurityWrapperService.Instance.MMSData.MemberLastName);
				if (!SecurityWrapperService.Instance.MMSData.MemberFirstName.ToUpper().Equals("FFFFFFFFFFFF") && !SecurityWrapperService.Instance.MMSData.MemberLastName.ToUpper().Equals("FFFFFFFFFFFF"))
				{
					str = str1;
				}
			}
			//this.ShowRegisterButton(LoginManager.IsUserLoggedIn);
		}
		this.registeredToLabel.text = str;
	}
}