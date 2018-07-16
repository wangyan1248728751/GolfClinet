using SkyTrakWrapper;
using System;
using System.Collections.Generic;

namespace Security
{
	public interface ISkyTrakSW
	{
		bool BatteryIsCharging
		{
			get;
		}

		float BoxBatteryPercent
		{
			get;
		}

		IEnumerable<RIPECommBoxDataParamsType> BoxList
		{
			get;
		}

		float BoxRoll
		{
			get;
		}

		float BoxTilt
		{
			get;
		}

		RIPEBoxConnectionType ConnectionType
		{
			get;
		}

		string ESN
		{
			get;
		}

		string FWVersion
		{
			get;
		}

		bool IsBoxForcedInAPMode
		{
			get;
		}

		bool IsConnected
		{
			get;
		}

		bool IsDiscovering
		{
			get;
		}

		bool IsLicenseChecked
		{
			get;
		}

		bool IsLicenseChecking
		{
			get;
		}

		bool IsMMSReady
		{
			get;
		}

		SkyTrakSW.MMSData MMSData
		{
			get;
		}

		int TotalShotsTaken
		{
			get;
		}

		void AddEsnToCache(string esn);

		void ArmBox();

		void BoxSetNetworkConfig(RIPENetworkConfigType config);

		void ConnectToDevice(string boxName);

		void DisarmBox();

		void ForceDirectConnect();

		void ForceNetworkConnect();

		void MakeTestShot(RIPESpeedParamsType speedParams, RIPESpinParamsType spinParams, RIPEFlightParamsType flightParams);

		void QueueUpdateMMS();

		void ScanWiFiNetworks();

		void SetAlignmentMode(bool set);

		void SetHandedness(bool isLefty);

		void SetShotModeNormal();

		void SetShotModePutting();

		void UpdateMMS();

		event Action<RIPESpeedParamsType, RIPESpinParamsType, RIPEFlightParamsType> OnBallLaunched;

		event Action OnConnected;

		event Action OnDisconnected;

		event Action OnFailReadBoxMms;

		event Action OnMMSUpdated;

		event Action<List<RIPENetworkScanListParamsType>> OnNetworkScanCompleted;
	}
}