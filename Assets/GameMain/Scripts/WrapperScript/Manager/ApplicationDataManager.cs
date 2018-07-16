using Converters;
using Data;
using Security;
using SkyTrakWrapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ApplicationDataManager
{
	private static ApplicationDataManager singleton;

	private List<string> userUsedESNs;

	private string userNameFromRegistration;

	private string userHandednessFromRegistration;

	private bool sessionGUIDFromAccountCreate;

	public bool UseAPIForTest;

	private const string tracerColorYellow = "fff600";

	private const string tracerColorMagenta = "df40bb";

	private const string tracerColorWhite = "ffffff";

	private const string tracerColorBlue = "3bb8ff";

	private const string tracerColorGreen = "b9ff54";

	private const string tracerColorOrange = "df5e2b";

	private List<TracerMaterial> tracerColors;

	public string WeatherLocationText = string.Empty;

	private List<int> _currentSessionUniqueIds = new List<int>();

	public string ESN
	{
		get
		{
			return SecurityWrapperService.Instance.ESN;
		}
	}

	public string FWVersion
	{
		get
		{
			return SecurityWrapperService.Instance.FWVersion;
		}
	}

	public static ApplicationDataManager instance
	{
		get
		{
			if (ApplicationDataManager.singleton == null)
			{
				ApplicationDataManager.singleton = new ApplicationDataManager();
			}
			return ApplicationDataManager.singleton;
		}
	}

	public string PreviousESN
	{
		get
		{
			if (this.userUsedESNs == null || this.userUsedESNs.Count <= 0)
			{
				return null;
			}
			return this.userUsedESNs[0];
		}
	}


	private ApplicationDataManager()
	{
		//this.CreateColorTracerArray();
		this.userUsedESNs = new List<string>();
		Messenger<TFlightData>.AddListener(MESSAGE_EVENTS.SHOT_COMPLETE, new Action<TFlightData>(this.MSNGR_SaveFlightData));
		if (PlayerPrefs.GetInt("SessionEnded", -1) == -1)
		{
			PlayerPrefs.SetInt("SessionEnded", 0);
		}
		if (PlayerPrefs.GetInt("DatabaseFullyCreated", -1) == -1)
		{
			return;
		}
	}

	public bool AssignTracerColorsToTFlightData(ref List<TFlightData> list, bool useClubName)
	{
		string item;
		string str;
		if (this.tracerColors.Count <= 0)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (i != 0)
			{
				bool flag = false;
				int num1 = 0;
				while (num1 < i)
				{
					if (!useClubName)
					{
						item = list[num1].clubTypeID;
						str = list[i].clubTypeID;
					}
					else
					{
						item = list[num1].clubName;
						str = list[i].clubName;
					}
					if (!item.Equals(str))
					{
						num1++;
					}
					else
					{
						flag = true;
						list[i].flightColor = list[num1].flightColor;
						break;
					}
				}
				if (!flag)
				{
					num++;
					TracerMaterial tracerMaterialByIndex = this.GetTracerMaterialByIndex(num);
					if (tracerMaterialByIndex != null)
					{
						list[i].flightColor = tracerMaterialByIndex.color;
					}
				}
			}
			else
			{
				TracerMaterial tracerMaterial = this.GetTracerMaterialByIndex(i);
				if (tracerMaterial != null)
				{
					list[i].flightColor = tracerMaterial.color;
				}
			}
		}
		return true;
	}

	public static Color32 ConvertHexColorToRGB(string hexValue)
	{
		byte num = byte.Parse(hexValue.Substring(0, 2), NumberStyles.HexNumber);
		byte num1 = byte.Parse(hexValue.Substring(2, 2), NumberStyles.HexNumber);
		byte num2 = byte.Parse(hexValue.Substring(4, 2), NumberStyles.HexNumber);
		return new Color32(num, num1, num2, 255);
	}

	private void CreateColorTracerArray()
	{
		List<string> strs = new List<string>()
			{
				"ffffff",
				"df5e2b",
				"b9ff54",
				"3bb8ff",
				"fff600",
				"df40bb"
			};
		this.tracerColors = new List<TracerMaterial>();
		foreach (string str in strs)
		{
			if (str != null)
			{
				if (str == "fff600")
				{
					Material material = Resources.Load<Material>("Tracers/TRC_Mat_Yellow");
					this.tracerColors.Add(new TracerMaterial(str, material));
					continue;
				}
				else if (str == "df40bb")
				{
					Material material1 = Resources.Load<Material>("Tracers/TRC_Mat_Magenta");
					this.tracerColors.Add(new TracerMaterial(str, material1));
					continue;
				}
				else if (str == "ffffff")
				{
					Material material2 = Resources.Load<Material>("Tracers/TRC_Mat_White");
					this.tracerColors.Add(new TracerMaterial(str, material2));
					continue;
				}
				else if (str == "3bb8ff")
				{
					Material material3 = Resources.Load<Material>("Tracers/TRC_Mat_Blue");
					this.tracerColors.Add(new TracerMaterial(str, material3));
					continue;
				}
				else if (str == "b9ff54")
				{
					Material material4 = Resources.Load<Material>("Tracers/TRC_Mat_Green");
					this.tracerColors.Add(new TracerMaterial(str, material4));
					continue;
				}
				else if (str == "df5e2b")
				{
					Material material5 = Resources.Load<Material>("Tracers/TRC_Mat_Orange");
					this.tracerColors.Add(new TracerMaterial(str, material5));
					continue;
				}
			}
			Debug.LogError("APPLICATION_DATA_MANAGER::Error occured while creating tracer materials.");
		}
	}

	public int GenerateLocalEsnID()
	{
		DateTime now = DateTime.Now;
		TimeSpan timeSpan = now.Subtract(Convert.ToDateTime("01/01/1970"));
		int totalSeconds = (int)timeSpan.TotalSeconds;
		while (this._currentSessionUniqueIds.Contains(totalSeconds))
		{
			totalSeconds++;
		}
		this._currentSessionUniqueIds.Add(totalSeconds);
		return totalSeconds;
	}

	public bool GetPlayerHandednessIsLefty()
	{
		return (!LoginManager.IsUserLoggedIn ? PlayerPrefsX.GetBool("OfflineModeIsLefty", false) : LoginManager.UserData.IsLefty);
	}

	public List<string> GetPreviousUsedESNList()
	{
		List<string> strs = new List<string>();
		foreach (string userUsedESN in this.userUsedESNs)
		{
			strs.Add(string.Copy(userUsedESN));
		}
		return strs;
	}

	public int GetTracerIndexByColor(Color32 color)
	{
		for (int i = 0; i < this.tracerColors.Count; i++)
		{
			if (color.r == this.tracerColors[i].color.r && color.g == this.tracerColors[i].color.g && color.b == this.tracerColors[i].color.b)
			{
				return i;
			}
		}
		return -1;
	}

	public TracerMaterial GetTracerMaterialByColor(Color32 color)
	{
		if (this.tracerColors.Count == 0)
		{
			return null;
		}
		TracerMaterial tracerMaterial = null;
		foreach (TracerMaterial tracerColor in this.tracerColors)
		{
			Color32 color32 = tracerColor.color;
			if (color32.r != color.r || color32.g != color.g || color32.b != color.b)
			{
				continue;
			}
			tracerMaterial = tracerColor;
		}
		return tracerMaterial;
	}

	public TracerMaterial GetTracerMaterialByIndex(int index)
	{
		if (this.tracerColors.Count == 0)
		{
			return null;
		}
		if (index < 0)
		{
			index = 0;
		}
		int count = index;
		if (count >= this.tracerColors.Count)
		{
			count %= this.tracerColors.Count;
		}
		return this.tracerColors[count];
	}

	public void MSNGR_SaveFlightData(TFlightData flightData)
	{
		if (LoginManager.IsUserLoggedIn)
		{
			Shot shot = (new ShotFlightDataConverter()).Convert(flightData);
			SessionAndActivityManager.Instance.AddShotToSession(shot);
			if (!MembershipManager.GetAccess(STSWMMSFeatureFlagType.Settings_Watch_Me))
			{
				AppLog.Log("Shot was not saved to watch-me file because feature flag WatchMe is disabled.", true);
			}
			else
			{
				AppLog.Log("Start saving shot to watch-me file.", true);
				WatchmeFile.SaveShot(flightData, shot);
			}
		}
	}

	public void SetPlayerHandednessIsLefty(bool isLefty, bool saveToDB = true)
	{
		if (!LoginManager.IsUserLoggedIn)
		{
			PlayerPrefsX.SetBool("OfflineModeIsLefty", isLefty);
		}
		else
		{
			LoginManager.UserData.Dexterity = (!isLefty ? "R" : "L");
			if (saveToDB)
			{
				Debug.Log("Save hand to profile and DB");
				AppLog.Log(string.Concat("ApplicationDataManager: Update current profile handedness", LoginManager.UserData.Dexterity), true);
				LoginManager.UpdateUser();
			}
		}
		if (CSimulationManager.instance != null)
		{
			CSimulationManager.instance.UpdateHMPlayerHandedness(isLefty);
		}
	}

	public void SetPreviousUsedESNList(List<string> esnList)
	{
		foreach (string str in esnList)
		{
			this.userUsedESNs.Add(string.Copy(str));
		}
	}

	public void SetUserDataFromRegistration(string userName, bool isLefty)
	{
		this.userNameFromRegistration = userName;
		if (!isLefty)
		{
			this.userHandednessFromRegistration = "R";
		}
		else
		{
			this.userHandednessFromRegistration = "L";
		}
	}

	public void StartSession()
	{
		SessionAndActivityManager.Instance.CreateNewSession();
	}
}