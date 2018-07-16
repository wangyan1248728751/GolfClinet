using System;
using UnityEngine;

public class CGolfCamera : MonoBehaviour
{
	public GameObject m_target;

	public GameObject m_pinTarget;

	public float m_transitionAcceleration = 0.5f;

	public float m_lookAtRate = 8f;

	public float m_followBehindCameraDelay = 0.75f;

	public float m_followBehindAngle = 40f;

	public float m_followBehindRadius = 5f;

	public float m_followBehindHeight = 10f;

	public float m_followBehindRate = 8f;

	public float m_followBehindHeightRate = 2f;

	public float m_followBehindRotationRate = 4f;

	public float m_followFrontAngle = 145f;

	public float m_followFrontRadius = 10f;

	public float m_followFrontHeight = 5f;

	public float m_followFrontRate = 8f;

	public float m_followFrontHeightRate = 2f;

	public float m_followFrontRotationRate = 4f;

	public bool m_continuallyOrbit;

	public float m_orbitRate = 2f;

	public float m_followFrontAnglePin = 145f;

	public float m_followFrontRadiusPin = 10f;

	public float m_followFrontHeightPin = 5f;

	public float m_followFrontRatePin = 8f;

	public float m_followFrontHeightRatePin = 2f;

	public float m_followFrontRotationRatePin = 4f;

	public bool m_continuallyOrbitPin;

	public float m_orbitRatePin = 2f;

	private IBehaviorInterface m_currentBehavior;

	private CGolfCamera.CAMERA_BEHAVIOR m_behaviorType;

	private const float PosOffsetMin = 0f;

	private const float PosOffsetMax = -0.3f;

	private const float RotationOffsetMin = 35f;

	private const float RotationOffsetMax = -35f;

	private float _cameraOffsetValue;

	private CBehaviorStatic m_behaviorStatic;

	private CBehaviorStaticCutPin m_behaviorStaticCutPin;

	private CBehaviorFollowBehind m_behaviorFollowBehind;

	private CBehaviorFollowFront m_behaviorFollowFront;

	private CBehavior45Degrees m_behavior45Degrees;

	private CBehavior45DegreesZoom m_behavior45DegreesZoom;

	private CBehaviorStaticDownrange m_behaviorStaticDownrange;

	private CBehaviorFollowFrontPin m_behaviorFollowFrontPin;

	private CBehaviorDynamic m_behaviorDynamic;

	private bool _isUserOverride;

	private CGolfCamera.CAMERA_BEHAVIOR overrideBehavior;

	public float CameraOffset
	{
		get
		{
			return this._cameraOffsetValue;
		}
	}

	public bool isUserOverride
	{
		get
		{
			return this._isUserOverride;
		}
	}

	public CGolfCamera()
	{
	}

	private Vector3 GetCameraPosOffset()
	{
		Vector3 cameraRotationOffset = this.GetCameraRotationOffset();
		float single = Mathf.Abs(-35f);
		float single1 = 90f / single * Mathf.Abs(cameraRotationOffset.y) * 3.14159274f / 180f;
		float single2 = Mathf.Abs(0.5f - this._cameraOffsetValue) * -0.3f * 2f * Mathf.Sin(single1);
		return new Vector3(0f, 0f, single2);
	}

	private Vector3 GetCameraRotationOffset()
	{
		float single = 35f - this._cameraOffsetValue * 70f;
		return new Vector3(0f, single, 0f);
	}

	public CGolfCamera.CAMERA_BEHAVIOR GetCurrentBehavior()
	{
		return this.m_behaviorType;
	}

	private void LateUpdate()
	{
		if (this.m_currentBehavior != null)
		{
			this.m_currentBehavior.Update();
		}
	}

	public void LookAtTarget(bool status)
	{
		if (this.m_currentBehavior != null)
		{
			this.m_currentBehavior.LookAtTarget(status);
		}
	}

	public void RemoveOverride()
	{
		this._isUserOverride = false;
	}

	private void SetBehaviorDynamic(bool resetAnchor)
	{
		if (resetAnchor)
		{
			this.m_behaviorDynamic.ResetCameraAnchorPosition(base.transform);
		}
		if (this.m_behaviorDynamic != null)
		{
			this.m_currentBehavior = this.m_behaviorDynamic;
			this.m_currentBehavior.Init(this.m_target);
		}
		this.m_behaviorType = CGolfCamera.CAMERA_BEHAVIOR.CAMERA_LAND_DYNAMIC;
	}

	private void SetBehaviorStaticCutBehindBall(bool resetAnchor)
	{
		if (resetAnchor)
		{
			this.m_behaviorStatic.ResetCameraAnchorPosition(base.transform);
		}
		if (this.m_behaviorStatic != null)
		{
			this.m_currentBehavior = this.m_behaviorStaticCutPin;
			((CBehaviorStaticCutPin)this.m_currentBehavior).SetSecondTarget(this.m_pinTarget);
			this.m_currentBehavior.Init(this.m_target);
		}
		this.m_behaviorType = CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_CUT_BEHIND_BALL;
	}

	public void SetBehaviorStaticCutBehindBall(bool resetAnchor, Vector3 initialPosition)
	{
		if (resetAnchor)
		{
			this.m_behaviorStatic.ResetCameraAnchorPosition(base.transform);
		}
		if (this.m_behaviorStatic != null)
		{
			this.m_behaviorStaticCutPin.SetSecondTarget(this.m_pinTarget);
			this.m_behaviorStaticCutPin.SetInitialPosition(initialPosition);
			this.m_currentBehavior = this.m_behaviorStaticCutPin;
			this.m_currentBehavior.Init(this.m_target);
		}
		this.m_behaviorType = CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_CUT_BEHIND_BALL;
	}

	private void SetBehaviorStaticUserPerspective(bool resetAnchor)
	{
		if (resetAnchor)
		{
			this.m_behaviorStatic.ResetCameraAnchorPosition(base.transform);
		}
		if (this.m_behaviorStatic != null)
		{
			this.m_currentBehavior = this.m_behaviorStatic;
			this.m_currentBehavior.Init(this.m_target);
		}
		this.m_behaviorType = CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE;
	}

	public void SetCameraBehavior(CGolfCamera.CAMERA_BEHAVIOR newBehavior)
	{
		if (this._isUserOverride)
		{
			newBehavior = this.overrideBehavior;
		}
		if (newBehavior != this.m_behaviorType)
		{
			switch (newBehavior)
			{
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_NO_BEHAVIOR:
					{
						this.m_behaviorType = newBehavior;
						break;
					}
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE:
					{
						this.SetBehaviorStaticUserPerspective(false);
						break;
					}
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_CUT_BEHIND_BALL:
					{
						this.SetBehaviorStaticCutBehindBall(false);
						break;
					}
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_FOLLOW_BEHIND_BALL:
					{
						this.m_currentBehavior = this.m_behaviorFollowBehind;
						this.m_currentBehavior.ResetAngle();
						this.m_currentBehavior.SetFollowAngle(this.m_followBehindAngle);
						this.m_currentBehavior.SetFollowRadius(this.m_followBehindRadius);
						this.m_currentBehavior.SetFollowHeight(this.m_followBehindHeight);
						this.m_currentBehavior.SetFollowRate(this.m_followBehindRate);
						this.m_currentBehavior.SetFollowHeightRate(this.m_followBehindHeightRate);
						this.m_currentBehavior.SetFollowRotationRate(this.m_followBehindRotationRate);
						this.m_currentBehavior.SetFollowTransitionAcceleration(this.m_transitionAcceleration);
						this.m_currentBehavior.SetFollowLookAtRotationRate(this.m_lookAtRate);
						this.m_currentBehavior.Init(this.m_target);
						this.m_behaviorType = newBehavior;
						break;
					}
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_FOLLOW_FRONT_BALL:
					{
						this.m_currentBehavior = this.m_behaviorFollowFront;
						this.m_currentBehavior.SetFollowAngle(this.m_followFrontAngle);
						this.m_currentBehavior.SetFollowRadius(this.m_followFrontRadius);
						this.m_currentBehavior.SetFollowHeight(this.m_followFrontHeight);
						this.m_currentBehavior.SetFollowRate(this.m_followFrontRate);
						this.m_currentBehavior.SetFollowHeightRate(this.m_followFrontHeightRate);
						this.m_currentBehavior.SetFollowRotationRate(this.m_followFrontRotationRate);
						this.m_currentBehavior.SetFollowTransitionAcceleration(this.m_transitionAcceleration);
						this.m_currentBehavior.SetFollowLookAtRotationRate(this.m_lookAtRate);
						((CBehaviorFollowFront)this.m_currentBehavior).ShouldContinuallyOrbit(this.m_continuallyOrbit);
						((CBehaviorFollowFront)this.m_currentBehavior).SetOrbitRate(this.m_orbitRate);
						this.m_currentBehavior.Init(this.m_target);
						this.m_behaviorType = newBehavior;
						break;
					}
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_FOLLOW_FRONT_BALL_PIN:
					{
						this.m_currentBehavior = this.m_behaviorFollowFrontPin;
						this.m_currentBehavior.SetFollowAngle(this.m_followFrontAnglePin);
						this.m_currentBehavior.SetFollowRadius(this.m_followFrontRadiusPin);
						this.m_currentBehavior.SetFollowHeight(this.m_followFrontHeightPin);
						this.m_currentBehavior.SetFollowRate(this.m_followFrontRatePin);
						this.m_currentBehavior.SetFollowHeightRate(this.m_followFrontHeightRatePin);
						this.m_currentBehavior.SetFollowRotationRate(this.m_followFrontRotationRatePin);
						this.m_currentBehavior.SetFollowTransitionAcceleration(this.m_transitionAcceleration);
						this.m_currentBehavior.SetFollowLookAtRotationRate(this.m_lookAtRate);
						((CBehaviorFollowFrontPin)this.m_currentBehavior).ShouldContinuallyOrbit(this.m_continuallyOrbitPin);
						((CBehaviorFollowFrontPin)this.m_currentBehavior).SetOrbitRate(this.m_orbitRate);
						((CBehaviorFollowFrontPin)this.m_currentBehavior).SetSecondTarget(this.m_pinTarget);
						this.m_currentBehavior.Init(this.m_target);
						this.m_behaviorType = newBehavior;
						break;
					}
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_45_DEGREES:
					{
						this.m_currentBehavior = this.m_behavior45Degrees;
						this.m_currentBehavior.Init(this.m_target);
						this.m_behaviorType = newBehavior;
						break;
					}
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_45_DEGREES_ZOOM:
					{
						this.m_currentBehavior = this.m_behavior45DegreesZoom;
						this.m_currentBehavior.Init(this.m_target);
						this.m_behaviorType = newBehavior;
						break;
					}
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_DOWNRANGE:
					{
						this.m_currentBehavior = this.m_behaviorStaticDownrange;
						this.m_currentBehavior.Init(this.m_target);
						this.m_behaviorType = newBehavior;
						break;
					}
				case CGolfCamera.CAMERA_BEHAVIOR.CAMERA_LAND_DYNAMIC:
					{
						this.SetBehaviorDynamic(false);
						break;
					}
			}
		}
	}

	public void SetOffset(float offsetVal)
	{
		if (this.m_currentBehavior != null)
		{
			this._cameraOffsetValue = offsetVal;
			this.m_currentBehavior.SetOffset(this.GetCameraPosOffset(), this.GetCameraRotationOffset());
		}
	}

	public void SetTarget(GameObject target)
	{
		if (target != null)
		{
			this.m_target = target;
		}
		if (this.m_currentBehavior != null)
		{
			this.m_currentBehavior.Init(this.m_target);
		}
	}

	private void Start()
	{
		if (this.m_pinTarget == null)
		{
			this.m_pinTarget = GameObject.FindGameObjectWithTag("FalgGreen");
		}
		this.m_behaviorType = CGolfCamera.CAMERA_BEHAVIOR.CAMERA_NO_BEHAVIOR;
		this.m_behaviorStatic = new CBehaviorStatic(base.transform);
		this.m_behaviorStaticCutPin = new CBehaviorStaticCutPin(base.transform);
		this.m_behaviorFollowBehind = new CBehaviorFollowBehind(base.transform);
		this.m_behaviorFollowFront = new CBehaviorFollowFront(base.transform);
		this.m_behavior45Degrees = new CBehavior45Degrees(base.transform);
		this.m_behavior45DegreesZoom = new CBehavior45DegreesZoom(base.transform);
		this.m_behaviorStaticDownrange = new CBehaviorStaticDownrange(base.transform);
		this.m_behaviorFollowFrontPin = new CBehaviorFollowFrontPin(base.transform);
		this.m_behaviorDynamic = new CBehaviorDynamic(base.transform);
		this.SetCameraBehavior(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_STATIC_USER_PERSPECTIVE);
	}

	public void SwapToFollowBehind()
	{
		this.SetCameraBehavior(CGolfCamera.CAMERA_BEHAVIOR.CAMERA_FOLLOW_BEHIND_BALL);
	}

	public void UserOverride(CGolfCamera.CAMERA_BEHAVIOR newBehavior)
	{
		this._isUserOverride = true;
		this.overrideBehavior = newBehavior;
		this.SetCameraBehavior(newBehavior);
	}

	public enum CAMERA_BEHAVIOR
	{
		CAMERA_NO_BEHAVIOR,
		CAMERA_STATIC_USER_PERSPECTIVE,
		CAMERA_STATIC_CUT_BEHIND_BALL,
		CAMERA_FOLLOW_BEHIND_BALL,
		CAMERA_FOLLOW_FRONT_BALL,
		CAMERA_FOLLOW_FRONT_BALL_PIN,
		CAMERA_45_DEGREES,
		CAMERA_45_DEGREES_ZOOM,
		CAMERA_STATIC_DOWNRANGE,
		CAMERA_LAND_DYNAMIC
	}
}