using Security;
using SkyTrakWrapper;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SkyTrak.NewDesign
{
	public class DeviceConnectionView : MonoBehaviour
	{
		private const string DontShowOverviewScreen = "DontShowOverviewScreen";

		[SerializeField]
		private GameObject _connectionOverviewScreen;

		[SerializeField]
		private GameObject _mainScreen;

		[SerializeField]
		private GameObject _directConnectionScreen;

		[SerializeField]
		private GameObject _networkConnectionScreen;

		[SerializeField]
		private GameObject _networkSetupWizardScreen;

		[SerializeField]
		private GameObject _usbModeConnectionScreen;

		[SerializeField]
		private SelectNetworkPopupView _selectNetworkPopupView;

		[SerializeField]
		private Toggle _dontShowAgainToggle;

		[SerializeField]
		private Button _proceedToNetworkModeButton;

		[SerializeField]
		private Button _networkSetupWizardButton;

		[SerializeField]
		private Button _scanButton;

		[SerializeField]
		private Button _connectButton;

		[SerializeField]
		private InputField _networkPasswordInput;

		[SerializeField]
		private Text _networkNamePlaceholder;

		[SerializeField]
		private Text _networkNameLabel;

		private float _lastCheckTime;

		private const float CheckConnectionInterval = 1f;

		private bool _checkSuccessConnect;

		private bool _checkDirectConnection;

		private bool _isUnitConnected;

		private bool _isUnitInDirectMode;

		private bool _isUnitInNetworkMode;

		private bool _isUnitInUSBMode;

		private const string SSIDKey = "SSID";

		private const string SecurityTypeKey = "SecurityType";

		private const string SignalLevelKey = "SignalLevel";

		private const string IsSupportedKey = "IsSupported";

		private const string PasswordKey = "Password";

		private const string SavedNetworksKey = "SavedNetworks";

		private JSONObject _savedNetworks;

		private List<RIPENetworkScanListParamsType> _scannedNetworks;

		private ManualNetworkParams _selectedNetwork;

		private void AddNewNetwork()
		{
			JSONObject jSONObject = new JSONObject();
			jSONObject.AddField("SSID", this._selectedNetwork.NetParams.SSID);
			jSONObject.AddField("SecurityType", this._selectedNetwork.NetParams.securityType);
			jSONObject.AddField("SignalLevel", this._selectedNetwork.NetParams.signalLevel);
			jSONObject.AddField("IsSupported", this._selectedNetwork.NetParams.isSupported > 0);
			jSONObject.AddField("Password", this._networkPasswordInput.text);
			this._savedNetworks.Add(jSONObject);
		}

		private void Awake()
		{
			this.LoadSavedNetworks();
			this._selectNetworkPopupView.OnNetworkSelected = new Action<ManualNetworkParams>(this.OnNetworkSelected);
			this._selectNetworkPopupView.OnScanCanceled = new Action(this.OnScanCanceled);
			this.OnShowPasswordCharactersChange(false);
		}

		private void FillNetworkListBySavedNetworks()
		{
			object obj;
			if (this._savedNetworks != null && this._savedNetworks.Count > 0)
			{
				List<RIPENetworkScanListParamsType> rIPENetworkScanListParamsTypes = new List<RIPENetworkScanListParamsType>();
				foreach (JSONObject _savedNetwork in this._savedNetworks.list)
				{
					RIPENetworkScanListParamsType rIPENetworkScanListParamsType = new RIPENetworkScanListParamsType()
					{
						SSID = _savedNetwork["SSID"].str,
						securityType = _savedNetwork["SecurityType"].str,
						signalLevel = _savedNetwork["SignalLevel"].f
					};
					if (!_savedNetwork["IsSupported"].b)
					{
						obj = null;
					}
					else
					{
						obj = 1;
					}
					rIPENetworkScanListParamsType.isSupported = (byte)obj;
					rIPENetworkScanListParamsTypes.Add(rIPENetworkScanListParamsType);
				}
				this._selectNetworkPopupView.SetNetworkList(rIPENetworkScanListParamsTypes);
			}
		}

		private void ForceDirectMode()
		{
			AppLog.Log("Force Direct Mode - Set IsInAPMode to 1", true);
			SecurityWrapperService.Instance.ForceDirectConnect();
		}

		private string GetSavedNetworkPassword(string ssid)
		{
			string item;
			if (this._savedNetworks != null && this._savedNetworks.Count > 0)
			{
				List<JSONObject>.Enumerator enumerator = this._savedNetworks.list.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						JSONObject current = enumerator.Current;
						if (!current["SSID"].str.Equals(ssid))
						{
							continue;
						}
						item = current["Password"].str;
						return item;
					}
					return string.Empty;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return item;
			}
			return string.Empty;
		}

		public void Hide()
		{
			this.HideAllScreens();
			base.gameObject.SetActive(false);
		}

		private void HideAllScreens()
		{
			this._connectionOverviewScreen.SetActive(false);
			this._mainScreen.SetActive(false);
			this._directConnectionScreen.SetActive(false);
			this._networkConnectionScreen.SetActive(false);
			this._networkSetupWizardScreen.SetActive(false);
			this._usbModeConnectionScreen.SetActive(false);
		}

		private void LoadSavedNetworks()
		{
			if (!PlayerPrefs.HasKey("SavedNetworks"))
			{
				this._savedNetworks = new JSONObject();
			}
			else
			{
				string str = PlayerPrefs.GetString("SavedNetworks");
				this._savedNetworks = new JSONObject(str, -2, false, false);
			}
		}

		public void OnConnectButtonClick()
		{
			if (string.IsNullOrEmpty(this._networkNameLabel.text))
			{
				return;
			}
			string str = this._networkPasswordInput.text;
			if (!this._selectedNetwork.NetParams.securityType.Contains("OPEN") && string.IsNullOrEmpty(str))
			{
				return;
			}
			RIPENetworkConfigType rIPENetworkConfigType = new RIPENetworkConfigType()
			{
				nodeToConfig = 1,
				password = str,
				securityType = this._selectedNetwork.NetParams.securityType,
				SSID = this._selectedNetwork.NetParams.SSID
			};
			SecurityWrapperService.Instance.BoxSetNetworkConfig(rIPENetworkConfigType);
			this._checkSuccessConnect = true;
		}

		public void OnDirectModeSelectClick()
		{
			this.HideAllScreens();
			this._directConnectionScreen.SetActive(true);
			this._checkDirectConnection = true;
		}

		public void OnNetworkModeSelectClick()
		{
			if (!this._isUnitConnected || this._isUnitInUSBMode)
			{
				this.OnDirectModeSelectClick();
			}
			else
			{
				this.OnProceedToNetworkModeClick();
			}
		}

		private void OnNetworkScanCompleted(List<RIPENetworkScanListParamsType> ripeNetworkScanListParamsTypes)
		{
			SecurityWrapperService.Instance.OnNetworkScanCompleted -= new Action<List<RIPENetworkScanListParamsType>>(this.OnNetworkScanCompleted);
			SecurityWrapperService.Instance.OnDisconnected -= new Action(this.OnScanCanceled);
			this._scannedNetworks = ripeNetworkScanListParamsTypes;
			this._selectNetworkPopupView.SetNetworkList(this._scannedNetworks);
		}

		private void OnNetworkSelected(ManualNetworkParams manualNetworkParams)
		{
			this._selectedNetwork = manualNetworkParams;
			this._networkNameLabel.text = this._selectedNetwork.NetParams.SSID;
			this._networkNameLabel.gameObject.SetActive(true);
			this._networkNamePlaceholder.gameObject.SetActive(false);
			if (!string.IsNullOrEmpty(manualNetworkParams.Password))
			{
				this._networkPasswordInput.text = manualNetworkParams.Password;
			}
			else
			{
				this._networkPasswordInput.text = this.GetSavedNetworkPassword(this._selectedNetwork.NetParams.SSID);
			}
			this._networkPasswordInput.GetComponent<Selectable>().Select();
		}

		public void OnNetworkSetupWizardClick()
		{
			this.FillNetworkListBySavedNetworks();
		}

		public void OnOverviewNextButtonClick()
		{
			if (this._dontShowAgainToggle.isOn)
			{
				PlayerPrefs.SetInt("DontShowOverviewScreen", 1);
			}
			this.HideAllScreens();
			this._mainScreen.SetActive(true);
		}

		public void OnProceedToNetworkModeClick()
		{
			this.HideAllScreens();
			this._networkConnectionScreen.SetActive(true);
		}

		public void OnScanButtonClick()
		{
			this._selectNetworkPopupView.ShowScanNetwork();
			SecurityWrapperService.Instance.OnNetworkScanCompleted += new Action<List<RIPENetworkScanListParamsType>>(this.OnNetworkScanCompleted);
			SecurityWrapperService.Instance.OnDisconnected += new Action(this.OnScanCanceled);
			try
			{
				SecurityWrapperService.Instance.ScanWiFiNetworks();
			}
			catch (Exception exception)
			{
				SecurityWrapperService.Instance.OnNetworkScanCompleted -= new Action<List<RIPENetworkScanListParamsType>>(this.OnNetworkScanCompleted);
				SecurityWrapperService.Instance.OnDisconnected -= new Action(this.OnScanCanceled);
				this._selectNetworkPopupView.Show(false);
			}
		}

		private void OnScanCanceled()
		{
			SecurityWrapperService.Instance.OnNetworkScanCompleted -= new Action<List<RIPENetworkScanListParamsType>>(this.OnNetworkScanCompleted);
			SecurityWrapperService.Instance.OnDisconnected -= new Action(this.OnScanCanceled);
			this._selectNetworkPopupView.SetNetworkList(null);
		}

		public void OnShowPasswordCharactersChange(bool val)
		{
			this._networkPasswordInput.contentType = (!val ? InputField.ContentType.Password : InputField.ContentType.Standard);
			this._networkPasswordInput.ForceLabelUpdate();
		}

		public void OnUsbModeSelectClick()
		{
			this.HideAllScreens();
			this._usbModeConnectionScreen.SetActive(true);
		}

		private void SaveLastSelectedNetwork()
		{
			bool flag = false;
			if (this._savedNetworks.Count > 0)
			{
				foreach (JSONObject _savedNetwork in this._savedNetworks.list)
				{
					if (!_savedNetwork["SSID"].str.Equals(this._selectedNetwork.NetParams.SSID))
					{
						continue;
					}
					this.UpdateSavedNetwork(_savedNetwork);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.AddNewNetwork();
			}
			PlayerPrefs.SetString("SavedNetworks", this._savedNetworks.ToString());
		}

		public void Show()
		{
			if (PlayerPrefs.GetInt("DontShowOverviewScreen", 0) != 1)
			{
				this._connectionOverviewScreen.SetActive(true);
			}
			else
			{
				this._mainScreen.SetActive(true);
			}
			base.gameObject.SetActive(true);
		}

		private void Update()
		{
			bool flag;
			bool flag1;
			if (SecurityWrapperService.Instance != null && Time.time - this._lastCheckTime > 1f)
			{
				this._isUnitConnected = (!SecurityWrapperService.Instance.IsConnected ? false : SecurityWrapperService.Instance.IsLicenseChecked);
				this._isUnitInDirectMode = SecurityWrapperService.Instance.ConnectionType == RIPEBoxConnectionType.RIPE_BOX_CONNECTION_DIRECT_MODE;
				this._isUnitInNetworkMode = SecurityWrapperService.Instance.ConnectionType == RIPEBoxConnectionType.RIPE_BOX_CONNECTION_NETWORK_MODE;
				this._isUnitInUSBMode = SecurityWrapperService.Instance.ConnectionType == RIPEBoxConnectionType.RIPE_BOX_CONNECTION_USB_MODE;
				Button button = this._scanButton;
				if (!this._isUnitConnected)
				{
					flag = false;
				}
				else
				{
					flag = (this._isUnitInDirectMode ? true : this._isUnitInNetworkMode);
				}
				button.interactable = flag;
				this._connectButton.interactable = (!this._isUnitConnected ? false : !string.IsNullOrEmpty(this._selectedNetwork.NetParams.SSID));
				this._proceedToNetworkModeButton.interactable = (!this._isUnitConnected ? false : this._isUnitInDirectMode);
				Button button1 = this._networkSetupWizardButton;
				if (!this._isUnitConnected)
				{
					flag1 = false;
				}
				else
				{
					flag1 = (this._isUnitInDirectMode ? true : this._isUnitInNetworkMode);
				}
				button1.interactable = flag1;
				if (this._checkDirectConnection && this._isUnitConnected)
				{
					if (this._isUnitInNetworkMode)
					{
						this.ForceDirectMode();
					}
					this._checkDirectConnection = false;
				}
				if (this._checkSuccessConnect && this._isUnitConnected)
				{
					if (this._isUnitInNetworkMode)
					{
						this.SaveLastSelectedNetwork();
					}
					this._checkSuccessConnect = false;
				}
				this._lastCheckTime = Time.time;
			}
		}

		private void UpdateSavedNetwork(JSONObject savedNetworkJson)
		{
			savedNetworkJson.SetField("SSID", JSONObject.CreateStringObject(this._selectedNetwork.NetParams.SSID));
			savedNetworkJson.SetField("SecurityType", JSONObject.CreateStringObject(this._selectedNetwork.NetParams.securityType));
			savedNetworkJson.SetField("SignalLevel", this._selectedNetwork.NetParams.signalLevel);
			savedNetworkJson.SetField("IsSupported", this._selectedNetwork.NetParams.isSupported > 0);
			savedNetworkJson.SetField("Password", JSONObject.CreateStringObject(this._networkPasswordInput.text));
		}
	}
}