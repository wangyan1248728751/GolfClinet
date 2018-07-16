using Data;
using Security;
using SkyTrak.NewDesign;
using SkyTrakWrapper;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeviceRegistrationView : MonoBehaviour
{
	private const string DeviceAlreadyRegistered = "Your device already registered";

	private const string DeviceSuccessRegistered = "Your device is now registered";

	private const string StartRegistrationError = "Device is not connected or you are offline";

	private Color _greenTextColor = new Color(0.539f, 0.7656f, 0f, 1f);

	//[SerializeField]
	//private GameObject _panelObj;

	//[SerializeField]
	//private GameObject _onlinePanelContent;

	//[SerializeField]
	//private GameObject _offlinePanelContent;

	//[SerializeField]
	//private GameObject _spinningObject;

	//[SerializeField]
	//private GameObject _doneCheckmarkObject;

	//[SerializeField]
	//private Button _registerDeviceButton;

	//[SerializeField]
	//private Button _goToNetworkModeButton;

	//[SerializeField]
	//private Text _registerResultLabel;

	//[SerializeField]
	//private Text _deviceCodeLabel;

	//[SerializeField]
	//private DeviceConnectionView _deviceConnectionView;

	//public DeviceRegistrationView()
	//{
	//}

	public void CheckAlreadyRegistered()
	{
		Debug.Log("regi:" + SecurityWrapperService.Instance.ConnectionType);
		//this.ResetScreen();
		if (SecurityWrapperService.Instance != null && SecurityWrapperService.Instance.IsConnected && SecurityWrapperService.Instance.MMSData.Registered)
		{
			
			//this._registerDeviceButton.interactable = false;
			//this._doneCheckmarkObject.SetActive(true);
			//this._registerResultLabel.text = "Your device already registered";
			//this._registerResultLabel.color = this._greenTextColor;
			//this._registerResultLabel.gameObject.SetActive(true);
		}
	}

	private void OnDeviceConnected()
	{
		this.UpdateDeviceCode();
		//this._registerDeviceButton.interactable = true;
	}

	private void OnDeviceDisconnected()
	{
		this.UpdateDeviceCode();
		//this._registerDeviceButton.interactable = false;
	}

	private void OnDisable()
	{
		SecurityWrapperService.Instance.OnConnected -= new Action(this.OnDeviceConnected);
		SecurityWrapperService.Instance.OnDisconnected -= new Action(this.OnDeviceDisconnected);
	}

	private void OnEnable()
	{
		SecurityWrapperService.Instance.OnConnected += new Action(this.OnDeviceConnected);
		SecurityWrapperService.Instance.OnDisconnected += new Action(this.OnDeviceDisconnected);
		this.UpdateDeviceCode();
	}

	//private void OnGoToNetworkModeClick()
	//{
	//	this.Show(false);
	//	if (this._deviceConnectionView != null)
	//	{
	//		this._deviceConnectionView.Show();
	//	}
	//}

	//private void OnRegisterDeviceDone()
	//{
	//	this._doneCheckmarkObject.SetActive(true);
	//	this._spinningObject.SetActive(false);
	//	this._registerResultLabel.text = "Your device is now registered";
	//	this._registerResultLabel.color = this._greenTextColor;
	//	this._registerResultLabel.gameObject.SetActive(true);
	//	SecurityWrapperService.Instance.UpdateMMS();
	//}

	//private void OnRegisterDeviceError(string error)
	//{
	//	this._registerResultLabel.text = error;
	//	this._registerResultLabel.color = Color.red;
	//	this._registerResultLabel.gameObject.SetActive(true);
	//	this._doneCheckmarkObject.SetActive(false);
	//	this._spinningObject.SetActive(false);
	//	this._registerDeviceButton.interactable = true;
	//}

	//public void OnStartRegistrationClick()
	//{
	//	if (!SecurityWrapperService.Instance.IsConnected || NetworkInternetVerification.instance.VerifyInternetAvailability() != NetworkInternetVerification.NET_STATES.VERIFIED_ONLINE)
	//	{
	//		this.OnRegisterDeviceError("Device is not connected or you are offline");
	//		return;
	//	}
	//	this.ResetScreen();
	//	this._registerDeviceButton.interactable = false;
	//	string eSN = SecurityWrapperService.Instance.ESN;
	//	int id = LoginManager.UserData.Id;
	//	//DeviceRegistration.RegisterSkytrack(eSN, id.ToString(), new Action(this.OnRegisterDeviceDone), new Action<string>(this.OnRegisterDeviceError));
	//	this._spinningObject.SetActive(true);
	//}

	//private void ResetScreen()
	//{
	//	this._spinningObject.SetActive(false);
	//	this._doneCheckmarkObject.SetActive(false);
	//	this._registerResultLabel.gameObject.SetActive(false);
	//	this._registerDeviceButton.interactable = true;
	//}

	//public void Show(bool show)
	//{
	//	this._panelObj.SetActive(show);
	//	if (!show)
	//	{
	//		return;
	//	}
	//	bool flag = (!SecurityWrapperService.Instance.IsConnected ? true : SecurityWrapperService.Instance.ConnectionType == RIPEBoxConnectionType.RIPE_BOX_CONNECTION_DIRECT_MODE);
	//	if (SecurityWrapperService.Instance.IsConnected && SecurityWrapperService.Instance.MMSData.Registered)
	//	{
	//		flag = false;
	//	}
	//	this._offlinePanelContent.SetActive(flag);
	//	this._onlinePanelContent.SetActive(!flag);
	//	this.CheckAlreadyRegistered();
	//}

	//private void Start()
	//{
	//	this._goToNetworkModeButton.onClick.AddListener(new UnityAction(this.OnGoToNetworkModeClick));
	//}

	private void UpdateDeviceCode()
	{
		if (SecurityWrapperService.Instance == null || !SecurityWrapperService.Instance.IsConnected)
		{
			//this._deviceCodeLabel.text = string.Empty;
			return;
		}
		string eSN = SecurityWrapperService.Instance.ESN;
		//this._deviceCodeLabel.text = eSN;
	}
}