using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class SoftwareVersionUpdateController
{
	private static string _receivedVersion;

	private static string _url;

	public const string MessageHeader = "Software Upgrade Available";

	public const string MessageContent = "There is a newer version of software available. Would you like to Upgrade?";

	public const string Button1Header = "YES";

	public const string Button2Header = "NO";

	private static void CheckVersion()
	{
		if (SoftwareVersion.GerShortVersionNumeric(SoftwareVersion.Version) >= SoftwareVersion.GerShortVersionNumeric(SoftwareVersionUpdateController._receivedVersion))
		{
			return;
		}
	}

	public static void GetAppVersion(string swVersion)
	{
		Dictionary<string, string> strs = new Dictionary<string, string>()
		{
			{ "dev", "E4F68-874E7-C80E5-1E8C3" },
			{ "softwareversion", swVersion }
		};
		WebService.CallWebService(ServerRequest.GetAppVersion, strs, new Action<WebServiceResponse>(SoftwareVersionUpdateController.GetAppVersionCallback), false);
	}

	private static void GetAppVersionCallback(WebServiceResponse response)
	{
		if (!response.Success)
		{
			AppLog.Log(string.Format("WEB CALL \"GET_APP_VERSION\" FAILED - code: {0}, message: {1}", response.Code, response.Message), true);
			return;
		}
		JSONObject field = response.Data.GetField("Software");
		if (field != null)
		{
			JSONObject jSONObject = field.GetField("Latest");
			if (jSONObject != null)
			{
				SoftwareVersionUpdateController._receivedVersion = jSONObject.GetField("Version").str;
				SoftwareVersionUpdateController._url = jSONObject.GetField("URL").str;
			}
		}
		SoftwareVersionUpdateController.CheckVersion();
	}

	public static void OnOkCancelClicked()
	{

	}

	public static void OnUpdateClicked()
	{
		string empty = string.Empty;
		string[] strArrays = SoftwareVersionUpdateController._url.Split(new char[] { '/' });
		for (int i = 0; i < (int)strArrays.Length; i++)
		{
			strArrays[i] = strArrays[i].Trim();
			empty = string.Concat(empty, strArrays[i]);
		}
		Application.OpenURL(empty);
		//MonoSingleton<UIData>.Singleton.MenuPopupData.HidePopup();
	}
}