using Data;
using System;
using UnityEngine;

namespace Converters
{
	internal class ShotFlightDataConverter : Converter<Shot, TFlightData>
	{
		public override TFlightData Convert(Shot shot)
		{
			TFlightData tFlightDatum = new TFlightData()
			{
				esnShotId = shot.Id,
				backSpin = shot.BackSpin,
				totalSpeedMPH = shot.BallSpeed,
				launchAngle = shot.VerticalLaunchAngle,
				horizontalAngle = shot.HorizontalLaunchAngle,
				side = shot.HorizontalLaunchAngle,
				descentAngle = shot.DescentAngle,
				sideSpin = shot.SideSpin,
				clubSpeed = shot.ClubHeadSpeed,
				carry = shot.CarryDistance,
				roll = shot.RollDistance,
				offline = shot.Offline,
				maxHeight = shot.Altitude,
				smashFactor = shot.SmashFactor,
				travelTime = shot.FlightTime,
				clubTypeID = shot.ClubType,
				clubName = shot.ClubName,
				dateOfHit = shot.ShotDate,
				isLefty = shot.Dexterity == "L",
				carryPosX = shot.CarryPosX,
				carryPosY = shot.CarryPosY,
				totalPosX = shot.TotalPosX,
				totalPosY = shot.TotalPosY,
				courseConditionId = shot.CourseConditionId
			};
			return tFlightDatum;
		}

		public override Shot Convert(TFlightData flightData)
		{
			Shot instance = DataEntry.GetInstance<Shot>(flightData.esnShotId);
			instance.BackSpin = flightData.backSpin;
			instance.BallSpeed = flightData.totalSpeedMPH;
			instance.VerticalLaunchAngle = flightData.launchAngle;
			instance.HorizontalLaunchAngle = flightData.side;
			instance.DescentAngle = flightData.descentAngle;
			instance.SideSpin = flightData.sideSpin;
			instance.HoleNumber = 0;
			instance.UnitOfMeasure = "I";
			instance.ClubHeadSpeed = flightData.clubSpeed;
			instance.CarryDistance = flightData.carry;
			instance.RollDistance = flightData.roll;
			instance.TotalDistance = (float)(Mathf.RoundToInt(flightData.carry) + Mathf.RoundToInt(flightData.roll));
			instance.Offline = flightData.offline;
			instance.Altitude = flightData.maxHeight;
			instance.SmashFactor = flightData.smashFactor;
			instance.FlightTime = flightData.travelTime;
			instance.IsFavorite = false;
			instance.ClubType = flightData.clubTypeID;
			instance.ClubName = flightData.clubName;
			instance.ShotDate = flightData.dateOfHit;
			instance.Dexterity = (!flightData.isLefty ? "R" : "L");
			instance.GameXStart = 0f;
			instance.GameYStart = 0f;
			instance.CarryPosX = flightData.carryPosX;
			instance.CarryPosY = flightData.carryPosY;
			instance.TotalPosX = flightData.totalPosX;
			instance.TotalPosY = flightData.totalPosY;
			instance.CourseConditionId = flightData.courseConditionId;
			return instance;
		}
	}
}