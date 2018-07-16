using SkyTrakWrapper;
using System;
using System.Collections;
using System.Reflection;

[Serializable]
public class MMSFeatureFlagsForInspector
{
	public bool Module_Practice;

	public bool Module_Challenges;

	public bool Module_CoursePlay;

	public bool Module_GameImprovement;

	public bool Module_Profile;

	public bool Module_Menus;

	public bool Module_Settings;

	public bool Challenges_ClosestToPin;

	public bool Challenges_LongDrive;

	public bool Challenges_Targets;

	public bool Challenges_HitTheCartGuy;

	public bool Challenges_BreakingWindows;

	public bool Challenges_Battleship;

	public bool Menus_SessionHistory;

	public bool Menus_NumericDisplay;

	public bool Menus_Settings;

	public bool Settings_CameraAngle_Dynamic;

	public bool Settings_CameraAngle_FirstPerson;

	public bool Settings_CameraAngle_DownRange;

	public bool Settings_CameraAngle_FollowTheBall;

	public bool Settings_CameraAngle_45Degree;

	public bool Settings_Tracers_AllShots;

	public bool Settings_Tracers_Last5;

	public bool Settings_Tracers_SingleShot;

	public bool Settings_GroundConditions_Normal;

	public bool Settings_GroundConditions_Soft;

	public bool Settings_GroundConditions_Firm;

	public bool Settings_Environmental_Wind;

	public bool Settings_Environmental_Humidity;

	public bool Settings_Environmental_Altitude;

	public bool Settings_GolferOrientation;

	public bool Settings_SideSpinSideAxis;

	public bool Profile_Main;

	public bool Profile_History;

	public bool Profile_About;

	public bool Profile_Stats;

	public bool Profile_SkyGolf360Link;

	public bool CoursePlay_WGT_CTTH_SingleCourse;

	public bool CoursePlay_WGT_CTTH_AllCourses;

	public bool CoursePlay_WGT_StrokePlay_AllCourses;

	public bool CoursePlay_WGT_FeatureFlag1;

	public bool CoursePlay_WGT_FeatureFlag2;

	public bool CoursePlay_TruGolf_TeaserWare;

	public bool CoursePlay_TruGolf_ConsumerSubscription;

	public bool CoursePlay_TruGolf_CommercialSubscribtion;

	public bool CoursePlay_TruGolf_Future;

	public bool CoursePlay_TruGolf_StandardPack0;

	public bool CoursePlay_TruGolf_CoursePack1;

	public bool CoursePlay_TruGolf_CoursePack2;

	public bool CoursePlay_TruGolf_CoursePack3;

	public bool CoursePlay_TruGolf_CoursePack4;

	public bool CoursePlay_TruGolf_CoursePack5;

	public bool CoursePlay_TruGolf_CoursePack6;

	public bool CoursePlay_TruGolf_CoursePack7;

	public bool CoursePlay_TruGolf_CoursePack8;

	public bool CoursePlay_TruGolf_CoursePack9_future;

	public bool CoursePlay_TruGolf_CoursePack10_future;

	public bool CoursePlay_TruGolf_CoursePack11_future;

	public bool CoursePlay_TruGolf_CoursePack12_future;

	public bool GameImprovement_FittingCenter;

	public bool GameImprovement_SkyPro;

	public bool CoursePlay_PerfectGolf_Driving_Range_and_Practice_Package;

	public bool CoursePlay_PerfectGolf_Basic_Simulation_Package;

	public bool CoursePlay_Perfect_Golf_Premium_Package;

	public bool CoursePlay_Perfect_Golf_Commercial_Package;

	public bool CoursePlay_ProTeeUnited_FullMembershipNoRestrictions;

	public bool CoursePlay_ProTeeUnited_FutureFlag1;

	public bool CoursePlay_ProTeeUnited_FutureFlag2;

	public bool CoursePlay_ProTeeUnited_FutureFlag3;

	public bool CoursePlay_ProTeeUnited_FutureFlag4;

	public bool CoursePlay_TestAccount_FlagOn;

	public bool CoursePlay_TestAccount_FlagOff;

	public bool CoursePlay_TBD_1;

	public bool CoursePlay_TBD_2;

	public bool CoursePlay_TBD_3;

	public bool Module_ClubCompare;

	public bool Settings_Watch_Me;

	public bool Settings_PracticeGreen;

	public bool GameImprovement_SkillsAssessment;

	public bool Settings_SkillsAssessmentExportEnabled;

	public bool CoursePlay_CreativeGolf3D_Basic_One_Time_Fee;

	public bool CoursePlay_CreativeGolf3D_Set1;

	public bool CoursePlay_CreativeGolf3D_Set2;

	public bool CoursePlay_CreativeGolf3D_Set3;

	public bool CoursePlay_CreativeGolf3D_Set4;

	public bool CoursePlay_CreativeGolf3D_Set5;

	public bool CoursePlay_CreativeGolf3D_Set6;

	public bool CoursePlay_CreativeGolf3D_Set7;

	public bool CoursePlay_CreativeGolf3D_Set8;

	public bool CoursePlay_CreativeGolf3D_Set9;

	public bool CoursePlay_CreativeGolf3D_Set10;

	public bool CoursePlay_CreativeGolf3D_Basic_Annual_Fee;

	public bool FitnessGolf_Flag;

	public bool V1_Interactive_Data_Integration;

	public bool V1_Interactive_FutureFlag1;

	public bool V1_Interactive_FutureFlag2;

	public bool SkyTrakFlag96;

	public bool SkyTrakFlag97;

	public bool SkyTrakFlag98;

	public bool SkyTrakFlag99;

	public bool GameImprovement_BagMapping;

	public bool Settings_DispersionCircles;

	public bool Settings_CameraAngleOffset;

	public bool Settings_Temperature;

	public bool Settings_DeviceLeveling_AlignmentMode;

	public bool SkyTrakFlag105;

	public bool SkyTrakFlag106;

	public bool SkyTrakFlag107;

	public bool SkyTrakFlag108;

	public bool SkyTrakFlag109;

	public bool SkyTrakFlag110;

	public bool SkyTrakFlag111;

	public bool SkyTrakFlag112;

	public bool SkyTrakFlag113;

	public bool SkyTrakFlag114;

	public bool SkyTrakFlag115;

	public bool SkyTrakFlag116;

	public bool SkyTrakFlag117;

	public bool SkyTrakFlag118;

	public bool SkyTrakFlag119;

	public bool SkyTrakFlag120;

	public bool SkyTrakFlag121;

	public bool SkyTrakFlag122;

	public bool SkyTrakFlag123;

	public bool SkyTrakFlag124;

	public bool SkyTrakFlag125;

	public bool SkyTrakFlag126;

	public bool SkyTrakFlag127;

	public bool SkyTrakFlag128;

	public bool SkyTrakFlag129;

	public bool SkyTrakFlag130;

	public bool SkyTrakFlag131;

	public bool SkyTrakFlag132;

	public bool SkyTrakFlag133;

	public bool SkyTrakFlag134;

	public bool SkyTrakFlag135;

	public bool SkyTrakFlag136;

	public bool SkyTrakFlag137;

	public bool SkyTrakFlag138;

	public bool SkyTrakFlag139;

	public bool SkyTrakFlag140;

	public bool SkyTrakFlag141;

	public bool SkyTrakFlag142;

	public bool SkyTrakFlag143;

	public bool SkyTrakFlag144;

	public bool SkyTrakFlag145;

	public bool SkyTrakFlag146;

	public bool SkyTrakFlag147;

	public bool SkyTrakFlag148;

	public bool SkyTrakFlag149;

	public bool SkyTrakFlag150;

	public bool SkyTrakFlag151;

	public bool SkyTrakFlag152;

	public bool SkyTrakFlag153;

	public bool SkyTrakFlag154;

	public bool SkyTrakFlag155;

	public bool SkyTrakFlag156;

	public bool SkyTrakFlag157;

	public bool SkyTrakFlag158;

	public bool SkyTrakFlag159;

	public bool SkyTrakFlag160;

	public bool SkyTrakFlag161;

	public bool SkyTrakFlag162;

	public bool SkyTrakFlag163;

	public bool SkyTrakFlag164;

	public bool SkyTrakFlag165;

	public bool SkyTrakFlag166;

	public bool SkyTrakFlag167;

	public bool SkyTrakFlag168;

	public bool SkyTrakFlag169;

	public bool SkyTrakFlag170;

	public bool SkyTrakFlag171;

	public bool SkyTrakFlag172;

	public bool SkyTrakFlag173;

	public bool SkyTrakFlag174;

	public bool SkyTrakFlag175;

	public bool SkyTrakFlag176;

	public bool SkyTrakFlag177;

	public bool SkyTrakFlag178;

	public bool SkyTrakFlag179;

	public bool SkyTrakFlag180;

	public bool SkyTrakFlag181;

	public bool SkyTrakFlag182;

	public bool SkyTrakFlag183;

	public bool SkyTrakFlag184;

	public bool SkyTrakFlag185;

	public bool SkyTrakFlag186;

	public bool SkyTrakFlag187;

	public bool SkyTrakFlag188;

	public bool SkyTrakFlag189;

	public bool SkyTrakFlag190;

	public bool SkyTrakFlag191;

	public bool SkyTrakFlag192;

	public bool SkyTrakFlag193;

	public bool SkyTrakFlag194;

	public bool SkyTrakFlag195;

	public bool SkyTrakFlag196;

	public bool SkyTrakFlag197;

	public bool SkyTrakFlag198;

	public bool SkyTrakFlag199;

	public bool SkyTrakFlag200;

	public bool SkyTrakFlag201;

	public bool SkyTrakFlag202;

	public bool SkyTrakFlag203;

	public bool SkyTrakFlag204;

	public bool SkyTrakFlag205;

	public bool SkyTrakFlag206;

	public bool SkyTrakFlag207;

	public bool SkyTrakFlag208;

	public bool SkyTrakFlag209;

	public bool SkyTrakFlag210;

	public bool SkyTrakFlag211;

	public bool SkyTrakFlag212;

	public bool SkyTrakFlag213;

	public bool SkyTrakFlag214;

	public bool SkyTrakFlag215;

	public bool SkyTrakFlag216;

	public bool SkyTrakFlag217;

	public bool SkyTrakFlag218;

	public bool SkyTrakFlag219;

	public bool SkyTrakFlag220;

	public bool SkyTrakFlag221;

	public bool SkyTrakFlag222;

	public bool SkyTrakFlag223;

	public bool SkyTrakFlag224;

	public bool SkyTrakFlag225;

	public bool SkyTrakFlag226;

	public bool SkyTrakFlag227;

	public bool SkyTrakFlag228;

	public bool SkyTrakFlag229;

	public bool SkyTrakFlag230;

	public bool SkyTrakFlag231;

	public bool SkyTrakFlag232;

	public bool SkyTrakFlag233;

	public bool SkyTrakFlag234;

	public bool SkyTrakFlag235;

	public bool SkyTrakFlag236;

	public bool SkyTrakFlag237;

	public bool SkyTrakFlag238;

	public bool SkyTrakFlag239;

	public bool SkyTrakFlag240;

	public bool SkyTrakFlag241;

	public bool SkyTrakFlag242;

	public bool SkyTrakFlag243;

	public bool SkyTrakFlag244;

	public bool SkyTrakFlag245;

	public bool SkyTrakFlag246;

	public bool SkyTrakFlag247;

	public bool SkyTrakFlag248;

	public bool SkyTrakFlag249;

	public bool SkyTrakFlag250;

	public bool SkyTrakFlag251;

	public bool SkyTrakFlag252;

	public bool SkyTrakFlag253;

	public bool SkyTrakFlag254;

	public bool SkyTrakFlag255;

	public MMSFeatureFlagsForInspector()
	{
	}

	public BitArray GetBitArray()
	{
		BitArray bitArrays = new BitArray(256, false);
		for (int i = 0; i < 256; i++)
		{
			STSWMMSFeatureFlagType sTSWMMSFeatureFlagType = (STSWMMSFeatureFlagType)i;
			FieldInfo field = this.GetType().GetField(sTSWMMSFeatureFlagType.ToString());
			bitArrays[i] = (bool)this.GetType().InvokeMember(field.Name, BindingFlags.GetField, null, this, null);
		}
		return bitArrays;
	}

	public void SetBitArray(BitArray arr)
	{
		for (int i = 0; i < 256; i++)
		{
			STSWMMSFeatureFlagType sTSWMMSFeatureFlagType = (STSWMMSFeatureFlagType)i;
			this.GetType().GetField(sTSWMMSFeatureFlagType.ToString()).SetValue(this, arr[i]);
		}
	}
}