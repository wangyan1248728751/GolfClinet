using SkyTrakWrapper;
using SkyTrakWrapper.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Security
{
	public class SecurityWrapperService : ISkyTrakSW
	{
		public const string NoESN = "NOESN";

		public const string SWCacheFilename = "cache.sqlite";

		private const int FrameRate = 60;

		private string _defaultMMSData;

		private string _persistentDataPath;

		protected SkyTrakWrapper.Interfaces.ILogger _logger;

		private bool _configWasChanged;

		private bool _isFWUpgradeAvailable;

		private bool _saveBoxParamsToLog;

		private float _licenseCheckTime = 3f;

		private RIPEBoxParamsType _boxParams;

		private List<RIPECommBoxDataParamsType> _boxList = new List<RIPECommBoxDataParamsType>();

		private SkyTrakSW.MMSData _mmsDataCopy;

		private bool _needToUpdateMMS;

		private static ISkyTrakSW _instance;

		private SkyTrakSW _wrapper;

		private float _licenseCheck;

		private bool _isLicenseChecking;

		private bool _isLicenseChecked;

		private Action<RIPESpeedParamsType, RIPESpinParamsType, RIPEFlightParamsType> OnBallLaunchedAction;

		private Action OnDisconnectedAction;

		private Action<List<RIPENetworkScanListParamsType>> OnNetworkScanCompletedAction;

		public static ISkyTrakSW Instance
		{
			get
			{
				return SecurityWrapperService._instance;
			}
		}

		bool Security.ISkyTrakSW.BatteryIsCharging
		{
			get
			{
				return this._boxParams.chargingState == RIPEBoxChargingStateType.RIPE_BATT_CHARGING;
			}
		}

		float Security.ISkyTrakSW.BoxBatteryPercent
		{
			get
			{
				return this._boxParams.batteryPercent;
			}
		}

		IEnumerable<RIPECommBoxDataParamsType> Security.ISkyTrakSW.BoxList
		{
			get
			{
				return this._boxList;
			}
		}

		float Security.ISkyTrakSW.BoxRoll
		{
			get
			{
				return this._boxParams.boxRoll;
			}
		}

		float Security.ISkyTrakSW.BoxTilt
		{
			get
			{
				return this._boxParams.boxTilt;
			}
		}

		RIPEBoxConnectionType Security.ISkyTrakSW.ConnectionType
		{
			get
			{
				RIPEBoxConnectionType connectedBoxConnectionType;
				if (this._wrapper != null)
				{
					try
					{
						connectedBoxConnectionType = this._wrapper.ConnectedBoxConnectionType;
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						AppLog.LogError(string.Concat("Exception during get connection type: ", exception.Message), true);
						AppLog.LogException(exception, true);
						return RIPEBoxConnectionType.RIPE_BOX_CONNECTION_UNKNOWN;
					}
					return connectedBoxConnectionType;
				}
				return RIPEBoxConnectionType.RIPE_BOX_CONNECTION_UNKNOWN;
			}
		}

		string Security.ISkyTrakSW.ESN
		{
			get
			{
				string eSN;
				if (this._mmsDataCopy == null)
				{
					return string.Empty;
				}
				try
				{
					eSN = this._mmsDataCopy.ESN;
				}
				catch (Exception exception)
				{
					eSN = string.Empty;
				}
				return eSN;
			}
		}

		string Security.ISkyTrakSW.FWVersion
		{
			get
			{
				string str;
				if (this._wrapper == null)
				{
					return "NONE";
				}
				try
				{
					float connectedBoxFWVersion = this._wrapper.ConnectedBoxFWVersion.majorVer;
					RIPEVersion rIPEVersion = this._wrapper.ConnectedBoxFWVersion;
					str = string.Format("{0:N3}", connectedBoxFWVersion + rIPEVersion.minorVer);
				}
				catch (Exception exception)
				{
					str = "NONE";
				}
				return str;
			}
		}

		bool Security.ISkyTrakSW.IsBoxForcedInAPMode
		{
			get
			{
				return this._boxParams.isBoxInAPMode == 1;
			}
		}

		bool Security.ISkyTrakSW.IsConnected
		{
			get
			{
				bool flag;
				if (this._wrapper == null)
				{
					return false;
				}
				try
				{
					flag = (!this._wrapper.IsConnected ? false : !this._configWasChanged);
				}
				catch (Exception exception)
				{
					flag = false;
				}
				return flag;
			}
		}

		bool Security.ISkyTrakSW.IsDiscovering
		{
			get
			{
				if (this._wrapper == null)
				{
					return false;
				}
				return this._wrapper.IsDiscovering;
			}
		}

		bool Security.ISkyTrakSW.IsLicenseChecked
		{
			get
			{
				if (this._wrapper == null)
				{
					return false;
				}
				return (!this._wrapper.IsConnected || !this._isLicenseChecked ? false : !this._configWasChanged);
			}
		}

		bool Security.ISkyTrakSW.IsLicenseChecking
		{
			get
			{
				bool flag;
				if (this._wrapper == null)
				{
					return false;
				}
				if (this._wrapper.IsConnecting || this._wrapper.IsLicenseChecking)
				{
					flag = true;
				}
				else
				{
					flag = (!this._wrapper.IsConnected ? false : !this._isLicenseChecked);
				}
				return flag;
			}
		}

		bool Security.ISkyTrakSW.IsMMSReady
		{
			get
			{
				bool flag;
				if (this._wrapper == null)
				{
					return false;
				}
				try
				{
					flag = this._wrapper.isMMSDataReady;
				}
				catch (Exception exception)
				{
					flag = false;
				}
				return flag;
			}
		}

		SkyTrakSW.MMSData Security.ISkyTrakSW.MMSData
		{
			get
			{
				if (this._mmsDataCopy == null)
				{
					return null;
				}
				return this._mmsDataCopy;
			}
		}

		int Security.ISkyTrakSW.TotalShotsTaken
		{
			get
			{
				int connectedBoxTotalShotsTaken;
				if (this._wrapper == null)
				{
					return 0;
				}
				try
				{
					connectedBoxTotalShotsTaken = this._wrapper.ConnectedBoxTotalShotsTaken;
				}
				catch (Exception exception)
				{
					connectedBoxTotalShotsTaken = 0;
				}
				return connectedBoxTotalShotsTaken;
			}
		}

		public SecurityWrapperService()
		{
		}

		private STSWNetworkStateType GetNetworkState()
		{
			//switch (NetworkInternetVerification.instance.VerifyInternetAvailability())
			//{
			//	case NetworkInternetVerification.NET_STATES.WAITING:
			//		{
			//			return STSWNetworkStateType.STSW_NETWORK_VERIFIED_OFFLINE;
			//		}
			//	case NetworkInternetVerification.NET_STATES.VERIFIED_ONLINE:
			//		{
			//			return STSWNetworkStateType.STSW_NETWORK_VERIFIED_ONLINE;
			//		}
			//	case NetworkInternetVerification.NET_STATES.VERIFIED_OFFLINE:
			//	case NetworkInternetVerification.NET_STATES.NETWORKS_UNAVAILABLE:
			//		{
			//			return STSWNetworkStateType.STSW_NETWORK_VERIFIED_OFFLINE;
			//		}
			//}
			return STSWNetworkStateType.STSW_NETWORK_VERIFIED_OFFLINE;
		}

		public void Init()
		{
			SecurityWrapperService._instance = this;
			Application.targetFrameRate = 60;
			Screen.sleepTimeout = -1;
			this._persistentDataPath = Application.persistentDataPath;
			string str = Path.Combine(Application.persistentDataPath, "cache.sqlite");
			STSWNetworkStateType networkState = this.GetNetworkState();
			AppLog.Log(string.Concat(new object[] { "NETWORK STATE: ", networkState, "(", NetworkInternetVerification.instance.VerifyInternetAvailability(), ")" }), true);
			if (networkState == STSWNetworkStateType.STSW_NETWORK_VERIFIED_ONLINE)
			{
				SoftwareVersionUpdateController.GetAppVersion(SoftwareVersion.ShortVersion);
			}
			this.InitLogger();
			AppLog.SetSdkLogPath();
			if (true)
			{
				SkyTrakWrapper.Interfaces.ILogger logger = this._logger;
				STSWServerUrlMode serverUrl = WebService.ServerUrl;
				string shortVersion = SoftwareVersion.ShortVersion;
				RuntimePlatform runtimePlatform = Application.platform;
				SkyTrakSW.Create(logger, networkState, serverUrl, 1, str, shortVersion, "WindowsPlayer", this._persistentDataPath, 1);
			}
			else
			{
				string currentWrapperLogPath = AppLog.CurrentWrapperLogPath;
				string shortVersion1 = SoftwareVersion.ShortVersion;
				RuntimePlatform runtimePlatform1 = Application.platform;
				SkyTrakSW.CreateEmulator(currentWrapperLogPath, str, shortVersion1, runtimePlatform1.ToString());
				if (!MonoSingleton<DeviceEmulatorInspector>.Singleton.UseOverdueMMS)
				{
					SkyTrakSW.Instance.Emulator.InitDefaultMMS();
				}
				else
				{
					SkyTrakSW.Instance.Emulator.InitMMS("010105A80F0000446F75626C65204561676C650000000000000000DDC64257000000000F0000E01E3D4B00000000FFFFFFFFFFFFFFFFFFFF00000000000000000000416C65786579000000000000000000000000000000000000000000000000000000000000000000004C61747573686B6F0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000F0052454532304F4F4F4F4F4F41610000006090C75600000000E0E3FFFFFFFFFFFF6090C7560000000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
				}
				SkyTrakSW.Instance.Emulator.PackMMS();
			}
			this._wrapper = SkyTrakSW.Instance;
			float version = this._wrapper.Version.majorVer;
			RIPEVersion rIPEVersion = this._wrapper.Version;
			AppLog.Log(string.Concat("SkyTrakSW version = ", version + rIPEVersion.minorVer), true);
			this.SetupDebugData();
			this._wrapper.OnDiscoverUpdated += new Action<List<RIPECommBoxDataParamsType>>(this.OnDiscoverUpdated);
			this._wrapper.OnDiscoverFail += new Action<RIPEErrType>(this.OnDiscoverFail);
			this._wrapper.OnStatusUpdated += new Action<RIPEBoxParamsType>(this.OnStatusUpdated);
			this._wrapper.OnConnected += new Action(this.OnConnected);
			this._wrapper.OnConnectFail += new Action<RIPEErrType>(this.OnConnectFail);
			this._wrapper.OnDisconnected += new Action(this.OnDisconnected);
			this._wrapper.OnMMSUpdated += new Action(this.OnMmsUpdated);
			this._wrapper.OnFirmWareUpgradeAvailable += new Action(this.OnFirmWareUpgradeAvailable);
			this._wrapper.OnFirmWareUpgradeError += new Action(this.OnFirmWareUpgradeError);
			this._wrapper.OnFirmWareUpgradeSuccess += new Action(this.OnFirmWareUpgradeSuccess);
			this._wrapper.OnFirmWareSDKIncompatible += new Action(this.OnFirmWareSdkIncompatible);
			this._wrapper.OnFirmWareSDKCustomercodeIncompatible += new Action(this.OnFirmWareSdkCustomercodeIncompatible);
			this._wrapper.OnShotStarted += new Action(this.OnShotStarted);
			this._wrapper.OnShotEnded += new Action<STSWShotParamsType>(this.OnShotEnded);
			this._wrapper.OnNetworkScanListUpdated += new Action<List<RIPENetworkScanListParamsType>>(this.OnNetworkScanListUpdated);
			this._wrapper.OnFailReadBoxMms += new Action(this.OnFailReadBoxMms);
			SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded);
		}

		private void InitLogger()
		{
			this._logger = new Logger();
			if (this._logger == null)
			{
				throw new Exception("ILogger not initialized in SecurityWrapperService");
			}
		}

		public void OnApplicationPause(bool pauseStatus)
		{
			if (this._wrapper != null)
			{
				if (!pauseStatus)
				{
					try
					{
						AppLog.Log("Trying to resume SW", true);
						this._wrapper.Resume();
						this.RearmUnit();
					}
					catch (Exception exception)
					{
						AppLog.Log("Exception during resume SW", true);
					}
				}
				else
				{
					try
					{
						AppLog.Log("Trying to pause SW", true);
						this._wrapper.Pause();
					}
					catch (Exception exception1)
					{
						AppLog.Log("Exception during pause SW", true);
					}
				}
			}
		}

		private void OnConnected()
		{
			AppLog.Log("OnConnected EVENT", true);
			try
			{
				this._saveBoxParamsToLog = true;
				this._mmsDataCopy = this._wrapper.CopyConnectedBoxMMSData();
				this.PrintMMS();
				if (this._isFWUpgradeAvailable)
				{
					this.UpgradeFirmware();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				AppLog.Log(string.Concat("Copy MMS Exception: ", exception.Message), true);
				AppLog.Log(string.Concat("Stacktrace: ", exception.StackTrace), true);
			}
			this._configWasChanged = false;
		}

		private void OnConnectFail(RIPEErrType ripeErrType)
		{
			AppLog.Log("OnConnectedFail EVENT", true);
			if (this.OnDisconnectedAction != null)
			{
				this.OnDisconnectedAction();
			}
			this.ResetLicenseEndStartDiscovering();
		}

		public void OnDisable()
		{
			if (this._wrapper != null)
			{
				this._wrapper.OnDiscoverUpdated -= new Action<List<RIPECommBoxDataParamsType>>(this.OnDiscoverUpdated);
				this._wrapper.OnDiscoverFail -= new Action<RIPEErrType>(this.OnDiscoverFail);
				this._wrapper.OnStatusUpdated -= new Action<RIPEBoxParamsType>(this.OnStatusUpdated);
				this._wrapper.OnConnected -= new Action(this.OnConnected);
				this._wrapper.OnConnectFail -= new Action<RIPEErrType>(this.OnConnectFail);
				this._wrapper.OnDisconnected -= new Action(this.OnDisconnected);
				this._wrapper.OnMMSUpdated -= new Action(this.OnMmsUpdated);
				this._wrapper.OnFirmWareUpgradeAvailable -= new Action(this.OnFirmWareUpgradeAvailable);
				this._wrapper.OnFirmWareUpgradeError -= new Action(this.OnFirmWareUpgradeError);
				this._wrapper.OnFirmWareUpgradeSuccess -= new Action(this.OnFirmWareUpgradeSuccess);
				this._wrapper.OnFirmWareSDKIncompatible -= new Action(this.OnFirmWareSdkIncompatible);
				this._wrapper.OnFirmWareSDKCustomercodeIncompatible -= new Action(this.OnFirmWareSdkCustomercodeIncompatible);
				this._wrapper.OnShotStarted -= new Action(this.OnShotStarted);
				this._wrapper.OnShotEnded -= new Action<STSWShotParamsType>(this.OnShotEnded);
				this._wrapper.OnNetworkScanListUpdated -= new Action<List<RIPENetworkScanListParamsType>>(this.OnNetworkScanListUpdated);
				this._wrapper.OnFailReadBoxMms -= new Action(this.OnFailReadBoxMms);
				if (this._wrapper.IsConnected)
				{
					this._wrapper.BoxDisonnect();
				}
				this._wrapper.Dispose();
			}
		}

		private void OnDisconnected()
		{
			if (this.OnDisconnectedAction != null)
			{
				this.OnDisconnectedAction();
			}
			this.ResetLicenseEndStartDiscovering();
		}

		private void OnDiscoverFail(RIPEErrType ripeErrType)
		{
			AppLog.Log(string.Concat("DISCOVER WAS FAIL: ", ripeErrType), true);
			AppLog.Log("START DISCOVERING AGAIN", true);
			this.ResetLicenseEndStartDiscovering();
		}

		private void OnDiscoverUpdated(List<RIPECommBoxDataParamsType> ripeCommBoxDataParamsTypes)
		{
			if (ripeCommBoxDataParamsTypes.Count == 0)
			{
				try
				{
					this._wrapper.Discover();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Discover Exception: ", exception.Message), true);
				}
				return;
			}
			foreach (RIPECommBoxDataParamsType ripeCommBoxDataParamsType in ripeCommBoxDataParamsTypes)
			{
				AppLog.Log(string.Concat("Discovered Box: ", ripeCommBoxDataParamsType.boxName), true);
				this._boxList.Clear();
				this._boxList.AddRange(ripeCommBoxDataParamsTypes);
			}
			try
			{
				this._wrapper.BoxConnect(ripeCommBoxDataParamsTypes[0].boxName);
			}
			catch (Exception exception3)
			{
				Exception exception2 = exception3;
				AppLog.Log(string.Concat("Box Connect Exception: ", exception2.Message), true);
			}
		}

		public void OnEnable()
		{
		}

		private void OnFailReadBoxMms()
		{
			AppLog.Log("OnFailReadBoxMms", true);
			//MonoSingleton<UIData>.Singleton.MenuPopupData.Show("ERROR", string.Concat("Unable to read device memory", Environment.NewLine, Environment.NewLine, "Please restart unit and connect again. If the problem persists, you will need to connect to the internet and sync data, then connect your unit."), 0f, false);
		}

		private void OnFirmWareSdkCustomercodeIncompatible()
		{
			AppLog.Log("FIRMWARE SDK CUSTOMER CODE INCOMPATIBLE", true);
		}

		private void OnFirmWareSdkIncompatible()
		{
			AppLog.Log("FIRMWARE SDK INCOMPATIBLE", true);
		}

		private void OnFirmWareUpgradeAvailable()
		{
			AppLog.Log("FW UPGRADE IS AVAILABLE", true);
			this._isFWUpgradeAvailable = true;
		}

		private void OnFirmWareUpgradeError()
		{
			AppLog.Log("UPGRADE FW ERROR", true);
		}

		private void OnFirmWareUpgradeSuccess()
		{
			AppLog.Log("UPGRADE FW SUCCESS", true);
		}

		private void OnMmsUpdated()
		{
			AppLog.Log("MMS was updated", true);
			try
			{
				this._mmsDataCopy = this._wrapper.CopyConnectedBoxMMSData();
				this.PrintMMS();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				AppLog.Log(string.Concat("Copy MMS Exception: ", exception.Message), true);
				AppLog.Log(string.Concat("Stacktrace: ", exception.StackTrace), true);
			}
			this.OnMMSUpdatedAction();
		}

		private void OnMMSUpdatedAction()
		{
			MembershipManager.SetMMSData();
		}

		private void OnNetworkScanListUpdated(List<RIPENetworkScanListParamsType> ripeNetworkScanListParamsTypes)
		{
			if (this.OnNetworkScanCompletedAction != null)
			{
				this.OnNetworkScanCompletedAction(ripeNetworkScanListParamsTypes);
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (this._needToUpdateMMS && scene.name.Equals("MainNew"))
			{
				if (SkyTrakSW.Instance.Emulator != null)
				{
					return;
				}
				this._needToUpdateMMS = false;
				SecurityWrapperService.Instance.UpdateMMS();
			}
		}

		private void OnShotEnded(STSWShotParamsType shotParams)
		{
			AppLog.Log("SHOT ENDED", true);
			Application.targetFrameRate = 60;
			if (shotParams.speedParams.horizontalAngleConfidence <= 0f || shotParams.speedParams.launchAngleConfidence <= 0f || shotParams.speedParams.totalSpeedConfidence <= 0f || shotParams.speedParams.totalSpeed <= 0f)
			{
				AppLog.Log(string.Format("Shot is wrong. Speed confidences: horAngleConfidence = {0}, launchAngleConfidence = {1}, totalSpeedConfidence = {2}, totalSpeed = {3}", new object[] { shotParams.speedParams.horizontalAngleConfidence, shotParams.speedParams.launchAngleConfidence, shotParams.speedParams.totalSpeedConfidence, shotParams.speedParams.totalSpeed }), true);
				((ISkyTrakSW)this).ArmBox();
				return;
			}
			if (shotParams.spinParams.measurementConfidence > 0f && !shotParams.spinParams.backSpin.NearEquals(0f, 4) && !shotParams.spinParams.sideSpin.NearEquals(0f, 4) && !shotParams.spinParams.totalSpin.NearEquals(0f, 4))
			{
				if (this.OnBallLaunchedAction != null)
				{
					this.OnBallLaunchedAction(shotParams.speedParams, shotParams.spinParams, shotParams.speedSpinFlightParams);
				}
				return;
			}
			AppLog.Log(string.Format("Shot is wrong. Spin confidence: measurementConfidence = {0}, backSpin = {1}, sideSpin = {2}, totalSpin = {3}", new object[] { shotParams.spinParams.measurementConfidence, shotParams.spinParams.backSpin, shotParams.spinParams.sideSpin, shotParams.spinParams.totalSpin }), true);
			((ISkyTrakSW)this).ArmBox();
		}

		private void OnShotStarted()
		{
			AppLog.Log("TRIGGER DETECTED - SHOT STARTED", true);
			Application.targetFrameRate = 10;
		}

		private void OnStatusUpdated(RIPEBoxParamsType ripeBoxParamsType)
		{
			if (this._saveBoxParamsToLog)
			{
				AppLog.Log(string.Format("Box Params: Handness State - {0}, Charging State - {1}, Battery Percent - {2}, IsBoxInAPMode = {3}", new object[] { ripeBoxParamsType.handednessState, ripeBoxParamsType.chargingState, ripeBoxParamsType.batteryPercent.ToString("F1"), ripeBoxParamsType.isBoxInAPMode }), true);
				this._saveBoxParamsToLog = false;
			}
			this._boxParams = ripeBoxParamsType;
		}

		private void PrintMMS()
		{
			if (this._mmsDataCopy == null)
			{
				return;
			}
			AppLog.Log(string.Concat("MMS: ", this._mmsDataCopy.ToString()), true);
		}

		private void RearmUnit()
		{
			CoroutinesTool coroutinesTool = (new GameObject()).AddComponent<CoroutinesTool>();
			coroutinesTool.StartCoroutine(this.StartRearmUnit());
		}

		private void ResetLicenseEndStartDiscovering()
		{
			this._licenseCheck = 0f;
			this._isLicenseChecking = false;
			this._isLicenseChecked = false;
			this._mmsDataCopy = null;
			this._wrapper.Discover();
		}

		void Security.ISkyTrakSW.AddEsnToCache(string esn)
		{
			AppLog.Log(string.Concat("esn:", esn), true);
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.AddBoxEsnToCache(esn);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Add Box Esn to Cache Exception: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.ArmBox()
		{
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.BoxArm();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Arm Box Exception: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.BoxSetNetworkConfig(RIPENetworkConfigType config)
		{
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.BoxSetNetworkConfig(config);
					this._configWasChanged = true;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Set Network Config Exception: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.ConnectToDevice(string boxName)
		{
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.BoxConnect(boxName);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Box Connect Exception: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.DisarmBox()
		{
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.BoxDisarm();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Disarm Box Exception: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.ForceDirectConnect()
		{
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.BoxBootForceDirectConnect();
					this._configWasChanged = true;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Force Direct Connect Exception: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.ForceNetworkConnect()
		{
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.BoxBootForceNetworkConnect();
					this._configWasChanged = true;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Force Network Connect Exception: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.MakeTestShot(RIPESpeedParamsType speedParams, RIPESpinParamsType spinParams, RIPEFlightParamsType flightParams)
		{
			if (this.OnBallLaunchedAction != null)
			{
				this.OnBallLaunchedAction(speedParams, spinParams, flightParams);
			}
		}

		void Security.ISkyTrakSW.QueueUpdateMMS()
		{
			this._needToUpdateMMS = true;
		}

		void Security.ISkyTrakSW.ScanWiFiNetworks()
		{
			try
			{
				this._wrapper.ScanWiFiNetwork();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				AppLog.LogException(exception, true);
				throw exception;
			}
		}

		void Security.ISkyTrakSW.SetAlignmentMode(bool set)
		{
			if (this._wrapper != null)
			{
				try
				{
					if (!set)
					{
						this._wrapper.BoxSetAssistAlignmentOff();
					}
					else
					{
						this._wrapper.BoxSetAssistAlignmentOn();
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Set Alignment Mode Exception: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.SetHandedness(bool isLefty)
		{
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.BoxSetHandedness((!isLefty ? RIPEBoxHandednessStateType.RIPE_HANDEDNESS_RIGHT : RIPEBoxHandednessStateType.RIPE_HANDEDNESS_LEFT));
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Set Handedness Exception: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.SetShotModeNormal()
		{
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.BoxSetShotModeNormal();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Exception during set normal mode: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.SetShotModePutting()
		{
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.BoxSetShotModePutting();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Exception during set putting mode: ", exception.Message), true);
				}
			}
		}

		void Security.ISkyTrakSW.UpdateMMS()
		{
			if (this.GetNetworkState() != STSWNetworkStateType.STSW_NETWORK_VERIFIED_ONLINE)
			{
				return;
			}
			this._isLicenseChecked = false;
			this._isLicenseChecking = false;
			this._mmsDataCopy = null;
			if (this._wrapper != null)
			{
				try
				{
					this._wrapper.UpdateConnectedBoxMMSData();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Exception during update MMS: ", exception.Message), true);
				}
			}
		}

		private void SetupDebugData()
		{
		}

		public void ShowEvaluationPeriodMessage(SkyTrakSW.MMSLicense license)
		{
			//MonoSingleton<UIData>.Singleton.EvaluationPeriodWindowData.Show(license, 5f);
		}

		public void Start()
		{
			if (this._wrapper != null)
			{
				this._wrapper.Discover();
			}
		}

		private IEnumerator StartRearmUnit()
		{
			if (_wrapper == null)
				yield return new WaitForSeconds(6f);

			AppLog.Log("Start Rearming", true);
			Instance.DisarmBox();

			yield return new WaitForSeconds(1f);
			Instance.ArmBox();

		}

		public void Update()
		{
			if (this._wrapper != null)
			{
				this._wrapper.Perform();
				this.UpdateCheckingLicenseStatus();
			}
		}

		private void UpdateCheckingLicenseStatus()
		{
			if (this._isLicenseChecked || this._mmsDataCopy == null)
			{
				return;
			}
			if (this._wrapper.IsConnected && !this._isLicenseChecking)
			{
				this._isLicenseChecking = true;
			}
			if (this._wrapper.IsConnected && this._isLicenseChecking)
			{
				this._licenseCheck += Time.deltaTime;
				if (this._licenseCheck >= this._licenseCheckTime)
				{
					this._isLicenseChecking = false;
					SkyTrakSW.MMSLicense license = this._mmsDataCopy.License;
					AppLog.Log(string.Format("License: Validation result = {0}, Header = {1}, Body = {2}, DaysLeft = {3}", new object[] { license.ValidationResult, license.Header, license.Body, license.DaysLeftInCurrentPeriod }), true);
					if (license.ValidationResult != STSWMMSValidationResultType.STSW_VALIDATION_MMS_VALID)
					{
						this.ShowEvaluationPeriodMessage(license);
					}
					if (license.ValidationResult == STSWMMSValidationResultType.STSW_VALIDATION_DEVICE_NOT_REGISTERED_BLOCK)
					{
						((ISkyTrakSW)this).DisarmBox();
					}
					else
					{
						((ISkyTrakSW)this).ArmBox();
					}
					this._isLicenseChecked = true;
					this.OnMMSUpdatedAction();
				}
			}
		}

		private void UpgradeFirmware()
		{
			if (this._wrapper != null)
			{
				try
				{
					this._isFWUpgradeAvailable = false;
					//MonoSingleton<UIData>.Singleton.BatteryPanelData.IncreaseWaitTimeFor(60f);
					AppLog.Log("TRY TO START UPGRADING FW", true);
					this._wrapper.UpgradeFirmWare();
					AppLog.Log("START UPGRADING FW", true);
					//MonoSingleton<UIData>.Singleton.MenuPopupData.Show("PLEASE WAIT", string.Concat(new string[] { "The firmware in your SkyTrak device is being updated.  Do not turn off the unit during this time.", Environment.NewLine, Environment.NewLine, "You will notice the unit reboot.  When this screen disappears, you may reconnect to the SkyTrak device.", Environment.NewLine, Environment.NewLine, "In some cases, you may need to close and restart the application." }), 20f, true);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					AppLog.Log(string.Concat("Upgrade FW Exception: ", exception.Message), true);
				}
			}
		}

		event Action<RIPESpeedParamsType, RIPESpinParamsType, RIPEFlightParamsType> Security.ISkyTrakSW.OnBallLaunched
		{
			add
			{
				this.OnBallLaunchedAction += value;
			}
			remove
			{
				this.OnBallLaunchedAction -= value;
			}
		}

		event Action Security.ISkyTrakSW.OnConnected
		{
			add
			{
				if (this._wrapper != null)
				{
					this._wrapper.OnConnected += value;
				}
			}
			remove
			{
				if (this._wrapper != null)
				{
					this._wrapper.OnConnected -= value;
				}
			}
		}

		event Action Security.ISkyTrakSW.OnDisconnected
		{
			add
			{
				this.OnDisconnectedAction += value;
			}
			remove
			{
				this.OnDisconnectedAction -= value;
			}
		}

		event Action Security.ISkyTrakSW.OnFailReadBoxMms
		{
			add
			{
				this._wrapper.OnFailReadBoxMms += value;
			}
			remove
			{
				this._wrapper.OnFailReadBoxMms -= value;
			}
		}

		event Action Security.ISkyTrakSW.OnMMSUpdated
		{
			add
			{
				if (this._wrapper != null)
				{
					this._wrapper.OnMMSUpdated += value;
				}
			}
			remove
			{
				if (this._wrapper != null)
				{
					this._wrapper.OnMMSUpdated -= value;
				}
			}
		}

		event Action<List<RIPENetworkScanListParamsType>> Security.ISkyTrakSW.OnNetworkScanCompleted
		{
			add
			{
				this.OnNetworkScanCompletedAction += value;
			}
			remove
			{
				this.OnNetworkScanCompletedAction -= value;
			}
		}
	}
}