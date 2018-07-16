using Security;
using SkyTrakWrapper;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public static class MembershipManager
{
	private const string HistoryFlagKey = "HistoryFlagKey";

	private const string StatsFlagKey = "StatsFlagKey";

	private static bool _lastHistoryFlag;

	private static bool _lastStatsFlag;

	private static SkyTrakSW.MMSData _mmsData;

	private static PermissionsSet _hasAccess;

	static MembershipManager()
	{
		MembershipManager.OnMMSUpdated = () => {
		};
		MembershipManager._mmsData = new SkyTrakSW.MMSData(IntPtr.Zero);
		MembershipManager._hasAccess = new PermissionsSet();
		MembershipManager.InitializePermissions();
		MembershipManager._lastHistoryFlag = PlayerPrefs.GetInt("HistoryFlagKey", 0) > 0;
		MembershipManager._lastStatsFlag = PlayerPrefs.GetInt("StatsFlagKey", 0) > 0;
	}

	public static bool GetAccess(STSWMMSFeatureFlagType flag)
	{
		//if (MonoSingleton<UIData>.Singleton.UpgradeRequiredWindowData.Ui.Root.activeSelf)
		//{
		//	MonoSingleton<UIData>.Singleton.UpgradeRequiredWindowData.Hide();
		//}
		if (SecurityWrapperService.Instance.IsConnected)
		{
			return MembershipManager._hasAccess.GetPermission(flag);
		}
		if (flag == STSWMMSFeatureFlagType.Profile_History)
		{
			return MembershipManager._lastHistoryFlag;
		}
		if (flag == STSWMMSFeatureFlagType.Profile_Stats)
		{
			return MembershipManager._lastStatsFlag;
		}
		return false;
	}

	public static void InitializePermissions()
	{
		MembershipManager._hasAccess.InitializeDefaults();
	}

	private static void SaveLastFlags()
	{
		MembershipManager._lastHistoryFlag = MembershipManager.GetAccess(STSWMMSFeatureFlagType.Profile_History);
		MembershipManager._lastStatsFlag = MembershipManager.GetAccess(STSWMMSFeatureFlagType.Profile_Stats);
		PlayerPrefs.SetInt("HistoryFlagKey", (!MembershipManager._lastHistoryFlag ? 0 : 1));
		PlayerPrefs.SetInt("StatsFlagKey", (!MembershipManager._lastStatsFlag ? 0 : 1));
	}

	public static void SetMMSData()
	{
		MembershipManager._mmsData = SecurityWrapperService.Instance.MMSData;
		MembershipManager._hasAccess.InitializeByBitArray(MembershipManager._mmsData.FeatureFlags);
		MembershipManager.OnMMSUpdated();
		MembershipManager.SaveLastFlags();
	}

	public static event Action OnMMSUpdated;
}