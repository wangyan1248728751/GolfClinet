using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace SkyTrakWrapper
{
	public enum STSWStateType
	{
		STSW_STATE_SLEEP,
		STSW_STATE_DISCOVERING,
		STSW_STATE_CONNECTING,
		STSW_STATE_CHECKING_LICENSE,
		STSW_STATE_CONNECTED
	}
	public enum STSWNetworkStateType
	{
		STSW_NETWORK_VERIFIED_OFFLINE = -1,
		STSW_NETWORK_UNAVAILABLE = 0,
		STSW_NETWORK_VERIFIED_ONLINE = 1
	}
	public enum STSWServerUrlMode
	{
		STSW_SERVER_URL_BETA_TESTING,
		STSW_SERVER_URL_QA,
		STSW_SERVER_URL_PRODUCTION
	}
	public enum RIPEOperationModeType
	{
		RIPE_MODE_DEFAULT,
		RIPE_MODE_LAST
	}
	public enum STSWLogLevelType
	{
		STSW_LOG_LEVEL_DEBUG,
		STSW_LOG_LEVEL_INFO,
		STSW_LOG_LEVEL_WARNING,
		STSW_LOG_LEVEL_ERROR
	}


	public struct STSWAbstLoggerType
	{
		public IntPtr handle;

		public LogMsgDelegate logMsgFunc;
	}
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void LogMsgDelegate(IntPtr handle, STSWLogLevelType level, IntPtr message);


	public struct RIPECommBoxDataParamsType
	{
		public string boxName;

		public string boxIP;

		public string adapterName;

		public string adapterIP;

		public RIPEAdapterType adapterType;

		public RIPEBoxConnectionType boxConnectionType;
	}

	public enum RIPEAdapterType
	{
		RIPE_INTERFACE_WIRELESS,
		RIPE_INTERFACE_ETHERNET,
		RIPE_INTERFACE_USB,
		RIPE_INTERFACE_UNKNOWN,
		RIPE_INTERFACE_LAST
	}

	public enum RIPEBoxConnectionType
	{
		RIPE_BOX_CONNECTION_UNKNOWN,
		RIPE_BOX_CONNECTION_DIRECT_MODE,
		RIPE_BOX_CONNECTION_NETWORK_MODE,
		RIPE_BOX_CONNECTION_USB_MODE,
		RIPE_BOX_CONNECTION_LAST
	}
	public struct RIPESpeedParamsType
	{
		public float totalSpeed;

		public float totalSpeedConfidence;

		public float launchAngle;

		public float launchAngleConfidence;

		public float horizontalAngle;

		public float horizontalAngleConfidence;

		public RIPEBoxBallPositionType startBallPositionStatus;
	}

	public enum RIPEBoxBallPositionType
	{
		RIPE_BALL_POSITION_OK,
		RIPE_BALL_POSITION_NEAR,
		RIPE_BALL_POSITION_FAR,
		RIPE_BALL_POSITION_UNKNOWN
	}

	public struct RIPESpinParamsType
	{
		public float totalSpin;

		public float backSpin;

		public float sideSpin;

		public float spinAxis;

		public float measurementConfidence;
	}

	public struct RIPEFlightParamsType
	{
		public float carry;

		public float side;

		public float maxHeight;

		public float flightDuration;

		public int validPointCount;

		public RIPEPointInSpaceType[] pts;
	}
	public struct RIPEPointInSpaceType
	{
		public double x;

		public double y;

		public double z;

		public double time;
	}

	public struct RIPEBoxParamsType
	{
		public RIPEBoxHandednessStateType handednessState;

		public RIPEBoxChargingStateType chargingState;

		public float batteryPercent;

		public float boxRoll;

		public float boxTilt;

		public int isBoxInAPMode;

		public int rssiLevel;
	}
	public enum RIPEBoxHandednessStateType
	{
		RIPE_HANDEDNESS_RIGHT,
		RIPE_HANDEDNESS_LEFT,
		RIPE_HANDEDNESS_LAST
	}
	public enum RIPEBoxChargingStateType
	{
		RIPE_BATT_NOT_CHARGING,
		RIPE_BATT_CHARGING
	}
	public struct RIPENetworkScanListParamsType
	{
		public string SSID;

		public float signalLevel;

		public string securityType;

		public byte isSupported;
	}

	public struct RIPEVersion
	{
		public float majorVer;

		public float minorVer;
	}
	public class STSWInitializationFailedException : Exception
	{
		public STSWInitializationFailedException()
		{
		}

		public STSWInitializationFailedException(string message) : base(message)
		{
		}

		public STSWInitializationFailedException(string message, Exception inner) : base(message, inner)
		{
		}

		protected STSWInitializationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
	public struct RIPENetworkConfigType
	{
		public int nodeToConfig;

		public string SSID;

		public string password;

		public string securityType;
	}
	public enum STSWMMSInfoType
	{
		STSW_MMS_INFO_REGISTERED,
		STSW_MMS_INFO_CUSTOMER_ID,
		STSW_MMS_INFO_MEMBERSHIP_TYPE,
		STSW_MMS_INFO_MEMBERSHIP_NAME,
		STSW_MMS_INFO_MEMBERSHIP_EXPIRE_DATE,
		STSW_MMS_INFO_GRACE_DAYS,
		STSW_MMS_INFO_NEW_UNIT,
		STSW_MMS_INFO_INITIAL_REGISTRATION_DATE,
		STSW_MMS_INFO_UNIT_TYPE,
		STSW_MMS_INFO_QUICK_ACCESS_CODE,
		STSW_MMS_INFO_MEMBER_FIRST_NAME,
		STSW_MMS_INFO_MEMBER_LAST_NAME,
		STSW_MMS_INFO_MEMBER_PHONE,
		STSW_MMS_INFO_NEW_SYSTEM_GRACE_DAYS,
		STSW_MMS_INFO_TOTAL_SHOTS_TAKEN,
		STSW_MMS_INFO_TOTAL_SHOTS_TAKEN_LAST_UPDATE_DATE,
		STSW_MMS_INFO_FIRST_USE_DATE,
		STSW_MMS_INFO_MEMBERSHIP_PACK_DATE,
		STSW_MMS_INFO_DEVICE_STATUS_FLAG,
		STSW_MMS_INFO_ESN,
		STSW_MMS_INFO_ESN_VERIFICATION_CODE
	}
	public struct STSWMMSValidationResultDataType
	{
		public STSWMMSValidationResultType result;

		public int dayLeft;
	}
	public enum STSWMMSValidationResultType
	{
		STSW_VALIDATION_MMS_VALID,
		STSW_VALIDATION_LICENSE_IS_ABOUT_TO_EXPIRE,
		STSW_VALIDATION_LICENSE_GRACE_PERIOD,
		STSW_VALIDATION_LICENSE_EXPIRED,
		STSW_VALIDATION_DEVICE_NOT_REGISTERED_TRIAL,
		STSW_VALIDATION_DEVICE_NOT_REGISTERED_BLOCK,
		STSW_VALIDATION_LAST
	}

	public enum STSWMMSFeatureFlagType
	{
		Module_Practice,
		Module_Challenges,
		Module_CoursePlay,
		Module_GameImprovement,
		Module_Profile,
		Module_Menus,
		Module_Settings,
		Challenges_ClosestToPin,
		Challenges_LongDrive,
		Challenges_Targets,
		Challenges_HitTheCartGuy,
		Challenges_BreakingWindows,
		Challenges_Battleship,
		Menus_SessionHistory,
		Menus_NumericDisplay,
		Menus_Settings,
		Settings_CameraAngle_Dynamic,
		Settings_CameraAngle_FirstPerson,
		Settings_CameraAngle_DownRange,
		Settings_CameraAngle_FollowTheBall,
		Settings_CameraAngle_45Degree,
		Settings_Tracers_AllShots,
		Settings_Tracers_Last5,
		Settings_Tracers_SingleShot,
		Settings_GroundConditions_Normal,
		Settings_GroundConditions_Soft,
		Settings_GroundConditions_Firm,
		Settings_Environmental_Wind,
		Settings_Environmental_Humidity,
		Settings_Environmental_Altitude,
		Settings_GolferOrientation,
		Settings_SideSpinSideAxis,
		Profile_Main,
		Profile_History,
		Profile_About,
		Profile_Stats,
		Profile_SkyGolf360Link,
		CoursePlay_WGT_CTTH_SingleCourse,
		CoursePlay_WGT_CTTH_AllCourses,
		CoursePlay_WGT_StrokePlay_AllCourses,
		CoursePlay_WGT_FeatureFlag1,
		CoursePlay_WGT_FeatureFlag2,
		CoursePlay_TruGolf_TeaserWare,
		CoursePlay_TruGolf_ConsumerSubscription,
		CoursePlay_TruGolf_CommercialSubscribtion,
		CoursePlay_TruGolf_Future,
		CoursePlay_TruGolf_StandardPack0,
		CoursePlay_TruGolf_CoursePack1,
		CoursePlay_TruGolf_CoursePack2,
		CoursePlay_TruGolf_CoursePack3,
		CoursePlay_TruGolf_CoursePack4,
		CoursePlay_TruGolf_CoursePack5,
		CoursePlay_TruGolf_CoursePack6,
		CoursePlay_TruGolf_CoursePack7,
		CoursePlay_TruGolf_CoursePack8,
		CoursePlay_TruGolf_CoursePack9_future,
		CoursePlay_TruGolf_CoursePack10_future,
		CoursePlay_TruGolf_CoursePack11_future,
		CoursePlay_TruGolf_CoursePack12_future,
		GameImprovement_FittingCenter,
		GameImprovement_SkyPro,
		CoursePlay_PerfectGolf_Driving_Range_and_Practice_Package,
		CoursePlay_PerfectGolf_Basic_Simulation_Package,
		CoursePlay_Perfect_Golf_Premium_Package,
		CoursePlay_Perfect_Golf_Commercial_Package,
		CoursePlay_ProTeeUnited_FullMembershipNoRestrictions,
		CoursePlay_ProTeeUnited_FutureFlag1,
		CoursePlay_ProTeeUnited_FutureFlag2,
		CoursePlay_ProTeeUnited_FutureFlag3,
		CoursePlay_ProTeeUnited_FutureFlag4,
		CoursePlay_TestAccount_FlagOn,
		CoursePlay_TestAccount_FlagOff,
		CoursePlay_TBD_1,
		CoursePlay_TBD_2,
		CoursePlay_TBD_3,
		Module_ClubCompare,
		Settings_Watch_Me,
		Settings_PracticeGreen,
		GameImprovement_SkillsAssessment,
		Settings_SkillsAssessmentExportEnabled,
		CoursePlay_CreativeGolf3D_Basic_One_Time_Fee,
		CoursePlay_CreativeGolf3D_Set1,
		CoursePlay_CreativeGolf3D_Set2,
		CoursePlay_CreativeGolf3D_Set3,
		CoursePlay_CreativeGolf3D_Set4,
		CoursePlay_CreativeGolf3D_Set5,
		CoursePlay_CreativeGolf3D_Set6,
		CoursePlay_CreativeGolf3D_Set7,
		CoursePlay_CreativeGolf3D_Set8,
		CoursePlay_CreativeGolf3D_Set9,
		CoursePlay_CreativeGolf3D_Set10,
		CoursePlay_CreativeGolf3D_Basic_Annual_Fee,
		FitnessGolf_Flag,
		V1_Interactive_Data_Integration,
		V1_Interactive_FutureFlag1,
		V1_Interactive_FutureFlag2,
		SkyTrakFlag96,
		SkyTrakFlag97,
		SkyTrakFlag98,
		SkyTrakFlag99,
		GameImprovement_BagMapping,
		Settings_DispersionCircles,
		Settings_CameraAngleOffset,
		Settings_Temperature,
		Settings_DeviceLeveling_AlignmentMode,
		SkyTrakFlag105,
		SkyTrakFlag106,
		SkyTrakFlag107,
		SkyTrakFlag108,
		SkyTrakFlag109,
		SkyTrakFlag110,
		SkyTrakFlag111,
		SkyTrakFlag112,
		SkyTrakFlag113,
		SkyTrakFlag114,
		SkyTrakFlag115,
		SkyTrakFlag116,
		SkyTrakFlag117,
		SkyTrakFlag118,
		SkyTrakFlag119,
		SkyTrakFlag120,
		SkyTrakFlag121,
		SkyTrakFlag122,
		SkyTrakFlag123,
		SkyTrakFlag124,
		SkyTrakFlag125,
		SkyTrakFlag126,
		SkyTrakFlag127,
		SkyTrakFlag128,
		SkyTrakFlag129,
		SkyTrakFlag130,
		SkyTrakFlag131,
		SkyTrakFlag132,
		SkyTrakFlag133,
		SkyTrakFlag134,
		SkyTrakFlag135,
		SkyTrakFlag136,
		SkyTrakFlag137,
		SkyTrakFlag138,
		SkyTrakFlag139,
		SkyTrakFlag140,
		SkyTrakFlag141,
		SkyTrakFlag142,
		SkyTrakFlag143,
		SkyTrakFlag144,
		SkyTrakFlag145,
		SkyTrakFlag146,
		SkyTrakFlag147,
		SkyTrakFlag148,
		SkyTrakFlag149,
		SkyTrakFlag150,
		SkyTrakFlag151,
		SkyTrakFlag152,
		SkyTrakFlag153,
		SkyTrakFlag154,
		SkyTrakFlag155,
		SkyTrakFlag156,
		SkyTrakFlag157,
		SkyTrakFlag158,
		SkyTrakFlag159,
		SkyTrakFlag160,
		SkyTrakFlag161,
		SkyTrakFlag162,
		SkyTrakFlag163,
		SkyTrakFlag164,
		SkyTrakFlag165,
		SkyTrakFlag166,
		SkyTrakFlag167,
		SkyTrakFlag168,
		SkyTrakFlag169,
		SkyTrakFlag170,
		SkyTrakFlag171,
		SkyTrakFlag172,
		SkyTrakFlag173,
		SkyTrakFlag174,
		SkyTrakFlag175,
		SkyTrakFlag176,
		SkyTrakFlag177,
		SkyTrakFlag178,
		SkyTrakFlag179,
		SkyTrakFlag180,
		SkyTrakFlag181,
		SkyTrakFlag182,
		SkyTrakFlag183,
		SkyTrakFlag184,
		SkyTrakFlag185,
		SkyTrakFlag186,
		SkyTrakFlag187,
		SkyTrakFlag188,
		SkyTrakFlag189,
		SkyTrakFlag190,
		SkyTrakFlag191,
		SkyTrakFlag192,
		SkyTrakFlag193,
		SkyTrakFlag194,
		SkyTrakFlag195,
		SkyTrakFlag196,
		SkyTrakFlag197,
		SkyTrakFlag198,
		SkyTrakFlag199,
		SkyTrakFlag200,
		SkyTrakFlag201,
		SkyTrakFlag202,
		SkyTrakFlag203,
		SkyTrakFlag204,
		SkyTrakFlag205,
		SkyTrakFlag206,
		SkyTrakFlag207,
		SkyTrakFlag208,
		SkyTrakFlag209,
		SkyTrakFlag210,
		SkyTrakFlag211,
		SkyTrakFlag212,
		SkyTrakFlag213,
		SkyTrakFlag214,
		SkyTrakFlag215,
		SkyTrakFlag216,
		SkyTrakFlag217,
		SkyTrakFlag218,
		SkyTrakFlag219,
		SkyTrakFlag220,
		SkyTrakFlag221,
		SkyTrakFlag222,
		SkyTrakFlag223,
		SkyTrakFlag224,
		SkyTrakFlag225,
		SkyTrakFlag226,
		SkyTrakFlag227,
		SkyTrakFlag228,
		SkyTrakFlag229,
		SkyTrakFlag230,
		SkyTrakFlag231,
		SkyTrakFlag232,
		SkyTrakFlag233,
		SkyTrakFlag234,
		SkyTrakFlag235,
		SkyTrakFlag236,
		SkyTrakFlag237,
		SkyTrakFlag238,
		SkyTrakFlag239,
		SkyTrakFlag240,
		SkyTrakFlag241,
		SkyTrakFlag242,
		SkyTrakFlag243,
		SkyTrakFlag244,
		SkyTrakFlag245,
		SkyTrakFlag246,
		SkyTrakFlag247,
		SkyTrakFlag248,
		SkyTrakFlag249,
		SkyTrakFlag250,
		SkyTrakFlag251,
		SkyTrakFlag252,
		SkyTrakFlag253,
		SkyTrakFlag254,
		SkyTrakFlag255
	}
	public struct STSWBoxShotsDataType
	{
		public int totalShotsTaken;

		public string swUuid;
	}
	public enum STSWBoxMmsReadModeType
	{
		STSW_BOX_MMS_SOFT_READ,
		STSW_BOX_MMS_FORCE_READ,
		STSW_BOX_MMS_NEVER_READ
	}
}