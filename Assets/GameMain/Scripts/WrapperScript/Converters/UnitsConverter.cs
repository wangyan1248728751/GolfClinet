using Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UnitsConverter
{
	[CompilerGenerated]
	private static Dictionary<string, int> f__switch_map1;

	private const string MetricEnabledKey = "MetricEnabled";

	private const string KPHEnabledKey = "KPHEnabled";

	private const string MeterAbbr = "M";

	private const string YardsAbbr = "YD";

	private const string MetersName = "METERS";

	private const string YardsName = "YARDS";

	private const string OfflineYardsString = "OFFLINE (yd)";

	private const string OfflineMetersString = "OFFLINE (m)";

	private const string MetersSpeed = "KPH";

	private const string YardsSpeed = "MPH";

	private const string Yardage = "YARDAGE";

	private const string Metric = "METRIC";

	private const float MILES_TO_KILOMETERS = 1.609344f;

	private Dictionary<string, bool> _settingsCashe = new Dictionary<string, bool>();

	public Action OnUnitsChanged;

	private static UnitsConverter _instance;

	public static string CurrentDistanceUnitsLong
	{
		get
		{
			return UnitsConverter.Instance.GetUnits("YARDS");
		}
	}

	public static string CurrentDistanceUnitsShort
	{
		get
		{
			return UnitsConverter.Instance.GetUnits("YDS");
		}
	}

	public static string CurrentSpeedUnits
	{
		get
		{
			return UnitsConverter.Instance.GetUnits("MPH");
		}
	}

	public static UnitsConverter Instance
	{
		get
		{
			if (UnitsConverter._instance == null)
			{
				UnitsConverter._instance = new UnitsConverter();
			}
			return UnitsConverter._instance;
		}
	}

	public bool IsKPHEnabled
	{
		get
		{
			string str = (!LoginManager.IsUserLoggedIn ? "KPHEnabled" : string.Format("{0}|{1}", LoginManager.UserData.Email, "KPHEnabled"));
			if (this._settingsCashe.ContainsKey(str))
			{
				return this._settingsCashe[str];
			}
			this._settingsCashe.Add(str, PlayerPrefs.GetInt(str, 0) > 0);
			return this._settingsCashe[str];
		}
		set
		{
			string str = (!LoginManager.IsUserLoggedIn ? "KPHEnabled" : string.Format("{0}|{1}", LoginManager.UserData.Email, "KPHEnabled"));
			PlayerPrefs.SetInt(str, (!value ? 0 : 1));
			if (!this._settingsCashe.ContainsKey(str))
			{
				this._settingsCashe.Add(str, value);
			}
			else
			{
				this._settingsCashe[str] = value;
			}
			if (this.OnUnitsChanged != null)
			{
				this.OnUnitsChanged();
			}
		}
	}

	public bool IsMetricsEnabled
	{
		get
		{
			string str = (!LoginManager.IsUserLoggedIn ? "MetricEnabled" : string.Format("{0}|{1}", LoginManager.UserData.Email, "MetricEnabled"));
			if (this._settingsCashe.ContainsKey(str))
			{
				return this._settingsCashe[str];
			}
			this._settingsCashe.Add(str, PlayerPrefs.GetInt(str, 0) > 0);
			return this._settingsCashe[str];
		}
		set
		{
			string str = (!LoginManager.IsUserLoggedIn ? "MetricEnabled" : string.Format("{0}|{1}", LoginManager.UserData.Email, "MetricEnabled"));
			PlayerPrefs.SetInt(str, (!value ? 0 : 1));
			if (!this._settingsCashe.ContainsKey(str))
			{
				this._settingsCashe.Add(str, value);
			}
			else
			{
				this._settingsCashe[str] = value;
			}
			if (this.OnUnitsChanged != null)
			{
				this.OnUnitsChanged();
			}
		}
	}

	public float ConvertToMeters(float yards)
	{
		return yards * 0.9144f;
	}

	public float ConvertToYards(float meters)
	{
		return meters / 0.9144f;
	}

	public float GetDistance(float yards)
	{
		if (!this.IsMetricsEnabled)
		{
			return yards;
		}
		return yards * 0.9144f;
	}

	public float GetSpeed(float mphSpeed)
	{
		if (!this.IsKPHEnabled)
		{
			return mphSpeed;
		}
		return mphSpeed * 1.609344f;
	}

	public string GetUnits(string currentValue)
	{
		int num;
		if (currentValue != null)
		{
			if (UnitsConverter.f__switch_map1 == null)
            {
				Dictionary<string, int> strs = new Dictionary<string, int>(14)
				{
					{ "M", 0 },
					{ "YD", 0 },
					{ "YDS", 0 },
					{ "METERS", 1 },
					{ "YARDS", 1 },
					{ "OFFLINE (m)", 2 },
					{ "OFFLINE (yd)", 2 },
					{ "OFFLINE", 2 },
					{ "KPH", 3 },
					{ "MPH", 3 },
					{ "/ MPH", 4 },
					{ "/ KPH", 4 },
					{ "YARDAGE", 5 },
					{ "METRIC", 5 }
				};
				UnitsConverter.f__switch_map1 = strs;
			}
			if (UnitsConverter.f__switch_map1.TryGetValue(currentValue, out num))
            {
				switch (num)
				{
					case 0:
						{
							currentValue = (!this.IsMetricsEnabled ? "YD" : "M");
							break;
						}
					case 1:
						{
							currentValue = (!this.IsMetricsEnabled ? "YARDS" : "METERS");
							break;
						}
					case 2:
						{
							currentValue = (!this.IsMetricsEnabled ? "OFFLINE (yd)" : "OFFLINE (m)");
							break;
						}
					case 3:
						{
							currentValue = (!this.IsKPHEnabled ? "MPH" : "KPH");
							break;
						}
					case 4:
						{
							currentValue = (!this.IsKPHEnabled ? "/ MPH" : "/ KPH");
							break;
						}
					case 5:
						{
							currentValue = (!this.IsMetricsEnabled ? "YARDAGE" : "METRIC");
							break;
						}
				}
			}
		}
		return currentValue;
	}
}