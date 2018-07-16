using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Club
{
	public const string ClubIconsResPath = "_Textures/clubs/";

	public const string sID_Undefined = "UDEF";

	public const string sID_Driver = "D";

	public const string sID_02Wood = "2W";

	public const string sID_03Wood = "3W";

	public const string sID_04Wood = "4W";

	public const string sID_05Wood = "5W";

	public const string sID_06Wood = "6W";

	public const string sID_07Wood = "7W";

	public const string sID_08Wood = "8W";

	public const string sID_09Wood = "9W";

	public const string sID_10Wood = "10W";

	public const string sID_11Wood = "11W";

	public const string sID_01Iron = "1I";

	public const string sID_02Iron = "2I";

	public const string sID_03Iron = "3I";

	public const string sID_04Iron = "4I";

	public const string sID_05Iron = "5I";

	public const string sID_06Iron = "6I";

	public const string sID_07Iron = "7I";

	public const string sID_08Iron = "8I";

	public const string sID_09Iron = "9I";

	public const string sID_PitchingWedge = "PW";

	public const string sID_GapWedge = "GW";

	public const string sID_SandWedge = "SW";

	public const string sID_LobWedge = "LW";

	public const string sID_Wedge40Deg = "40/W";

	public const string sID_Wedge41Deg = "41/W";

	public const string sID_Wedge42Deg = "42/W";

	public const string sID_Wedge43Deg = "43/W";

	public const string sID_Wedge44Deg = "44/W";

	public const string sID_Wedge45Deg = "45/W";

	public const string sID_Wedge46Deg = "46/W";

	public const string sID_Wedge47Deg = "47/W";

	public const string sID_Wedge48Deg = "48/W";

	public const string sID_Wedge49Deg = "49/W";

	public const string sID_Wedge50Deg = "50/W";

	public const string sID_Wedge51Deg = "51/W";

	public const string sID_Wedge52Deg = "52/W";

	public const string sID_Wedge53Deg = "53/W";

	public const string sID_Wedge54Deg = "54/W";

	public const string sID_Wedge55Deg = "55/W";

	public const string sID_Wedge56Deg = "56/W";

	public const string sID_Wedge57Deg = "57/W";

	public const string sID_Wedge58Deg = "58/W";

	public const string sID_Wedge59Deg = "59/W";

	public const string sID_Wedge60Deg = "60/W";

	public const string sID_Wedge61Deg = "61/W";

	public const string sID_Wedge62Deg = "62/W";

	public const string sID_Wedge63Deg = "63/W";

	public const string sID_Wedge64Deg = "64/W";

	public const string sID_Wedge65Deg = "65/W";

	public const string sID_Putter = "P";

	public const string sID_Hybrid15Deg = "15/H";

	public const string sID_Hybrid16Deg = "16/H";

	public const string sID_Hybrid17Deg = "17/H";

	public const string sID_Hybrid18Deg = "18/H";

	public const string sID_Hybrid19Deg = "19/H";

	public const string sID_Hybrid20Deg = "20/H";

	public const string sID_Hybrid21Deg = "21/H";

	public const string sID_Hybrid22Deg = "22/H";

	public const string sID_Hybrid23Deg = "23/H";

	public const string sID_Hybrid24Deg = "24/H";

	public const string sID_Hybrid25Deg = "25/H";

	public const string sID_Hybrid26Deg = "26/H";

	public const string sID_Hybrid27Deg = "27/H";

	public const string sID_Hybrid28Deg = "28/H";

	public const string sID_01Hybrid = "1H";

	public const string sID_02Hybrid = "2H";

	public const string sID_03Hybrid = "3H";

	public const string sID_04Hybrid = "4H";

	public const string sID_05Hybrid = "5H";

	public const string sID_06Hybrid = "6H";

	public const string sID_07Hybrid = "7H";

	public const string sID_08Hybrid = "8H";

	public const string sID_09Hybrid = "9H";

	public const string sUndefined = "Undefined";

	public const string sDriver = "Driver";

	public const string sHybrid = "Hybrid";

	public const string sWood = "Wood";

	public const string sIron = "Iron";

	public const string sPutter = "Putter";

	public const string sWedge = "Wedge";

	public const string sSandWedge = "Sand Wedge";

	public const string sChipper = "Chipper";

	public const string NoneString = "NONE";

	public const string UndefinedString = "UNDEFINED";

	public const string DriverString = "DRIVER";

	public const string HybridString = "HYBRID";

	public const string WoodString = "WOOD";

	public const string IronString = "IRON";

	public const string PutterString = "PUTTER";

	public const string WedgeString = "WEDGE";

	public const string SandWedgeString = "SAND WEDGE";

	public const string ChipperString = "CHIPPER";

	[CompilerGenerated]
	private static Dictionary<string, int> f__switch_map0;

	private readonly static Dictionary<string, List<string>> _clubTypesMap;

	private readonly static Dictionary<string, string> _clubNamesMap;

	private static Dictionary<string, string> ClubSpriteNames;

	private static Dictionary<string, string> ClubHighresSpriteNames;

	private static Dictionary<string, Club.ClubIconType> ClubIconTypes;

	private readonly static Color[] _clubColors;

	private string _number;

	public int clubID;

	public string clubName;

	private bool _isEquipped;

	public Texture2D clubTexture;

	private string _type;

	public bool isEquipped
	{
		get
		{
			return this._isEquipped;
		}
	}

	public string number
	{
		get
		{
			return this._number;
		}
	}

	public string type
	{
		get
		{
			return this._type;
		}
	}

	static Club()
	{
		Dictionary<string, List<string>> strs = new Dictionary<string, List<string>>();
		List<string> strs1 = new List<string>()
		{
			"UDEF"
		};
		strs.Add("Undefined", strs1);
		strs1 = new List<string>()
		{
			"D"
		};
		strs.Add("Driver", strs1);
		strs1 = new List<string>()
		{
			"15/H",
			"16/H",
			"17/H",
			"18/H",
			"19/H",
			"20/H",
			"21/H",
			"22/H",
			"23/H",
			"24/H",
			"25/H",
			"26/H",
			"27/H",
			"28/H",
			"1H",
			"2H",
			"3H",
			"4H",
			"5H",
			"6H",
			"7H",
			"8H",
			"9H"
		};
		strs.Add("Hybrid", strs1);
		strs1 = new List<string>()
		{
			"2W",
			"3W",
			"4W",
			"5W",
			"6W",
			"7W",
			"8W",
			"9W",
			"10W",
			"11W"
		};
		strs.Add("Wood", strs1);
		strs1 = new List<string>()
		{
			"1I",
			"2I",
			"3I",
			"4I",
			"5I",
			"6I",
			"7I",
			"8I",
			"9I"
		};
		strs.Add("Iron", strs1);
		strs1 = new List<string>()
		{
			"P"
		};
		strs.Add("Putter", strs1);
		strs1 = new List<string>()
		{
			"50/W",
			"52/W",
			"54/W",
			"56/W",
			"58/W",
			"60/W",
			"64/W",
			"PW",
			"GW",
			"LW"
		};
		strs.Add("Wedge", strs1);
		strs1 = new List<string>()
		{
			"SW"
		};
		strs.Add("Sand Wedge", strs1);
		Club._clubTypesMap = strs;
		Dictionary<string, string> strs2 = new Dictionary<string, string>()
		{
			{ "UDEF", "UNDEFINED" },
			{ "D", "DRIVER" },
			{ "2W", "2 WOOD" },
			{ "3W", "3 WOOD" },
			{ "4W", "4 WOOD" },
			{ "5W", "5 WOOD" },
			{ "6W", "6 WOOD" },
			{ "7W", "7 WOOD" },
			{ "8W", "8 WOOD" },
			{ "9W", "9 WOOD" },
			{ "15/H", "HYBRID 15°" },
			{ "16/H", "HYBRID 16°" },
			{ "17/H", "HYBRID 17°" },
			{ "18/H", "HYBRID 18°" },
			{ "19/H", "HYBRID 19°" },
			{ "20/H", "HYBRID 20°" },
			{ "21/H", "HYBRID 21°" },
			{ "22/H", "HYBRID 22°" },
			{ "23/H", "HYBRID 23°" },
			{ "1H", "1 HYBRID" },
			{ "2H", "2 HYBRID" },
			{ "3H", "3 HYBRID" },
			{ "4H", "4 HYBRID" },
			{ "5H", "5 HYBRID" },
			{ "6H", "6 HYBRID" },
			{ "7H", "7 HYBRID" },
			{ "8H", "8 HYBRID" },
			{ "9H", "9 HYBRID" },
			{ "1I", "1 IRON" },
			{ "2I", "2 IRON" },
			{ "3I", "3 IRON" },
			{ "4I", "4 IRON" },
			{ "5I", "5 IRON" },
			{ "6I", "6 IRON" },
			{ "7I", "7 IRON" },
			{ "8I", "8 IRON" },
			{ "9I", "9 IRON" },
			{ "PW", "PW" },
			{ "GW", "GW" },
			{ "SW", "SW" },
			{ "LW", "LW" },
			{ "50/W", "50°" },
			{ "52/W", "52°" },
			{ "54/W", "54°" },
			{ "56/W", "56°" },
			{ "58/W", "58°" },
			{ "60/W", "60°" },
			{ "64/W", "64°" },
			{ "P", "PUTTER" }
		};
		Club._clubNamesMap = strs2;
		strs2 = new Dictionary<string, string>()
		{
			{ "NONE", "none-selected" },
			{ "Undefined", "none-selected" },
			{ "Hybrid", "hybrid" },
			{ "Wedge", "wood_btn" },
			{ "Sand Wedge", "wood_btn" },
			{ "Iron", "wood_btn" },
			{ "Putter", "putter2" },
			{ "Wood", "iron_btn" },
			{ "Driver", "iron_btn" }
		};
		Club.ClubSpriteNames = strs2;
		strs2 = new Dictionary<string, string>()
		{
			{ "NONE", "none_selected" },
			{ "Undefined", "none_selected" },
			{ "Hybrid", "hybrid" },
			{ "Wedge", "iron" },
			{ "Sand Wedge", "iron" },
			{ "Iron", "iron" },
			{ "Putter", "putter" },
			{ "Wood", "driver" },
			{ "Driver", "driver" }
		};
		Club.ClubHighresSpriteNames = strs2;
		Dictionary<string, Club.ClubIconType> strs3 = new Dictionary<string, Club.ClubIconType>()
		{
			{ "NONE", Club.ClubIconType.Undefined },
			{ "Undefined", Club.ClubIconType.Undefined },
			{ "Hybrid", Club.ClubIconType.Hybrid },
			{ "Wedge", Club.ClubIconType.Iron },
			{ "Sand Wedge", Club.ClubIconType.Iron },
			{ "Iron", Club.ClubIconType.Iron },
			{ "Putter", Club.ClubIconType.Putter },
			{ "Wood", Club.ClubIconType.Wood },
			{ "Driver", Club.ClubIconType.Wood }
		};
		Club.ClubIconTypes = strs3;
		Club._clubColors = new Color[] { Color.white, new Color(0.8745098f, 0.368627459f, 0.168627456f), new Color(0.7254902f, 1f, 0.329411775f), new Color(0.235294119f, 0.721568644f, 1f), Color.yellow, Color.magenta };
	}

	public Club(string name, string type, string loftID)
	{
		this._type = type;
		this.clubName = string.Copy(name);
		this._number = string.Copy(loftID);
		this._isEquipped = false;
	}

	public Club Copy()
	{
		Club club = new Club(this.clubName, this._type, this._number);
		if (this._isEquipped)
		{
			club.SetClubEquipped(true);
		}
		return club;
	}

	public static int GetAdaptedColorIndex(int number)
	{
		if (number + 1 > (int)Club._clubColors.Length)
		{
			number %= (int)Club._clubColors.Length;
		}
		return number;
	}

	public static Color GetClubColorByIndex(int index)
	{
		return Club._clubColors[Club.GetAdaptedColorIndex(index)];
	}

	public static string GetClubHighresSpriteName(string clubName)
	{
		string clubIDFromName = Club.GetClubIDFromName(clubName);
		if (string.IsNullOrEmpty(clubIDFromName))
		{
			return string.Empty;
		}
		string typeFromID = Club.GetTypeFromID(clubIDFromName);
		if (string.IsNullOrEmpty(typeFromID))
		{
			return string.Empty;
		}
		if (!Club.ClubHighresSpriteNames.ContainsKey(typeFromID))
		{
			return string.Empty;
		}
		return Club.ClubHighresSpriteNames[typeFromID];
	}

	public static string GetClubIDFromName(string clubString)
	{
		string str = clubString;
		if (str.Equals("NONE"))
		{
			str = "UNDEFINED";
		}
		return (
			from pair in Club._clubNamesMap
			where str.Contains(pair.Value)
			select pair.Key).FirstOrDefault<string>();
	}

	public static string GetClubNameFromID(string clubID)
	{
		if (!Club._clubNamesMap.ContainsKey(clubID))
		{
			return clubID;
		}
		return Club._clubNamesMap[clubID];
	}

	public static string GetClubNameFromString(string clubString)
	{
		return (
			from pair in Club._clubNamesMap
			where clubString.Contains(pair.Value)
			select pair.Value).FirstOrDefault<string>();
	}

	public static string GetClubSpriteName(string clubName)
	{
		string clubIDFromName = Club.GetClubIDFromName(clubName);
		if (string.IsNullOrEmpty(clubIDFromName))
		{
			return string.Empty;
		}
		string typeFromID = Club.GetTypeFromID(clubIDFromName);
		if (string.IsNullOrEmpty(typeFromID))
		{
			return string.Empty;
		}
		if (!Club.ClubSpriteNames.ContainsKey(typeFromID))
		{
			return string.Empty;
		}
		return Club.ClubSpriteNames[typeFromID];
	}

	public static Club.ClubIconType GetIconType(string clubName)
	{
		string clubIDFromName = Club.GetClubIDFromName(clubName);
		if (string.IsNullOrEmpty(clubIDFromName))
		{
			return Club.ClubIconType.Undefined;
		}
		string typeFromID = Club.GetTypeFromID(clubIDFromName);
		if (string.IsNullOrEmpty(typeFromID))
		{
			return Club.ClubIconType.Undefined;
		}
		if (!Club.ClubIconTypes.ContainsKey(typeFromID))
		{
			return Club.ClubIconType.Undefined;
		}
		return Club.ClubIconTypes[typeFromID];
	}

	public static string GetTypeFromID(string clubID)
	{
		string key;
		Dictionary<string, List<string>>.Enumerator enumerator = Club._clubTypesMap.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, List<string>> current = enumerator.Current;
				if (!current.Value.Contains(clubID))
				{
					continue;
				}
				key = current.Key;
				return key;
			}
			return string.Empty;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return key;
	}

	public static string GetTypeFromString(string typeString)
	{
		int num;
		if (typeString != null)
		{
			if (Club.f__switch_map0 == null)
            {
				Dictionary<string, int> strs = new Dictionary<string, int>(9)
				{
					{ "UNDEFINED", 0 },
					{ "DRIVER", 1 },
					{ "HYBRID", 2 },
					{ "WOOD", 3 },
					{ "IRON", 4 },
					{ "PUTTER", 5 },
					{ "WEDGE", 6 },
					{ "SAND WEDGE", 7 },
					{ "CHIPPER", 8 }
				};
				Club.f__switch_map0 = strs;
			}
			if (Club.f__switch_map0.TryGetValue(typeString, out num))
            {
				switch (num)
				{
					case 0:
						{
							return "Undefined";
						}
					case 1:
						{
							return "Driver";
						}
					case 2:
						{
							return "Hybrid";
						}
					case 3:
						{
							return "Wood";
						}
					case 4:
						{
							return "Iron";
						}
					case 5:
						{
							return "Putter";
						}
					case 6:
						{
							return "Wedge";
						}
					case 7:
						{
							return "Sand Wedge";
						}
					case 8:
						{
							return "Chipper";
						}
				}
			}
		}
		return string.Empty;
	}

	public void SetClubEquipped(bool equiped)
	{
		this._isEquipped = equiped;
	}

	public enum ClubIconType
	{
		Undefined,
		Wood,
		Iron,
		Hybrid,
		Putter
	}

	public struct ClubNums
	{
		private int _min;

		private int _max;

		public int max
		{
			get
			{
				return this._max;
			}
		}

		public int min
		{
			get
			{
				return this._min;
			}
		}

		public ClubNums(int min, int max)
		{
			this._min = min;
			this._max = max;
		}
	}
}