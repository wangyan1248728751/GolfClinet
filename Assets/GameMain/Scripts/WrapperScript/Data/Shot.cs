using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Data
{
	[DataTable("Shot")]
	public class Shot : DataEntry
	{
		private const int ClubNameServerCap = 16;

		private Activity _activity;

		[DataColumn("activityId", false, false, null)]
		[Synchronizable("esnActivityId", true, true)]
		public Activity Activity
		{
			get
			{
				return this._activity;
			}
			set
			{
				if (this._activity == value)
				{
					return;
				}
				if (this._activity != null)
				{
					Activity activity = this._activity;
					this._activity = null;
					activity.RemoveShot(this);
				}
				this._activity = value;
				if (this._activity != null)
				{
					this._activity.AddShot(this);
				}
			}
		}

		[DataColumn("altitude", false, false, null)]
		[Synchronizable("maxHeight", true, true)]
		public float Altitude
		{
			get;
			set;
		}

		[DataColumn("backSpin", false, false, null)]
		[Synchronizable]
		public float BackSpin
		{
			get;
			set;
		}

		[DataColumn("ballSpeed", false, false, null)]
		[Synchronizable]
		public float BallSpeed
		{
			get;
			set;
		}

		[DataColumn("carryDistance", false, false, null)]
		[Synchronizable]
		public float CarryDistance
		{
			get;
			set;
		}

		[DataColumn("carryPosX", false, false, null)]
		[Synchronizable]
		public float CarryPosX
		{
			get;
			set;
		}

		[DataColumn("carryPosY", false, false, null)]
		[Synchronizable]
		public float CarryPosY
		{
			get;
			set;
		}

		[Synchronizable("shotValue", false, true)]
		public float ChallengeScore
		{
			set
			{
				if (this.Score < 0f && value >= 0f)
				{
					this.Score = value;
				}
			}
		}

		[DataColumn("challengeScoreDetail", false, false, null)]
		[Synchronizable]
		public int ChallengeScoreDetail
		{
			get;
			set;
		}

		[DataColumn("clubHeadSpeed", false, false, null)]
		[Synchronizable("clubheadSpeed", true, true)]
		public float ClubHeadSpeed
		{
			get;
			set;
		}

		[Obsolete("This data element is no longer in use")]
		[Synchronizable("uClubId", true, false)]
		public string ClubId
		{
			get
			{
				return string.Empty;
			}
		}

		[DataColumn("clubName", false, false, null)]
		public string ClubName
		{
			get;
			set;
		}

		[Synchronizable("clubName", true, true)]
		private string ClubNameToSync
		{
			get
			{
				string serverSafe;
				if (this.ClubName == null)
				{
					serverSafe = null;
				}
				else
				{
					serverSafe = Shot.ConvertClubNameToServerSafe(this.ClubName);
				}
				return serverSafe;
			}
			set
			{
				this.ClubName = Shot.ConvertClubNameFromServerSafe(value);
			}
		}

		[DataColumn("clubType", false, false, null)]
		[Synchronizable]
		public string ClubType
		{
			get;
			set;
		}

		[DataColumn("courseConditionId", false, false, null)]
		[Synchronizable("courseConditionsId", true, true)]
		public int CourseConditionId
		{
			get;
			set;
		}

		[DataColumn("descentAngle", false, false, null)]
		[Synchronizable("angleOfDescent", true, true)]
		public float DescentAngle
		{
			get;
			set;
		}

		[DataColumn("deviceTimeStamp", false, false, null)]
		[Synchronizable("device_TimeStamp", true, true)]
		public string DeviceTimeStamp
		{
			get;
			set;
		}

		[DataColumn("dexterity", false, false, null)]
		[Synchronizable]
		public string Dexterity
		{
			get;
			set;
		}

		public float DistanceToPin
		{
			get
			{
				return Mathf.Sqrt(this.XOffset * this.XOffset + this.YOffset * this.YOffset);
			}
		}

		[Synchronizable("ESN", true, true)]
		public string Esn
		{
			get
			{
				return this.Activity.Esn;
			}
		}

		[DataColumn("flightTime", false, false, null)]
		[Synchronizable]
		public float FlightTime
		{
			get;
			set;
		}

		[DataColumn("gameXStart", false, false, null)]
		[Synchronizable("gameXstart", true, true)]
		public float GameXStart
		{
			get;
			set;
		}

		[DataColumn("gameYStart", false, false, null)]
		[Synchronizable("gameYstart", true, true)]
		public float GameYStart
		{
			get;
			set;
		}

		[DataColumn("holeNumber", false, false, null)]
		[Synchronizable]
		public int HoleNumber
		{
			get;
			set;
		}

		[DataColumn("horizontalLaunchAngle", false, false, null)]
		[Synchronizable]
		public float HorizontalLaunchAngle
		{
			get;
			set;
		}

		[Synchronizable("shotId", true, true)]
		public override int Id
		{
			get
			{
				return base.Id;
			}
		}

		[DataColumn("isFavorite", false, false, null)]
		[Synchronizable]
		public bool IsFavorite
		{
			get;
			set;
		}

		[DataColumn("shotNumber", false, false, null)]
		[Synchronizable]
		public int NumberInActivity
		{
			get;
			set;
		}

		[DataColumn("offline", false, false, null)]
		[Synchronizable]
		public float Offline
		{
			get;
			set;
		}

		[DataColumn("rollDistance", false, false, null)]
		[Synchronizable]
		public float RollDistance
		{
			get;
			set;
		}

		[DataColumn("score", false, false, -1f)]
		[Synchronizable("score", true, true)]
		public float Score
		{
			get;
			set;
		}

		[DataColumn("time", false, false, null)]
		[Synchronizable("clientDateCreated", true, true)]
		public DateTime ShotDate
		{
			get;
			set;
		}

		[DataColumn("sideSpin", false, false, null)]
		[Synchronizable]
		public float SideSpin
		{
			get;
			set;
		}

		[DataColumn("smashFactor", false, false, null)]
		[Synchronizable("PTI_smashFactor", true, true)]
		public float SmashFactor
		{
			get;
			set;
		}

		[DataColumn("totalDistance", false, false, null)]
		[Synchronizable]
		public float TotalDistance
		{
			get;
			set;
		}

		[DataColumn("totalPosX", false, false, null)]
		[Synchronizable]
		public float TotalPosX
		{
			get;
			set;
		}

		[DataColumn("totalPosY", false, false, null)]
		[Synchronizable]
		public float TotalPosY
		{
			get;
			set;
		}

		[DataColumn("UOM", false, false, null)]
		[Synchronizable]
		public string UnitOfMeasure
		{
			get;
			set;
		}

		[Synchronizable("CustomerID", true, true)]
		public User User
		{
			get
			{
				return this.Activity.User;
			}
			set
			{
				this.Activity.User = value;
			}
		}

		[DataColumn("verticalLaunchAngle", false, false, null)]
		[Synchronizable]
		public float VerticalLaunchAngle
		{
			get;
			set;
		}

		[DataColumn("xOffset", false, false, null)]
		[Synchronizable("gameXfinish", true, true)]
		public float XOffset
		{
			get;
			set;
		}

		[DataColumn("yOffset", false, false, null)]
		[Synchronizable("gameYfinish", true, true)]
		public float YOffset
		{
			get;
			set;
		}

		private Shot(int id) : base(id)
		{
		}

		private static string ConvertClubNameFromServerSafe(string clubName)
		{
			if (string.IsNullOrEmpty(clubName))
			{
				return string.Empty;
			}
			clubName = clubName.Replace("%B0", "°");
			clubName = clubName.Replace("%23", "#");
			return clubName;
		}

		private static string ConvertClubNameToServerSafe(string clubName)
		{
			string str = string.Copy(clubName);
			if (str.Contains("°"))
			{
				string[] strArrays = str.Split(new string[] { "°" }, StringSplitOptions.RemoveEmptyEntries);
				str = string.Concat(strArrays[0], "%B0");
				if ((int)strArrays.Length > 1)
				{
					for (int i = 1; i < (int)strArrays.Length; i++)
					{
						str = string.Concat(str, strArrays[i]);
					}
				}
			}
			if (str.Contains("#"))
			{
				string[] strArrays1 = str.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
				str = string.Concat(strArrays1[0], "%23");
				if ((int)strArrays1.Length > 1)
				{
					for (int j = 1; j < (int)strArrays1.Length; j++)
					{
						str = string.Concat(str, strArrays1[j]);
					}
				}
			}
			if (str.Length > 16)
			{
				AppLog.Log("ClubNameServerSafe::ClubName is too large, sending truncated string", true);
				str = str.Substring(0, 16);
			}
			return str;
		}
	}
}