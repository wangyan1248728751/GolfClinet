//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using Utilities;

//namespace Data
//{
//	internal static class SynchronizationManager
//	{
//		private const bool PullOnlyOnce = true;

//		private const bool PullUntilSuccess = true;

//		private const int DataEntriesAmountPerPush = 10;

//		private const int SynchronizationTimerPeriod = 10;

//		private const int MaxSessionsToBePulledPerPull = 100;

//		private static volatile SynchronizationState _state;

//		private static bool _isReset;

//		private static volatile bool _isSyncInProgress;

//		private static volatile bool _isPulledOnce;

//		private static string Esn
//		{
//			get
//			{
//				string eSN = ApplicationDataManager.instance.ESN;
//				if (string.IsNullOrEmpty(eSN))
//				{
//					eSN = ApplicationDataManager.instance.PreviousESN;
//				}
//				return eSN;
//			}
//		}

//		public static SynchronizationState State
//		{
//			get
//			{
//				return (SynchronizationState)SynchronizationManager._state;
//			}
//			private set
//			{
//				SynchronizationManager._state = value;
//			}
//		}

//		static SynchronizationManager()
//		{
//		}

//		private static void GetSessionsListCallback(WebServiceResponse response, Action onFinish, Action<string> onError)
//		{
//			if (!response.Success)
//			{
//				onError(string.Format("Error occured while pulling the sessions list. Code: {0}. Message: {1}", response.Code, response.Message));
//				return;
//			}
//			Session[] array = DataEntry.DeserializeFromColumnsAndDataJson<Session>(response.Data["data"]).ToArray<Session>();
//			List<Session> sessions = new List<Session>();
//			Session[] sessionArray = array;
//			int num = 0;
//			while (num < (int)sessionArray.Length)
//			{
//				Session session = sessionArray[num];
//				Session session1 = DatabaseManager.SelectWhere<Session>("uuid", session.Uuid, -1).FirstOrDefault<Session>();
//				if (session1 == null)
//				{
//					session.IsPulled = false;
//					SynchronizationManager.MarkPulled<Session>(session, true);
//					sessions.Add(session);
//				}
//				else if (!session1.IsPulled)
//				{
//					sessions.Add(session1);
//				}
//				if (sessions.Count < 100)
//				{
//					num++;
//				}
//				else
//				{
//					break;
//				}
//			}
//			if (sessions.Count <= 0)
//			{
//				SynchronizationManager._isPulledOnce = true;
//				onFinish();
//			}
//			else
//			{
//				Session[] array1 = (
//					from s in sessions
//					where !s.IsPulled
//					select s).ToArray<Session>();
//				SynchronizationManager.State = SynchronizationState.Pulling;
//				SynchronizationManager.PullSessions(array1, onFinish, onError);
//			}
//		}

//		private static void MarkPulled<T>(T entry, bool updateDatabase)
//		where T : DataEntry
//		{
//			entry.IsDirty = false;
//			entry.IsSynchronized = true;
//			entry.IsOnServer = true;
//			entry.IsReadyToBeSynchronized = true;
//			if (updateDatabase)
//			{
//				DatabaseManager.InsertOrUpdate<T>(entry);
//			}
//		}

//		private static void MarkPushed<T>(T entry)
//		where T : DataEntry
//		{
//			entry.IsSynchronized = true;
//			entry.IsOnServer = true;
//			DatabaseManager.Update<T>(entry);
//		}

//		private static void MarkPushStarted<T>(T entry)
//		where T : DataEntry
//		{
//			entry.IsDirty = false;
//			entry.IsSynchronized = false;
//			DatabaseManager.Update<T>(entry);
//		}

//		private static void OnPullFinish()
//		{
//			AppLog.Log("Pull-synchronization is finished", true);
//			SynchronizationManager.State = SynchronizationState.Done;
//			SynchronizationManager._isSyncInProgress = false;
//		}

//		private static void OnPushFinish()
//		{
//			AppLog.Log("Push-synchronization is finished", true);
//			if (!SynchronizationManager._isPulledOnce)
//			{
//				SynchronizationManager.State = SynchronizationState.InProgress;
//			}
//			else
//			{
//				SynchronizationManager.State = SynchronizationState.Done;
//			}
//			SynchronizationManager._isSyncInProgress = false;
//		}

//		private static void OnSynchronizationError(string message)
//		{
//			AppLog.LogError(string.Format("Error occured during the synchronization: {0}", message), true);
//			SynchronizationManager.State = SynchronizationState.Error;
//			SynchronizationManager._isSyncInProgress = false;
//		}

//		private static T[] ParseEntries<T>(JsonTextReader reader)
//		where T : DataEntry
//		{
//			List<T> ts = new List<T>();
//			int num2 = 0;
//			string tableName = ReflectionUtilities.GetTableName<T>();
//			DataEntry.DeserializeFromColumnsAndDataJsonAsync<T>(reader, (T e) => {
//				ts.Add(e);
//				int num = num2 + 1;
//				int num1 = num;
//				num2 = num;
//				if (num1 % 10 == 0)
//				{
//					AppLog.Log(string.Format("{0} {1} is parsed", tableName, num2), true);
//				}
//			}, null);
//			return ts.ToArray();
//		}

//		private static void Pull(Action onFinish, Action<string> onError)
//		{
//			JSONObject jSONObject = new JSONObject();
//			jSONObject.AddField("customerId", LoginManager.UserData.Id.ToString());
//			DateTime dateTime = new DateTime(2012, 1, 1);
//			jSONObject.AddField("createdAfter", dateTime.ToString("yyyy-MM-dd HH':'mm':'ss"));
//			jSONObject.AddField("createdBefore", DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss"));
//			JSONObject jSONObject1 = new JSONObject();
//			jSONObject1.AddField("data", jSONObject);
//			Dictionary<string, string> strs = new Dictionary<string, string>()
//			{
//				{ "session", LoginManager.Token },
//				{ "dev", "E4F68-874E7-C80E5-1E8C3" },
//				{ "data", jSONObject1.ToString() }
//			};
//			WebService.CallWebService(ServerRequest.GetRemoteSessionData, strs, (WebServiceResponse r) => SynchronizationManager.GetSessionsListCallback(r, onFinish, onError), true);
//		}

//		private static void PullSessionCallback(WebServiceResponse response, Session[] sessions, Action onFinish, Action<string> onError)
//		{
//			if (!response.Success)
//			{
//				onError(string.Format("Error occured while pulling sessions. Code: {0}. Message: {1}", response.Code, response.Message));
//				return;
//			}
//			Session[] sessionArray = null;
//			Activity[] activityArray = null;
//			Shot[] shotArray = null;
//			try
//			{
//				byte[] bytes = Encoding.UTF8.GetBytes(response.RawResponse);
//				using (MemoryStream memoryStream = new MemoryStream(bytes))
//				{
//					using (StreamReader streamReader = new StreamReader(memoryStream))
//					{
//						using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
//						{
//							while (jsonTextReader.Read())
//							{
//								if (jsonTextReader.TokenType == JsonToken.PropertyName)
//								{
//									if ((jsonTextReader.Value as string ?? string.Empty).ToLowerInvariant() == "session")
//									{
//										sessionArray = SynchronizationManager.ParseEntries<Session>(jsonTextReader);
//										AppLog.Log(string.Format("{0} sessions are pulled", (int)sessionArray.Length), true);
//										continue;
//									}
//								}
//								if (jsonTextReader.TokenType == JsonToken.PropertyName)
//								{
//									if ((jsonTextReader.Value as string ?? string.Empty).ToLowerInvariant() == "activities")
//									{
//										activityArray = SynchronizationManager.ParseEntries<Activity>(jsonTextReader);
//										AppLog.Log(string.Format("{0} activities are pulled", (int)activityArray.Length), true);
//										continue;
//									}
//								}
//								if (jsonTextReader.TokenType != JsonToken.PropertyName)
//								{
//									continue;
//								}
//								if ((jsonTextReader.Value as string ?? string.Empty).ToLowerInvariant() != "shots")
//								{
//									continue;
//								}
//								shotArray = SynchronizationManager.ParseEntries<Shot>(jsonTextReader);
//								AppLog.Log(string.Format("{0} shots are pulled", (int)shotArray.Length), true);
//							}
//						}
//					}
//				}
//				if (sessionArray == null)
//				{
//					onError(string.Format("Session JSON not found in response to get sessions", new object[0]));
//				}
//				else if (activityArray == null)
//				{
//					onError(string.Format("Activities JSON not found in response to get sessions", new object[0]));
//				}
//				else if (shotArray != null)
//				{
//					AppLog.Log("Saving shots to DB...", true);
//					((IEnumerable<Shot>)shotArray).ForEach<Shot>((Shot sh) => SynchronizationManager.MarkPulled<Shot>(sh, true));
//					AppLog.Log("Saving activities to DB...", true);
//					Activity[] activityArray1 = activityArray;
//					for (int i = 0; i < (int)activityArray1.Length; i++)
//					{
//						Activity user = activityArray1[i];
//						if (user.Session != null)
//						{
//							SynchronizationManager.MarkPulled<Activity>(user, false);
//							if (user.Parent != null)
//							{
//								user.Parent.User = user.User;
//							}
//						}
//					}
//					Activity[] activityArray2 = activityArray;
//					for (int j = 0; j < (int)activityArray2.Length; j++)
//					{
//						DatabaseManager.InsertOrUpdate<Activity>(activityArray2[j]);
//					}
//					AppLog.Log(string.Format("Saving sessions to DB...", new object[0]), true);
//					Session[] sessionArray1 = sessionArray;
//					for (int k = 0; k < (int)sessionArray1.Length; k++)
//					{
//						Session session = sessionArray1[k];
//						Session session1 = sessions.FirstOrDefault<Session>((Session s) => s.Uuid.Equals(session.Uuid, StringComparison.InvariantCultureIgnoreCase));
//						if (session1 != null)
//						{
//							session.CopyTo(session1);
//							session1.IsPulled = true;
//							DatabaseManager.Update<Session>(session1);
//						}
//					}
//					onFinish();
//				}
//				else
//				{
//					onError(string.Format("Shots JSON not found in response to get sessions", new object[0]));
//				}
//			}
//			catch (Exception exception1)
//			{
//				Exception exception = exception1;
//				AppLog.LogException(exception, true);
//				onError(exception.Message);
//			}
//		}

//		private static void PullSessions(Session[] sessions, Action onFinish, Action<string> onError)
//		{
//			if ((int)sessions.Length == 0)
//			{
//				onFinish();
//				return;
//			}
//			AppLog.Log(string.Format("Pulling {0} sessions...", (int)sessions.Length), true);
//			JSONObject jSONObject = new JSONObject();
//			jSONObject.AddField("ESN", SynchronizationManager.Esn);
//			jSONObject.AddField("device_SessionUuid", (
//				from s in (IEnumerable<Session>)sessions
//				select s.Uuid).Aggregate<string>((string s1, string s2) => string.Concat(s1, ",", s2)));
//			JSONObject jSONObject1 = new JSONObject();
//			jSONObject1.AddField("data", jSONObject);
//			Dictionary<string, string> strs = new Dictionary<string, string>()
//			{
//				{ "session", LoginManager.Token },
//				{ "dev", "E4F68-874E7-C80E5-1E8C3" },
//				{ "data", jSONObject1.ToString() }
//			};
//			WebService.CallWebService(ServerRequest.GetRemoteSessionObj, strs, (WebServiceResponse r) => SynchronizationManager.PullSessionCallback(r, sessions, onFinish, onError), true);
//		}

//		private static void Push(DataEntry[] dirtyData, Action onFinish, Action<string> onError)
//		{
//			if (onFinish == null)
//			{
//				throw new ArgumentNullException("onFinish");
//			}
//			if (onError == null)
//			{
//				throw new ArgumentNullException("onError");
//			}
//			SynchronizationManager.State = SynchronizationState.Pushing;
//			int num2 = 1 + ((IEnumerable<DataEntry>)dirtyData).Count<DataEntry>((DataEntry e) => e is Activity) + ((IEnumerable<DataEntry>)dirtyData).Count<DataEntry>((DataEntry e) => e is Session);
//			bool flag1 = false;
//			Action action = () => {
//				int num = num2 - 1;
//				int num1 = num;
//				num2 = num;
//				if (num1 == 0 && !flag1)
//				{
//					onFinish();
//				}
//			};
//			Action<string> action1 = (string m) => {
//				bool flag = !flag1;
//				flag1 = true;
//				if (flag)
//				{
//					onError(m);
//				}
//			};
//			SynchronizationManager.PushShots((
//				from e in (IEnumerable<DataEntry>)dirtyData
//				where e is Shot
//				select e).Cast<Shot>().ToArray<Shot>(), action, action1);
//			foreach (Activity activity in (
//				from e in (IEnumerable<DataEntry>)dirtyData
//				where e is Activity
//				select e).Cast<Activity>())
//			{
//				SynchronizationManager.PushActivity(activity, action, action1);
//			}
//			foreach (Session session in (
//				from e in (IEnumerable<DataEntry>)dirtyData
//				where e is Session
//				select e).Cast<Session>())
//			{
//				SynchronizationManager.PushSession(session, action, action1);
//			}
//		}

//		private static void PushActivity(Activity activity, Action onFinish, Action<string> onError)
//		{
//			JSONObject jSONObject = new JSONObject();
//			jSONObject.AddField("data", DataEntry.SerializeToJson<Activity>(activity));
//			Dictionary<string, string> strs = new Dictionary<string, string>()
//			{
//				{ "session", LoginManager.Token },
//				{ "dev", "E4F68-874E7-C80E5-1E8C3" },
//				{ "data", jSONObject.ToString() }
//			};
//			WebService.CallWebService(ServerRequest.UpdateActivity, strs, (WebServiceResponse r) => SynchronizationManager.PushActivityCallback(r, activity, onFinish, onError), true);
//			SynchronizationManager.MarkPushStarted<Activity>(activity);
//		}

//		private static void PushActivityCallback(WebServiceResponse response, Activity activity, Action onFinish, Action<string> onError)
//		{
//			if (!response.Success)
//			{
//				if (response.Code != 3002)
//				{
//					onError(string.Format("Error occured while pushing activity {0}. Code: {1}. Message: {2}", activity.Id, response.Code, response.Message));
//					return;
//				}
//				activity.IsOnServer = true;
//				DatabaseManager.Update<Activity>(activity);
//				SynchronizationManager.PushActivity(activity, onFinish, onError);
//			}
//			SynchronizationManager.MarkPushed<Activity>(activity);
//			onFinish();
//		}

//		private static void PushOrPull()
//		{
//			if (string.IsNullOrEmpty(LoginManager.Token) || LoginManager.UserData == null)
//			{
//				return;
//			}
//			if (SynchronizationManager._isSyncInProgress)
//			{
//				return;
//			}
//			if (SynchronizationManager.State == SynchronizationState.Pulling || SynchronizationManager.State == SynchronizationState.Pushing)
//			{
//				return;
//			}
//			SynchronizationState state = SynchronizationManager.State;
//			if (string.IsNullOrEmpty(SynchronizationManager.Esn))
//			{
//				SynchronizationManager.State = SynchronizationState.Waiting;
//				return;
//			}
//			if (SynchronizationManager._isReset)
//			{
//				SynchronizationManager._isReset = false;
//				SynchronizationManager._isPulledOnce = false;
//			}
//			SynchronizationManager._isSyncInProgress = true;
//			DataEntry[] dirtyDataEntries = DatabaseManager.GetDirtyDataEntries(10);
//			if ((int)dirtyDataEntries.Length > 0)
//			{
//				if (!NetworkUtilities.IsInternetAvailable())
//				{
//					SynchronizationManager.State = SynchronizationState.NotPossible;
//					SynchronizationManager._isSyncInProgress = false;
//					return;
//				}
//				SynchronizationManager.UpdateCurrentSessionEndTime();
//				AppLog.Log("Push-synchronization is starting", true);
//				SynchronizationManager.Push(dirtyDataEntries, new Action(SynchronizationManager.OnPushFinish), new Action<string>(SynchronizationManager.OnSynchronizationError));
//			}
//			else if ((0 != 0 || !SynchronizationManager._isPulledOnce) && NetworkUtilities.IsInternetAvailable())
//			{
//				AppLog.Log("Pull-synchronization is starting", true);
//				SynchronizationManager.Pull(new Action(SynchronizationManager.OnPullFinish), new Action<string>(SynchronizationManager.OnSynchronizationError));
//			}
//			else
//			{
//				SynchronizationManager._isSyncInProgress = false;
//			}
//		}

//		private static void PushSession(Session session, Action onFinish, Action<string> onError)
//		{
//			JSONObject jSONObject = new JSONObject();
//			jSONObject.AddField("data", DataEntry.SerializeToJson<Session>(session));
//			Dictionary<string, string> strs = new Dictionary<string, string>()
//			{
//				{ "session", LoginManager.Token },
//				{ "dev", "E4F68-874E7-C80E5-1E8C3" },
//				{ "data", jSONObject.ToString() }
//			};
//			WebService.CallWebService(ServerRequest.UpdateSession, strs, (WebServiceResponse r) => SynchronizationManager.PushSessionCallback(r, session, onFinish, onError), true);
//			SynchronizationManager.MarkPushStarted<Session>(session);
//		}

//		private static void PushSessionCallback(WebServiceResponse response, Session session, Action onFinish, Action<string> onError)
//		{
//			if (!response.Success)
//			{
//				if (response.Code != 3002)
//				{
//					onError(string.Format("Error occured while pushing session {0} ({1}). Code: {2}. Message: {3}", new object[] { session.Id, session.Uuid, response.Code, response.Message }));
//					return;
//				}
//				session.IsOnServer = true;
//				DatabaseManager.Update<Session>(session);
//				SynchronizationManager.PushSession(session, onFinish, onError);
//			}
//			SynchronizationManager.MarkPushed<Session>(session);
//			onFinish();
//		}

//		private static void PushShots(Shot[] shots, Action onFinish, Action<string> onError)
//		{
//			if ((int)shots.Length == 0)
//			{
//				onFinish();
//				return;
//			}
//			JSONObject jSONObject = new JSONObject();
//			jSONObject.AddField("ESN", SynchronizationManager.Esn);
//			jSONObject.AddField("useVersion", "1");
//			jSONObject.AddField("data", DataEntry.SerializeToColumnsAndDataJson<Shot>(shots));
//			Dictionary<string, string> strs = new Dictionary<string, string>()
//			{
//				{ "session", LoginManager.Token },
//				{ "dev", "E4F68-874E7-C80E5-1E8C3" },
//				{ "useVersion", "0" },
//				{ "data", jSONObject.ToString() }
//			};
//			WebService.CallWebService(ServerRequest.AddShots, strs, (WebServiceResponse r) => SynchronizationManager.PushShotsCallback(r, shots, onFinish, onError), true);
//			((IEnumerable<Shot>)shots).ForEach<Shot>(new Action<Shot>(SynchronizationManager.MarkPushStarted<Shot>));
//		}

//		private static void PushShotsCallback(WebServiceResponse response, Shot[] shots, Action onFinish, Action<string> onError)
//		{
//			if (!response.Success)
//			{
//				onError(string.Format("Error occured while pushing shots. Code: {0}. Message: {1}", response.Code, response.Message));
//				return;
//			}
//			((IEnumerable<Shot>)shots).ForEach<Shot>(new Action<Shot>(SynchronizationManager.MarkPushed<Shot>));
//			onFinish();
//		}

//		public static void Reset()
//		{
//			SynchronizationManager._isReset = true;
//			SynchronizationManager._isSyncInProgress = false;
//			SynchronizationManager.State = SynchronizationState.Waiting;
//		}

//		public static void StartSynchronizationTimer()
//		{
//			Scheduler.instance.AddDelegate(new Scheduler.delegateMethod(SynchronizationManager.SynchronizationTimerCallback), 10);
//		}

//		private static void SynchronizationTimerCallback()
//		{
//			ThreadingUtilities.Run(new Action(SynchronizationManager.PushOrPull), null, (Exception ex) => AppLog.LogException(ex, true));
//		}

//		private static void UpdateCurrentSessionEndTime()
//		{
//			Activity activity = (
//				from a in (IEnumerable<Activity>)SessionAndActivityManager.Instance.CurrentSessionData.Activities
//				orderby a.EndTime descending
//				select a).FirstOrDefault<Activity>();
//			if (activity == null)
//			{
//				return;
//			}
//			if (SessionAndActivityManager.Instance.CurrentSessionData.EndTime == activity.EndTime)
//			{
//				return;
//			}
//			SessionAndActivityManager.Instance.CurrentSessionData.EndTime = activity.EndTime;
//			SessionAndActivityManager.Instance.CurrentSessionData.IsDirty = true;
//			DatabaseManager.Update<Session>(SessionAndActivityManager.Instance.CurrentSessionData);
//		}
//	}
//}