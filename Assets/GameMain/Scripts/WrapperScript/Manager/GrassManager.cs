using System;
using System.Collections.Generic;
using UnityEngine;

public class GrassManager
{
	private static GrassManager _instance;

	private GrassManager.GrassType defaultGrassType;

	public double[] terrainBounceCoeff = new double[] { 0.11, 0.07, 0.15, 0.05 };

	public double[] terrainSpinFrictionCoeff = new double[] { 0.7, 0.9, 0.4, 0.9 };

	public double[] frictionCoeff = new double[] { 3, 3.8, 2.2, 2 };

	public GrassManager.GrassType CurrentGrassType
	{
		get
		{
			return this.defaultGrassType;
		}
	}

	public static GrassManager instance
	{
		get
		{
			if (GrassManager._instance == null)
			{
				GrassManager._instance = new GrassManager();
			}
			return GrassManager._instance;
		}
	}

	static GrassManager()
	{
	}

	private GrassManager()
	{
		this.defaultGrassType = GrassManager.GrassType.GRASS_TYPE_NORMAL;
	}

	public void ChangeDefaultMaterial(string softness)
	{
		if (softness != null)
		{
			if (softness == "NORMAL")
			{
				this.defaultGrassType = GrassManager.GrassType.GRASS_TYPE_NORMAL;
			}
			else if (softness == "SOFT")
			{
				this.defaultGrassType = GrassManager.GrassType.GRASS_TYPE_SOFT;
			}
			else if (softness == "FIRM")
			{
				this.defaultGrassType = GrassManager.GrassType.GRASS_TYPE_FIRM;
			}
		}
	}

	public double getBounceCoeff(GrassManager.GrassType grassType)
	{
		return this.terrainBounceCoeff[(int)grassType];
	}

	public double getFrictionCoeff(GrassManager.GrassType grassType)
	{
		return this.frictionCoeff[(int)grassType];
	}

	public GrassManager.GrassType getGrassType(double x, double z, List<SimulationGreen> greens)
	{
		GrassManager.GrassType grassType = this.defaultGrassType;
		if (greens == null)
		{
			return grassType;
		}
		foreach (SimulationGreen green in greens)
		{
			double radius = (double)green.Radius;
			if (x <= (double)green.Origin.x - radius || x >= (double)green.Origin.x + radius || z <= (double)green.Origin.z - radius || z >= (double)green.Origin.z + radius)
			{
				continue;
			}
			grassType = GrassManager.GrassType.GRASS_TYPE_GREEN;
			break;
		}
		return grassType;
	}

	public double getSpinFrictionCoeff(GrassManager.GrassType grassType)
	{
		return this.terrainSpinFrictionCoeff[(int)grassType];
	}

	public void SetGrassType(GrassManager.GrassType type)
	{
		this.defaultGrassType = type;
	}

	public enum GrassType
	{
		GRASS_TYPE_NORMAL,
		GRASS_TYPE_SOFT,
		GRASS_TYPE_FIRM,
		GRASS_TYPE_GREEN
	}
}