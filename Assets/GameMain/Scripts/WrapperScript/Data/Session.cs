using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Data
{
	[DataTable("Session")]
	public class Session : DataEntry
	{
		private string _uuid;

		private readonly Dictionary<int, Activity> _activities = new Dictionary<int, Activity>();

		[Synchronizable("action", true, true)]
		public string Action
		{
			get
			{
				return (!base.IsOnServer ? "add" : "update");
			}
		}

		public Activity[] Activities
		{
			get
			{
				return this._activities.Values.ToArray<Activity>();
			}
		}

		[Synchronizable("sessionDurationSeconds", true, true)]
		public int Duration
		{
			get
			{
				return (int)(this.EndTime - this.StartTime).TotalSeconds;
			}
		}

		[DataColumn("endTime", false, false, null)]
		[Synchronizable("sessionEndTime", true, true)]
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

		[DataColumn("id", true, true, null)]
		public override int Id
		{
			get
			{
				return base.Id;
			}
		}

		[DataColumn("isPulled", false, false, null)]
		public bool IsPulled
		{
			get;
			set;
		}

		[DataColumn("startTime", false, false, null)]
		[Synchronizable("sessionStartTime", true, true)]
		public DateTime StartTime
		{
			get;
			set;
		}

		[DataColumn("timeStamp", false, false, null)]
		[Synchronizable("device_TimeStamp", true, true)]
		public string TimeStamp
		{
			get;
			set;
		}

		[DataColumn("uuid", false, false, null)]
		[Synchronizable("device_SessionUuid", true, true)]
		public string Uuid
		{
			get
			{
				return this._uuid;
			}
			set
			{
				this._uuid = value.ToLower();
			}
		}

		private Session(int id) : base(id)
		{
			this.IsPulled = true;
		}

		public void AddActivity(Activity activity)
		{
			if (this._activities.ContainsKey(activity.Id))
			{
				throw new DataEntryException(string.Format("Activity with ID {0} is already added to activity with ID {1}", activity.Id, this.Id));
			}
			if (activity.Session != this)
			{
				activity.Session = this;
				return;
			}
			this._activities[activity.Id] = activity;
		}

		public void CopyTo(Session session)
		{
			session.Uuid = this.Uuid;
			session.Esn = this.Esn;
			session.TimeStamp = this.TimeStamp;
			session.StartTime = this.StartTime;
			session.EndTime = this.EndTime;
		}

		public void RemoveActivity(Activity activity)
		{
			if (!this._activities.ContainsKey(activity.Id))
			{
				throw new DataEntryException(string.Format("Activity with ID {0} is not added to activity with ID {1}", activity.Id, this.Id));
			}
			if (activity.Session == this)
			{
				activity.Session = null;
				return;
			}
			this._activities.Remove(activity.Id);
		}
	}
}