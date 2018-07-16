using System;
using System.Runtime.CompilerServices;

namespace Data
{
	[DataTable("User")]
	public class User : DataEntry
	{
		public const string SPlayerIsLefty = "L";

		public const string SPlayerIsRighty = "R";

		public const string ActionAdd = "add";

		public const string ActionUpdate = "update";

		public const string CustomerIdSyncName = "customerid";

		[Synchronizable("action", true, false)]
		public string Action
		{
			get;
			set;
		}

		[DataColumn("dateOfBirth", false, false, null)]
		[Synchronizable("dateOfBirth", true, false)]
		public string BirthDate
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

		[DataColumn("email", false, false, null)]
		[Synchronizable]
		public string Email
		{
			get;
			set;
		}

		[Synchronizable("esn", true, false)]
		private string Esn
		{
			get
			{
				return ApplicationDataManager.instance.ESN;
			}
		}

		[DataColumn("firstName", false, false, null)]
		[Synchronizable("firstName", true, true)]
		public string FirstName
		{
			get;
			set;
		}

		[DataColumn("gender", false, false, null)]
		[Synchronizable]
		public string Gender
		{
			get;
			set;
		}

		[Synchronizable("customerid", true, true)]
		public override int Id
		{
			get
			{
				return base.Id;
			}
			protected set
			{
				base.Id = value;
			}
		}

		private new bool IsDeleted
		{
			get
			{
				return false;
			}
		}

		public bool IsLefty
		{
			get
			{
				return this.Dexterity.Equals("L");
			}
		}

		[DataColumn("lastName", false, false, null)]
		[Synchronizable("lastName", true, true)]
		public string LastName
		{
			get;
			set;
		}

		[DataColumn("status", false, false, null)]
		public int Status
		{
			get;
			set;
		}

		[DataColumn("userName", false, false, null)]
		[Synchronizable("username", true, true)]
		public string UserName
		{
			get;
			set;
		}

		private User(int id) : base(id)
		{
			this.Action = "update";
			this.Status = 2;
		}

		public void SetUserStatus(User.UserStatus status)
		{
			this.Status = (int)status;
			this.BirthDate = string.Empty;
		}

		public enum UserStatus
		{
			Guest = 2,
			DeviceOwner = 8
		}
	}
}