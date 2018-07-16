using System;
using System.Collections.Generic;

public class TFlightTrajectory
{
	private readonly List<TFlightPoint> _carryTrajectory;

	private readonly List<TFlightPoint> _rollTrajectory;

	private readonly List<TFlightPoint> _totalTrajectory;

	public IList<TFlightPoint> CarryTrajectory
	{
		get
		{
			return this._carryTrajectory;
		}
	}

	public IList<TFlightPoint> RollTrajectory
	{
		get
		{
			return this._rollTrajectory;
		}
	}

	public IList<TFlightPoint> TotalTrajectory
	{
		get
		{
			return this._totalTrajectory;
		}
	}

	public TFlightTrajectory(List<TFlightPoint> carryTrajectory, List<TFlightPoint> rollTrajectory)
	{
		this._carryTrajectory = carryTrajectory;
		this._rollTrajectory = rollTrajectory;
		this._totalTrajectory = new List<TFlightPoint>(carryTrajectory.Count + rollTrajectory.Count);
		this._totalTrajectory.AddRange(carryTrajectory);
		this._totalTrajectory.AddRange(rollTrajectory);
	}
}