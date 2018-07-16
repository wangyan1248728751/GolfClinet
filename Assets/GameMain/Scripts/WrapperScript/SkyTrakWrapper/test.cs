using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.IO;
using SkyTrakWrapper.Interfaces;
using Security;

//namespace SkyTrakWrapper
//{
	public class test : Single<test>
	{
		//public static IntPtr num;

	public void testClick()
	{
		//Debug.Log("click!!!" + num);

		//CSimulationManager.instance.DestroyBall(CSimulationManager.instance.m_ball);

		//CSimulationManager.instance.ResetScene();
		//SecurityWrapperService.Instance.ArmBox();
		//CSimulationManager.instance.GetNewBall();
		//CSimulationManager.instance.UpdateHMPlayerHandedness(false);

		SkyTrakWrapper.STSWShotParamsType _ShotParams = new SkyTrakWrapper.STSWShotParamsType();

		//_ShotParams.speedParams.totalSpeed = 25.572872f;
		_ShotParams.speedParams.totalSpeed = UnityEngine.Random.Range(10.0f, 40.0f);
		_ShotParams.speedParams.launchAngle = 16.149473f;
		//_ShotParams.speedParams.horizontalAngle = 3.741888f;
		_ShotParams.speedParams.horizontalAngle = UnityEngine.Random.Range(-20.0f, 20.0f);
		_ShotParams.speedParams.startBallPositionStatus = 0;

		_ShotParams.spinParams.totalSpin = 3357.903564f;
		_ShotParams.spinParams.backSpin = -3357.560791f;
		//_ShotParams.spinParams.sideSpin = 47.968414f;
		_ShotParams.spinParams.sideSpin = UnityEngine.Random.Range(-5000.0f, 5000.0f);
		_ShotParams.spinParams.spinAxis = 0.818511f;
		_ShotParams.spinParams.measurementConfidence = 1.000000f;

		//_ShotParams.speedFlightParams.maxHeight = 9.239542f;
		//_ShotParams.speedFlightParams.flightDuration = 3.28f;


		SecurityWrapperService.Instance.MakeTestShot(_ShotParams.speedParams, _ShotParams.spinParams, _ShotParams.speedFlightParams);

	}

		
	}
//}

public abstract class Single<T> where T : new()
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
				instance = new T();
			return instance;
		}
	}
}
