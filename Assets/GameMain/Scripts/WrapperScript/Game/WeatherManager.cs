using System;
using UnityEngine;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
	public static WeatherManager instance;

	public double windSpeed;

	public float windDirection;

	public double humindity = 50;

	public double altitude;

	public double temperature = 70;

	public static double barometer;

	[SerializeField]
	private Text windSpeedLabel;

	[SerializeField]
	private Text windSpeedLabelNumericDisplay;

	[SerializeField]
	private GameObject windArrow;

	[SerializeField]
	private GameObject windArrowNumericDisplay;

	[SerializeField]
	private GameObject _windUI;

	private float windArrowdegrees;

	static WeatherManager()
	{
		WeatherManager.barometer = 29.92;
	}

	public WeatherManager()
	{
	}

	private void Awake()
	{
		if (WeatherManager.instance == null)
		{
			WeatherManager.instance = this;
		}
	}

	private float GetArrowDegrees(float windDirection)
	{
		return 90f + windDirection;
	}

	public void SetAltitude(float alt)
	{
		this.altitude = (double)alt;
		CBallFlightManager.GetInstance().SetWeather(this.altitude, this.temperature, this.humindity, WeatherManager.barometer, this.windSpeed, (double)this.windDirection);
	}

	public void SetHumidity(WeatherManager.Humidity humid)
	{
		string str = humid.ToString();
		str = str.Remove(0, 1);
		this.humindity = (double)int.Parse(str);
		CBallFlightManager.GetInstance().SetWeather(this.altitude, this.temperature, this.humindity, WeatherManager.barometer, this.windSpeed, (double)this.windDirection);
		this.SetWindUI();
	}

	public void SetTemp(float temp)
	{
		this.temperature = (double)temp;
		CBallFlightManager.GetInstance().SetWeather(this.altitude, this.temperature, this.humindity, WeatherManager.barometer, this.windSpeed, (double)this.windDirection);
	}

	public void SetWindDirection(WeatherManager.WindDirection direction)
	{
		switch (direction)
		{
			case WeatherManager.WindDirection.NONE:
				{
					this.windDirection = 270f;
					break;
				}
			case WeatherManager.WindDirection.RANDOM:
				{
					this.windDirection = (float)(45 * UnityEngine.Random.Range(0, 8));
					break;
				}
			case WeatherManager.WindDirection.NORTH:
				{
					this.windDirection = 90f;
					break;
				}
			case WeatherManager.WindDirection.NORTH_EAST:
				{
					this.windDirection = 45f;
					break;
				}
			case WeatherManager.WindDirection.EAST:
				{
					this.windDirection = 0f;
					break;
				}
			case WeatherManager.WindDirection.SOUTH_EAST:
				{
					this.windDirection = 315f;
					break;
				}
			case WeatherManager.WindDirection.SOUTH:
				{
					this.windDirection = 270f;
					break;
				}
			case WeatherManager.WindDirection.SOUTH_WEST:
				{
					this.windDirection = 225f;
					break;
				}
			case WeatherManager.WindDirection.WEST:
				{
					this.windDirection = 180f;
					break;
				}
			case WeatherManager.WindDirection.NORTH_WEST:
				{
					this.windDirection = 135f;
					break;
				}
		}
		this.windArrowdegrees = this.GetArrowDegrees(this.windDirection);
		CBallFlightManager.GetInstance().SetWeather(this.altitude, this.temperature, this.humindity, WeatherManager.barometer, this.windSpeed, (double)this.windDirection);
		this.SetWindUI();
	}

	public void SetWindSpeed(WeatherManager.WindSpeed speed)
	{
		switch (speed)
		{
			case WeatherManager.WindSpeed.Speed_0:
				{
					this.windSpeed = 0;
					break;
				}
			case WeatherManager.WindSpeed.Speed_0_5:
				{
					this.windSpeed = (double)UnityEngine.Random.Range(0, 5);
					break;
				}
			case WeatherManager.WindSpeed.Speed_5_10:
				{
					this.windSpeed = (double)UnityEngine.Random.Range(5, 10);
					break;
				}
			case WeatherManager.WindSpeed.Speed_10_15:
				{
					this.windSpeed = (double)UnityEngine.Random.Range(10, 15);
					break;
				}
			case WeatherManager.WindSpeed.Speed_15_20:
				{
					this.windSpeed = (double)UnityEngine.Random.Range(15, 20);
					break;
				}
			case WeatherManager.WindSpeed.Speed_20_25:
				{
					this.windSpeed = (double)UnityEngine.Random.Range(20, 25);
					break;
				}
		}
		CBallFlightManager.GetInstance().SetWeather(this.altitude, this.temperature, this.humindity, WeatherManager.barometer, this.windSpeed, (double)this.windDirection);
		this.SetWindUI();
	}

	private void SetWindUI()
	{
		//this.windSpeedLabel.text = string.Format("WIND {0} {1}", this.windSpeed.ToString(), UnitsConverter.CurrentSpeedUnits);
		//this.windSpeedLabelNumericDisplay.text = string.Format("WIND {0} {1}", this.windSpeed.ToString(), UnitsConverter.CurrentSpeedUnits);
		//this.windArrow.transform.localEulerAngles = new Vector3(0f, 0f, this.windArrowdegrees);
		//this.windArrowNumericDisplay.transform.localEulerAngles = new Vector3(0f, 0f, this.windArrowdegrees);
	}

	public void ShowUI(bool show)
	{
		this._windUI.SetActive(show);
	}

	public enum Humidity
	{
		p10,
		p20,
		p30,
		p40,
		p50,
		p60,
		p70,
		p80,
		p90,
		p100
	}

	public enum WindDirection
	{
		NONE,
		RANDOM,
		NORTH,
		NORTH_EAST,
		EAST,
		SOUTH_EAST,
		SOUTH,
		SOUTH_WEST,
		WEST,
		NORTH_WEST
	}

	public enum WindSpeed
	{
		Speed_0,
		Speed_0_5,
		Speed_5_10,
		Speed_10_15,
		Speed_15_20,
		Speed_20_25
	}
}