using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SkyTrakWrapper
{
	public struct STSWInitType
	{
		public RIPEOperationModeType operationMode;

		public int generateFlightData;

		public IntPtr loggerData;

		public STSWNetworkStateType networkState;

		public int ripeEmulatorMode;

		public IntPtr boxCacheFilePath;

		public STSWServerUrlMode serverUrlMode;

		public IntPtr softwareVersion;

		public IntPtr platform;

		public string sdkDataFolderPath;

		public int sdkDebugLevel;
	}
}