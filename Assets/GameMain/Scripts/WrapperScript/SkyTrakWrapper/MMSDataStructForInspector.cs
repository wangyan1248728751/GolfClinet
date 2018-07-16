using Security;
using System;
using UnityEngine;

[Serializable]
public class MMSDataStructForInspector : MMSDataStruct
{
	[SerializeField]
	public string mMembershipExpireDate;

	[SerializeField]
	public string mInitialRegistrationDate;

	[SerializeField]
	public string mTotalShotsTakenLastUpdatedDate;

	[SerializeField]
	public string mFirstUseDate;

	[SerializeField]
	public string mMembershipPackDate;

	[SerializeField]
	public MMSFeatureFlagsForInspector mFeatureFlags;

	public string SFirstUseDate
	{
		get
		{
			this.mFirstUseDate = this.FirstUseDate.ToString("G");
			return this.mFirstUseDate;
		}
		set
		{
			this.mFirstUseDate = value;
			this.FirstUseDate = this.ConvertStringToDate(this.mFirstUseDate);
		}
	}

	public string SInitialRegistrationDate
	{
		get
		{
			this.mInitialRegistrationDate = this.InitialRegistrationDate.ToString("G");
			return this.mInitialRegistrationDate;
		}
		set
		{
			this.mInitialRegistrationDate = value;
			this.InitialRegistrationDate = this.ConvertStringToDate(this.mInitialRegistrationDate);
		}
	}

	public string SMembershipExpireDate
	{
		get
		{
			this.mMembershipExpireDate = this.MembershipExpireDate.ToString("G");
			return this.mMembershipExpireDate;
		}
		set
		{
			this.mMembershipExpireDate = value;
			this.MembershipExpireDate = this.ConvertStringToDate(this.mMembershipExpireDate);
		}
	}

	public string SMembershipPackDate
	{
		get
		{
			this.mMembershipPackDate = this.MembershipPackDate.ToString("G");
			return this.mMembershipPackDate;
		}
		set
		{
			this.mMembershipPackDate = value;
			this.MembershipPackDate = this.ConvertStringToDate(this.mMembershipPackDate);
		}
	}

	public string STotalShotsTakenLastUpdatedDate
	{
		get
		{
			this.mTotalShotsTakenLastUpdatedDate = this.TotalShotsTakenLastUpdatedDate.ToString("G");
			return this.mTotalShotsTakenLastUpdatedDate;
		}
		set
		{
			this.mTotalShotsTakenLastUpdatedDate = value;
			this.TotalShotsTakenLastUpdatedDate = this.ConvertStringToDate(this.mTotalShotsTakenLastUpdatedDate);
		}
	}

	public MMSDataStructForInspector()
	{
	}

	private DateTime ConvertStringToDate(string dateString)
	{
		DateTime dateTime;
		if (string.IsNullOrEmpty(dateString))
		{
			Debug.LogWarning("SecurityWrapperDebug: date string is empty");
			return new DateTime(1970, 1, 1);
		}
		if (DateTime.TryParse(dateString, out dateTime))
		{
			return dateTime;
		}
		Debug.LogWarning("SecurityWrapperDebug: parsing date string was fail");
		return new DateTime(1970, 1, 1);
	}

	public void FromString(string str)
	{
	}

	public override string ToString()
	{
		return string.Empty;
	}
}