using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CBall : MonoBehaviour
{
	public Color32 tracerColor = Color.white;

	public ParticleSystem tx_luodi;
	public ParticleSystem tx_luoshui;

	//[SerializeField]
	//private GameObject _ballMarkerPrefab;

	//[SerializeField]
	//private Transform _ballMarkersParent;

	//private BallMarkerUGUI _ballMarker;

	private CBall.MotionType m_motionType;

	private BallTrajectory m_trajectory;

	private TrailRenderer ballTrail;

	private Material mat;

	private Material mat2;

	private TracerManager _tracerManager
	{
		get
		{
			return null;
			//return CGameManager.instance.UiHolder.TracerManager;
		}
	}

	public bool hasBallLanded
	{
		get
		{
			if (this.m_trajectory == null)
			{
				return false;
			}
			return this.m_trajectory.HasBounced();
		}
	}

	public bool isFlightComplete
	{
		get
		{
			if (this.m_trajectory == null)
			{
				return false;
			}
			return this.m_trajectory.IsAtEnd();
		}
	}

	public bool isInFlight
	{
		get
		{
			if (this.m_trajectory != null && this.m_trajectory.m_indexHint != 0 && this.m_trajectory.m_indexHint >= this.m_trajectory.firstBounceFrame)
			{
				return true;
			}
			return false;
		}
	}

	public bool isInMotion
	{
		get
		{
			if (this.m_motionType != CBall.MotionType.Fixed)
			{
				return true;
			}
			return false;
		}
	}

	private void Awake()
	{
		this.m_motionType = CBall.MotionType.Fixed;
		this.m_trajectory = null;
		this.ballTrail = base.GetComponent<TrailRenderer>();
		//this.mat = this.ballTrail.material;
		//this.mat2 = Resources.Load("Tracers/Tracer_MAT_3", typeof(Material)) as Material;
		//this.SetTracerInvis(true);
		Transform vector3 = base.gameObject.transform;
		float single = base.gameObject.transform.position.x;
		Vector3 vector31 = base.gameObject.transform.position;
		vector3.position = new Vector3(single, vector31.y, 0f);
	}

	public void DestroySelf()
	{
		this._tracerManager.RemoveBallWithTracer(this);
		CGameManager.instance.RequestDeleteBall(this);
		Destroy(base.gameObject);
	}

	public void DestroyTracerFromBall()
	{
		if (this.ballTrail != null)
		{
			this.mat = null;
			this.mat2 = null;
			Destroy(this.ballTrail);
			this.ballTrail = null;
		}
	}

	public BallTrajectory GetTrajectory()
	{
		return this.m_trajectory;
	}

	public void HideMarker()
	{
		//if (this._ballMarker != null)
		//{
		//	this._ballMarker.gameObject.SetActive(false);
		//}
	}

	public void HideSelf()
	{
		this._tracerManager.RemoveBallWithTracer(this);
		this.DestroyTracerFromBall();
		base.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		//if (this._ballMarker != null)
		//{
		//	Destroy(this._ballMarker.gameObject);
		//}
	}

	public void PurgeTrajectoryBuffer()
	{
		this.m_motionType = CBall.MotionType.Fixed;
		this.m_trajectory.m_points.Clear();
	}

	//public void SetNewTracerMaterial(TracerMaterial newMat)
	//{
	//	if (newMat != null)
	//	{
	//		this.mat = newMat.material;
	//		this.tracerColor = newMat.color;
	//	}
	//}

	private void SetTracerInvis(bool invis)
	{
		if (invis)
		{
			this.ballTrail.material = this.mat2;
			return;
		}
		this.ballTrail.material = this.mat;
	}

	public void SetTrailActive(bool active)
	{
		if (this.ballTrail != null)
		{
			this.SetTracerInvis(!active);
		}
	}

	public void SetTrailScales(float newSizeStart, float newSizeEnd)
	{
		if (this.ballTrail != null)
		{
			AnimationCurve animationCurve = new AnimationCurve();
			animationCurve.AddKey(0f, newSizeStart);
			animationCurve.AddKey(1f, newSizeEnd);
			this.ballTrail.widthCurve = animationCurve;
		}
	}

	public void SetTrajectory(BallTrajectory trajectory)
	{
		this.m_trajectory = trajectory;
		Transform vector3 = base.transform;
		vector3.position = vector3.position + new Vector3(0f, 0f, -0.1f);
		base.StartCoroutine(this.WaitFor(0.1f, () =>
		{
			this.m_trajectory.ResetTime();
			base.transform.position = this.m_trajectory.GetCurrentPosition();
			this.m_motionType = CBall.MotionType.Trajectory;
		}));
	}

	public void SetupMarker(int playerNumber)
	{
		//if (this._ballMarker == null)
		//{
		//	GameObject gameObject = Object.Instantiate<GameObject>(this._ballMarkerPrefab, this._ballMarkersParent);
		//	this._ballMarker = gameObject.GetComponent<BallMarkerUGUI>();
		//	this._ballMarker.SetUp(base.transform, playerNumber);
		//	gameObject.SetActive(false);
		//}
	}

	public void ShowMarker()
	{
		//if (this._ballMarker != null)
		//{
		//	this._ballMarker.gameObject.SetActive(true);
		//}
	}

	private void Update()
	{
		if (this.m_motionType == CBall.MotionType.Trajectory && !this.isFlightComplete)
		{
			this.m_trajectory.AdvanceTime(Time.deltaTime);
			Vector3 currentPosition = this.m_trajectory.GetCurrentPosition();
			base.transform.position = currentPosition;
		}
	}

	private IEnumerator WaitFor(float time, Action action)
	{
		yield return new WaitForSeconds(time);
		action.Invoke();
	}

	private enum MotionType
	{
		Fixed,
		Trajectory,
		Dynamic
	}
}