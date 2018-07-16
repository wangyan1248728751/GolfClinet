using System;
using UnityEngine;

public abstract class IBehaviorInterface
{
	protected GameObject m_target;

	protected Transform m_cameraTransform;

	protected Vector3 m_targetOffset;

	protected float m_followAngle;

	protected float m_followRadius;

	protected float m_followHeight;

	protected float m_followRate;

	protected float m_followHeightRate;

	protected float m_followRotationRate;

	protected float m_transitionAcceleration;

	protected float m_lookAtRate;

	protected bool m_lookAtTarget;

	public IBehaviorInterface(Transform cameraTransform)
	{
		this.SetCameraTransform(cameraTransform);
	}

	public virtual GameObject GetTarget()
	{
		return this.m_target;
	}

	public abstract void Init(GameObject target);

	public virtual void LookAtTarget(bool status)
	{
		this.m_lookAtTarget = status;
	}

	public void ResetAngle()
	{
		this.m_cameraTransform.rotation = Quaternion.identity;
	}

	public virtual void SetCameraTransform(Transform cameraTransform)
	{
		if (cameraTransform != null)
		{
			this.m_cameraTransform = cameraTransform;
		}
	}

	public void SetFollowAngle(float angle)
	{
		this.m_followAngle = angle;
	}

	public void SetFollowHeight(float height)
	{
		this.m_followHeight = height;
	}

	public void SetFollowHeightRate(float rate)
	{
		this.m_followHeightRate = rate;
	}

	public void SetFollowLookAtRotationRate(float rate)
	{
		this.m_lookAtRate = rate;
	}

	public void SetFollowRadius(float radius)
	{
		this.m_followRadius = radius;
	}

	public void SetFollowRate(float rate)
	{
		this.m_followRate = rate;
	}

	public void SetFollowRotationRate(float rate)
	{
		this.m_followRotationRate = rate;
	}

	public void SetFollowTransitionAcceleration(float acceleration)
	{
		this.m_transitionAcceleration = acceleration;
	}

	public virtual void SetOffset(Vector3 posOffset, Vector3 rotationOffset)
	{
	}

	public virtual void SetTarget(GameObject target)
	{
		if (target != null)
		{
			this.m_target = target;
		}
	}

	public abstract void Update();
}