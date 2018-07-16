using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities;

public class SessionAndActivityManager
{
	private static SessionAndActivityManager _sessionManagerInstance;

	private readonly Stack<Activity> _activitiesStack = new Stack<Activity>();

	public Session CurrentSessionData
	{
		get;
		private set;
	}

	public static SessionAndActivityManager Instance
	{
		get
		{
			SessionAndActivityManager sessionAndActivityManager = SessionAndActivityManager._sessionManagerInstance;
			if (sessionAndActivityManager == null)
			{
				sessionAndActivityManager = new SessionAndActivityManager();
				SessionAndActivityManager._sessionManagerInstance = sessionAndActivityManager;
			}
			return sessionAndActivityManager;
		}
	}

	private SessionAndActivityManager()
	{
		//SynchronizationManager.StartSynchronizationTimer();
		//DatabaseManager.CleanUp();
		SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>((Scene s, LoadSceneMode m) =>
		{
			if (s.name == "MainNew")
			{
				//DatabaseManager.CleanUp();
			}
		});
	}

	public void AddShotToSession(Shot shot)
	{
		if (this.CurrentSessionData == null)
		{
			return;
		}
		if ((int)this.CurrentSessionData.Activities.Length == 0)
		{
			return;
		}
		AppLog.Log(string.Concat("Shot was complete. Adding shot to session; EsnShotID=", shot.Id), true);
		shot.Activity = this._activitiesStack.Peek();
		shot.Activity.IsDeleted = false;
		shot.Activity.IsDirty = true;
		//DatabaseManager.InsertOrUpdate<Activity>(shot.Activity);
		shot.NumberInActivity = (int)this._activitiesStack.Peek().Shots.Length;
		//DatabaseManager.Insert<Shot>(shot);
	}

	public void CloseAndDeleteCurrentActivities()
	{
		if (this._activitiesStack.Count == 0)
		{
			AppLog.LogError("Activities stack has no activities", true);
			return;
		}
		Activity activity1 = this._activitiesStack.Peek();
		Activity parent = activity1.Parent ?? activity1;
		((IEnumerable<Activity>)parent.Subactivities).ForEach<Activity>((Activity activity) =>
		{
			((IEnumerable<Shot>)activity.Shots).ForEach<Shot>((Shot shot) =>
			{
				shot.IsDeleted = true;
				shot.IsDirty = true;
				//DatabaseManager.Update<Shot>(shot);
			});
			activity.IsDeleted = true;
			activity.IsDirty = true;
			//DatabaseManager.Update<Activity>(activity);
		});
		parent.IsDeleted = true;
		parent.IsDirty = true;
		//DatabaseManager.Update<Activity>(parent);
		this._activitiesStack.Clear();
	}

	public void CloseSubActivity()
	{
		Activity now = this._activitiesStack.Peek();
		if (now.Parent == null)
		{
			return;
		}
		now.EndTime = DateTime.Now;
		now.IsDirty = true;
		//DatabaseManager.Update<Activity>(now);
		this._activitiesStack.Pop();
		AppLog.Log(string.Concat("Active activity now is: ", this._activitiesStack.Peek().Id), true);
	}

	public Activity CreateActivity()
	{
		if (this.CurrentSessionData == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(ApplicationDataManager.instance.ESN) || ApplicationDataManager.instance.ESN == "NOESN" || string.IsNullOrEmpty(ApplicationDataManager.instance.FWVersion) || CGameManager.instance == null || !LoginManager.IsUserLoggedIn)
		{
			return null;
		}
		this.CurrentSessionData.Esn = ApplicationDataManager.instance.ESN;
		//DatabaseManager.Update<Session>(this.CurrentSessionData);
		Activity instance = DataEntry.GetInstance<Activity>(ApplicationDataManager.instance.GenerateLocalEsnID());
		instance.Esn = ApplicationDataManager.instance.ESN;
		instance.User = LoginManager.UserData;
		instance.TypeId = Activity.GetTypeIdForGameRulesType(CGameManager.instance.currentGameType);
		instance.SubTypeId = Activity.GetSubTypeIdForGameRulesType(CGameManager.instance.currentGameType);
		instance.StartTime = DateTime.Now.ToLocalTime();
		instance.EndTime = DateTime.Now.ToLocalTime();
		instance.CourseId = null;
		instance.CourseConditionsId = null;
		instance.TeePositionId = null;
		instance.Session = this.CurrentSessionData;
		AppLog.Log(string.Concat("Create new activity Id =", instance.Id), true);
		this._activitiesStack.Push(instance);
		return instance;
	}

	public void CreateNewSession()
	{
		if (!LoginManager.IsUserLoggedIn)
		{
			this.CurrentSessionData = null;
			return;
		}
		DateTime now = DateTime.Now;
		//this.CurrentSessionData = DataEntry.GetInstance<Session>(DatabaseManager.GetAvailableId<Session>());
		this.CurrentSessionData.Uuid = Guid.NewGuid().ToString();
		this.CurrentSessionData.StartTime = now;
		this.CurrentSessionData.EndTime = now;
		this.CurrentSessionData.TimeStamp = SessionAndActivityManager.UnixTimestampFromDateTime(now).ToString();
		this.CurrentSessionData.IsReadyToBeSynchronized = true;
		//DatabaseManager.Insert<Session>(this.CurrentSessionData);
	}

	public Activity CreateSubActivity()
	{
		if (!LoginManager.IsUserLoggedIn)
		{
			return null;
		}
		Activity activity = this._activitiesStack.Peek();
		//DatabaseManager.InsertOrUpdate<Activity>(activity);
		Activity activity1 = this.CreateActivity();
		activity1.Parent = activity;
		return activity1;
	}

	public void DeleteShotFromSession(int esnShotId)
	{
		if (this.CurrentSessionData == null)
		{
			return;
		}
		Activity[] activities = this.CurrentSessionData.Activities;
		for (int i = 0; i < (int)activities.Length; i++)
		{
			Activity activity = activities[i];
			Shot shot = activity.Shots.FirstOrDefault<Shot>((Shot sh) => sh.Id == esnShotId);
			if (shot != null)
			{
				shot.IsDeleted = true;
				shot.IsDirty = true;
				//DatabaseManager.Update<Shot>(shot);
			}
			if (shot != null)
			{
				if (((IEnumerable<Shot>)activity.Shots).All<Shot>((Shot sh) => sh.IsDeleted))
				{
					activity.IsDeleted = true;
					activity.IsDirty = true;
					//DatabaseManager.Update<Activity>(activity);
				}
			}
		}
		this.OnShotWasDeleted();
	}

	public void FinishMainActivity()
	{
		if (!LoginManager.IsUserLoggedIn)
		{
			return;
		}
		if (this._activitiesStack.Count == 0)
		{
			AppLog.LogError("Activities stack has no activities", true);
			return;
		}
		if (this._activitiesStack.Count != 1)
		{
			throw new InvalidOperationException(string.Format("Activities stack contains 0 or more than 1 activities: [ {0} ]", (this._activitiesStack.Count != 0 ? (
				from a in this._activitiesStack
				select a.Id.ToString()).Aggregate<string>((string s1, string s2) => string.Concat(s1, ", ", s2)) : string.Empty)));
		}
		Activity now = this._activitiesStack.Pop();
		now.EndTime = DateTime.Now;
		now.IsDirty = true;
		//DatabaseManager.Update<Activity>(now);
	}

	public Shot GetLastShotFromCurrentActivity()
	{
		if (this.CurrentSessionData == null)
		{
			return null;
		}
		if ((int)this.CurrentSessionData.Activities.Length == 0)
		{
			return null;
		}
		Activity activity = this._activitiesStack.Peek();
		if ((int)activity.Shots.Length == 0)
		{
			return null;
		}
		return activity.Shots[(int)activity.Shots.Length - 1];
	}

	private static long UnixTimestampFromDateTime(DateTime date)
	{
		long ticks = date.Ticks;
		DateTime dateTime = new DateTime(1970, 1, 1);
		return (ticks - dateTime.Ticks) / (long)10000000;
	}

	public event Action OnShotWasDeleted;
}