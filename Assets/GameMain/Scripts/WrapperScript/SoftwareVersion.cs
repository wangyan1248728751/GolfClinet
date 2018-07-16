using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SoftwareVersion
{
	public const string VersionFolder = "Version";

	public const string VersionFilename = "VersionText";

	private const string VersionFilenameBin = "Version";

	public const string SoftwareVersionKey = "SoftwareVersion";

	public const string PrevSoftwareVersionKey = "PrevSoftwareVersion";

	private static VersionData _versionData;

	public static string BuildNumber
	{
		get
		{
			return SoftwareVersion._versionData.BuildNumber;
		}
	}

	public static bool IsVersionChanged
	{
		get
		{
			if (PlayerPrefs.HasKey("SoftwareVersion") && PlayerPrefs.GetString("SoftwareVersion").Equals(SoftwareVersion.Version))
			{
				return false;
			}
			string str = (!PlayerPrefs.HasKey("SoftwareVersion") ? string.Empty : PlayerPrefs.GetString("SoftwareVersion"));
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString("SoftwareVersion", SoftwareVersion.Version);
			if (!string.IsNullOrEmpty(str))
			{
				PlayerPrefs.SetString("PrevSoftwareVersion", str);
			}
			return true;
		}
	}

	public static string PrevVersion
	{
		get
		{
			return PlayerPrefs.GetString("PrevSoftwareVersion", string.Empty);
		}
	}

	public static string ShortVersion
	{
		get
		{
			string str = string.Format("{0}.{1}", SoftwareVersion._versionData.MajorVersion, SoftwareVersion._versionData.MinorVersion);
			if (!string.IsNullOrEmpty(SoftwareVersion._versionData.PatchVersion))
			{
				str = string.Concat(str, ".", SoftwareVersion._versionData.PatchVersion);
			}
			return str;
		}
	}

	public static string Version
	{
		get
		{
			return SoftwareVersion._versionData.ToString();
		}
	}

	public static string VersionWithPlatform
	{
		get
		{
			return string.Format("V{0}_{1}", SoftwareVersion.ShortVersion, SoftwareVersion.GetPlatformPostfix());
		}
	}

	static SoftwareVersion()
	{
		SoftwareVersion._versionData = SoftwareVersion.ReadVersion();
	}

	public static int GerShortVersionNumeric(string version)
	{
		string[] strArrays = version.Split(new char[] { '.' });
		for (int i = 0; i < (int)strArrays.Length; i++)
		{
			strArrays[i] = strArrays[i].Trim();
		}
		string str = strArrays[0];
		string str1 = strArrays[1];
		string str2 = strArrays[2];
		string str3 = string.Format("{0}{1}{2}", str, str1, str2);
		return Convert.ToInt32(str3);
	}

	private static string GetPlatformPostfix()
	{
		switch (Application.platform)
		{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.OSXDashboardPlayer:
				{
					return "Mac";
				}
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				{
					return "Win";
				}
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
			case RuntimePlatform.WindowsPlayer | RuntimePlatform.OSXDashboardPlayer:
			case RuntimePlatform.PS3:
			case RuntimePlatform.XBOX360:
				{
					return string.Empty;
				}
			case RuntimePlatform.IPhonePlayer:
				{
					return "iOS";
				}
			case RuntimePlatform.Android:
				{
					return "And";
				}
			default:
				{
					return string.Empty;
				}
		}
	}

	private static VersionData ReadVersion()
	{
		VersionData versionDatum;
		TextAsset textAsset = Resources.Load("Version/Version") as TextAsset;
		if (textAsset == null)
		{
			versionDatum = new VersionData();
		}
		else
		{
			using (Stream memoryStream = new MemoryStream(Convert.FromBase64String(textAsset.text)))
			{
				versionDatum = (VersionData)(new BinaryFormatter()).Deserialize(memoryStream);
			}
		}
		return versionDatum;
	}
}

[Serializable]
public class VersionData
{
	public string MajorVersion;

	public string MinorVersion;

	public string PatchVersion;

	public string BuildMonthVersion;

	public string BuildDayVersion;

	public string BuildYearVersion;

	public string BuildHhVersion;

	public string BuildMmVersion;

	public string BuildSsVersion;

	public string BuildNumber = "0";

	public VersionData()
	{
		string str = "0";
		string str1 = str;
		this.PatchVersion = str;
		string str2 = str1;
		str1 = str2;
		this.MinorVersion = str2;
		this.MajorVersion = str1;
		DateTime now = DateTime.Now;
		this.BuildMonthVersion = now.Month.ToString();
		this.BuildDayVersion = now.Day.ToString();
		this.BuildYearVersion = now.Year.ToString();
		this.BuildHhVersion = now.Hour.ToString();
		this.BuildMmVersion = now.Minute.ToString();
		this.BuildSsVersion = now.Second.ToString();
	}

	public override string ToString()
	{
		return string.Format("{0}.{1}.{2}.{3}.{4}.{5}.{6}.{7}.{8}", new object[] { this.MajorVersion, this.MinorVersion, this.PatchVersion, this.BuildMonthVersion, this.BuildDayVersion, this.BuildYearVersion, this.BuildHhVersion, this.BuildMmVersion, this.BuildSsVersion });
	}
}