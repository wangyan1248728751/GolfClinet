using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class CFlightDataStorage : MonoBehaviour
{
	private static CFlightDataStorage _instance;

	private List<TFlightData> recentFlights = new List<TFlightData>();

	public bool hasLanded;

	private bool isLastFlightOk;

	public static CFlightDataStorage instance
	{
		get
		{
			return CFlightDataStorage._instance;
		}
	}

	public bool IsLastFlightOk
	{
		get
		{
			return this.isLastFlightOk;
		}
	}

	public TFlightData ReplayShotData
	{
		get;
		set;
	}

	static CFlightDataStorage()
	{
	}

	public CFlightDataStorage()
	{
	}

	public int AddFlight(TFlightData flight)
	{
		this.PruneStorage();
		flight.SetFlagDataIsGood();
		this.recentFlights.Add(flight);
		this.isLastFlightOk = true;
		return this.recentFlights.Count - 1;
	}

	private void Awake()
	{
		if (CFlightDataStorage._instance == null)
		{
			CFlightDataStorage._instance = this;
		}
	}

	public void ClearAllData()
	{
		this.recentFlights.Clear();
	}

	public void DeleteFlight(string timeOfHit)
	{
		TFlightData item = null;
		int num = 0;
		while (num < this.recentFlights.Count)
		{
			if (this.recentFlights[num].timeOfHit != timeOfHit)
			{
				num++;
			}
			else
			{
				this.recentFlights[num].ballTransform.GetComponent<CBall>().HideSelf();
				item = this.recentFlights[num];
				if (this.OnFlightWasDeleted != null)
				{
					this.OnFlightWasDeleted(item);
				}
				this.recentFlights[num].MarkDataForDelete();
				break;
			}
		}
		this.PruneStorage();
	}

	public void DeleteFlight(TFlightData flight)
	{
		if (flight == null)
		{
			return;
		}
		this.isLastFlightOk = false;
		this.DeleteFlight(flight.timeOfHit);
	}

	public float GetAVGTotalCarry()
	{
		if (this.recentFlights.Count == 0)
		{
			return 0f;
		}
		int num = 0;
		int num1 = 0;
		string clubIDFromName = Club.GetClubIDFromName(CGameManager.instance.GetCurrentClubName());
		foreach (TFlightData recentFlight in this.recentFlights)
		{
			if (recentFlight.clubTypeID != clubIDFromName)
			{
				continue;
			}
			float distance = UnitsConverter.Instance.GetDistance(recentFlight.carry);
			num += Mathf.RoundToInt(distance);
			num1++;
		}
		return (num1 != 0 ? (float)num / (float)num1 : 0f);
	}

	public float GetAVGTotalDistance()
	{
		if (this.recentFlights.Count == 0)
		{
			return 0f;
		}
		int num = 0;
		int num1 = 0;
		int num2 = 0;
		string clubIDFromName = Club.GetClubIDFromName(CGameManager.instance.GetCurrentClubName());
		foreach (TFlightData recentFlight in this.recentFlights)
		{
			if (recentFlight.clubTypeID != clubIDFromName)
			{
				continue;
			}
			float distance = UnitsConverter.Instance.GetDistance(recentFlight.carry);
			float single = UnitsConverter.Instance.GetDistance(recentFlight.roll);
			num += Mathf.RoundToInt(distance);
			num1 += Mathf.RoundToInt(single);
			num2++;
		}
		int num3 = (num2 != 0 ? num / num2 : 0);
		return (float)(num3 + (num2 != 0 ? num1 / num2 : 0));
	}

	public List<TFlightData> GetFlights()
	{
		return this.recentFlights;
	}

	public TFlightData GetMostRecentFlight()
	{
		if (CGameManager.instance != null && CGameManager.instance.IsReplay)
		{
			return this.ReplayShotData;
		}
		if (this.recentFlights.Count == 0)
		{
			return null;
		}
		return this.recentFlights[this.recentFlights.Count - 1];
	}

	private void PruneStorage()
	{
		bool flag = true;
		Label0:
		while (flag)
		{
			if (this.recentFlights == null || this.recentFlights.Count == 0)
			{
				flag = false;
			}
			else
			{
				int num = 0;
				while (num < this.recentFlights.Count)
				{
					if (!this.recentFlights[num].isValid)
					{
						CGameManager.instance.RequestDeleteBall(this.recentFlights[num].ballTransform.GetComponent<CBall>());
						SessionAndActivityManager.Instance.DeleteShotFromSession(this.recentFlights[num].esnShotId);
						this.recentFlights.RemoveAt(num);
						goto Label0;
					}
					else if (num + 1 != this.recentFlights.Count)
					{
						num++;
					}
					else
					{
						flag = false;
						goto Label0;
					}
				}
			}
		}
	}

	public event Action<TFlightData> OnFlightWasDeleted;
}