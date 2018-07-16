using SkyTrakWrapper;
using System;

public class FlightData
{
	private static FlightData instance;

	private RIPESpeedParamsType oldSpeedParams;

	private RIPESpinParamsType oldSpinParams;

	private RIPEFlightParamsType oldFlightParams;

	public static FlightData Instance
	{
		get
		{
			if (FlightData.instance == null)
			{
				FlightData.instance = new FlightData();
			}
			return FlightData.instance;
		}
	}

	public RIPEFlightParamsType GetOldFlightParams()
	{
		return this.oldFlightParams;
	}

	public RIPESpeedParamsType GetOldSpeedParams()
	{
		return this.oldSpeedParams;
	}

	public RIPESpinParamsType GetOldSpinParams()
	{
		return this.oldSpinParams;
	}

	public void SetOldFlightParams(RIPEFlightParamsType newerData)
	{
		this.oldFlightParams = newerData;
	}

	public void SetOldSpeedParams(RIPESpeedParamsType newerData)
	{
		this.oldSpeedParams = newerData;
	}

	public void SetOldSpinParams(RIPESpinParamsType newerData)
	{
		this.oldSpinParams = newerData;
	}
}