using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Data
{
	[DataTable("Activity")]
	public class Activity : DataEntry
	{
		private Session _session;

		private Activity _parent;

		private readonly Dictionary<int, Shot> _shots = new Dictionary<int, Shot>();

		private readonly Dictionary<int, Activity> _subactivities = new Dictionary<int, Activity>();

		private readonly static Dictionary<int, int> OldTypeIdToNewTypeId;

		private readonly static Dictionary<int, int> OldTypeIdToNewSubTypeId;

		[Synchronizable("action", true, true)]
		public string Action
		{
			get
			{
				return (!base.IsOnServer ? "add" : "update");
			}
		}

		[DataColumn("additionalData", false, false, null)]
		[Synchronizable]
		public string AdditionalData
		{
			get;
			set;
		}

		[DataColumn("courseConditionsId", false, false, null)]
		[Synchronizable]
		public string CourseConditionsId
		{
			get;
			set;
		}

		[DataColumn("courseId", false, false, null)]
		[Synchronizable]
		public string CourseId
		{
			get;
			set;
		}

		[DataColumn("distanceToTarget", false, false, null)]
		[Synchronizable("challengeTarget", true, true)]
		public float DistanceToTarget
		{
			get;
			set;
		}

		[Synchronizable("activityDurationSeconds", true, true)]
		public int Duration
		{
			get
			{
				return (int)(this.EndTime - this.StartTime).TotalSeconds;
			}
		}

		[DataColumn("endTime", false, false, null)]
		[Synchronizable("activityEndTime", true, true)]
		public DateTime EndTime
		{
			get;
			set;
		}

		[DataColumn("esn", false, false, null)]
		[Synchronizable("ESN", true, true)]
		public string Esn
		{
			get;
			set;
		}

		[DataColumn("versionFirmware", false, false, null)]
		[Synchronizable]
		public string FirmwareVersion
		{
			get
			{
				return ApplicationDataManager.instance.FWVersion;
			}
		}

		[Synchronizable("esnActivityId", true, true)]
		public override int Id
		{
			get
			{
				return base.Id;
			}
		}

		[DataColumn("groupId", false, false, 0)]
		[Synchronizable]
		public Activity Parent
		{
			get
			{
				return this._parent;
			}
			set
			{
				if (this._parent != null)
				{
					this._parent._subactivities.Remove(this.Id);
				}
				this._parent = value;
				if (this._parent != null)
				{
					this._parent._subactivities.Add(this.Id, this);
				}
			}
		}

		[Synchronizable("serverCalcChallenge", true, true)]
		public string ServerCalcChallenge
		{
			get
			{
				return "no";
			}
		}

		[DataColumn("sessionId", false, false, null)]
		public Session Session
		{
			get
			{
				return this._session;
			}
			set
			{
				if (this._session == value)
				{
					return;
				}
				if (this._session != null)
				{
					Session session = this._session;
					this._session = null;
					session.RemoveActivity(this);
				}
				this._session = value;
				if (this._session != null)
				{
					this._session.AddActivity(this);
				}
			}
		}

		[Synchronizable("sessionUuid", true, true)]
		public string SessionUuid
		{
			get
			{
				return this.Session.Uuid;
			}
			set
			{
				//this.Session = DatabaseManager.SelectWhere<Session>("uuid", value.ToLower(), 1).FirstOrDefault<Session>();
			}
		}

		public Shot[] Shots
		{
			get
			{
				return this._shots.Values.ToArray<Shot>();
			}
		}

		[Synchronizable("versionSoftware", true, true)]
		public string _SoftwareVersion
		{
			get
			{
				return SoftwareVersion.VersionWithPlatform;
			}
		}

		[DataColumn("startTime", false, false, null)]
		[Synchronizable("activityStartTime", true, true)]
		public DateTime StartTime
		{
			get;
			set;
		}

		public Activity[] Subactivities
		{
			get
			{
				return this._subactivities.Values.ToArray<Activity>();
			}
		}

		[DataColumn("subTypeId", false, false, null)]
		[Synchronizable("activitySubTypeId", true, true)]
		public int SubTypeId
		{
			get;
			set;
		}

		[DataColumn("teePositionId", false, false, null)]
		[Synchronizable]
		public string TeePositionId
		{
			get;
			set;
		}

		[DataColumn("typeId", false, false, null)]
		[Synchronizable("activityTypeId", true, true)]
		public int TypeId
		{
			get;
			set;
		}

		[DataColumn("customerId", false, false, null)]
		[Synchronizable]
		public User User
		{
			get;
			set;
		}

		static Activity()
		{
			Dictionary<int, int> nums = new Dictionary<int, int>()
			{
				{ 1, 101 },
				{ 10, 101 },
				{ 3, 102 },
				{ 4, 102 },
				{ 5, 102 },
				{ 6, 103 },
				{ 11, 103 }
			};
			Activity.OldTypeIdToNewTypeId = nums;
			nums = new Dictionary<int, int>()
			{
				{ 1, 101 },
				{ 10, 201 },
				{ 3, 101 },
				{ 4, 102 },
				{ 5, 103 },
				{ 6, 101 },
				{ 11, 201 }
			};
			Activity.OldTypeIdToNewSubTypeId = nums;
		}

		private Activity(int id) : base(id)
		{
		}

		public void AddShot(Shot shot)
		{
			if (this._shots.ContainsKey(shot.Id))
			{
				throw new DataEntryException(string.Format("Shot with ID {0} is already added to activity with ID {1}", shot.Id, this.Id));
			}
			if (shot.Activity != this)
			{
				shot.Activity = this;
				return;
			}
			this._shots[shot.Id] = shot;
		}

		public static Activity.ChallengesSubType GetChallengesSubTypeForGameRulesType(CGameManager.GAME_RULES_TYPE gameRulesType)
		{
			switch (gameRulesType)
			{
				case CGameManager.GAME_RULES_TYPE.LongDrive:
					{
						return Activity.ChallengesSubType.LongestDrive;
					}
				case CGameManager.GAME_RULES_TYPE.ClosestToPin:
					{
						return Activity.ChallengesSubType.ClosestToPin;
					}
				case CGameManager.GAME_RULES_TYPE.Bullseye:
					{
						return Activity.ChallengesSubType.TargetPractice;
					}
			}
			throw new ArgumentException(string.Format("Game rules type {0} has no match for challenges sub types", new object[0]));
		}

		public static CGameManager.GAME_RULES_TYPE GetGameRulesForTypeId(int typeId, int subTypeId)
		{
			switch (typeId)
			{
				case 101:
					{
						return CGameManager.GAME_RULES_TYPE.Practice;
					}
				case 102:
					{
						switch (subTypeId)
						{
							case 101:
								{
									return CGameManager.GAME_RULES_TYPE.ClosestToPin;
								}
							case 102:
								{
									return CGameManager.GAME_RULES_TYPE.LongDrive;
								}
							case 103:
								{
									return CGameManager.GAME_RULES_TYPE.Bullseye;
								}
						}
						throw new NotSupportedException(string.Format("Sub type ID {0} for type ID {1} is not supported", subTypeId, typeId));
					}
				case 103:
					{
						Activity.GameImprovementsSubType gameImprovementsSubType = (Activity.GameImprovementsSubType)subTypeId;
						if (gameImprovementsSubType != Activity.GameImprovementsSubType.SkillsAssessmentMain)
						{
							if (gameImprovementsSubType != Activity.GameImprovementsSubType.BagMappingMain)
							{
								if (gameImprovementsSubType == Activity.GameImprovementsSubType.SkillsAssessmentSubActivity)
								{
									return CGameManager.GAME_RULES_TYPE.SkillsAssessment;
								}
								if (gameImprovementsSubType != Activity.GameImprovementsSubType.BagMappingSubActivity)
								{
									throw new NotSupportedException(string.Format("Sub type ID {0} for type ID {1} is not supported", subTypeId, typeId));
								}
							}
							return CGameManager.GAME_RULES_TYPE.BagMapping;
						}
						return CGameManager.GAME_RULES_TYPE.SkillsAssessment;
					}
			}
			throw new NotSupportedException(string.Format("Type ID {0} is not supported", typeId));
		}

		public Activity[] GetSubactivitiesRecursively(bool includeCurrent)
		{
			List<Activity> activities = new List<Activity>();
			if (includeCurrent)
			{
				activities.Add(this);
			}
			Stack<Activity> activities1 = new Stack<Activity>();
			activities1.Push(this);
			while (activities1.Count > 0)
			{
				Activity[] subactivities = activities1.Pop().Subactivities;
				activities.AddRange(subactivities);
				Activity[] activityArray = subactivities;
				for (int i = 0; i < (int)activityArray.Length; i++)
				{
					activities1.Push(activityArray[i]);
				}
			}
			return activities.ToArray();
		}

		public static int GetSubTypeIdForGameRulesType(CGameManager.GAME_RULES_TYPE gameRulesType)
		{
			switch (gameRulesType)
			{
				case CGameManager.GAME_RULES_TYPE.Practice:
					{
						return 101;
					}
				case CGameManager.GAME_RULES_TYPE.LongDrive:
					{
						return 102;
					}
				case CGameManager.GAME_RULES_TYPE.ClosestToPin:
					{
						return 101;
					}
				case CGameManager.GAME_RULES_TYPE.Bullseye:
					{
						return 103;
					}
				case CGameManager.GAME_RULES_TYPE.SkillsAssessment:
					{
						return 101;
					}
				case CGameManager.GAME_RULES_TYPE.BagMapping:
					{
						return 102;
					}
			}
			throw new NotSupportedException(string.Format("Game rules type {0} is not supported", gameRulesType));
		}

		public static int GetTypeIdForGameRulesType(CGameManager.GAME_RULES_TYPE gameRulesType)
		{
			switch (gameRulesType)
			{
				case CGameManager.GAME_RULES_TYPE.Practice:
					{
						return 101;
					}
				case CGameManager.GAME_RULES_TYPE.LongDrive:
				case CGameManager.GAME_RULES_TYPE.ClosestToPin:
				case CGameManager.GAME_RULES_TYPE.Bullseye:
					{
						return 102;
					}
				case CGameManager.GAME_RULES_TYPE.SkillsAssessment:
				case CGameManager.GAME_RULES_TYPE.BagMapping:
					{
						return 103;
					}
			}
			throw new NotSupportedException(string.Format("Game rules type {0} is not supported", gameRulesType));
		}

		public void RemoveShot(Shot shot)
		{
			if (!this._shots.ContainsKey(shot.Id))
			{
				throw new DataEntryException(string.Format("Shot with ID {0} is not added to activity with ID {1}", shot.Id, this.Id));
			}
			if (shot.Activity == this)
			{
				shot.Activity = null;
				return;
			}
			this._shots.Remove(shot.Id);
		}

		internal override void WillBeSavedToDatabase()
		{
			base.WillBeSavedToDatabase();
			if (this.TypeId > 100)
			{
				return;
			}
			if (!Activity.OldTypeIdToNewTypeId.ContainsKey(this.TypeId) || !Activity.OldTypeIdToNewSubTypeId.ContainsKey(this.TypeId))
			{
				throw new NotSupportedException(string.Format("Type ID {0} is not supported and cannot be converted to conform to the new standard", this.TypeId));
			}
			int typeId = this.TypeId;
			int subTypeId = this.SubTypeId;
			this.TypeId = Activity.OldTypeIdToNewTypeId[typeId];
			this.SubTypeId = Activity.OldTypeIdToNewSubTypeId[typeId];
			if (this.SubTypeId == 201)
			{
				this.SubTypeId = 201 + subTypeId;
			}
		}

		public enum ChallengesSubType
		{
			ClosestToPin = 101,
			LongestDrive = 102,
			TargetPractice = 103
		}

		public enum GameImprovementsSubType
		{
			SkillsAssessmentMain = 101,
			BagMappingMain = 102,
			SkillsAssessmentSubActivity = 201,
			BagMappingSubActivity = 202
		}

		public enum PracticeSubType
		{
			Practice = 101,
			PracticeGreen = 201
		}

		public enum Type
		{
			Practice = 101,
			Challenges = 102,
			GameImprovements = 103
		}
	}
}