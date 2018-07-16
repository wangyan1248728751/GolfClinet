//using Converters;
//using Data;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading;
//using UnityEngine;
//using Utilities;

//public static class DatabaseManager
//{
//	private const string DatabaseVersion = "1.7.12";

//	private const string DatabaseFilename = "EtrGolf.db";

//	public const string NoDataTag = "NO_DATA";

//	public readonly static string DatabasePath;

//	private readonly static object DatabaseLock;

//	static DatabaseManager()
//	{
//		DatabaseManager.DatabasePath = string.Concat(Application.persistentDataPath, "/EtrGolf.db");
//		DatabaseManager.DatabaseLock = new object();
//	}

//	internal static void CleanUp()
//	{
//		DatabaseManager.TryExecuteQuery(string.Format("DELETE FROM {0} WHERE isReadyToBeSynchronized = \"0\"", ReflectionUtilities.GetTableName<Shot>()), null);
//		DatabaseManager.TryExecuteQuery(string.Format("DELETE FROM {0} WHERE isReadyToBeSynchronized = \"0\"", ReflectionUtilities.GetTableName<Activity>()), null);
//	}

//	[DebuggerHidden]
//	private static IEnumerator CreateDatabase(Action onFinish)
//	{
//		DatabaseManager.< CreateDatabase > c__Iterator0 variable = null;
//		return variable;
//	}

//	private static object CreateInstanceOnQueryStep(Type type, SQLiteQuery query)
//	{
//		if (!type.IsSubclassOf(typeof(DataEntry)))
//		{
//			throw new ArgumentException(string.Format("Type {0} is not subclass of {1}", type.Name, typeof(DataEntry).Name));
//		}
//		object obj = ReflectionUtilities.Construct(type, new object[] { query.GetInteger("id") });
//		DatabaseManager.ForEachDataColumnProperty(type, (PropertyInfo p, DataColumnAttribute a) =>
//		{
//			if (!a.IsPrimaryKey)
//			{
//				obj.SetValue(p, DatabaseManager.GetValueFromSqliteQuery(query, a.Name, p.PropertyType));
//			}
//		});
//		return obj;
//	}

//	private static T CreateInstanceOnQueryStep<T>(SQLiteQuery query)
//	where T : DataEntry
//	{
//		return (T)DatabaseManager.CreateInstanceOnQueryStep(typeof(T), query);
//	}

//	internal static void CreateTable<T>()
//	where T : DataEntry
//	{
//		StringBuilder stringBuilder = new StringBuilder(string.Format("CREATE TABLE IF NOT EXISTS \"{0}\" (", ReflectionUtilities.GetTableName<T>()));
//		DatabaseManager.ForEachDataColumnProperty<T>((PropertyInfo p, DataColumnAttribute a) => stringBuilder.AppendFormat("`{0}` {1} {2},", a.Name, DatabaseManager.GetSqliteType(p.PropertyType), (!a.IsPrimaryKey ? string.Empty : "NOT NULL PRIMARY KEY")));
//		if (stringBuilder[stringBuilder.Length - 1] == ',')
//		{
//			stringBuilder.Remove(stringBuilder.Length - 1, 1);
//		}
//		stringBuilder.Append(")");
//		DatabaseManager.TryExecuteQuery(stringBuilder.ToString(), null);
//	}

//	private static void CreateTables()
//	{
//		Debug.Log("creating tables");
//		DatabaseManager.CreateTable<Shot>();
//		DatabaseManager.CreateTable<Activity>();
//		DatabaseManager.CreateTable<Session>();
//		DatabaseManager.CreateTable<User>();
//		foreach (string str in new List<string>()
//		{
//			"CREATE TABLE if not exists [PlayerData] ( [PlayerDataID] INTEGER  PRIMARY KEY NOT NULL, [customerId] nvarchar  DEFAULT \"null\", [esnUserId] nvarchar  DEFAULT \"null\", [firstName] nvarchar  DEFAULT \"null\", [lastName] nvarchar  DEFAULT \"null\", [status] nvarchar  DEFAULT \"null\", [email] nvarchar  DEFAULT \"null\", [Phone] NVARCHAR  DEFAULT \"null\", [gender] nvarchar  DEFAULT \"null\", [FindUs] NVARCHAR  DEFAULT \"null\", [Address1] NVARCHAR  DEFAULT \"null\", [Address2] NVARCHAR  DEFAULT \"null\", [City] NVARCHAR  DEFAULT \"null\", [Country] NVARCHAR  DEFAULT \"null\", [State] NVARCHAR  DEFAULT \"null\", [Zip] NVARCHAR  DEFAULT \"null\", [skyTrakProfileName] nvarchar  DEFAULT \"null\", [skyTrakUserName] nvarchar  DEFAULT \"null\", [skyGolfUserName] nvarchar  DEFAULT \"null\", [skyGolfNickName] nvarchar  DEFAULT \"null\", [skyGolfId] nvarchar  DEFAULT \"null\", [hdcp] nvarchar  DEFAULT \"null\", [handedness] nvarchar  DEFAULT \"null\", [height] nvarchar  DEFAULT \"null\", [wristToFloor] nvarchar  DEFAULT \"null\", [longestFinger] nvarchar  DEFAULT \"null\", [handLength] nvarchar  DEFAULT \"null\", [handsize] nvarchar  DEFAULT \"null\", [locale] nvarchar  DEFAULT \"null\", [birthdate] nvarchar  DEFAULT \"null\", [DATE_CREATED] nvarchar  DEFAULT \"null\", [DATE_UPDATED] nvarchar  DEFAULT \"null\", [skyTrakUserId] nvarchar  DEFAULT \"null\" );",
//			"CREATE TABLE if not exists [Version] ([VersionId] NVARCHAR  NULL)"
//		})
//		{
//			DatabaseManager.TryExecuteQuery(str, null);
//		}
//		DatabaseManager.TryExecuteQuery(string.Format("INSERT OR REPLACE INTO Version (VersionId) VALUES (\"{0}\")", "1.7.12"), null);
//		PlayerPrefsX.SetBool("DatabaseFullyCreated", true);
//	}

//	internal static bool DoesChallengeHaveLeaderboard(int activityId)
//	{
//		if (!LoginManager.IsUserLoggedIn)
//		{
//			return false;
//		}
//		return DatabaseManager.TryExecuteQuery<int>(string.Format("SELECT count(*) AS shotCount FROM Activity JOIN Shot ON Shot.activityId = Activity.id AND Shot.score >= 0 WHERE Activity.id = {0} AND Activity.customerId = {1}", activityId, LoginManager.UserData.Id), (SQLiteQuery q) => q.GetInteger("shotCount"))[0] >= 3;
//	}

//	internal static bool Exists<T>(int id)
//	where T : DataEntry
//	{
//		return DatabaseManager.Exists(typeof(T), id);
//	}

//	internal static bool Exists(Type type, int id)
//	{
//		if (!type.IsSubclassOf(typeof(DataEntry)))
//		{
//			throw new ArgumentException(string.Format("Type {0} is not subclass of {1}", type.Name, typeof(DataEntry).Name));
//		}
//		return DatabaseManager.TryExecuteQuery<int>(string.Format("SELECT count(*) AS `count` FROM \"{0}\" WHERE `id` = {1}", ReflectionUtilities.GetTableName(type), id), (SQLiteQuery q) => q.GetInteger("count"))[0] > 0;
//	}

//	internal static bool Exists<T>(Dictionary<string, object> values)
//	where T : DataEntry
//	{
//		if (values == null || values.Count == 0)
//		{
//			throw new ArgumentException("No values provided", "values");
//		}
//		return DatabaseManager.TryExecuteQuery<int>(string.Format("SELECT count(*) AS `count` FROM \"{0}\" WHERE {1}", ReflectionUtilities.GetTableName<T>(), (
//			from kvp in values
//			select string.Format("`{0}` = \"{1}\"", kvp.Key, kvp.Value)).Aggregate<string>((string s1, string s2) => string.Concat(s1, " AND ", s2))), (SQLiteQuery q) => q.GetInteger("count"))[0] > 0;
//	}

//	private static void ForEachDataColumnProperty<T>(Action<PropertyInfo, DataColumnAttribute> action)
//	where T : DataEntry
//	{
//		DatabaseManager.ForEachDataColumnProperty(typeof(T), action);
//	}

//	private static void ForEachDataColumnProperty(Type type, Action<PropertyInfo, DataColumnAttribute> action)
//	{
//		if (action == null)
//		{
//			throw new ArgumentNullException("action");
//		}
//		foreach (KeyValuePair<PropertyInfo, DataColumnAttribute> dataColumnProperty in ReflectionUtilities.GetDataColumnProperties(type))
//		{
//			action(dataColumnProperty.Key, dataColumnProperty.Value);
//		}
//	}

//	private static string GameRulesToSqlCondition(CGameManager.GAME_RULES_TYPE[] rules)
//	{
//		return (
//			from r in (IEnumerable<CGameManager.GAME_RULES_TYPE>)rules
//			select string.Format("(Activity.typeId = {0} AND Activity.subTypeId = {1})", Activity.GetTypeIdForGameRulesType(r), Activity.GetSubTypeIdForGameRulesType(r))).Aggregate<string>((string s1, string s2) => string.Format("{0} OR {1}", s1, s2));
//	}

//	private static DataPoint[] GetActivitiesReport(DatabaseManager.ActivitiesReportQuery reportType)
//	{
//		string str;
//		if (!LoginManager.IsUserLoggedIn)
//		{
//			return new DataPoint[0];
//		}
//		switch (reportType)
//		{
//			case DatabaseManager.ActivitiesReportQuery.Practice:
//				{
//					str = string.Concat("typeId = ", 101);
//					break;
//				}
//			case DatabaseManager.ActivitiesReportQuery.Challenges:
//				{
//					str = string.Concat("typeId = ", 102);
//					break;
//				}
//			case DatabaseManager.ActivitiesReportQuery.All:
//				{
//					str = "typeId > 0";
//					break;
//				}
//			default:
//				{
//					throw new NotSupportedException(string.Format("Report type {0} is not supported", reportType));
//				}
//		}
//		return DatabaseManager.TryExecuteQuery<DataPoint>(string.Format("SELECT startTime, endTime FROM Activity WHERE customerId = \"{0}\" AND {1} AND groupId = 0 AND isDeleted = 0", LoginManager.UserData.Id, str), (SQLiteQuery q) => new DataPoint()
//		{
//			Start = q.GetString("startTime").ParseDateTime(),
//			End = q.GetString("endTime").ParseDateTime()
//		});
//	}

//	internal static DataSummary GetActivitiesReportForNonPractice()
//	{
//		DataPoint[] activitiesReport = DatabaseManager.GetActivitiesReport(DatabaseManager.ActivitiesReportQuery.Challenges);
//		DataSummary dataSummary = new DataSummary()
//		{
//			Count = (int)activitiesReport.Length,
//			Time = (float)((IEnumerable<DataPoint>)activitiesReport).Sum<DataPoint>((DataPoint d) => (d.End - d.Start).TotalSeconds)
//		};
//		return dataSummary;
//	}

//	internal static DataSummary GetActivitiesReportForPractice()
//	{
//		DataPoint[] activitiesReport = DatabaseManager.GetActivitiesReport(DatabaseManager.ActivitiesReportQuery.Practice);
//		DataSummary dataSummary = new DataSummary()
//		{
//			Count = (int)activitiesReport.Length,
//			Time = (float)((IEnumerable<DataPoint>)activitiesReport).Sum<DataPoint>((DataPoint d) => (d.End - d.Start).TotalSeconds)
//		};
//		return dataSummary;
//	}

//	private static Shot[] GetActivityShots(Activity activity)
//	{
//		if (!LoginManager.IsUserLoggedIn)
//		{
//			return new Shot[0];
//		}
//		return DatabaseManager.TryExecuteQuery<Shot>(string.Format("SELECT Shot.* FROM Shot JOIN Activity ON Activity.id = Shot.activityId WHERE Shot.isDeleted = 0 AND Activity.customerId = \"{0}\" AND Shot.activityId = {1}", LoginManager.UserData.Id, activity.Id), (SQLiteQuery q) =>
//		{
//			int integer = q.GetInteger("id");
//			return (!DataEntry.InstanceExists<Shot>(integer) ? DatabaseManager.CreateInstanceOnQueryStep<Shot>(q) : DataEntry.GetInstance<Shot>(integer));
//		});
//	}

//	internal static int GetAvailableId<T>()
//	where T : DataEntry
//	{
//		return DatabaseManager.TryExecuteQuery<int>(string.Format("SELECT max(id) AS `maxId` FROM \"{0}\"", ReflectionUtilities.GetTableName<T>()), (SQLiteQuery q) => (!q.IsNULL("maxId") ? q.GetInteger("maxId") + 1 : 1))[0];
//	}

//	internal static ChallengeHistoryData GetChallengeHistory(int activityId, Activity.ChallengesSubType type)
//	{
//		if (!LoginManager.IsUserLoggedIn)
//		{
//			return new ChallengeHistoryData();
//		}
//		ChallengeHistoryData challengeHistoryDatum = new ChallengeHistoryData();
//		DatabaseManager.TryExecuteQuery(string.Format("SELECT Shot.score as shotScore, {2} Activity.distanceToTarget, Activity.startTime AS startTime FROM Activity JOIN Shot ON Shot.activityId = Activity.id WHERE Activity.id = {0} AND Activity.customerId = {1} ORDER BY Shot.time", activityId, LoginManager.UserData.Id, (type != Activity.ChallengesSubType.LongestDrive ? string.Empty : "Shot.carryDistance as carry, Shot.rollDistance as roll,")), (SQLiteQuery q) =>
//		{
//			float num = (float)q.GetDouble("shotScore");
//			if (type != Activity.ChallengesSubType.LongestDrive)
//			{
//				challengeHistoryDatum.AllShotScores.Add(num);
//			}
//			else if (!num.NearEquals(0f, 4))
//			{
//				float distance = UnitsConverter.Instance.GetDistance((float)q.GetDouble("carry"));
//				float single = UnitsConverter.Instance.GetDistance((float)q.GetDouble("roll"));
//				challengeHistoryDatum.AllShotScores.Add((float)(Mathf.RoundToInt(distance) + Mathf.RoundToInt(single)));
//			}
//			else
//			{
//				challengeHistoryDatum.AllShotScores.Add(num);
//			}
//			challengeHistoryDatum.DistanceToPin = (float)q.GetDouble("distanceToTarget");
//			challengeHistoryDatum.ChallengeStartTime = q.GetString("startTime");
//		});
//		return challengeHistoryDatum;
//	}

//	internal static ChallengeTopScore[] GetChallengeTopScores(Activity.ChallengesSubType type, int count, DatabaseManager.QueryAggregateFunc selectMethod, bool ascending = false)
//	{
//		if (!LoginManager.IsUserLoggedIn)
//		{
//			return new ChallengeTopScore[0];
//		}
//		object[] str = new object[] { 102, (int)type, LoginManager.UserData.Id, null, null, null, null };
//		str[3] = (!ascending ? "DESC" : string.Empty);
//		str[4] = count;
//		str[5] = selectMethod.ToString();
//		str[6] = (type != Activity.ChallengesSubType.LongestDrive ? "(Shot.score)" : string.Format("(Round(Shot.carryDistance{0}) + Round(Shot.rollDistance{0}))", (!UnitsConverter.Instance.IsMetricsEnabled ? string.Empty : "*0.9144")));
//		return DatabaseManager.TryExecuteQuery<ChallengeTopScore>(string.Format("SELECT {5}{6} as bestScore, Activity.id, Shot.id, Activity.startTime AS startTime, Activity.distanceToTarget FROM Shot JOIN Activity ON Shot.activityId = Activity.id WHERE Shot.score > 0 AND Activity.typeId = {0} AND Activity.subTypeId = {1} AND Activity.customerId = {2} GROUP BY 2 ORDER BY bestScore {3} LIMIT {4}", str), (SQLiteQuery q) => new ChallengeTopScore()
//		{
//			Score = (float)q.GetDouble("bestScore"),
//			Date = q.GetString("startTime"),
//			DistanceToPin = (float)q.GetDouble("distanceToTarget")
//		});
//	}

//	internal static DataEntry[] GetDirtyDataEntries(int limit)
//	{
//		List<DataEntry> dataEntries = new List<DataEntry>();
//		dataEntries.AddRange(DatabaseManager.SelectDirtyShots(limit));
//		if (limit - dataEntries.Count > 0)
//		{
//			dataEntries.AddRange(DatabaseManager.SelectReferencedDirtyActivities(limit - dataEntries.Count));
//		}
//		if (limit - dataEntries.Count > 0)
//		{
//			dataEntries.AddRange(DatabaseManager.SelectReferencedDirtySessions(limit - dataEntries.Count));
//		}
//		return dataEntries.ToArray();
//	}

//	public static HistoryActivityData[] GetHistoryActivityDataByUser(string customerId, DatabaseManager.QueryType type)
//	{
//		Dictionary<int, HistoryActivityData> nums = new Dictionary<int, HistoryActivityData>();
//		if (string.IsNullOrEmpty(customerId))
//		{
//			return new HistoryActivityData[0];
//		}
//		string str = string.Format("\r\nSELECT DISTINCT\r\n\tinnerSelect.activityId as activityId,\r\n\tinnerSelect.activityTypeId as activityTypeId,\r\n\tinnerSelect.activitySubTypeId as activitySubTypeId,\r\n\tinnerSelect.groupId as groupId,\r\n\tShot.clubName as clubName,\r\n\tShot.clubtype as clubType,\r\n\tinnerSelect.activityStartTime as activityStartTime,\r\n\tinnerSelect.activityEndTime as activityEndTime\r\nFROM\r\n\t(SELECT\r\n\t\t\tActivity.id AS activityId,\r\n\t\t\tActivity.typeId AS activityTypeId,\r\n\t\t\tActivity.subTypeId AS activitySubTypeId,\r\n\t\t\tActivity.groupId AS groupId,\r\n\t\t\tActivity.customerId AS customerId,\r\n\t\t\tActivity.startTime AS activityStartTime,\r\n\t\t\tActivity.endTime AS activityEndTime\r\n\t\tFROM Activity\r\n\t\tLEFT OUTER JOIN\r\n\t\t\t(SELECT \r\n\t\t\t\tid as subActivityId, \r\n\t\t\t\tgroupId\r\n\t\t\tFROM Activity\r\n\t\t\tWHERE \r\n\t\t\t\tcustomerId = \"{0}\") AS subActivities\r\n\t\tON subActivities.groupId = Activity.id\r\n\t\tWHERE\r\n\t\t\tActivity.customerId = \"{0}\" AND\r\n\t\t\tActivity.isDeleted = 0 AND\r\n\t\t\t({1})\r\n\tUNION ALL\r\n\tSELECT\r\n\t\t\tsubActivities.subActivityId AS activityId,\r\n\t\t\tsubActivities.subActivityTypeId AS activityTypeId,\r\n\t\t\tsubActivities.subActivitySubTypeId AS activitySubTypeId,\r\n\t\t\tActivity.id AS groupId,\r\n\t\t\tActivity.customerId AS customerId,\r\n\t\t\tsubActivities.activityStartTime AS activityStartTime,\r\n\t\t\tsubActivities.activityEndTime AS activityEndTime\r\n\t\tFROM Activity\r\n\t\tLEFT OUTER JOIN\r\n\t\t\t(SELECT \r\n\t\t\t\tid AS subActivityId,\r\n\t\t\t\ttypeId as subActivityTypeId,\r\n\t\t\t\tsubTypeId as subActivitySubTypeId,\r\n\t\t\t\tstartTime AS activityStartTime,\r\n\t\t\t\tendTime AS activityEndTime,\r\n\t\t\t\tgroupId\r\n\t\t\tFROM Activity\r\n\t\t\tWHERE \r\n\t\t\t\tcustomerId = \"{0}\") AS subActivities\r\n\t\tON subActivities.groupId = Activity.id\r\n\t\tWHERE\r\n\t\t\tActivity.customerId = \"{0}\" AND\r\n\t\t\tActivity.isDeleted = 0 AND\r\n\t\t\t({1})\r\n\t) AS innerSelect\r\nLEFT OUTER JOIN\r\n\tShot\r\nON Shot.activityId = innerSelect.activityId AND\r\n   Shot.isDeleted = 0\r\nWHERE \r\n\tinnerSelect.activityId IS NOT NULL\r\nORDER BY innerSelect.activityStartTime DESC", customerId, DatabaseManager.HistoryQueryTypeToSqlCondition(type));
//		DatabaseManager.TryExecuteQuery(str, (SQLiteQuery q) => DatabaseManager.ProcessHistoryActivityDataQuery(q, nums));
//		return nums.Values.ToArray<HistoryActivityData>();
//	}

//	public static void GetHistoryActivityDataByUserAsync(string customerId, DatabaseManager.QueryType type, Action<HistoryActivityData[]> callback, Action<Exception> onError)
//	{
//		if (callback == null)
//		{
//			throw new ArgumentNullException("callback");
//		}
//		ThreadingUtilities.Run<HistoryActivityData[]>(() => DatabaseManager.GetHistoryActivityDataByUser(customerId, type), (HistoryActivityData[] h) => callback(h), onError);
//	}

//	public static BagMappingGameData GetHistoryBagMapping(int mainActivityId, DateTime activityStartTime)
//	{
//		Dictionary<int, BagMappingPointSetup> nums = new Dictionary<int, BagMappingPointSetup>();
//		DatabaseManager.TryExecuteQuery(string.Format("\r\nSELECT Activity.id as activityId, Activity.additionalData, Shot.*\r\nFROM Activity\r\nJOIN Shot ON Shot.activityId = Activity.id\r\nWHERE \r\n    Activity.groupId = {0} AND \r\n    Activity.typeId = {1} AND \r\n    Activity.subTypeId = {2} AND\r\n    Shot.isDeleted = 0", mainActivityId, 103, 202), (SQLiteQuery q) =>
//		{
//			int integer = q.GetInteger("activityId");
//			int num = q.GetInteger("id");
//			Shot shot = (!DataEntry.InstanceExists<Shot>(num) ? DatabaseManager.CreateInstanceOnQueryStep<Shot>(q) : DataEntry.GetInstance<Shot>(num));
//			if (!nums.ContainsKey(integer))
//			{
//				nums[integer] = new BagMappingPointSetup(shot.ClubName);
//			}
//			nums[integer].AddShot(new BagMappingShotData((new ShotFlightDataConverter()).Convert(shot), new Vector2(shot.TotalPosX, shot.TotalPosY), new Vector2(shot.CarryPosX, shot.CarryPosY)));
//		});
//		float single = 0f;
//		float single1 = 0f;
//		DatabaseManager.TryExecuteQuery(string.Format("\r\nSELECT Activity.additionalData\r\nFROM Activity\r\nWHERE \r\n    Activity.id = {0} AND \r\n    Activity.typeId = {1} AND \r\n    Activity.subTypeId = {2} AND \r\n    Activity.isDeleted = 0", mainActivityId, 103, 102), (SQLiteQuery q) =>
//		{
//			JSONObject jSONObject = new JSONObject(q.GetString("additionalData").Replace("\\\"", "\""), -2, false, false);
//			single = (!jSONObject.HasField("IntendedGap") ? 0f : float.Parse(jSONObject.GetField("IntendedGap").str));
//			single1 = (!jSONObject.HasField("GapTolerance") ? 0f : float.Parse(jSONObject.GetField("GapTolerance").str));
//		});
//		return new BagMappingGameData(nums.Values.ToArray<BagMappingPointSetup>(), single, single1, activityStartTime);
//	}

//	public static TFlightData[] GetHistoryShotsAsFlightData(int mainActivityId)
//	{
//		List<int> nums = new List<int>()
//		{
//			mainActivityId
//		};
//		if (DatabaseManager.QueryExistsSubActivity(mainActivityId))
//		{
//			nums.AddRange(
//				from a in (IEnumerable<Activity>)DatabaseManager.GetSubactivitiesForParentActivity(mainActivityId)
//				select a.Id);
//		}
//		return nums.SelectMany<int, TFlightData>((int activity) => DatabaseManager.TryExecuteQuery<TFlightData>(string.Format("SELECT * FROM Shot WHERE activityId = {0} AND isDeleted = 0", activity), (SQLiteQuery qr) =>
//		{
//			Shot shot;
//			int integer = qr.GetInteger("id");
//			shot = (!DataEntry.InstanceExists<Shot>(integer) ? DatabaseManager.CreateInstanceOnQueryStep<Shot>(qr) : DataEntry.GetInstance<Shot>(integer));
//			return (new ShotFlightDataConverter()).Convert(shot);
//		})).ToArray<TFlightData>();
//	}

//	public static void GetHistoryShotsAsFlightDataAsync(int mainActivityId, Action<TFlightData[]> callback, Action<Exception> onError)
//	{
//		if (callback == null)
//		{
//			throw new ArgumentNullException("callback");
//		}
//		ThreadingUtilities.Run<TFlightData[]>(() => DatabaseManager.GetHistoryShotsAsFlightData(mainActivityId), (TFlightData[] f) => callback(f), onError);
//	}

//	public static SAGameData GetHistorySkillAssessments(int mainActivityId, DateTime activityStartTime)
//	{
//		Dictionary<int, SkillAssessmentTargetSetup> nums = new Dictionary<int, SkillAssessmentTargetSetup>();
//		DatabaseManager.TryExecuteQuery(string.Format("\r\nSELECT Activity.id as activityId, Activity.distanceToTarget, Shot.*\r\nFROM Activity\r\nJOIN Shot ON Shot.activityId = Activity.id\r\nWHERE \r\n    Activity.groupId = {0} AND \r\n    Activity.typeId = {1} AND \r\n    Activity.subTypeId = {2} AND \r\n    Activity.isDeleted = 0 AND \r\n    Shot.isDeleted = 0", mainActivityId, 103, 201), (SQLiteQuery q) =>
//		{
//			int integer = q.GetInteger("activityId");
//			float num = (float)q.GetDouble("distanceToTarget");
//			int integer1 = q.GetInteger("id");
//			Shot shot = (!DataEntry.InstanceExists<Shot>(integer1) ? DatabaseManager.CreateInstanceOnQueryStep<Shot>(q) : DataEntry.GetInstance<Shot>(integer1));
//			if (!nums.ContainsKey(integer))
//			{
//				nums[integer] = new SkillAssessmentTargetSetup(num, shot.ClubName);
//			}
//			nums[integer].AddShot(new SkillAssessmentShotData(shot.DistanceToPin, shot.XOffset, shot.YOffset, (new ShotFlightDataConverter()).Convert(shot)));
//		});
//		return new SAGameData(nums.Values.ToArray<SkillAssessmentTargetSetup>(), activityStartTime);
//	}

//	private static T GetInstanceIfExistsOrCreateOnQueryStep<T>(SQLiteQuery query)
//	where T : DataEntry
//	{
//		int integer = query.GetInteger("id");
//		return (!DataEntry.InstanceExists<T>(integer) ? DatabaseManager.CreateInstanceOnQueryStep<T>(query) : DataEntry.GetInstance<T>(integer));
//	}

//	internal static DataPoint[] GetReportForAll()
//	{
//		return DatabaseManager.GetActivitiesReport(DatabaseManager.ActivitiesReportQuery.All);
//	}

//	internal static DataPoint[] GetReportForChallenge()
//	{
//		return DatabaseManager.GetActivitiesReport(DatabaseManager.ActivitiesReportQuery.Challenges);
//	}

//	internal static DataPoint[] GetReportForPractice()
//	{
//		return DatabaseManager.GetActivitiesReport(DatabaseManager.ActivitiesReportQuery.Practice);
//	}

//	private static string GetSqliteType(Type type)
//	{
//		if (type == typeof(int) || type == typeof(short) || type == typeof(long))
//		{
//			return "INTEGER";
//		}
//		if (type == typeof(bool))
//		{
//			return "BOOLEAN";
//		}
//		if (type == typeof(string) || type == typeof(DateTime))
//		{
//			return "TEXT";
//		}
//		if (type == typeof(float) || type == typeof(double))
//		{
//			return "REAL";
//		}
//		if (!type.IsSubclassOf(typeof(DataEntry)))
//		{
//			throw new NotSupportedException(string.Format("Type {0} is not supported in DB", type));
//		}
//		return "INTEGER";
//	}

//	private static string GetStringFromDb(SQLiteQuery queryObj, string stringToGet)
//	{
//		return DatabaseManager.VerifyDataFromDb(queryObj.GetString(stringToGet));
//	}

//	private static Activity[] GetSubactivitiesForParentActivity(int parentActivityId)
//	{
//		if (!LoginManager.IsUserLoggedIn)
//		{
//			return new Activity[0];
//		}
//		return DatabaseManager.TryExecuteQuery<Activity>(string.Format("SELECT * FROM Activity WHERE customerId = \"{0}\" and groupId = {1}", LoginManager.UserData.Id, parentActivityId), (SQLiteQuery q) =>
//		{
//			int integer = q.GetInteger("id");
//			return (!DataEntry.InstanceExists<Activity>(integer) ? DatabaseManager.CreateInstanceOnQueryStep<Activity>(q) : DataEntry.GetInstance<Activity>(integer));
//		});
//	}

//	public static PracticeGreensPerformanceData[] GetTargetGreenPerformanceForParentActivity(int parentActivityId)
//	{
//		List<Shot> list = ((IEnumerable<Activity>)DatabaseManager.GetSubactivitiesForParentActivity(parentActivityId)).SelectMany<Activity, Shot>(new Func<Activity, IEnumerable<Shot>>(DatabaseManager.GetActivityShots)).ToList<Shot>();
//		List<PracticeGreensPerformanceData> practiceGreensPerformanceDatas = new List<PracticeGreensPerformanceData>();
//		foreach (List<Shot> shots in PracticeGreensController.SplitShotsByTargetField(list))
//		{
//			if (shots != null)
//			{
//				foreach (KeyValuePair<string, List<Shot>> keyValuePair in PracticeGreensController.SplitShotsByClubName(shots))
//				{
//					foreach (KeyValuePair<int, List<Shot>> keyValuePair1 in PracticeGreensController.SplitShotsByDistance(keyValuePair.Value))
//					{
//						practiceGreensPerformanceDatas.Add(PracticeGreensController.GetPerfomance(keyValuePair1.Value));
//					}
//				}
//			}
//		}
//		foreach (PracticeGreensPerformanceData practiceGreensPerformanceData in practiceGreensPerformanceDatas)
//		{
//			Debug.Log(practiceGreensPerformanceData);
//		}
//		return practiceGreensPerformanceDatas.ToArray();
//	}

//	public static void GetTargetGreenPerformanceForParentActivityAsync(int parentActivityId, Action<PracticeGreensPerformanceData[]> callback, Action<Exception> onError)
//	{
//		if (callback == null)
//		{
//			throw new ArgumentNullException("callback");
//		}
//		ThreadingUtilities.Run<PracticeGreensPerformanceData[]>(() => DatabaseManager.GetTargetGreenPerformanceForParentActivity(parentActivityId), (PracticeGreensPerformanceData[] d) => callback(d), onError);
//	}

//	public static List<ScoreListObject> GetTopScores(Activity.ChallengesSubType type, int numberHighScores, DatabaseManager.QueryAggregateFunc selectMethod, bool descending)
//	{
//		if (!LoginManager.IsUserLoggedIn)
//		{
//			return new List<ScoreListObject>();
//		}
//		ChallengeTopScore[] challengeTopScores = DatabaseManager.GetChallengeTopScores(type, numberHighScores, selectMethod, !descending);
//		List<ScoreListObject> scoreListObjects = new List<ScoreListObject>();
//		ChallengeTopScore[] challengeTopScoreArray = challengeTopScores;
//		for (int i = 0; i < (int)challengeTopScoreArray.Length; i++)
//		{
//			ChallengeTopScore challengeTopScore = challengeTopScoreArray[i];
//			ScoreListObject scoreListObject = new ScoreListObject()
//			{
//				date = DateTime.Parse(challengeTopScore.Date),
//				highScore = challengeTopScore.Score,
//				distanceToPin = challengeTopScore.DistanceToPin
//			};
//			scoreListObjects.Add(scoreListObject);
//		}
//		return scoreListObjects;
//	}

//	public static void GetTopScoresAsync(Activity.ChallengesSubType type, int numberHighScores, DatabaseManager.QueryAggregateFunc selectMethod, bool descending, Action<List<ScoreListObject>> callback, Action<Exception> onError)
//	{
//		if (callback == null)
//		{
//			throw new ArgumentNullException("callback");
//		}
//		ThreadingUtilities.Run<List<ScoreListObject>>(() => DatabaseManager.GetTopScores(type, numberHighScores, selectMethod, descending), (List<ScoreListObject> s) => callback(s), onError);
//	}

//	public static string GetUserDexterity(int customerId)
//	{
//		string[] strArrays = DatabaseManager.TryExecuteQuery<string>(string.Format("SELECT dexterity FROM User WHERE id={0}", customerId), (SQLiteQuery q) => q.GetString("dexterity"));
//		return ((int)strArrays.Length <= 0 ? string.Empty : strArrays[0]);
//	}

//	private static string GetValueFromSqliteQuery(SQLiteQuery query, string field, Type type)
//	{
//		if (type == typeof(int) || type == typeof(short) || type == typeof(long))
//		{
//			return query.GetInteger(field).ToString();
//		}
//		if (type == typeof(bool))
//		{
//			return query.GetInteger(field).ToString();
//		}
//		if (type == typeof(string) || type == typeof(DateTime))
//		{
//			return query.GetString(field);
//		}
//		if (type == typeof(float) || type == typeof(double))
//		{
//			return query.GetDouble(field).ToString(CultureInfo.InvariantCulture);
//		}
//		if (!type.IsSubclassOf(typeof(DataEntry)))
//		{
//			throw new NotSupportedException(string.Format("Type {0} is not supported in DB", type));
//		}
//		return query.GetInteger(field).ToString();
//	}

//	private static string HistoryQueryTypeToSqlCondition(DatabaseManager.QueryType queryType)
//	{
//		switch (queryType)
//		{
//			case DatabaseManager.QueryType.Practices:
//				{
//					return DatabaseManager.GameRulesToSqlCondition(new CGameManager.GAME_RULES_TYPE[] { CGameManager.GAME_RULES_TYPE.Practice });
//				}
//			case DatabaseManager.QueryType.Challenges:
//				{
//					break;
//					//return DatabaseManager.GameRulesToSqlCondition(new CGameManager.GAME_RULES_TYPE[] { typeof(< PrivateImplementationDetails >).GetField("$field-9D95B31B2F75E5E789075D5528E36C6F8F058969").FieldHandle });
//				}
//			case DatabaseManager.QueryType.GameImprovements:
//				{
//					return DatabaseManager.GameRulesToSqlCondition(new CGameManager.GAME_RULES_TYPE[] { CGameManager.GAME_RULES_TYPE.SkillsAssessment, CGameManager.GAME_RULES_TYPE.BagMapping });
//				}
//		}
//		throw new NotSupportedException(string.Format("History query type {0} is not supported", queryType));
//	}

//	public static void Init(Action<string> OnStatusChanged, Action onFinish)
//	{
//		bool flag = File.Exists(DatabaseManager.DatabasePath);
//		if (flag && !DatabaseManager.IsDatabaseNew())
//		{
//			File.Delete(DatabaseManager.DatabasePath);
//			flag = false;
//		}
//		if (!flag)
//		{
//			Debug.Log("Create new DB");
//			if (OnStatusChanged != null)
//			{
//				OnStatusChanged("Building Local Database");
//			}
//			CoroutinesTool coroutinesTool = (new GameObject()).AddComponent<CoroutinesTool>();
//			coroutinesTool.StartCoroutine(DatabaseManager.CreateDatabase(() =>
//			{
//				if (onFinish != null)
//				{
//					onFinish();
//				}
//				Object.Destroy(coroutinesTool.gameObject, 1f);
//			}));
//		}
//		else if (onFinish != null)
//		{
//			onFinish();
//		}
//	}

//	internal static void Insert<T>(T entry)
//	where T : DataEntry
//	{
//		entry.WillBeSavedToDatabase();
//		string str = (
//			from kvp in ReflectionUtilities.GetDataColumnProperties<T>()
//			select string.Format("`{0}`", kvp.Value.Name)).Aggregate<string>((string s1, string s2) => string.Concat(s1, ", ", s2));
//		string str1 = (
//			from  in ReflectionUtilities.GetDataColumnProperties<T>()


//			select string.Format("'{0}'", entry.GetValue<T>(kvp.Key))).Aggregate<string>((string s1, string s2) => string.Concat(s1, ", ", s2));
//		if (ReflectionUtilities.IsPrimaryKeyAutoincrement<T>())
//		{
//			T t = entry;
//			PropertyInfo primaryKeyInfo = ReflectionUtilities.GetPrimaryKeyInfo<T>();
//			int availableId = DatabaseManager.GetAvailableId<T>();
//			t.SetValue<T>(primaryKeyInfo, availableId.ToString());
//		}
//		string str2 = string.Format("INSERT INTO \"{0}\" ({1}) VALUES ({2})", ReflectionUtilities.GetTableName<T>(), str, str1);
//		DatabaseManager.TryExecuteQuery(str2, null);
//	}

//	internal static void InsertOrUpdate<T>(T entry)
//	where T : DataEntry
//	{
//		if (!DatabaseManager.Exists<T>(entry.Id))
//		{
//			DatabaseManager.Insert<T>(entry);
//		}
//		else
//		{
//			DatabaseManager.Update<T>(entry);
//		}
//	}

//	private static bool IsDatabaseNew()
//	{
//		bool[] flagArray = DatabaseManager.TryExecuteQuery<bool>("SELECT VersionId FROM Version", (SQLiteQuery q) => string.Equals(DatabaseManager.GetStringFromDb(q, "VersionId"), "1.7.12"));
//		if (flagArray == null || (int)flagArray.Length <= 0)
//		{
//			return false;
//		}
//		return flagArray[0];
//	}

//	public static void MarkSessionDeleted(DatabaseManager.QueryType type, string year, string monthday)
//	{
//		if (!LoginManager.IsUserLoggedIn)
//		{
//			return;
//		}
//		DatabaseManager.TryExecuteQuery(string.Format("UPDATE Activity SET isDeleted = 1 WHERE customerId = \"{0}\" AND substr(startTime,6,5) = \"{1}\" AND substr(startTime,0,5) = \"{2}\" AND ({3})", new object[] { LoginManager.UserData.Id, monthday.Trim(), year, DatabaseManager.HistoryQueryTypeToSqlCondition(type) }), null);
//	}

//	private static void ProcessHistoryActivityDataQuery(SQLiteQuery query, IDictionary<int, HistoryActivityData> activities)
//	{
//		HistoryActivityData item;
//		bool flag;
//		int integer = query.GetInteger("activityId");
//		int num = query.GetInteger("groupId");
//		int integer1 = query.GetInteger("activityTypeId");
//		int num1 = query.GetInteger("activitySubTypeId");
//		bool flag1 = num != 0;
//		if (flag1 || activities.ContainsKey(integer))
//		{
//			flag = (!flag1 ? false : !activities.ContainsKey(num));
//		}
//		else
//		{
//			flag = true;
//		}
//		bool flag2 = flag;
//		DateTime dateTime = query.GetString("activityStartTime").ParseDateTime();
//		DateTime dateTime1 = query.GetString("activityEndTime").ParseDateTime();
//		int num2 = (!flag1 ? integer : num);
//		if (!flag2)
//		{
//			item = activities[num2];
//			if (!flag1)
//			{
//				item.SetTime(dateTime, dateTime1);
//			}
//		}
//		else
//		{
//			item = new HistoryActivityData(num2);
//			item.SetTime(dateTime, dateTime1);
//			activities.Add(num2, item);
//		}
//		string str = DatabaseManager.VerifyDataFromDb(query.GetString("clubName"));
//		if (string.IsNullOrEmpty(str))
//		{
//			str = DatabaseManager.VerifyDataFromDb(query.GetString("clubType"));
//		}
//		if (!string.IsNullOrEmpty(str))
//		{
//			item.AddClub(str);
//		}
//		if (!flag1)
//		{
//			item.SetActivityType(Activity.GetGameRulesForTypeId(integer1, num1));
//		}
//	}

//	public static bool QueryExistsSubActivity(int parentEsnActivityId)
//	{
//		if (!LoginManager.IsUserLoggedIn)
//		{
//			return false;
//		}
//		Dictionary<string, object> strs = new Dictionary<string, object>()
//		{
//			{ "customerId", LoginManager.UserData.Id },
//			{ "groupId", parentEsnActivityId }
//		};
//		return DatabaseManager.Exists<Activity>(strs);
//	}

//	public static bool SearchForUserByEsnUserId(string esnUserId)
//	{
//		return DatabaseManager.TryExecuteQuery<int>(string.Format("SELECT count(*) AS `count` FROM User WHERE esnUserId = {0}", esnUserId), (SQLiteQuery q) => q.GetInteger("count"))[0] > 0;
//	}

//	public static UserLoginData SearchForUserBySkyGolfUserName(string skyGolfUserName)
//	{
//		KeyValuePair<UserLoginData, bool>[] keyValuePairArray = DatabaseManager.TryExecuteQuery<KeyValuePair<UserLoginData, bool>>("SELECT id, userName, email FROM User", (SQLiteQuery q) =>
//		{
//			bool flag;
//			int integer = q.GetInteger("id");
//			string str = q.GetString("userName");
//			string str1 = q.GetString("email");
//			flag = (string.Equals(str, skyGolfUserName, StringComparison.InvariantCultureIgnoreCase) ? true : string.Equals(str1, skyGolfUserName, StringComparison.InvariantCultureIgnoreCase));
//			return new KeyValuePair<UserLoginData, bool>(new UserLoginData()
//			{
//				SkyGolgUserName = str,
//				Email = str1,
//				CustomerId = integer
//			}, flag);
//		});
//		if (!((IEnumerable<KeyValuePair<UserLoginData, bool>>)keyValuePairArray).Any<KeyValuePair<UserLoginData, bool>>((KeyValuePair<UserLoginData, bool> kvp) => kvp.Value))
//		{
//			return null;
//		}
//		KeyValuePair<UserLoginData, bool> keyValuePair = ((IEnumerable<KeyValuePair<UserLoginData, bool>>)keyValuePairArray).First<KeyValuePair<UserLoginData, bool>>((KeyValuePair<UserLoginData, bool> kvp) => kvp.Value);
//		return keyValuePair.Key;
//	}

//	internal static T Select<T>(int id)
//	where T : DataEntry
//	{
//		T[] tArray = DatabaseManager.SelectWhere<T>("id", id.ToString(), -1);
//		return ((int)tArray.Length != 0 ? tArray[0] : (T)null);
//	}

//	internal static object Select(Type type, int id)
//	{
//		if (!type.IsSubclassOf(typeof(DataEntry)))
//		{
//			throw new ArgumentException(string.Format("Type {0} is not subclass of {1}", type.Name, typeof(DataEntry).Name));
//		}
//		object[] objArray = DatabaseManager.SelectWhere(type, "id", id.ToString(), -1);
//		return ((int)objArray.Length != 0 ? objArray[0] : null);
//	}

//	internal static T[] SelectAll<T>(int limit = -1)
//	where T : DataEntry
//	{
//		return DatabaseManager.TryExecuteQuery<T>(string.Concat(string.Format("SELECT * FROM \"{0}\"", ReflectionUtilities.GetTableName<T>()), (limit <= -1 ? string.Empty : string.Concat(" LIMIT ", limit))), (SQLiteQuery q) =>
//		{
//			int integer = q.GetInteger("id");
//			if (!DataEntry.InstanceExists<T>(integer))
//			{
//				return DatabaseManager.CreateInstanceOnQueryStep<T>(q);
//			}
//			return DataEntry.GetInstance<T>(integer);
//		});
//	}

//	private static Shot[] SelectDirtyShots(int limit)
//	{
//		return DatabaseManager.TryExecuteQuery<Shot>(string.Format("\r\n            SELECT *\r\n            FROM Shot\r\n            WHERE isReadyToBeSynchronized = \"1\" AND (isDirty = \"1\" OR isSynchronized = \"0\")\r\n            LIMIT {0}", limit), new Func<SQLiteQuery, Shot>(DatabaseManager.GetInstanceIfExistsOrCreateOnQueryStep<Shot>));
//	}

//	private static Activity[] SelectReferencedDirtyActivities(int limit)
//	{
//		return DatabaseManager.TryExecuteQuery<Activity>(string.Format("\r\n            SELECT *\r\n            FROM Activity\r\n            WHERE\r\n                isReadyToBeSynchronized = \"1\" AND (isDirty = \"1\" OR isSynchronized = \"0\") AND\r\n                id IN (SELECT DISTINCT activityId AS aId FROM Shot\r\n                       UNION\r\n                       SELECT DISTINCT groupId AS aId FROM Activity WHERE id IN (SELECT DISTINCT activityId FROM Shot))\r\n            LIMIT {0}", limit), new Func<SQLiteQuery, Activity>(DatabaseManager.GetInstanceIfExistsOrCreateOnQueryStep<Activity>));
//	}

//	private static Session[] SelectReferencedDirtySessions(int limit)
//	{
//		return DatabaseManager.TryExecuteQuery<Session>(string.Format("\r\n            SELECT *\r\n            FROM Session\r\n            WHERE \r\n                isReadyToBeSynchronized = \"1\" AND (isDirty = \"1\" OR isSynchronized = \"0\") AND\r\n                id IN (SELECT DISTINCT sessionId FROM Activity)\r\n            LIMIT {0}", limit), new Func<SQLiteQuery, Session>(DatabaseManager.GetInstanceIfExistsOrCreateOnQueryStep<Session>));
//	}

//	internal static T[] SelectWhere<T>(string column, string value, int limit = -1)
//	where T : DataEntry
//	{
//		return DatabaseManager.SelectWhere(typeof(T), column, value, limit).Cast<T>().ToArray<T>();
//	}

//	internal static object[] SelectWhere(Type type, string column, string value, int limit = -1)
//	{
//		if (!type.IsSubclassOf(typeof(DataEntry)))
//		{
//			throw new ArgumentException(string.Format("Type {0} is not subclass of {1}", type.Name, typeof(DataEntry).Name));
//		}
//		return DatabaseManager.TryExecuteQuery<object>(string.Concat(string.Format("SELECT * FROM \"{0}\" WHERE `{1}` = \"{2}\"", ReflectionUtilities.GetTableName(type), column, value), (limit <= -1 ? string.Empty : string.Concat(" LIMIT ", limit))), (SQLiteQuery q) =>
//		{
//			int integer = q.GetInteger("id");
//			if (!DataEntry.InstanceExists(type, integer))
//			{
//				return DatabaseManager.CreateInstanceOnQueryStep(type, q);
//			}
//			return DataEntry.GetInstance(type, integer);
//		});
//	}

//	private static void TryExecuteQuery(string query, Action<SQLiteQuery> step = null)
//	{
//		DatabaseManager.TryExecuteQuery<object>(query, (SQLiteQuery q) =>
//		{
//			if (step != null)
//			{
//				step(q);
//			}
//			return new object();
//		});
//	}

//	private static T[] TryExecuteQuery<T>(string query, Func<SQLiteQuery, T> step)
//	{
//		T[] tArray;
//		SQLiteDB sQLiteDB = null;
//		SQLiteQuery sQLiteQuery = null;
//		List<T> ts = new List<T>();
//		object databaseLock = DatabaseManager.DatabaseLock;
//		Monitor.Enter(databaseLock);
//		try
//		{
//			try
//			{
//				try
//				{
//					sQLiteDB = new SQLiteDB();
//					sQLiteDB.Open(DatabaseManager.DatabasePath);
//					sQLiteQuery = new SQLiteQuery(sQLiteDB, query);
//					while (sQLiteQuery.Step())
//					{
//						if (step == null)
//						{
//							continue;
//						}
//						ts.Add(step(sQLiteQuery));
//					}
//				}
//				catch (Exception exception1)
//				{
//					Exception exception = exception1;
//					AppLog.LogError(string.Format("Unhandled exception occured while executing query:\n{0}\nException: {1}\nStack trace:\n{2}", query, exception, exception.StackTrace), true);
//					tArray = null;
//					return tArray;
//				}
//			}
//			finally
//			{
//				if (sQLiteQuery != null)
//				{
//					sQLiteQuery.Release();
//				}
//				if (sQLiteDB != null)
//				{
//					sQLiteDB.Close();
//				}
//			}
//			return ts.ToArray();
//		}
//		finally
//		{
//			Monitor.Exit(databaseLock);
//		}
//		return tArray;
//	}

//	internal static void Update<T>(T entry)
//	where T : DataEntry
//	{
//		entry.WillBeSavedToDatabase();
//		int primaryKey = entry.GetPrimaryKey<T>();
//		string str = (
//			from kvp in ReflectionUtilities.GetDataColumnProperties<T>()
//			where !kvp.Value.IsPrimaryKey
//			select kvp into



//			select string.Format("`{0}` = '{1}'", kvp.Value.Name, entry.GetValue<T>(kvp.Key))).Aggregate<string>((string s1, string s2) => string.Concat(s1, ", ", s2));
//		DatabaseManager.TryExecuteQuery(string.Format("UPDATE \"{0}\" SET {1} WHERE `{2}` = \"{3}\"", new object[] { ReflectionUtilities.GetTableName<T>(), str, ReflectionUtilities.GetPrimaryKeyInfo<T>().GetCustomAttribute<DataColumnAttribute>(true).Name, primaryKey }), null);
//	}

//	private static string VerifyDataFromDb(string data)
//	{
//		return (string.IsNullOrEmpty(data) || data.Contains("NO_DATA") ? string.Empty : data);
//	}

//	private static string VerifyDataToDb(string data)
//	{
//		return (!string.IsNullOrEmpty(data) ? data : "NO_DATA");
//	}

//	private enum ActivitiesReportQuery
//	{
//		Practice,
//		Challenges,
//		All
//	}

//	public enum QueryAggregateFunc
//	{
//		MIN,
//		MAX,
//		SUM
//	}

//	public enum QueryType
//	{
//		Practices,
//		Challenges,
//		GameImprovements
//	}
//}