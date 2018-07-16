using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CBallFlightManager : MonoBehaviour
{
	private static CBallFlightManager instance;

	public double PhysicsFPS = 120;

	public double mAltitude;

	public double mTemperature;

	public double mHumidity;

	public double mBarometer;

	public double mWindSpeed;

	public double mWindDirection;

	public double[] TrajDownRange = new double[10000];

	public double[] TrajOffLine = new double[10000];

	public double[] TrajHeight = new double[10000];

	public double[] TrajSpin = new double[10000];

	public double[] TrajAngle = new double[10000];

	private long TrajDt;

	public long FirstBounceFrame = (long)-1;

	public bool DoDrag = true;

	public bool KeepAboveGround;

	public double SpeedFPS;

	public double SpeedMPH;

	public double EffectiveLoft;

	public double AngleOfAttack;

	public double AngleOfFace;

	public double AngleOfPath;

	private double loftClub = 10;

	private double ClubMass = 200;

	public List<SimulationGreen> Greens;

	private double[] IC = new double[10];

	public double[] RES = new double[10];

	private double Density;

	private double Visc;

	private double BallDiam;

	private double BallWeight;

	private double InitialHAG;

	private double Mass;

	private double Area;

	private double FDrag;

	private double FLift;

	private double BallCircumference;

	private double Velocity;

	private double LaunchAngle;

	private double DispAngle;

	private double SideAngle;

	private double BackSpin;

	private double SideSpin;

	private double Riflespin;

	private double TrajTime;

	private double MaxAlt;

	private double DescendingAngle;

	public double Carry;

	public double Total;

	public double TDisp;

	public double CDisp;

	public double Roll;

	private double Vmag;

	private double Wmag;

	private Vector3 mWind = new Vector3();

	private double BallDiameterInches;

	private double[] K1 = new double[10];

	private double[] K2 = new double[10];

	private double[] K3 = new double[10];

	private double[] K4 = new double[10];

	private double[] Y = new double[10];

	private double[] Yaux = new double[10];

	private double[] F = new double[10];

	private double[] Hold = new double[10];

	private const int MAX_STEPS = 10000;

	public double terrainBounceCoeff;

	public double terrainSpinFrictionCoeff;

	public float frictionCoeff;

	public double CLDisplay;

	public double CDDisplay;

	public Material m_ballTrailMaterial;

	public float DescentAngle
	{
		get;
		private set;
	}

	static CBallFlightManager()
	{
	}

	public CBallFlightManager()
	{
	}

	private double adjustCLForSpinRate(double cl, double Spin, bool isBounce)
	{
		double num = cl;
		if (Spin > 8200)
		{
			if (cl > 0.65)
			{
				num = 0.65;
			}
		}
		else if (Spin > 7150)
		{
			if (cl > 0.63)
			{
				num = 0.63;
			}
		}
		else if (Spin > 6600)
		{
			if (cl > 0.58)
			{
				num = 0.58;
			}
		}
		else if (Spin > 5400)
		{
			if (cl > 0.41)
			{
				num = 0.41;
			}
		}
		else if (Spin > 4500)
		{
			if (cl > 0.4)
			{
				num = 0.4;
			}
		}
		else if (Spin > 3900)
		{
			if (cl > 0.35)
			{
				num = 0.35;
			}
		}
		else if (!isBounce && this.LaunchAngle <= 10 && this.BackSpin <= 3900 && this.SpeedMPH >= 60)
		{
			num = cl * 1.05;
		}
		else if (cl > 0.3)
		{
			num = 0.3;
		}
		return num;
	}

	private void Awake()
	{
		if (CBallFlightManager.instance == null)
		{
			CBallFlightManager.instance = this;
		}
	}

	private void CalcTraj(double tstrt, bool isBounce)
	{
		double hold = 0;
		double[] numArray = new double[10000];
		double num = 9;
		double physicsFPS = 1 / this.PhysicsFPS;
		double num1 = tstrt;
		for (long i = (long)1; (double)i <= num; i += (long)1)
		{
			this.Y[checked(i)] = this.IC[checked(i)];
		}
		this.TrajEqu(num1, this.Y, this.F, isBounce);
		while (this.Y[2] > -this.InitialHAG + this.BallDiam * 0.5 && this.TrajDt < (long)9999)
		{
			for (long j = (long)1; (double)j <= num; j += (long)1)
			{
				this.Yaux[checked(j)] = this.Y[checked(j)];
			}
			this.TrajEqu(num1, this.Yaux, this.F, isBounce);
			for (long k = (long)1; (double)k <= num; k += (long)1)
			{
				this.K1[checked(k)] = this.F[checked(k)] * physicsFPS;
				this.Yaux[checked(k)] = this.Y[checked(k)] + this.K1[checked(k)] / 2;
			}
			double num2 = num1 + physicsFPS / 2;
			this.TrajEqu(num2, this.Yaux, this.F, isBounce);
			for (long l = (long)1; (double)l <= num; l += (long)1)
			{
				this.K2[checked(l)] = this.F[checked(l)] * physicsFPS;
				this.Yaux[checked(l)] = this.Y[checked(l)] + this.K2[checked(l)] / 2;
			}
			num2 = num1 + physicsFPS / 2;
			this.TrajEqu(num2, this.Yaux, this.F, isBounce);
			for (long m = (long)1; (double)m <= num; m += (long)1)
			{
				this.K3[checked(m)] = this.F[checked(m)] * physicsFPS;
				this.Yaux[checked(m)] = this.Y[checked(m)] + this.K3[checked(m)];
			}
			num2 = num1 + physicsFPS;
			this.TrajEqu(num2, this.Yaux, this.F, isBounce);
			for (long n = (long)1; (double)n <= num; n += (long)1)
			{
				this.K4[checked(n)] = physicsFPS * this.F[checked(n)];
				this.Hold[checked(n)] = this.Y[checked(n)];
				Y[checked(n)] += (this.K1[checked(n)] + 2 * this.K2[checked(n)] + 2 * this.K3[checked(n)] + this.K4[checked(n)]) / 6;
			}
			num1 += physicsFPS;
			this.TrajSpin[checked(this.TrajDt)] = this.Y[9] * 60 / 2 / 3.14159274101257;
			this.TrajDownRange[checked(this.TrajDt)] = this.Y[1];
			this.TrajOffLine[checked(this.TrajDt)] = this.Y[3];
			this.TrajHeight[checked(this.TrajDt)] = (this.Y[2] < 0 ? 0 : this.Y[2]);
			if (this.TrajDt <= (long)0)
			{
				numArray[checked(this.TrajDt)] = 0;
			}
			else
			{
				numArray[checked(this.TrajDt)] = (double)(57.2957764f * Mathf.Atan((float)((this.TrajHeight[checked(this.TrajDt)] - this.TrajHeight[checked((this.TrajDt - (long)1))]) / (this.TrajDownRange[checked(this.TrajDt)] - this.TrajDownRange[checked((this.TrajDt - (long)1))]))));
			}
			this.TrajDt += (long)1;
			this.TrajEqu(num1, this.Y, this.F, isBounce);
		}
		this.DescendingAngle = numArray[checked((this.TrajDt - (long)3))];
		if (this.TrajDt < (long)10000)
		{
			hold = num1;
			for (long o = (long)1; (double)o <= num; o += (long)1)
			{
				this.RES[checked(o)] = this.Hold[checked(o)];
			}
		}
		else
		{
			hold = num1 - physicsFPS + physicsFPS * -this.Hold[2] / (this.Y[2] - this.Hold[2]);
			for (long p = (long)1; (double)p <= num; p += (long)1)
			{
				this.RES[checked(p)] = this.Hold[checked(p)] + (this.Y[checked(p)] - this.Hold[checked(p)]) * -this.Hold[2] / (this.Y[2] - this.Hold[2]);
			}
		}
		this.TrajTime = hold;
	}

	public void CalculateCollision(double BallSpeed_MPH, double LaunchAngle, double SideAngle, double BackSpin, double SideSpin)
	{
		double ballSpeedMPH = BallSpeed_MPH * 5280 / 3600;
		this.SpeedFPS = 0;
		this.SpeedMPH = 0;
		this.EffectiveLoft = 0;
		this.AngleOfAttack = 0;
		this.AngleOfFace = 0;
		double ballWeight = this.BallWeight / 16 / 32.174048;
		double ballDiam = this.BallDiam / 2;
		double num = 0.4 * ballWeight * ballDiam * ballDiam;
		double clubMass = this.ClubMass / 28.349 / 16 / 32.174048;
		double num1 = 1 - 0.0011 * ballSpeedMPH * 3600 / 5280;
		double backSpin = BackSpin * 0.104719758033752 * num / ballDiam;
		double num2 = (double)Mathf.Sqrt((float)(ballWeight * ballSpeedMPH * (ballWeight * ballSpeedMPH) - backSpin * backSpin));
		this.EffectiveLoft = (1 + num1) * backSpin * (1 / clubMass + 1 / ballWeight + ballDiam * ballDiam / num);
		this.EffectiveLoft = this.EffectiveLoft / (num2 * (1 / clubMass + 1 / ballWeight));
		this.EffectiveLoft = (double)Mathf.Atan((float)this.EffectiveLoft) * 180 / 3.14159274101257;
		this.SpeedFPS = backSpin * (1 / clubMass + 1 / ballWeight + ballDiam * ballDiam / num) / (double)Mathf.Sin((float)(this.EffectiveLoft / 180 * 3.14159274101257));
		this.SpeedMPH = this.SpeedFPS * 3600 / 5280;
		double num3 = (double)Mathf.Atan2((float)backSpin, (float)num2) * 57.295777918682;
		this.AngleOfAttack = LaunchAngle + num3 - this.loftClub;
		backSpin = SideSpin * 0.104719758033752 * num / ballDiam;
		num2 = (double)Mathf.Sqrt((float)(ballWeight * ballSpeedMPH * (ballWeight * ballSpeedMPH) - backSpin * backSpin));
		num3 = (double)Mathf.Atan2((float)backSpin, (float)num2) * 57.295777918682;
		this.AngleOfFace = (1 + num1) * backSpin * (1 / clubMass + 1 / ballWeight + ballDiam * ballDiam / num);
		this.AngleOfFace = this.AngleOfFace / (num2 * (1 / clubMass + 1 / ballWeight));
		this.AngleOfFace = (double)Mathf.Atan((float)this.AngleOfFace) * 57.295777918682;
		this.AngleOfFace *= -1;
		this.AngleOfPath = this.AngleOfFace - SideAngle - num3;
		if (SideAngle < 0)
		{
			this.AngleOfPath = (double)(-1f * Mathf.Abs((float)this.AngleOfPath));
		}
		if (SideAngle > 0)
		{
			this.AngleOfPath = (double)Mathf.Abs((float)this.AngleOfPath);
		}
		double launchAngle = LaunchAngle + num3 - this.EffectiveLoft;
		if ((double)Mathf.Abs((float)(launchAngle - this.AngleOfAttack)) > 2.5)
		{
			this.AngleOfAttack = launchAngle;
		}
		this.AngleOfPath = 0;
		this.AngleOfFace = 0;
		if (SideAngle >= 2)
		{
			this.AngleOfPath = 2;
			if (SideSpin <= -200)
			{
				this.AngleOfFace = 2;
			}
			if (SideSpin > -200 && SideSpin < 200)
			{
				this.AngleOfFace = 0;
			}
			if (SideSpin >= 200)
			{
				this.AngleOfFace = -2;
			}
		}
		if (SideAngle <= -2)
		{
			this.AngleOfPath = -2;
			if (SideSpin <= -200)
			{
				this.AngleOfFace = 2;
			}
			if (SideSpin > -200 && SideSpin < 200)
			{
				this.AngleOfFace = 0;
			}
			if (SideSpin >= 200)
			{
				this.AngleOfFace = -2;
			}
		}
		if (SideAngle > -2 && SideAngle < 2)
		{
			this.AngleOfPath = 0;
			if (SideAngle > -2 && SideAngle < 2 && SideSpin <= -200)
			{
				this.AngleOfFace = 2;
			}
			if (SideSpin > -200 && SideSpin < 200)
			{
				this.AngleOfFace = 0;
			}
			if (SideAngle > -2 && SideAngle < 2 && SideSpin >= 200)
			{
				this.AngleOfFace = -2;
			}
		}
	}

	public BallTrajectory CalculateFlightTrajectory(Vector3 origin, double BallSpeed, double LaunchAngle, double BackSpin, double SideSpin, double SideAngle, bool Lefty, List<SimulationGreen> greens)
	{
		BallTrajectory ballTrajectory = new BallTrajectory();
		//this.Greens = greens;
		this.SetTerrainCoefficients(0, 0);
		this.Flight(BallSpeed, LaunchAngle, BackSpin, SideSpin, SideAngle, Lefty, (double)(origin.y * 3.28084f));
		
		Assert(this.TrajDt >= (long)2);
		Assert(this.TrajOffLine[0] == 0);
		Assert(this.TrajHeight[0] == 0);
		Assert(this.TrajDownRange[0] == 0);
		ballTrajectory.AddPoint(origin, 0f);  
		double physicsFPS = 1 / this.PhysicsFPS;
		for (int i = 1; (long)i < this.TrajDt; i++)
		{
			Vector3 vector3 = new Vector3((float)this.TrajOffLine[i], (float)this.TrajHeight[i], (float)this.TrajDownRange[i]) * 0.3048f;
			ballTrajectory.AddPoint(origin + vector3, (float)physicsFPS);
			//AppLog.Log(string.Format("m_launchPosition {0}", vector3));
		}
		ballTrajectory.firstBounceFrame = (int)this.FirstBounceFrame;
		return ballTrajectory;
	}

	private double CD(double RE, double Spin, double SR)
	{
		double rE;
		double num = 0.172266390960959;
		double num1 = 0.0522071044095622;
		double num2 = 3.2497205334972E-05;
		double num3 = -0.022730041114111;
		double num4 = 6.99288852467078E-10;
		double num5 = -1.07224417003079E-05;
		switch (0)
		{
			case 0:
				{
					num = 0.3095053115;
					num1 = -0.18414800399;
					num2 = 5.2632453E-05;
					num3 = 0.0598910311;
					num4 = -6.32361E-10;
					num5 = -1.8255E-05;
					rE = num + num1 * RE + num2 * Spin + num3 * (double)Mathf.Pow((float)RE, 2f) + num4 * (double)Mathf.Pow((float)Spin, 2f) + num5 * RE * Spin;
					if (rE > 0.5)
					{
						rE = 0.5;
					}
					break;
				}
			case 1:
				{
					rE = num + num1 * RE + num2 * Spin + num3 * (double)Mathf.Pow((float)RE, 2f) + num4 * (double)Mathf.Pow((float)Spin, 2f) + num5 * RE * Spin;
					if (rE > 0.5)
					{
						rE = 0.5;
					}
					rE *= 1.02;
					break;
				}
			case 2:
				{
					num = 0.172266390960959;
					num1 = 0.0522071044095622;
					num2 = 3.2497205334972E-05;
					num3 = -0.022730041114111;
					num4 = 6.99288852467078E-10;
					num5 = -1.07224417003079E-05;
					rE = num + num1 * RE + num2 * Spin + num3 * (double)Mathf.Pow((float)RE, 2f) + num4 * (double)Mathf.Pow((float)Spin, 2f) + num5 * RE * Spin;
					if (rE > 0.5)
					{
						rE = 0.5;
					}
					break;
				}
			case 3:
				{
					num = 0.2777731795;
					num1 = -0.0026661728;
					num2 = 1.5926112063E-05;
					num3 = -0.028693497538;
					num4 = 1.2336128919E-09;
					num5 = -3.810805E-06;
					rE = num + num1 * RE + num2 * Spin + num3 * (double)Mathf.Pow((float)RE, 2f) + num4 * (double)Mathf.Pow((float)Spin, 2f) + num5 * RE * Spin;
					if (rE > 0.5)
					{
						rE = 0.5;
					}
					break;
				}
			case 4:
			case 6:
			case 7:
				{
					rE = 0.56 * SR + 0.351;
					rE *= 0.9;
					break;
				}
			case 5:
				{
					rE = num + num1 * RE + num2 * Spin + num3 * (double)Mathf.Pow((float)RE, 2f) + num4 * (double)Mathf.Pow((float)Spin, 2f) + num5 * RE * Spin;
					if (rE > 0.5)
					{
						rE = 0.5;
					}
					rE *= 1.08;
					break;
				}
			default:
				{
					return -1;
				}
		}
		return rE;
	}

	private double CL(double RE, double Spin, double SR, bool isBounce)
	{
		double num = -0.277193964076082;
		double num1 = 0.51197284742553;
		double num2 = 0.0392236078626905;
		double num3 = -0.33318986737117;
		double num4 = 0.117747332136713;
		double num5 = 9.80654368085475E-05;
		double num6 = -1.24097559728608E-08;
		double num7 = 6.52083457782501E-13;
		if (BALL_TYPE.NormalTrajGolfBall != BALL_TYPE.NormalTrajGolfBall)
		{
			return -1;
		}
		num = -0.15358054204;
		num1 = 0.37112932871;
		num2 = -0.0819053099;
		num3 = -0.14063922122;
		num4 = 0.10447989711;
		num5 = 6.331757E-05;
		num6 = -6.73445E-09;
		num7 = 4.1060976E-13;
		double rE = num + num1 / RE + num2 / (double)Mathf.Pow((float)RE, 2f) + num3 / (double)Mathf.Pow((float)RE, 3f) + num4 / (double)Mathf.Pow((float)RE, 4f) + num5 * Spin + num6 * (double)Mathf.Pow((float)Spin, 2f) + num7 * (double)Mathf.Pow((float)Spin, 3f);
		rE = this.adjustCLForSpinRate(rE, Spin, isBounce);
		return rE;
	}

	private void DefaultValues()
	{
		this.GetFlightParameters();
		this.mAltitude = 0;
		this.mTemperature = 70;
		this.mHumidity = 50;
		this.mBarometer = 29.92;
		this.mWindSpeed = 0;
		this.mWindDirection = 0;
		this.mWind.x = 0f;
		this.mWind.y = 0f;
		this.mWind.z = 0f;
	}

	public void Flight(double BallSpeed, double LaunchAngle, double BackSpin, double SideSpin, double SideAngle, bool Lefty, double StartingHeight)
	{
		this.InitialHAG = StartingHeight;
		BallSpeed *= 5280;
		BallSpeed /= 3600;
		double num = 0;
		double num1 = num;
		this.Roll = num;
		double num2 = num1;
		num1 = num2;
		this.CDisp = num2;
		double num3 = num1;
		num1 = num3;
		this.Carry = num3;
		double num4 = num1;
		num1 = num4;
		this.TDisp = num4;
		this.Total = num1;
		this.BackSpin = BackSpin;
		this.LaunchAngle = LaunchAngle;
		LaunchAngle *= 0.0174532930056254;
		SideAngle *= 0.0174532930056254;
		if (BallSpeed < 5)
		{
			Debug.Log("Wouldve returned from flight here due to speed");
		}
		this.Area = 0.785398185253143 * (double)Mathf.Pow((float)this.BallDiam, 2f);
		this.Mass = this.BallWeight / 16 / 32.174048;
		double num5 = this.mBarometer * (double)Mathf.Pow((float)(1 - 6.8763E-06 * this.mAltitude), 5.2558f);
		this.Density = 2.31 + (0.144 - 0.0002 * (this.mTemperature - 50)) * (num5 - 30.12) / 1.8 - 0.005 * (this.mTemperature - 75) - (0.01 + 0.0005 * (this.mTemperature - 50)) * (this.mHumidity - 45) / 90;
		this.Density /= 1000;
		this.Visc = 232.9 * (double)Mathf.Pow(10f, -7f) * (double)Mathf.Pow((float)(this.mTemperature + 459.9), 1.5f) / (this.mTemperature + 675.9) / this.Density / 1000;
		for (int i = 0; i < 10000; i++)
		{
			this.TrajDownRange[i] = 0;
			this.TrajOffLine[i] = 0;
			this.TrajHeight[i] = 0;
			this.TrajAngle[i] = 0;
		}
		this.TrajDt = (long)1;
		this.IC[1] = 0;
		this.IC[2] = 0;
		this.IC[3] = 0;
		this.IC[4] = BallSpeed * (double)Mathf.Cos((float)LaunchAngle) * (double)Mathf.Cos((float)SideAngle);
		this.IC[5] = BallSpeed * (double)Mathf.Sin((float)LaunchAngle);
		this.IC[6] = BallSpeed * (double)Mathf.Cos((float)LaunchAngle) * (double)Mathf.Sin((float)SideAngle);
		this.IC[7] = 0;
		this.IC[8] = SideSpin * 2 * 3.14159274101257 / 60;
		this.IC[9] = BackSpin * 2 * 3.14159274101257 / 60;
		this.CalcTraj(0, false);
		this.FirstBounceFrame = this.TrajDt;
		this.DescentAngle = Mathf.Abs((float)this.DescendingAngle);
		this.Carry = this.RES[1] / 3;
		this.CDisp = this.RES[3] / 3;
		float single = 4.5f;
		for (float j = (float)this.IC[5]; this.TrajDt < (long)9999 && Mathf.Abs(j) > single; j = (float)this.IC[5])
		{
			for (long k = (long)1; k < (long)10; k += (long)1)
			{
				this.IC[checked(k)] = this.RES[checked(k)];
			}
			this.SetTerrainCoefficients(this.IC[1], this.IC[3]);
			this.IC[5] *= -this.terrainBounceCoeff;
			double c = -(this.IC[9] / 6.28318548202515) * this.BallCircumference * this.terrainSpinFrictionCoeff;
			double c1 = -(this.IC[8] / 6.28318548202515) * this.BallCircumference * this.terrainSpinFrictionCoeff;
			this.IC[4] += c;
			this.IC[6] += c1;
			double num6 = 1 - this.terrainSpinFrictionCoeff;
			this.IC[8] *= num6;
			this.IC[9] *= num6;
			this.CalcTraj(0, true);
		}
		for (long l = (long)1; l < (long)10; l += (long)1)
		{
			this.IC[checked(l)] = this.RES[checked(l)];
		}
		this.IC[5] = 0;
		this.IC[2] = -this.InitialHAG + this.BallDiam * 0.5;
		bool flag = false;
		while (this.TrajDt < (long)9999 && !flag)
		{
			flag = this.UpdateRoll();
		}
		this.TrajDownRange[checked(this.TrajDt)] = this.IC[1];
		this.TrajOffLine[checked(this.TrajDt)] = this.IC[3];
		this.TrajHeight[checked(this.TrajDt)] = this.IC[2];
		this.TrajDt += (long)1;
		this.Total = this.IC[1] / 3;
		this.TDisp = this.IC[3] / 3;
		this.Carry = (double)Mathf.Sqrt((float)(this.Carry * this.Carry + this.CDisp * this.CDisp));
		this.Total = (double)Mathf.Sqrt((float)(this.Total * this.Total + this.TDisp * this.TDisp));
		this.Roll = this.Total - this.Carry;
		this.MaxAlt = 0;
		for (int m = 0; (long)m < this.TrajDt - (long)1; m++)
		{
			if (this.MaxAlt < this.TrajHeight[m])
			{
				this.MaxAlt = this.TrajHeight[m];
			}
		}
		this.MaxAlt /= 3;
	}

	private void GetFlightParameters()
	{
	}

	public static CBallFlightManager GetInstance()
	{
		return CBallFlightManager.instance;
	}

	private double GetSpinRate()
	{
		return (double)Mathf.Sqrt((float)(this.IC[7] * this.IC[7] + this.IC[8] * this.IC[8] + this.IC[9] * this.IC[9]));
	}

	public void SetTerrainCoefficients(double x, double z)
	{
		//GrassManager.GrassType grassType = GrassManager.instance.getGrassType(x, z, this.Greens);
		//this.terrainBounceCoeff = GrassManager.instance.getBounceCoeff(grassType);
		//Debug.Log(string.Concat("terrain bounce is ", this.terrainBounceCoeff));
		//this.terrainSpinFrictionCoeff = GrassManager.instance.getSpinFrictionCoeff(grassType);
		//Debug.Log(string.Concat("terrain spin is ", this.terrainSpinFrictionCoeff));
		//this.frictionCoeff = (float)GrassManager.instance.getFrictionCoeff(grassType);
		//Debug.Log(string.Concat("terrain friction is ", this.frictionCoeff));
	}

	public void SetWeather(double Altitude, double Temperature, double Humidity, double Barometer, double WindSpeed, double WindDirection)
	{
		Debug.Log(string.Concat(new object[] { "Values being passed in: temp: ", Temperature, " Humidity: ", Humidity, " WindSpeed: ", WindSpeed }));
		this.mAltitude = Altitude;
		this.mTemperature = Temperature;
		this.mHumidity = Humidity;
		this.mBarometer = Barometer;
		this.mWindSpeed = WindSpeed;
		this.mWindDirection = WindDirection;
		this.mWind.z = (float)(WindSpeed * 88 / 60) * Mathf.Cos((float)(WindDirection * 3.14159274101257 / 180));
		this.mWind.y = 0f;
		this.mWind.x = (float)(WindSpeed * 88 / 60) * Mathf.Sin((float)(WindDirection * 3.14159274101257 / 180));
	}

	private void Start()
	{
		this.BallDiameterInches = 1.68; //球直径英寸
		this.BallDiam = this.BallDiameterInches / 12;
		this.BallCircumference = this.BallDiam * 3.14159274101257;
		this.BallWeight = 1.62;
		this.DefaultValues();
	}

	private void TrajEqu(double t, double[] Y, double[] F, bool isBounce)
	{
		Vector3 y = new Vector3();
		Vector3 vector3 = new Vector3();
		double num = 1;
		double num1 = 1;
		y.x = (float)Y[4] + this.mWind.x;
		y.y = (float)Y[5] + this.mWind.y;
		y.z = (float)Y[6] + this.mWind.z;
		vector3.x = (float)Y[7];
		vector3.y = (float)Y[8];
		vector3.z = (float)Y[9];
		this.Vmag = (double)y.magnitude;
		this.Wmag = (double)vector3.magnitude;
		if ((double)Mathf.Abs((float)this.Vmag) < 0.1 || (double)Mathf.Abs((float)this.Wmag) < 0.1)
		{
			F[1] = Y[4];
			F[2] = Y[5];
			F[3] = Y[6];
			F[4] = 0;
			F[5] = -32.174048;
			F[6] = 0;
			F[7] = 0;
			F[8] = 0;
			F[9] = 0;
			return;
		}
		double vmag = this.Vmag * this.BallDiam / this.Visc / 100000;
		double wmag = this.Wmag * this.BallDiam / 2 / this.Vmag;
		double wmag1 = this.Wmag / 6.28318548202515 * 60;
		double num2 = this.CD(vmag, wmag1, wmag);
		double num3 = this.CL(vmag, wmag1, wmag, isBounce);
		this.CLDisplay = num3;
		this.CDDisplay = num2;
		this.FDrag = num * num2 * this.Density * this.Area * this.Vmag * this.Vmag * 0.5;
		this.FLift = num1 * num3 * this.Density * this.Area * this.Vmag * this.Vmag * 0.5;
		y.Normalize();
		vector3.Normalize();
		Vector3 vector31 = -y;
		Vector3 vector32 = Vector3.Cross(-y, vector3);
		F[1] = Y[4];
		F[2] = Y[5];
		F[3] = Y[6];
		if (this.DoDrag)
		{
			F[4] = ((double)vector31.x * this.FDrag + (double)vector32.x * this.FLift) / this.Mass;
			F[5] = ((double)vector31.y * this.FDrag + (double)vector32.y * this.FLift) / this.Mass - 32.174048;
			F[6] = ((double)vector31.z * this.FDrag + (double)vector32.z * this.FLift) / this.Mass;
		}
		F[7] = -2E-05 * this.Wmag * this.Vmag * 2 / this.BallDiam * Y[7] / this.Wmag;
		F[8] = -2E-05 * this.Wmag * this.Vmag * 2 / this.BallDiam * Y[8] / this.Wmag;
		F[9] = -2E-05 * this.Wmag * this.Vmag * 2 / this.BallDiam * Y[9] / this.Wmag;
	}

	private bool UpdateRoll()
	{
		Vector3 vector3 = Vector3.up;
		Vector3 vector31 = new Vector3((float)this.IC[4], (float)this.IC[5], (float)this.IC[6]);
		float single = 32.174f;
		Vector3 mass = (single * Vector3.up) * (float)this.Mass;
		float single1 = Mathf.Abs(Vector3.Dot(mass, vector3 * -1f));
		float single2 = this.frictionCoeff * single1;
		float physicsFPS = 1f / (float)this.PhysicsFPS;
		float mass1 = single2 * (1f / (float)this.Mass) * physicsFPS;
		Vector3 vector32 = new Vector3((float)this.IC[4], (float)this.IC[5], (float)this.IC[6]);
		vector32.Normalize();
		vector32 *= -mass1;
		bool flag = false;
		if (vector31.sqrMagnitude >= vector32.sqrMagnitude)
		{
			vector31 += vector32;
			this.IC[4] = (double)vector31.x;
			this.IC[5] = 0;
			this.IC[6] = (double)vector31.z;
		}
		else
		{
			flag = true;
		}
		this.IC[1] = this.IC[1] + this.IC[4] * (double)physicsFPS;
		this.IC[2] = this.IC[2] + this.IC[5] * (double)physicsFPS;
		this.IC[3] = this.IC[3] + this.IC[6] * (double)physicsFPS;
		this.TrajSpin[checked(this.TrajDt)] = this.IC[9] * 60 / 2 / 3.14159274101257;
		this.TrajDownRange[checked(this.TrajDt)] = this.IC[1];
		this.TrajOffLine[checked(this.TrajDt)] = this.IC[3];
		this.TrajHeight[checked(this.TrajDt)] = this.IC[2];
		if (this.TrajDt <= (long)0)
		{
			this.TrajAngle[checked(this.TrajDt)] = 0;
		}
		else
		{
			this.TrajAngle[checked(this.TrajDt)] = (double)(57.2957764f * Mathf.Atan((float)((this.TrajHeight[checked(this.TrajDt)] - this.TrajHeight[checked((this.TrajDt - (long)1))]) / (this.TrajDownRange[checked(this.TrajDt)] - this.TrajDownRange[checked((this.TrajDt - (long)1))]))));
		}
		this.TrajDt += (long)1;
		return flag;
	}

	public static void Assert(bool condition)
	{
		if (!condition)
		{
			Debug.LogError("Assert fail.");
			Debug.Break();
		}
	}
	public enum BALL_TYPE
	{
		NormalTrajGolfBall,
		RangeGolfBall,
		HiTrajGolfBall,
		LoTrajGolfBall,
		Baseball,
		RubberBaseball,
		CorkSoftball,
		Softball,
		NoBall
	}
}