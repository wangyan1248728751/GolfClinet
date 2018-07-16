using SkyTrakWrapper.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Golf;

namespace SkyTrakWrapper
{
	public class SkyTrakSW : IDisposable
	{
		private IntPtr stswHandle;

		private ILogger externLogger;

		private STSWAbstLoggerType loggerData;

		private IntPtr fileLoggerPtr;

		private SkyTrakSW.RIPEEmulator ripeEmulator;

		private readonly static object s_lock;

		private static SkyTrakSW instance;

		protected Action<List<RIPECommBoxDataParamsType>> OnDiscoverUpdatedInvoker;

		protected Action<List<RIPENetworkScanListParamsType>> OnNetworkScanListUpdatedInvoker;

		protected Action<RIPEErrType> OnDiscoverFailInvoker;

		protected Action OnConnectedInvoker;

		protected Action<RIPEErrType> OnConnectFailInvoker;

		protected Action OnMMSUpdatedInvoker;

		protected Action OnStartedDisconnectionInvoker;

		protected Action OnDisconnectedInvoker;

		protected Action<RIPEBoxParamsType> OnStatusUpdatedInvoker;

		protected Action OnShotStartedInvoker;

		protected Action<STSWShotParamsType> OnShotEndedInvoker;

		protected Action OnFirmWareUpgradeSuccessInvoker;

		protected Action OnFirmWareUpgradeErrorInvoker;

		protected Action OnFirmWareUpgradeStartedInvoker;

		protected Action OnFirmWareUpgradeAvailableInvoker;

		protected Action OnFirmWareSDKIncompatibleInvoker;

		protected Action OnFirmWareSDKCustomercodeIncompatibleInvoker;

		protected Action OnFailedReadBoxMmsInvoker;

		public RIPEBoxConnectionType ConnectedBoxConnectionType
		{
			get
			{
				RIPEBoxConnectionType rIPEBoxConnectionType = RIPEBoxConnectionType.RIPE_BOX_CONNECTION_UNKNOWN;
				SkyTrakSW.InternalInterface.STSWGetBoxConnectionType(this.stswHandle, ref rIPEBoxConnectionType);
				return rIPEBoxConnectionType;
			}
		}

		public RIPEVersion ConnectedBoxFWVersion
		{
			get
			{
				RIPEVersion rIPEVersion = new RIPEVersion()
				{
					majorVer = 0f,
					minorVer = 0f
				};
				RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWGetBoxFWVersion(this.stswHandle, ref rIPEVersion);
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					throw new STSWRunTimeException(rIPEErrType, "Could get box FW version - no active connections");
				}
				return rIPEVersion;
			}
		}

		public int ConnectedBoxTotalShotsTaken
		{
			get
			{
				STSWBoxShotsDataType sTSWBoxShotsDataType = new STSWBoxShotsDataType();
				if (SkyTrakSW.InternalInterface.STSWBoxGetShotsData(this.stswHandle, ref sTSWBoxShotsDataType) != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					return 0;
				}
				return sTSWBoxShotsDataType.totalShotsTaken;
			}
		}

		public SkyTrakSW.RIPEEmulator Emulator
		{
			get
			{
				return this.ripeEmulator;
			}
		}

		public static SkyTrakSW Instance
		{
			get
			{
				if (SkyTrakSW.instance == null)
				{
					throw new STSWNotIntializedException("SkyTrakSW object was not created");
				}
				return SkyTrakSW.instance;
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.WrapperState == STSWStateType.STSW_STATE_CONNECTED;
			}
		}

		public bool IsConnecting
		{
			get
			{
				return this.WrapperState == STSWStateType.STSW_STATE_CONNECTING;
			}
		}

		public bool IsDiscovering
		{
			get
			{
				return this.WrapperState == STSWStateType.STSW_STATE_DISCOVERING;
			}
		}

		public bool IsLicenseChecking
		{
			get
			{
				return this.WrapperState == STSWStateType.STSW_STATE_CHECKING_LICENSE;
			}
		}

		public bool isMMSDataReady
		{
			get
			{
				IntPtr zero = IntPtr.Zero;
				RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWGetMMS(this.stswHandle, ref zero);
				bool flag = rIPEErrType == RIPEErrType.RIPE_ERR_SUCCESS;
				SkyTrakSW.InternalInterface.STSWFreeMMS(zero);
				return flag;
			}
		}

		public bool IsSleeping
		{
			get
			{
				return this.WrapperState == STSWStateType.STSW_STATE_SLEEP;
			}
		}

		public RIPEVersion RIPESDKVersion
		{
			get
			{
				RIPEVersion rIPEVersion = new RIPEVersion()
				{
					majorVer = 0f,
					minorVer = 0f
				};
				SkyTrakSW.InternalInterface.STSWGetRIPEVersion(this.stswHandle, ref rIPEVersion);
				return rIPEVersion;
			}
		}

		public RIPEVersion Version
		{
			get
			{
				RIPEVersion rIPEVersion = new RIPEVersion()
				{
					majorVer = 0f,
					minorVer = 0f
				};
				SkyTrakSW.InternalInterface.STSWGetVersion(ref rIPEVersion);
				return rIPEVersion;
			}
		}

		public STSWStateType WrapperState
		{
			get
			{
				STSWStateType sTSWStateType = STSWStateType.STSW_STATE_SLEEP;
				SkyTrakSW.InternalInterface.STSWGetState(this.stswHandle, ref sTSWStateType);
				return sTSWStateType;
			}
		}

		static SkyTrakSW()
		{
			SkyTrakSW.s_lock = new object();
			SkyTrakSW.instance = null;
		}

		private SkyTrakSW(ILogger logger, STSWNetworkStateType networkState, STSWServerUrlMode serverUrlMode, int generateFlightData, string cacheDbPath, string softwareVer, string platform, string sdkLogFolderPath, int sdkDebugLevel, bool emulator)
		{
			this.stswHandle = IntPtr.Zero;
			this.loggerData = new STSWAbstLoggerType();
			this.fileLoggerPtr = IntPtr.Zero;
			this.OnDiscoverUpdatedInvoker = (List<RIPECommBoxDataParamsType> argument0) => {
			};
			this.OnNetworkScanListUpdatedInvoker = (List<RIPENetworkScanListParamsType> argument1) => {
			};
			this.OnDiscoverFailInvoker = (RIPEErrType argument2) => {
			};
			this.OnConnectedInvoker = () => {
			};
			this.OnConnectFailInvoker = (RIPEErrType argument3) => {
			};
			this.OnMMSUpdatedInvoker = () => {
			};
			this.OnStartedDisconnectionInvoker = () => {
			};
			this.OnDisconnectedInvoker = () => {
			};
			this.OnStatusUpdatedInvoker = (RIPEBoxParamsType argument4) => {
			};
			this.OnShotStartedInvoker = () => {
			};
			this.OnShotEndedInvoker = (STSWShotParamsType argument5) => {
			};
			this.OnFirmWareUpgradeSuccessInvoker = () => {
			};
			this.OnFirmWareUpgradeErrorInvoker = () => {
			};
			this.OnFirmWareUpgradeStartedInvoker = () => {
			};
			this.OnFirmWareUpgradeAvailableInvoker = () => {
			};
			this.OnFirmWareSDKIncompatibleInvoker = () => {
			};
			this.OnFirmWareSDKCustomercodeIncompatibleInvoker = () => {
			};
			this.OnFailedReadBoxMmsInvoker = () => {
			};
			this.externLogger = logger;
			this.stswHandle = new IntPtr();
			this.loggerData.handle = GCHandle.ToIntPtr(GCHandle.Alloc(this));
			this.loggerData.logMsgFunc = new LogMsgDelegate(this.LoggerCallback);
			STSWInitType sTSWInitType = new STSWInitType()
			{
				operationMode = RIPEOperationModeType.RIPE_MODE_DEFAULT,
				generateFlightData = generateFlightData,
				loggerData = IntPtr.Zero,
				networkState = networkState,
				ripeEmulatorMode = (!emulator ? 0 : 1),
				boxCacheFilePath = Marshal.StringToHGlobalAnsi(cacheDbPath),
				serverUrlMode = serverUrlMode,
				softwareVersion = Marshal.StringToHGlobalAnsi(softwareVer),
				platform = Marshal.StringToHGlobalAnsi(platform),
				sdkDataFolderPath = sdkLogFolderPath,
				sdkDebugLevel = sdkDebugLevel
			};
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWInitEx(ref this.stswHandle, ref sTSWInitType, ref this.loggerData);
			Marshal.FreeHGlobal(sTSWInitType.boxCacheFilePath);
			Marshal.FreeHGlobal(sTSWInitType.softwareVersion);
			Marshal.FreeHGlobal(sTSWInitType.platform);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				this.stswHandle = IntPtr.Zero;
				throw new STSWInitializationFailedException(string.Concat("STSWInit failed with code ", rIPEErrType.ToString("D")));
			}
			if (emulator)
			{
				this.ripeEmulator = new SkyTrakSW.RIPEEmulator(this.stswHandle);
			}
		}

		private SkyTrakSW(string loggerFilePath, STSWNetworkStateType networkState, STSWServerUrlMode serverUrlMode, int generateFlightData, string cacheDbPath, string softwareVer, string platform, string sdkLogFolderPath, int sdkDebugLevel, bool emulator)
		{
			this.stswHandle = IntPtr.Zero;
			this.loggerData = new STSWAbstLoggerType();
			this.fileLoggerPtr = IntPtr.Zero;
			this.OnDiscoverUpdatedInvoker = (List<RIPECommBoxDataParamsType> argument0) => {
			};
			this.OnNetworkScanListUpdatedInvoker = (List<RIPENetworkScanListParamsType> argument1) => {
			};
			this.OnDiscoverFailInvoker = (RIPEErrType argument2) => {
			};
			this.OnConnectedInvoker = () => {
			};
			this.OnConnectFailInvoker = (RIPEErrType argument3) => {
			};
			this.OnMMSUpdatedInvoker = () => {
			};
			this.OnStartedDisconnectionInvoker = () => {
			};
			this.OnDisconnectedInvoker = () => {
			};
			this.OnStatusUpdatedInvoker = (RIPEBoxParamsType argument4) => {
			};
			this.OnShotStartedInvoker = () => {
			};
			this.OnShotEndedInvoker = (STSWShotParamsType argument5) => {
			};
			this.OnFirmWareUpgradeSuccessInvoker = () => {
			};
			this.OnFirmWareUpgradeErrorInvoker = () => {
			};
			this.OnFirmWareUpgradeStartedInvoker = () => {
			};
			this.OnFirmWareUpgradeAvailableInvoker = () => {
			};
			this.OnFirmWareSDKIncompatibleInvoker = () => {
			};
			this.OnFirmWareSDKCustomercodeIncompatibleInvoker = () => {
			};
			this.OnFailedReadBoxMmsInvoker = () => {
			};
			this.stswHandle = new IntPtr();
			IntPtr hGlobalAnsi = Marshal.StringToHGlobalAnsi(loggerFilePath);
			SkyTrakSW.InternalInterface.STSWCreateFileLogger(ref this.fileLoggerPtr, hGlobalAnsi, 0);
			Marshal.FreeHGlobal(hGlobalAnsi);
			STSWInitType sTSWInitType = new STSWInitType()
			{
				operationMode = RIPEOperationModeType.RIPE_MODE_DEFAULT,
				generateFlightData = generateFlightData,
				loggerData = this.fileLoggerPtr,
				networkState = networkState,
				ripeEmulatorMode = (!emulator ? 0 : 1),
				boxCacheFilePath = Marshal.StringToHGlobalAnsi(cacheDbPath),
				serverUrlMode = serverUrlMode,
				softwareVersion = Marshal.StringToHGlobalAnsi(softwareVer),
				platform = Marshal.StringToHGlobalAnsi(platform),
				sdkDataFolderPath = sdkLogFolderPath,
				sdkDebugLevel = sdkDebugLevel
			};
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWInit(ref this.stswHandle, ref sTSWInitType);
			Marshal.FreeHGlobal(sTSWInitType.boxCacheFilePath);
			Marshal.FreeHGlobal(sTSWInitType.softwareVersion);
			Marshal.FreeHGlobal(sTSWInitType.platform);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				this.stswHandle = IntPtr.Zero;
				throw new STSWInitializationFailedException(string.Concat("STSWInit failed with code ", rIPEErrType.ToString("D")));
			}
			if (emulator)
			{
				this.ripeEmulator = new SkyTrakSW.RIPEEmulator(this.stswHandle);
			}
		}

		public void AddBoxEsnToCache(string esn)
		{
			AppLog.Log(string.Concat(new string[] { string.Concat("ssss", esn) }), true);
			IntPtr hGlobalAnsi = Marshal.StringToHGlobalAnsi(esn);
			RIPEErrType cache = SkyTrakSW.InternalInterface.STSWAddBoxToCache(this.stswHandle, hGlobalAnsi);
			Marshal.FreeHGlobal(hGlobalAnsi);
			if (cache != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(cache, "RIPE SDK in active state. Can not modify cache now");
			}
		}

		public void BoxArm()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxArm(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				if (rIPEErrType != RIPEErrType.RIPE_ERR_UNAVAILABLE)
				{
					throw new STSWRunTimeException(rIPEErrType, "Could not arm box - no active connections");
				}
				throw new STSWRunTimeException(rIPEErrType, "Could not arm blocked box");
			}
		}

		public void BoxBootForceDirectConnect()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxBootForceDirectConnect(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could boot box direct connect - no active connections");
			}
		}

		public void BoxBootForceNetworkConnect()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxBootForceNetworkConnect(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could boot box network connect - no active connections");
			}
		}

		public void BoxConnect(string boxName)
		{
			IntPtr hGlobalAnsi = Marshal.StringToHGlobalAnsi(boxName);
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxConnect(this.stswHandle, hGlobalAnsi);
			Marshal.FreeHGlobal(hGlobalAnsi);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could not start discovering");
			}
		}

		public void BoxDisarm()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxDisarm(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could not disarm box - no active connections");
			}
		}

		public void BoxDisonnect()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxDisconnect(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could not disconnect from box - no active connections");
			}
		}

		public void BoxSetAssistAlignmentOff()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxSetAssistAlignmentOff(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could set box alignment - no active connections");
			}
		}

		public void BoxSetAssistAlignmentOn()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxSetAssistAlignmentOn(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could set box alignment - no active connections");
			}
		}

		public void BoxSetHandedness(RIPEBoxHandednessStateType handedness)
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxSetHandedness(this.stswHandle, handedness);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could set box handedness - no active connections");
			}
		}

		public void BoxSetNetworkConfig(RIPENetworkConfigType networkConfigData)
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxSetNetworkConfig(this.stswHandle, ref networkConfigData);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could set box network config - no active connections");
			}
		}

		public void BoxSetShotModeNormal()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxSetShotModeNormal(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could set box shot mode - no active connections");
			}
		}

		public void BoxSetShotModePutting()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxSetShotModePutting(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could set box shot mode - no active connections");
			}
		}

		private void CloseSTSWHandle()
		{
			if (this.stswHandle == IntPtr.Zero)
			{
				return;
			}
			SkyTrakSW.InternalInterface.STSWEDeInit(this.stswHandle);
			this.stswHandle = IntPtr.Zero;
			if (this.fileLoggerPtr != IntPtr.Zero)
			{
				SkyTrakSW.InternalInterface.STSWFreeFileLogger(this.fileLoggerPtr);
				this.fileLoggerPtr = IntPtr.Zero;
			}
		}

		public SkyTrakSW.MMSData CopyConnectedBoxMMSData()
		{
			IntPtr zero = IntPtr.Zero;
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWGetMMS(this.stswHandle, ref zero);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could not get MMS from wrapper. Probably box is not connected");
			}
			return new SkyTrakSW.MMSData(zero);
		}

		public static SkyTrakSW Create(ILogger logger, STSWNetworkStateType networkState, STSWServerUrlMode serverUrlMode, int generateFlightData, string cacheDbPath, string softwareVer, string platform, string sdkLogFolderPath, int sdkDebugLevel)
		{
			STSWInitializationFailedException sTSWInitializationFailedException = null;
			lock (s_lock)
			{
				try
				{
					SkyTrakSW skyTrakSW = new SkyTrakSW(logger, networkState, serverUrlMode, generateFlightData, cacheDbPath, softwareVer, platform, sdkLogFolderPath, sdkDebugLevel, false);
					Interlocked.Exchange<SkyTrakSW>(ref SkyTrakSW.instance, skyTrakSW);
				}
				catch (STSWInitializationFailedException sTSWInitializationFailedException1)
				{
					sTSWInitializationFailedException = sTSWInitializationFailedException1;
				}
			}
			if (sTSWInitializationFailedException != null)
			{
				throw sTSWInitializationFailedException;
			}
			return SkyTrakSW.instance;
		}

		public static SkyTrakSW Create(string loggerFilePath, STSWNetworkStateType networkState, STSWServerUrlMode serverUrlMode, int generateFlightData, string cacheDbPath, string softwareVer, string platform, string sdkLogFolderPath, int sdkDebugLevel)
		{
			STSWInitializationFailedException sTSWInitializationFailedException = null;
			lock (s_lock)
			{
				try
				{
					SkyTrakSW skyTrakSW = new SkyTrakSW(loggerFilePath, networkState, serverUrlMode, generateFlightData, cacheDbPath, softwareVer, platform, sdkLogFolderPath, sdkDebugLevel, false);
					Interlocked.Exchange<SkyTrakSW>(ref SkyTrakSW.instance, skyTrakSW);
				}
				catch (STSWInitializationFailedException sTSWInitializationFailedException1)
				{
					sTSWInitializationFailedException = sTSWInitializationFailedException1;
				}
			}
			if (sTSWInitializationFailedException != null)
			{
				throw sTSWInitializationFailedException;
			}
			return SkyTrakSW.instance;
		}

		public static SkyTrakSW CreateEmulator(ILogger logger, string cacheDbPath, string softwareVer, string platform)
		{
			STSWInitializationFailedException sTSWInitializationFailedException = null;
			lock (null)
			{
				try
				{
					SkyTrakSW skyTrakSW = new SkyTrakSW(logger, STSWNetworkStateType.STSW_NETWORK_VERIFIED_ONLINE, STSWServerUrlMode.STSW_SERVER_URL_BETA_TESTING, 0, cacheDbPath, softwareVer, platform, string.Empty, 0, true);
					Interlocked.Exchange<SkyTrakSW>(ref SkyTrakSW.instance, skyTrakSW);
				}
				catch (STSWInitializationFailedException sTSWInitializationFailedException1)
				{
					sTSWInitializationFailedException = sTSWInitializationFailedException1;
				}
			}
			if (sTSWInitializationFailedException != null)
			{
				throw sTSWInitializationFailedException;
			}
			return SkyTrakSW.instance;
		}

		public static SkyTrakSW CreateEmulator(string loggerFilePath, string cacheDbPath, string softwareVer, string platform)
		{
			STSWInitializationFailedException sTSWInitializationFailedException = null;
			lock (null)
			{
				try
				{
					SkyTrakSW skyTrakSW = new SkyTrakSW(loggerFilePath, STSWNetworkStateType.STSW_NETWORK_VERIFIED_ONLINE, STSWServerUrlMode.STSW_SERVER_URL_BETA_TESTING, 0, cacheDbPath, softwareVer, platform, string.Empty, 0, true);
					Interlocked.Exchange<SkyTrakSW>(ref SkyTrakSW.instance, skyTrakSW);
				}
				catch (STSWInitializationFailedException sTSWInitializationFailedException1)
				{
					sTSWInitializationFailedException = sTSWInitializationFailedException1;
				}
			}
			if (sTSWInitializationFailedException != null)
			{
				throw sTSWInitializationFailedException;
			}
			return SkyTrakSW.instance;
		}

		public void Discover()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWDiscover(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could not start discovering");
			}
		}

		public void Dispose()
		{
			this.CloseSTSWHandle();
			GC.SuppressFinalize(this);
		}

		~SkyTrakSW()
		{
			this.CloseSTSWHandle();
		}

		private void LoggerCallback(IntPtr handle, STSWLogLevelType level, IntPtr msgPtr)
		{
			SkyTrakSW target = (SkyTrakSW)GCHandle.FromIntPtr(handle).Target;
			string stringAnsi = Marshal.PtrToStringAnsi(msgPtr);
			switch (level)
			{
				case STSWLogLevelType.STSW_LOG_LEVEL_DEBUG:
					{
						target.externLogger.Debug(stringAnsi);
						break;
					}
				case STSWLogLevelType.STSW_LOG_LEVEL_INFO:
					{
						target.externLogger.Info(stringAnsi);
						break;
					}
				case STSWLogLevelType.STSW_LOG_LEVEL_WARNING:
					{
						target.externLogger.Warning(stringAnsi);
						eventCatch(stringAnsi);
						break;
					}
				case STSWLogLevelType.STSW_LOG_LEVEL_ERROR:
					{
						target.externLogger.Error(stringAnsi);
						break;
					}
			}
		}

		public void Pause()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxConnectionPause(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could pause RIPE");
			}
		}

		public void Perform()
		{
			int num = 0;
			SkyTrakSW.InternalInterface.STSWPerform(this.stswHandle, ref num);
			while (num > 0)
			{
				IntPtr intPtr = SkyTrakSW.InternalInterface.STSWReadMsg(this.stswHandle);
				if (intPtr != IntPtr.Zero)
				{
					this.ProcessMessage(intPtr);
					SkyTrakSW.InternalInterface.STSWFreeMsg(intPtr);
					num--;
				}
				else
				{
					break;
				}
			}
		}

		private void ProcessConnected(IntPtr dataPtr)
		{
			this.OnConnectedInvoker();
		}

		private void ProcessConnectFail(IntPtr dataPtr)
		{
			RIPEErrType rIPEErrType = (RIPEErrType)Marshal.ReadInt32(dataPtr);
			this.OnConnectFailInvoker(rIPEErrType);
		}

		private void ProcessDisconnected(IntPtr dataPtr)
		{
			this.OnDisconnectedInvoker();
		}

		private void ProcessDiscoverFail(IntPtr dataPtr)
		{
			RIPEErrType rIPEErrType = (RIPEErrType)Marshal.ReadInt32(dataPtr);
			this.OnDiscoverFailInvoker(rIPEErrType);
		}

		private void ProcessDiscoverUpdated(IntPtr dataPtr)
		{
			List<RIPECommBoxDataParamsType> rIPECommBoxDataParamsTypes = new List<RIPECommBoxDataParamsType>();
			for (IntPtr i = SkyTrakSW.InternalInterface.STSWBoxListGetHead(dataPtr); SkyTrakSW.InternalInterface.STSWBoxListNodeIsEnd(i) == 0; i = SkyTrakSW.InternalInterface.STSWBoxListNodeGetNext(i))
			{
				RIPECommBoxDataParamsType rIPECommBoxDataParamsType = new RIPECommBoxDataParamsType();

				//SkyTrakSW.InternalInterface.STSWBoxListNodeGetData(i, ref rIPECommBoxDataParamsType);
				//接口炸了怎么办???

				//喊用户手动输入设备SSID!!!
				//rIPECommBoxDataParamsType.boxName = "SKYTRAK_C47F5102A924";//手动输入
				rIPECommBoxDataParamsType.boxName = GameEntry.GameData.boxName;

				rIPECommBoxDataParamsType.boxIP = "USB";
				rIPECommBoxDataParamsType.adapterType = RIPEAdapterType.RIPE_INTERFACE_USB;
				rIPECommBoxDataParamsType.boxConnectionType = RIPEBoxConnectionType.RIPE_BOX_CONNECTION_USB_MODE;

				rIPECommBoxDataParamsTypes.Add(rIPECommBoxDataParamsType);

				AppLog.Log(string.Format("name:{0},ip:{1}", rIPECommBoxDataParamsType.boxName, rIPECommBoxDataParamsType.boxIP), true);

			}
			this.OnDiscoverUpdatedInvoker(rIPECommBoxDataParamsTypes);
		}

		private void ProcessMessage(IntPtr msgPtr)
		{
			STSWMsgDataType structure = (STSWMsgDataType)Marshal.PtrToStructure(msgPtr, typeof(STSWMsgDataType));
			switch (structure.type)
			{
				case STSWMsgType.STSW_MSG_DISCOVER_UPDATED:
					{
						this.ProcessDiscoverUpdated(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_DISCOVER_FAIL:
					{
						this.ProcessDiscoverFail(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_CONNECTED:
					{
						this.ProcessConnected(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_CONNECT_FAIL:
					{
						this.ProcessConnectFail(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_MMS_UPDATED:
					{
						this.ProcessMMSUpdated(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_STARTED_DISCONNECTION:
					{
						this.ProcessStartedDisconnection(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_DISCONNECTED:
					{
						this.ProcessDisconnected(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_STATUS_UPDATED:
					{
						this.ProcessStatusUpdated(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_SHOT_STARTED:
					{
						this.ProcessShotStarted(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_SHOT_ENDED:
					{
						this.ProcessShotEnded(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_FW_UPGRADE_SUCCESS:
					{
						this.OnFirmWareUpgradeSuccessInvoker();
						break;
					}
				case STSWMsgType.STSW_MSG_FW_UPGRADE_ERROR:
					{
						this.OnFirmWareUpgradeErrorInvoker();
						break;
					}
				case STSWMsgType.STSW_MSG_FW_UPGRADE_STARTED:
					{
						this.OnFirmWareUpgradeStartedInvoker();
						break;
					}
				case STSWMsgType.STSW_MSG_FW_UPGRADE_AVAILABLE:
					{
						this.OnFirmWareUpgradeAvailableInvoker();
						break;
					}
				case STSWMsgType.STSW_MSG_FW_SDK_INCOMPATIBLE:
					{
						this.OnFirmWareSDKIncompatibleInvoker();
						break;
					}
				case STSWMsgType.STSW_MSG_FW_SDK_CUSTOMERCODE_INCOMPATIBLE:
					{
						this.OnFirmWareSDKCustomercodeIncompatibleInvoker();
						break;
					}
				case STSWMsgType.STSW_MSG_NETWORK_SCAN_LIST_UPDATED:
					{
						this.ProcessNetworkScanListUpdated(structure.data);
						break;
					}
				case STSWMsgType.STSW_MSG_FAILED_READ_BOX_MMS:
					{
						this.OnFailedReadBoxMmsInvoker();
						break;
					}
			}
		}

		private void ProcessMMSUpdated(IntPtr dataPtr)
		{
			this.OnMMSUpdatedInvoker();
		}

		private void ProcessNetworkScanListUpdated(IntPtr dataPtr)
		{
			List<RIPENetworkScanListParamsType> rIPENetworkScanListParamsTypes = new List<RIPENetworkScanListParamsType>();
			for (IntPtr i = SkyTrakSW.InternalInterface.STSWNetworkScanListGetHead(dataPtr); SkyTrakSW.InternalInterface.STSWNetworkScanListNodeIsEnd(i) == 0; i = SkyTrakSW.InternalInterface.STSWNetworkScanListNodeGetNext(i))
			{
				RIPENetworkScanListParamsType rIPENetworkScanListParamsType = new RIPENetworkScanListParamsType();
				SkyTrakSW.InternalInterface.STSWNetworkScanListNodeGetData(i, ref rIPENetworkScanListParamsType);
				rIPENetworkScanListParamsTypes.Add(rIPENetworkScanListParamsType);
			}
			this.OnNetworkScanListUpdatedInvoker(rIPENetworkScanListParamsTypes);
		}

		STSWShotParamsType _ShotParams;

		private void ProcessShotEnded(IntPtr dataPtr)
		{
			STSWShotParamsType structure = _ShotParams;
			//这个接口也炸了
			//STSWShotParamsType structure = (STSWShotParamsType)Marshal.PtrToStructure(dataPtr, typeof(STSWShotParamsType));
			this.OnShotEndedInvoker(structure);
		}

		private void eventCatch(string str)
		{
			if (Regex.IsMatch(str, "RIPE_EVT_DATA_TRIGGERDETECTED"))
			{
				_ShotParams = new STSWShotParamsType();
				GameEntry.Event.Fire(null, new DeviceShotStateEventArgs(DeviceShotStateEventArgs.DeviceShotState.Start));
			}
			if (Regex.IsMatch(str, "Got RIPE_EVT_DATA_SPEEDREADY"))
			{
				string[] strParams = new string[7];
				int index = 0;
				Regex regex = new Regex(@"[=][-]?\d*\.\d*");
				foreach (Match match in regex.Matches(str))
				{
					strParams[index] = match.Value.Remove(0, 1);
					index++;
				}

				_ShotParams.speedParams.totalSpeed = float.Parse(strParams[0]);
				_ShotParams.speedParams.totalSpeedConfidence = float.Parse(strParams[1]);
				_ShotParams.speedParams.launchAngle = float.Parse(strParams[2]);
				_ShotParams.speedParams.launchAngleConfidence = float.Parse(strParams[3]);
				_ShotParams.speedParams.horizontalAngle = float.Parse(strParams[4]);
				_ShotParams.speedParams.horizontalAngleConfidence = float.Parse(strParams[5]);
				strParams[6] = strParams[6].Remove(1);
				_ShotParams.speedParams.startBallPositionStatus = (RIPEBoxBallPositionType)int.Parse(strParams[6]);

				if (_ShotParams.speedParams.horizontalAngleConfidence < 0.5)
					GameEntry.Event.Fire(null, new DeviceShotStateEventArgs(DeviceShotStateEventArgs.DeviceShotState.Error));
			}
			if (Regex.IsMatch(str, "Got RIPE_EVT_DATA_SPINREADY"))
			{
				string[] strParams = new string[5];
				int index = 0;
				Regex regex = new Regex(@"[=][-]?\d*\.\d*");
				foreach (Match match in regex.Matches(str))
				{
					strParams[index] = match.Value.Remove(0, 1);
					index++;
				}

				_ShotParams.spinParams.totalSpin = float.Parse(strParams[0]);
				_ShotParams.spinParams.backSpin = float.Parse(strParams[1]);
				_ShotParams.spinParams.sideSpin = float.Parse(strParams[2]);
				_ShotParams.spinParams.spinAxis = float.Parse(strParams[3]);
				_ShotParams.spinParams.measurementConfidence = float.Parse(strParams[4]);

				if (_ShotParams.spinParams.measurementConfidence < 0.5)
					GameEntry.Event.Fire(null, new DeviceShotStateEventArgs(DeviceShotStateEventArgs.DeviceShotState.Error));
				else
					GameEntry.Event.Fire(null, new DeviceShotStateEventArgs(DeviceShotStateEventArgs.DeviceShotState.End));
			}
		}

		private void ProcessShotStarted(IntPtr dataPtr)
		{
			this.OnShotStartedInvoker();
		}

		private void ProcessStartedDisconnection(IntPtr dataPtr)
		{
			this.OnStartedDisconnectionInvoker();
		}

		private void ProcessStatusUpdated(IntPtr dataPtr)
		{
			RIPEBoxParamsType structure = (RIPEBoxParamsType)Marshal.PtrToStructure(dataPtr, typeof(RIPEBoxParamsType));
			this.OnStatusUpdatedInvoker(structure);
		}

		public void Resume()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxConnectionResume(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could resume RIPE");
			}
		}

		public void ScanWiFiNetwork()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxScanNetwork(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Couldscan network with box");
			}
		}

		public void SetBoxMmsReadMode(STSWBoxMmsReadModeType mode)
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWSetBoxMMSReadMode(this.stswHandle, mode);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, string.Concat("Could not set box MMS Read Mode - ", rIPEErrType.ToString()));
			}
		}

		public void UpdateConnectedBoxMMSData()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWUpdateMMS(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could not update MMS. Probably box is not connected");
			}
		}

		public void UpgradeFirmWare()
		{
			RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWBoxUpgradeFirmWare(this.stswHandle);
			if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
			{
				throw new STSWRunTimeException(rIPEErrType, "Could start firwware upgrade");
			}
		}

		public event Action OnConnected
		{
			add
			{
				this.OnConnectedInvoker += value;
			}
			remove
			{
				this.OnConnectedInvoker -= value;
			}
		}

		public event Action<RIPEErrType> OnConnectFail
		{
			add
			{
				this.OnConnectFailInvoker += value;
			}
			remove
			{
				this.OnConnectFailInvoker -= value;
			}
		}

		public event Action OnDisconnected
		{
			add
			{
				this.OnDisconnectedInvoker += value;
			}
			remove
			{
				this.OnDisconnectedInvoker -= value;
			}
		}

		public event Action<RIPEErrType> OnDiscoverFail
		{
			add
			{
				this.OnDiscoverFailInvoker += value;
			}
			remove
			{
				this.OnDiscoverFailInvoker -= value;
			}
		}

		public event Action<List<RIPECommBoxDataParamsType>> OnDiscoverUpdated
		{
			add
			{
				this.OnDiscoverUpdatedInvoker += value;
			}
			remove
			{
				this.OnDiscoverUpdatedInvoker -= value;
			}
		}

		public event Action OnFailReadBoxMms
		{
			add
			{
				this.OnFailedReadBoxMmsInvoker += value;
			}
			remove
			{
				this.OnFailedReadBoxMmsInvoker -= value;
			}
		}

		public event Action OnFirmWareSDKCustomercodeIncompatible
		{
			add
			{
				this.OnFirmWareSDKCustomercodeIncompatibleInvoker += value;
			}
			remove
			{
				this.OnFirmWareSDKCustomercodeIncompatibleInvoker -= value;
			}
		}

		public event Action OnFirmWareSDKIncompatible
		{
			add
			{
				this.OnFirmWareSDKIncompatibleInvoker += value;
			}
			remove
			{
				this.OnFirmWareSDKIncompatibleInvoker -= value;
			}
		}

		public event Action OnFirmWareUpgradeAvailable
		{
			add
			{
				this.OnFirmWareUpgradeAvailableInvoker += value;
			}
			remove
			{
				this.OnFirmWareUpgradeAvailableInvoker -= value;
			}
		}

		public event Action OnFirmWareUpgradeError
		{
			add
			{
				this.OnFirmWareUpgradeErrorInvoker += value;
			}
			remove
			{
				this.OnFirmWareUpgradeErrorInvoker -= value;
			}
		}

		public event Action OnFirmWareUpgradeStarted
		{
			add
			{
				this.OnFirmWareUpgradeStartedInvoker += value;
			}
			remove
			{
				this.OnFirmWareUpgradeStartedInvoker -= value;
			}
		}

		public event Action OnFirmWareUpgradeSuccess
		{
			add
			{
				this.OnFirmWareUpgradeSuccessInvoker += value;
			}
			remove
			{
				this.OnFirmWareUpgradeSuccessInvoker -= value;
			}
		}

		public event Action OnMMSUpdated
		{
			add
			{
				this.OnMMSUpdatedInvoker += value;
			}
			remove
			{
				this.OnMMSUpdatedInvoker -= value;
			}
		}

		public event Action<List<RIPENetworkScanListParamsType>> OnNetworkScanListUpdated
		{
			add
			{
				this.OnNetworkScanListUpdatedInvoker += value;
			}
			remove
			{
				this.OnNetworkScanListUpdatedInvoker -= value;
			}
		}

		public event Action<STSWShotParamsType> OnShotEnded
		{
			add
			{
				this.OnShotEndedInvoker += value;
			}
			remove
			{
				this.OnShotEndedInvoker -= value;
			}
		}

		public event Action OnShotStarted
		{
			add
			{
				this.OnShotStartedInvoker += value;
			}
			remove
			{
				this.OnShotStartedInvoker -= value;
			}
		}

		public event Action OnStartedDisconnection
		{
			add
			{
				this.OnStartedDisconnectionInvoker += value;
			}
			remove
			{
				this.OnStartedDisconnectionInvoker -= value;
			}
		}

		public event Action<RIPEBoxParamsType> OnStatusUpdated
		{
			add
			{
				this.OnStatusUpdatedInvoker += value;
			}
			remove
			{
				this.OnStatusUpdatedInvoker -= value;
			}
		}

		private class InternalInterface
		{
			public const string STSWIMPORT = "SkyTrakSW";

			public InternalInterface()
			{
			}

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWAddBoxToCache(IntPtr stswHnd, IntPtr esn);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxArm(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxBootForceDirectConnect(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxBootForceNetworkConnect(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxConnect(IntPtr stswHnd, IntPtr boxName);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxConnectionPause(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxConnectionResume(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxDisarm(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxDisconnect(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxGetFirmwareUpgradeProgress(IntPtr stswHnd, UIntPtr progressPercent);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxGetShotsData(IntPtr stswHnd, ref STSWBoxShotsDataType shotsData);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern IntPtr STSWBoxListGetHead(IntPtr boxList);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern void STSWBoxListNodeGetData(IntPtr node, ref RIPECommBoxDataParamsType boxData);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern IntPtr STSWBoxListNodeGetNext(IntPtr node);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern int STSWBoxListNodeIsEnd(IntPtr node);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxScanNetwork(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxSetAssistAlignmentOff(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxSetAssistAlignmentOn(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxSetHandedness(IntPtr stswHnd, RIPEBoxHandednessStateType handedness);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxSetNetworkConfig(IntPtr stswHnd, ref RIPENetworkConfigType networkConfigData);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxSetShotModeNormal(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxSetShotModePutting(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWBoxUpgradeFirmWare(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWCreateFileLogger(ref IntPtr fileLoggerPtr, IntPtr ptr, int append);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWDiscover(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, EntryPoint = "STSWDeInit", ExactSpelling = false)]
			public static extern RIPEErrType STSWEDeInit(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorInitMMS(IntPtr stswHnd, IntPtr esn);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorPackMMS(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorReadMMSAllFlags(IntPtr stswHnd, IntPtr buf, int bufSize);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorReadMMSFlag(IntPtr stswHnd, STSWMMSFeatureFlagType flag, ref int value);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorReadMMSInfoInt(IntPtr stswHnd, STSWMMSInfoType info, ref int prop);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorReadMMSInfoInt64(IntPtr stswHnd, STSWMMSInfoType info, ref long prop);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorReadMMSInfoStr(IntPtr stswHnd, STSWMMSInfoType info, ref IntPtr ptr);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorUnpackMMS(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorWriteMMSFlag(IntPtr stswHnd, STSWMMSFeatureFlagType flag, int value);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorWriteMMSInfoInt(IntPtr stswHnd, STSWMMSInfoType info, int prop);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorWriteMMSInfoInt64(IntPtr stswHnd, STSWMMSInfoType info, long prop);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWEmulatorWriteMMSInfoStr(IntPtr stswHnd, STSWMMSInfoType info, IntPtr ptr);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWFreeFileLogger(IntPtr fileLoggerPtr);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWFreeMMS(IntPtr mmsPtr);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWFreeMsg(IntPtr msg);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWGetBoxConnectionType(IntPtr stswHnd, ref RIPEBoxConnectionType connectionType);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWGetBoxFWVersion(IntPtr stswHnd, ref RIPEVersion version);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWGetMMS(IntPtr stswHnd, ref IntPtr mmsPtr);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWGetMMSValidationResult(IntPtr mmsPtr, ref STSWMMSValidationResultDataType data);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWGetRIPEVersion(IntPtr stswHnd, ref RIPEVersion version);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWGetState(IntPtr stswHnd, ref STSWStateType state);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWGetVersion(ref RIPEVersion version);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWInit(ref IntPtr stswHnd, ref STSWInitType initData);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWInitEx(ref IntPtr stswHnd, ref STSWInitType initData, ref STSWAbstLoggerType loggerData);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern IntPtr STSWNetworkScanListGetHead(IntPtr networkScanList);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern void STSWNetworkScanListNodeGetData(IntPtr node, ref RIPENetworkScanListParamsType boxData);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern IntPtr STSWNetworkScanListNodeGetNext(IntPtr node);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern int STSWNetworkScanListNodeIsEnd(IntPtr node);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWPerform(IntPtr stswHnd, ref int msgNum);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWReadMMSAllFlags(IntPtr mmsPtr, IntPtr buf, int bufSize);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWReadMMSFlag(IntPtr mmsPtr, STSWMMSFeatureFlagType flag, ref int value);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWReadMMSInfoInt(IntPtr mmsPtr, STSWMMSInfoType info, ref int prop);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWReadMMSInfoInt64(IntPtr mmsPtr, STSWMMSInfoType info, ref long prop);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWReadMMSInfoStr(IntPtr mmsPtr, STSWMMSInfoType info, ref IntPtr ptr);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern IntPtr STSWReadMsg(IntPtr stswHnd);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWSetBoxMMSReadMode(IntPtr stswHnd, STSWBoxMmsReadModeType readMode);

			[DllImport("SkyTrakSW", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
			public static extern RIPEErrType STSWUpdateMMS(IntPtr stswHnd);
		}

		public class MMSData : IDisposable
		{
			private IntPtr stswMMSPtr = IntPtr.Zero;

			private IntPtr skyTrakSWPtr = IntPtr.Zero;

			public int CustomerID
			{
				get
				{
					return this.ReadInt(STSWMMSInfoType.STSW_MMS_INFO_CUSTOMER_ID);
				}
				set
				{
					this.WriteInt(STSWMMSInfoType.STSW_MMS_INFO_CUSTOMER_ID, value);
				}
			}

			public byte DataFormatVersion
			{
				get
				{
					return (byte)0;
				}
			}

			public byte DeviceStatusFlag
			{
				get
				{
					return Convert.ToByte(this.ReadInt(STSWMMSInfoType.STSW_MMS_INFO_DEVICE_STATUS_FLAG));
				}
				set
				{
					this.WriteInt(STSWMMSInfoType.STSW_MMS_INFO_DEVICE_STATUS_FLAG, Convert.ToInt32(value));
				}
			}

			public string ESN
			{
				get
				{
					return this.ReadStr(STSWMMSInfoType.STSW_MMS_INFO_ESN);
				}
			}

			public string ESNVerificationCode
			{
				get
				{
					return this.ReadStr(STSWMMSInfoType.STSW_MMS_INFO_ESN_VERIFICATION_CODE);
				}
			}

			public BitArray FeatureFlags
			{
				get
				{
					int num = 32;
					byte[] numArray = new byte[num];
					IntPtr intPtr = Marshal.AllocHGlobal(num);
					RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
					if (this.stswMMSPtr != IntPtr.Zero)
					{
						rIPEErrType = SkyTrakSW.InternalInterface.STSWReadMMSAllFlags(this.stswMMSPtr, intPtr, num);
					}
					else if (this.skyTrakSWPtr != IntPtr.Zero)
					{
						rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorReadMMSAllFlags(this.skyTrakSWPtr, intPtr, num);
					}
					if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
					{
						Marshal.FreeHGlobal(intPtr);
						throw new STSWRunTimeException(rIPEErrType, "Could not read MMS all Flags");
					}
					Marshal.Copy(intPtr, numArray, 0, num);
					Marshal.FreeHGlobal(intPtr);
					return SkyTrakSW.MMSData.GetBitArray(numArray, 0, num);
				}
			}

			public DateTime FirstUseDate
			{
				get
				{
					return SkyTrakSW.MMSData.DateTimeFromSecondsSinceEpoch(this.ReadInt64(STSWMMSInfoType.STSW_MMS_INFO_FIRST_USE_DATE));
				}
				set
				{
					this.WriteInt64(STSWMMSInfoType.STSW_MMS_INFO_FIRST_USE_DATE, SkyTrakSW.MMSData.DateTimeToSecondsSinceEpoch(value));
				}
			}

			public int GraceDays
			{
				get
				{
					return this.ReadInt(STSWMMSInfoType.STSW_MMS_INFO_GRACE_DAYS);
				}
				set
				{
					this.WriteInt(STSWMMSInfoType.STSW_MMS_INFO_GRACE_DAYS, value);
				}
			}

			public DateTime InitialRegistrationDate
			{
				get
				{
					return SkyTrakSW.MMSData.DateTimeFromSecondsSinceEpoch(this.ReadInt64(STSWMMSInfoType.STSW_MMS_INFO_INITIAL_REGISTRATION_DATE));
				}
				set
				{
					this.WriteInt64(STSWMMSInfoType.STSW_MMS_INFO_INITIAL_REGISTRATION_DATE, SkyTrakSW.MMSData.DateTimeToSecondsSinceEpoch(value));
				}
			}

			public SkyTrakSW.MMSLicense License
			{
				get
				{
					STSWMMSValidationResultDataType sTSWMMSValidationResultDataType = new STSWMMSValidationResultDataType();
					RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
					if (this.stswMMSPtr != IntPtr.Zero)
					{
						rIPEErrType = SkyTrakSW.InternalInterface.STSWGetMMSValidationResult(this.stswMMSPtr, ref sTSWMMSValidationResultDataType);
					}
					if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
					{
						throw new STSWRunTimeException(rIPEErrType, "Could not read MMS License");
					}
					return new SkyTrakSW.MMSLicense(sTSWMMSValidationResultDataType);
				}
			}

			public string MemberFirstName
			{
				get
				{
					return this.ReadStr(STSWMMSInfoType.STSW_MMS_INFO_MEMBER_FIRST_NAME);
				}
				set
				{
					this.WriteStr(STSWMMSInfoType.STSW_MMS_INFO_MEMBER_FIRST_NAME, value);
				}
			}

			public string MemberLastName
			{
				get
				{
					return this.ReadStr(STSWMMSInfoType.STSW_MMS_INFO_MEMBER_LAST_NAME);
				}
				set
				{
					this.WriteStr(STSWMMSInfoType.STSW_MMS_INFO_MEMBER_LAST_NAME, value);
				}
			}

			public string MemberPhone
			{
				get
				{
					return this.ReadStr(STSWMMSInfoType.STSW_MMS_INFO_MEMBER_PHONE);
				}
				set
				{
					this.WriteStr(STSWMMSInfoType.STSW_MMS_INFO_MEMBER_PHONE, value);
				}
			}

			public DateTime MembershipExpireDate
			{
				get
				{
					return SkyTrakSW.MMSData.DateTimeFromSecondsSinceEpoch(this.ReadInt64(STSWMMSInfoType.STSW_MMS_INFO_MEMBERSHIP_EXPIRE_DATE));
				}
				set
				{
					this.WriteInt64(STSWMMSInfoType.STSW_MMS_INFO_MEMBERSHIP_EXPIRE_DATE, SkyTrakSW.MMSData.DateTimeToSecondsSinceEpoch(value));
				}
			}

			public string MembershipName
			{
				get
				{
					return this.ReadStr(STSWMMSInfoType.STSW_MMS_INFO_MEMBERSHIP_NAME);
				}
				set
				{
					this.WriteStr(STSWMMSInfoType.STSW_MMS_INFO_MEMBERSHIP_NAME, value);
				}
			}

			public DateTime MembershipPackDate
			{
				get
				{
					return SkyTrakSW.MMSData.DateTimeFromSecondsSinceEpoch(this.ReadInt64(STSWMMSInfoType.STSW_MMS_INFO_MEMBERSHIP_PACK_DATE));
				}
				set
				{
					this.WriteInt64(STSWMMSInfoType.STSW_MMS_INFO_MEMBERSHIP_PACK_DATE, SkyTrakSW.MMSData.DateTimeToSecondsSinceEpoch(value));
				}
			}

			public byte MembershipType
			{
				get
				{
					return Convert.ToByte(this.ReadInt(STSWMMSInfoType.STSW_MMS_INFO_MEMBERSHIP_TYPE));
				}
				set
				{
					this.WriteInt(STSWMMSInfoType.STSW_MMS_INFO_MEMBERSHIP_TYPE, Convert.ToInt32(value));
				}
			}

			public int NewSystemGraceDays
			{
				get
				{
					return this.ReadInt(STSWMMSInfoType.STSW_MMS_INFO_NEW_SYSTEM_GRACE_DAYS);
				}
				set
				{
					this.WriteInt(STSWMMSInfoType.STSW_MMS_INFO_NEW_SYSTEM_GRACE_DAYS, value);
				}
			}

			public bool NewUnit
			{
				get
				{
					return this.ReadInt(STSWMMSInfoType.STSW_MMS_INFO_NEW_UNIT) != 0;
				}
				set
				{
					this.WriteInt(STSWMMSInfoType.STSW_MMS_INFO_NEW_UNIT, Convert.ToInt32(value));
				}
			}

			public string QuickAccessCode
			{
				get
				{
					return this.ReadStr(STSWMMSInfoType.STSW_MMS_INFO_QUICK_ACCESS_CODE);
				}
				set
				{
					this.WriteStr(STSWMMSInfoType.STSW_MMS_INFO_QUICK_ACCESS_CODE, value);
				}
			}

			public bool Registered
			{
				get
				{
					return Convert.ToBoolean(this.ReadInt(STSWMMSInfoType.STSW_MMS_INFO_REGISTERED));
				}
				set
				{
					this.WriteInt(STSWMMSInfoType.STSW_MMS_INFO_REGISTERED, Convert.ToInt32(value));
				}
			}

			public int TotalShotsTaken
			{
				get
				{
					return this.ReadInt(STSWMMSInfoType.STSW_MMS_INFO_TOTAL_SHOTS_TAKEN);
				}
				set
				{
					this.WriteInt(STSWMMSInfoType.STSW_MMS_INFO_TOTAL_SHOTS_TAKEN, value);
				}
			}

			public DateTime TotalShotsTakenLastUpdatedDate
			{
				get
				{
					return SkyTrakSW.MMSData.DateTimeFromSecondsSinceEpoch(this.ReadInt64(STSWMMSInfoType.STSW_MMS_INFO_TOTAL_SHOTS_TAKEN_LAST_UPDATE_DATE));
				}
				set
				{
					this.WriteInt64(STSWMMSInfoType.STSW_MMS_INFO_TOTAL_SHOTS_TAKEN_LAST_UPDATE_DATE, SkyTrakSW.MMSData.DateTimeToSecondsSinceEpoch(value));
				}
			}

			public string UnitType
			{
				get
				{
					return this.ReadStr(STSWMMSInfoType.STSW_MMS_INFO_UNIT_TYPE);
				}
				set
				{
					this.WriteStr(STSWMMSInfoType.STSW_MMS_INFO_UNIT_TYPE, value);
				}
			}

			public MMSData(IntPtr MMSPtr)
			{
				this.stswMMSPtr = MMSPtr;
			}

			private void CloseSTSWMMS()
			{
				if (this.stswMMSPtr == IntPtr.Zero)
				{
					return;
				}
				SkyTrakSW.InternalInterface.STSWFreeMMS(this.stswMMSPtr);
				this.stswMMSPtr = IntPtr.Zero;
			}

			public static SkyTrakSW.MMSData CreateEmulatorMMS(IntPtr stswPtr)
			{
				return new SkyTrakSW.MMSData(IntPtr.Zero)
				{
					skyTrakSWPtr = stswPtr
				};
			}

			private static DateTime DateTimeFromSecondsSinceEpoch(long secondsSinceEpoch)
			{
				DateTime dateTime;
				try
				{
					DateTime dateTime1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					dateTime = dateTime1.AddSeconds((double)secondsSinceEpoch);
				}
				catch (Exception exception)
				{
					if (SkyTrakSW.Instance.externLogger != null)
					{
						SkyTrakSW.Instance.externLogger.Warning(string.Concat("Exception during DateTimeFromSecondsSinceEpoch (secondsSinceEpoch = ", secondsSinceEpoch, ")"));
					}
					dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				}
				return dateTime;
			}

			private static long DateTimeToSecondsSinceEpoch(DateTime dt)
			{
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				return Convert.ToInt64(dt.Subtract(dateTime).TotalSeconds);
			}

			public void Dispose()
			{
				this.CloseSTSWMMS();
				GC.SuppressFinalize(this);
			}

			~MMSData()
			{
				this.CloseSTSWMMS();
			}

			private static BitArray GetBitArray(byte[] byteArray, int startIndex, int nbrBytes)
			{
				BitArray bitArrays = new BitArray(8 * nbrBytes);
				int num = startIndex;
				int num1 = 0;
				while (num < startIndex + nbrBytes)
				{
					for (int i = 0; i < 8; i++)
					{
						bitArrays[i + 8 * num1] = (byteArray[num] & 1 << (i & 31)) > 0;
					}
					num1++;
					num++;
				}
				return bitArrays;
			}

			public bool GetFeatureFlag(STSWMMSFeatureFlagType flag)
			{
				return this.ReadFlag(flag);
			}

			private bool GetFlagValue(STSWMMSFeatureFlagType flag)
			{
				if (this.FeatureFlags == null || this.FeatureFlags.Count < (int)flag + (int)STSWMMSFeatureFlagType.Module_Challenges)
				{
					return false;
				}
				return this.FeatureFlags.Get((int)flag);
			}

			public bool ReadFlag(STSWMMSFeatureFlagType flag)
			{
				int num = 0;
				RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
				if (this.stswMMSPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWReadMMSFlag(this.stswMMSPtr, flag, ref num);
				}
				else if (this.skyTrakSWPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorReadMMSFlag(this.skyTrakSWPtr, flag, ref num);
				}
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					throw new STSWRunTimeException(rIPEErrType, string.Concat("Could not read MMS flag with code: ", flag.ToString("D")));
				}
				return num != 0;
			}

			private int ReadInt(STSWMMSInfoType info)
			{
				int num = 0;
				RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
				if (this.stswMMSPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWReadMMSInfoInt(this.stswMMSPtr, info, ref num);
				}
				else if (this.skyTrakSWPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorReadMMSInfoInt(this.skyTrakSWPtr, info, ref num);
				}
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					throw new STSWRunTimeException(rIPEErrType, string.Concat("Could not read MMS property with code: ", info.ToString("D")));
				}
				return num;
			}

			private long ReadInt64(STSWMMSInfoType info)
			{
				long num = (long)0;
				RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
				if (this.stswMMSPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWReadMMSInfoInt64(this.stswMMSPtr, info, ref num);
				}
				else if (this.skyTrakSWPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorReadMMSInfoInt64(this.skyTrakSWPtr, info, ref num);
				}
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					throw new STSWRunTimeException(rIPEErrType, string.Concat("Could not read MMS property with code: ", info.ToString("D")));
				}
				return num;
			}

			private string ReadStr(STSWMMSInfoType info)
			{
				IntPtr zero = IntPtr.Zero;
				RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
				if (this.stswMMSPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWReadMMSInfoStr(this.stswMMSPtr, info, ref zero);
				}
				else if (this.skyTrakSWPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorReadMMSInfoStr(this.skyTrakSWPtr, info, ref zero);
				}
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS || zero == IntPtr.Zero)
				{
					throw new STSWRunTimeException(rIPEErrType, string.Concat("Could not read MMS property with code: ", info.ToString("D")));
				}
				return Marshal.PtrToStringAnsi(zero);
			}

			public void SetFeatureFlag(STSWMMSFeatureFlagType flag, bool val)
			{
				this.WriteFlag(flag, val);
			}

			public override string ToString()
			{
				JSONObject jSONObject = new JSONObject();
				jSONObject.AddField("CustomerID", this.CustomerID.ToString());
				jSONObject.AddField("DataFormatVersion", this.DataFormatVersion.ToString());
				jSONObject.AddField("Registered", this.Registered);
				jSONObject.AddField("MembershipType", this.MembershipType.ToString());
				jSONObject.AddField("MembershipName", (this.MembershipName != null ? this.MembershipName.Replace("\0", string.Empty) : "null"));
				jSONObject.AddField("GraceDays", this.GraceDays.ToString());
				jSONObject.AddField("NewUnit", this.NewUnit);
				jSONObject.AddField("UnitType", (this.UnitType != null ? this.UnitType.Replace("\0", string.Empty) : "null"));
				jSONObject.AddField("QuickAccessCode", (this.QuickAccessCode != null ? this.QuickAccessCode.Replace("\0", string.Empty) : "null"));
				jSONObject.AddField("MemberFirstName", (this.MemberFirstName != null ? this.MemberFirstName.Replace("\0", string.Empty) : "null"));
				jSONObject.AddField("MemberLastName", (this.MemberLastName != null ? this.MemberLastName.Replace("\0", string.Empty) : "null"));
				jSONObject.AddField("MemberPhone", (this.MemberPhone != null ? this.MemberPhone.Replace("\0", string.Empty) : "null"));
				jSONObject.AddField("NewSystemGraceDays", this.NewSystemGraceDays.ToString());
				jSONObject.AddField("ESNVerificationCode", (this.ESNVerificationCode != null ? this.ESNVerificationCode.Replace("\0", string.Empty) : "null"));
				jSONObject.AddField("TotalShotsTaken", this.TotalShotsTaken.ToString());
				jSONObject.AddField("TotalShotsTakenLastUpdatedDate", this.TotalShotsTakenLastUpdatedDate.ToString("G"));
				jSONObject.AddField("FirstUseDate", this.FirstUseDate.ToString("G"));
				jSONObject.AddField("MembershipPackDate", this.MembershipPackDate.ToString("G"));
				jSONObject.AddField("MembershipExpireDate", this.MembershipExpireDate.ToString("G"));
				jSONObject.AddField("InitialRegistrationDate", this.InitialRegistrationDate.ToString("G"));
				jSONObject.AddField("DeviceStatusFlag", this.DeviceStatusFlag.ToString());
				for (int i = 0; i < Enum.GetValues(typeof(STSWMMSFeatureFlagType)).Length; i++)
				{
					STSWMMSFeatureFlagType sTSWMMSFeatureFlagType = (STSWMMSFeatureFlagType)i;
					bool flagValue = this.GetFlagValue(sTSWMMSFeatureFlagType);
					int num = (int)sTSWMMSFeatureFlagType;
					jSONObject.AddField(string.Format("({0}){1}", num.ToString(), sTSWMMSFeatureFlagType.ToString()), flagValue);
				}
				return jSONObject.ToString();
			}

			public void WriteFlag(STSWMMSFeatureFlagType flag, bool val)
			{
				int num = 0;
				if (val)
				{
					num = 1;
				}
				RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
				if (this.skyTrakSWPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorWriteMMSFlag(this.skyTrakSWPtr, flag, num);
				}
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					throw new STSWRunTimeException(rIPEErrType, string.Concat("Could not write MMS flag with code: ", flag.ToString("D")));
				}
			}

			private void WriteInt(STSWMMSInfoType info, int val)
			{
				RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
				if (this.skyTrakSWPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorWriteMMSInfoInt(this.skyTrakSWPtr, info, val);
				}
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					throw new STSWRunTimeException(rIPEErrType, string.Concat("Could not write MMS property with code: ", info.ToString("D")));
				}
			}

			private void WriteInt64(STSWMMSInfoType info, long val)
			{
				RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
				if (this.skyTrakSWPtr != IntPtr.Zero)
				{
					rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorWriteMMSInfoInt64(this.skyTrakSWPtr, info, val);
				}
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					throw new STSWRunTimeException(rIPEErrType, string.Concat("Could not write MMS property with code: ", info.ToString("D")));
				}
			}

			private void WriteStr(STSWMMSInfoType info, string val)
			{
				IntPtr zero = IntPtr.Zero;
				RIPEErrType rIPEErrType = RIPEErrType.RIPE_ERR_UNAVAILABLE;
				if (this.skyTrakSWPtr != IntPtr.Zero)
				{
					IntPtr hGlobalAnsi = Marshal.StringToHGlobalAnsi(val);
					rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorWriteMMSInfoStr(this.skyTrakSWPtr, info, hGlobalAnsi);
					Marshal.FreeHGlobal(hGlobalAnsi);
				}
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					throw new STSWRunTimeException(rIPEErrType, string.Concat("Could not write MMS property with code: ", info.ToString("D")));
				}
			}
		}

		public class MMSLicense
		{
			private STSWMMSValidationResultDataType validationData;

			private string header;

			private string body;

			public string Body
			{
				get
				{
					return this.body;
				}
			}

			public int DaysLeftInCurrentPeriod
			{
				get
				{
					return this.validationData.dayLeft;
				}
			}

			public string Header
			{
				get
				{
					return this.header;
				}
			}

			public STSWMMSValidationResultType ValidationResult
			{
				get
				{
					return this.validationData.result;
				}
			}

			public MMSLicense(STSWMMSValidationResultDataType vd)
			{
				this.validationData = vd;
				switch (this.validationData.result)
				{
					case STSWMMSValidationResultType.STSW_VALIDATION_LICENSE_IS_ABOUT_TO_EXPIRE:
						{
							this.header = string.Format("Your Membership is About to Expire", new object[0]);
							this.body = string.Format("You have {0} days prior to membership expiration.", this.validationData.dayLeft);
							break;
						}
					case STSWMMSValidationResultType.STSW_VALIDATION_LICENSE_GRACE_PERIOD:
						{
							this.header = string.Format("Your Membership has Expired", new object[0]);
							this.body = string.Format("You have {0} days to renew your membership.", this.validationData.dayLeft);
							break;
						}
					case STSWMMSValidationResultType.STSW_VALIDATION_LICENSE_EXPIRED:
						{
							this.header = string.Empty;
							this.body = string.Empty;
							break;
						}
					case STSWMMSValidationResultType.STSW_VALIDATION_DEVICE_NOT_REGISTERED_TRIAL:
						{
							this.header = string.Format("30 Day Trial Period", new object[0]);
							this.body = string.Format("You have {0} days to register your SkyTrak unit before the software is disabled.", this.validationData.dayLeft);
							break;
						}
					case STSWMMSValidationResultType.STSW_VALIDATION_DEVICE_NOT_REGISTERED_BLOCK:
						{
							this.header = string.Format("30 Day Trial Period has ended", new object[0]);
							this.body = string.Format("You must register your SkyTrak unit to re-enable software.", new object[0]);
							break;
						}
					default:
						{
							goto case STSWMMSValidationResultType.STSW_VALIDATION_LICENSE_EXPIRED;
						}
				}
			}
		}

		public class RIPEEmulator
		{
			private IntPtr stswHandle = IntPtr.Zero;

			private bool isMMSReady;

			private SkyTrakSW.MMSData memoryMMS;

			public SkyTrakSW.MMSData MMS
			{
				get
				{
					if (!this.isMMSReady)
					{
						throw new STSWRunTimeException(RIPEErrType.RIPE_ERR_UNAVAILABLE, "MMS was not init or unpacked from emulator.");
					}
					return this.memoryMMS;
				}
			}

			public RIPEEmulator(IntPtr hnd)
			{
				this.stswHandle = hnd;
				this.memoryMMS = SkyTrakSW.MMSData.CreateEmulatorMMS(this.stswHandle);
			}

			public void InitDefaultMMS()
			{
				string str;
				RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorInitMMS(this.stswHandle, IntPtr.Zero);
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					str = (rIPEErrType != RIPEErrType.RIPE_ERR_UNAVAILABLE ? "SkyTrakSW internal error." : "This method available only in emulator mode.");
					throw new STSWRunTimeException(rIPEErrType, str);
				}
				this.isMMSReady = true;
			}

			public void InitMMS(string hex)
			{
				string str;
				IntPtr hGlobalAnsi = Marshal.StringToHGlobalAnsi(hex);
				RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorInitMMS(this.stswHandle, hGlobalAnsi);
				Marshal.FreeHGlobal(hGlobalAnsi);
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					if (rIPEErrType != RIPEErrType.RIPE_ERR_UNAVAILABLE)
					{
						str = (rIPEErrType != RIPEErrType.RIPE_ERR_PARAMETER_INVALID ? "SkyTrakSW internal error." : "Could not load MMS from your HEX.");
					}
					else
					{
						str = "This method available only in emulator mode.";
					}
					throw new STSWRunTimeException(rIPEErrType, str);
				}
				this.isMMSReady = true;
			}

			public void PackMMS()
			{
				string str;
				RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorPackMMS(this.stswHandle);
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					str = (rIPEErrType != RIPEErrType.RIPE_ERR_UNAVAILABLE ? "Could not load MMS from SkyTrakSW memory to emulator. Probably SkyTrakSW MMS is empty." : "This method available only in emulator mode.");
					throw new STSWRunTimeException(rIPEErrType, str);
				}
			}

			public void UnpackMMS()
			{
				string str;
				RIPEErrType rIPEErrType = SkyTrakSW.InternalInterface.STSWEmulatorUnpackMMS(this.stswHandle);
				if (rIPEErrType != RIPEErrType.RIPE_ERR_SUCCESS)
				{
					str = (rIPEErrType != RIPEErrType.RIPE_ERR_UNAVAILABLE ? "Could not load MMS from emulator to SkyTrakSW memory. Probably emulator MMS is empty." : "This method available only in emulator mode.");
					throw new STSWRunTimeException(rIPEErrType, str);
				}
				this.isMMSReady = true;
			}
		}
	}
}