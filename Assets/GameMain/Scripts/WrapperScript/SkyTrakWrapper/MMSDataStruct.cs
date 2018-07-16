using SkyTrakWrapper;
using System;
using System.Collections;

namespace Security
{
	public class MMSDataStruct
	{
		public byte DataFormatVersion;

		public bool Registered;

		public int CustomerId;

		public byte MembershipType;

		public string MembershipName;

		public DateTime MembershipExpireDate;

		public int GraceDays;

		public bool NewUnit;

		public DateTime InitialRegistrationDate;

		public string UnitType;

		public string QuickAccessCode;

		public string MemberFirstName;

		public string MemberLastName;

		public string MemberPhone;

		public int NewSystemGraceDays;

		public string EsnVerificationCode;

		public int TotalShotsTaken;

		public DateTime TotalShotsTakenLastUpdatedDate;

		public DateTime FirstUseDate;

		public DateTime MembershipPackDate;

		public byte DeviceStatusFlag;

		public BitArray FeatureFlags;

		public BitArray Reserved;

		public MMSDataStruct()
		{
			this.MembershipExpireDate = new DateTime(1970, 1, 1);
			this.InitialRegistrationDate = new DateTime(1970, 1, 1);
			this.TotalShotsTakenLastUpdatedDate = new DateTime(1970, 1, 1);
			this.FirstUseDate = new DateTime(1970, 1, 1);
			this.MembershipPackDate = new DateTime(1970, 1, 1);
			this.FeatureFlags = new BitArray(256);
		}

		public void Copy(MMSDataStruct mmsDataStruct)
		{
			this.DataFormatVersion = mmsDataStruct.DataFormatVersion;
			this.Registered = mmsDataStruct.Registered;
			this.CustomerId = mmsDataStruct.CustomerId;
			this.MembershipType = mmsDataStruct.MembershipType;
			this.MembershipName = mmsDataStruct.MembershipName;
			this.MembershipExpireDate = mmsDataStruct.MembershipExpireDate;
			this.GraceDays = mmsDataStruct.GraceDays;
			this.NewUnit = mmsDataStruct.NewUnit;
			this.InitialRegistrationDate = mmsDataStruct.InitialRegistrationDate;
			this.UnitType = mmsDataStruct.UnitType;
			this.QuickAccessCode = mmsDataStruct.QuickAccessCode;
			this.MemberFirstName = mmsDataStruct.MemberFirstName;
			this.MemberLastName = mmsDataStruct.MemberLastName;
			this.MemberPhone = mmsDataStruct.MemberPhone;
			this.NewSystemGraceDays = mmsDataStruct.NewSystemGraceDays;
			this.EsnVerificationCode = mmsDataStruct.EsnVerificationCode;
			this.TotalShotsTaken = mmsDataStruct.TotalShotsTaken;
			this.TotalShotsTakenLastUpdatedDate = mmsDataStruct.TotalShotsTakenLastUpdatedDate;
			this.FirstUseDate = mmsDataStruct.FirstUseDate;
			this.MembershipPackDate = mmsDataStruct.MembershipPackDate;
			this.DeviceStatusFlag = mmsDataStruct.DeviceStatusFlag;
			this.FeatureFlags = mmsDataStruct.FeatureFlags;
		}

		private bool GetFlagValue(STSWMMSFeatureFlagType flag)
		{
			if (this.FeatureFlags == null || this.FeatureFlags.Count < (int)flag + (int)STSWMMSFeatureFlagType.Module_Challenges)
			{
				return false;
			}
			return this.FeatureFlags.Get((int)flag);
		}

		public override string ToString()
		{
			JSONObject jSONObject = new JSONObject();
			jSONObject.AddField("DataFormatVersion", (int)this.DataFormatVersion);
			jSONObject.AddField("Registered", this.Registered);
			jSONObject.AddField("CustomerID", this.CustomerId);
			jSONObject.AddField("MembershipType", (int)this.MembershipType);
			jSONObject.AddField("MembershipName", (this.MembershipName != null ? this.MembershipName.Replace("\0", string.Empty) : "null"));
			jSONObject.AddField("GraceDays", this.GraceDays);
			jSONObject.AddField("NewUnit", this.NewUnit);
			jSONObject.AddField("UnitType", (this.UnitType != null ? this.UnitType.Replace("\0", string.Empty) : "null"));
			jSONObject.AddField("QuickAccessCode", (this.QuickAccessCode != null ? this.QuickAccessCode.Replace("\0", string.Empty) : "null"));
			jSONObject.AddField("MemberFirstName", (this.MemberFirstName != null ? this.MemberFirstName.Replace("\0", string.Empty) : "null"));
			jSONObject.AddField("MemberLastName", (this.MemberLastName != null ? this.MemberLastName.Replace("\0", string.Empty) : "null"));
			jSONObject.AddField("MemberPhone", (this.MemberPhone != null ? this.MemberPhone.Replace("\0", string.Empty) : "null"));
			jSONObject.AddField("NewSystemGraceDays", this.NewSystemGraceDays);
			jSONObject.AddField("ESNVerificationCode", (this.EsnVerificationCode != null ? this.EsnVerificationCode.Replace("\0", string.Empty) : "null"));
			jSONObject.AddField("TotalShotsTaken", this.TotalShotsTaken);
			jSONObject.AddField("TotalShotsTakenLastUpdatedDate", this.TotalShotsTakenLastUpdatedDate.ToString("G"));
			jSONObject.AddField("FirstUseDate", this.FirstUseDate.ToString("G"));
			jSONObject.AddField("MembershipPackDate", this.MembershipPackDate.ToString("G"));
			jSONObject.AddField("MembershipExpireDate", this.MembershipExpireDate.ToString("G"));
			jSONObject.AddField("InitialRegistrationDate", this.InitialRegistrationDate.ToString("G"));
			jSONObject.AddField("DeviceStatusFlag", (int)this.DeviceStatusFlag);
			for (int i = 0; i < Enum.GetValues(typeof(STSWMMSFeatureFlagType)).Length; i++)
			{
				STSWMMSFeatureFlagType sTSWMMSFeatureFlagType = (STSWMMSFeatureFlagType)i;
				bool flagValue = this.GetFlagValue(sTSWMMSFeatureFlagType);
				jSONObject.AddField(sTSWMMSFeatureFlagType.ToString(), flagValue);
			}
			return jSONObject.ToString();
		}
	}
}