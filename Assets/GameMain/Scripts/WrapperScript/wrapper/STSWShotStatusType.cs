using System;

namespace SkyTrakWrapper
{
	public enum STSWShotStatusType
	{
		STSW_SHOT_ST_VALID,
		STSW_SHOT_ST_INVALID,
		STSW_SHOT_ST__LAST
	}

	public struct STSWShotParamsType
	{
		public RIPESpeedParamsType speedParams;

		public RIPESpinParamsType spinParams;

		public RIPEFlightParamsType speedFlightParams;

		public RIPEFlightParamsType speedSpinFlightParams;

		public STSWShotStatusType status;
	}

	public struct STSWMsgDataType
	{
		public STSWMsgType type;

		public IntPtr data;
	}

	public enum STSWMsgType
	{
		STSW_MSG_DISCOVER_UPDATED,
		STSW_MSG_DISCOVER_FAIL,
		STSW_MSG_CONNECTED,
		STSW_MSG_CONNECT_FAIL,
		STSW_MSG_MMS_UPDATED,
		STSW_MSG_STARTED_DISCONNECTION,
		STSW_MSG_DISCONNECTED,
		STSW_MSG_STATUS_UPDATED,
		STSW_MSG_SHOT_STARTED,
		STSW_MSG_SHOT_ENDED,
		STSW_MSG_FW_UPGRADE_SUCCESS,
		STSW_MSG_FW_UPGRADE_ERROR,
		STSW_MSG_FW_UPGRADE_STARTED,
		STSW_MSG_FW_UPGRADE_AVAILABLE,
		STSW_MSG_FW_SDK_INCOMPATIBLE,
		STSW_MSG_FW_SDK_CUSTOMERCODE_INCOMPATIBLE,
		STSW_MSG_NETWORK_SCAN_LIST_UPDATED,
		STSW_MSG_FAILED_READ_BOX_MMS,
		STSW_MSG__LAST
	}

}