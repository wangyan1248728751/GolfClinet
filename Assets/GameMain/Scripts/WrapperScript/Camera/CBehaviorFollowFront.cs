using System;
using UnityEngine;

public class CBehaviorFollowFront : IBehaviorInterface
{
	private bool m_continuallyOrbit;

	private float m_orbitRate;

	public CBehaviorFollowFront(Transform cameraTransform) : base(cameraTransform)
	{
	}

	public override void Init(GameObject target)
	{
		if (target != null)
		{
			this.m_target = target;
		}
	}

	public void SetOrbitRate(float rate)
	{
		this.m_orbitRate = rate;
	}

	public void ShouldContinuallyOrbit(bool shouldOrbit)
	{
		this.m_continuallyOrbit = shouldOrbit;
	}

	public override void Update()
	{
		if (this.m_target != null)
		{
			CBehaviorFollowFront mTransitionAcceleration = this;
			mTransitionAcceleration.m_transitionAcceleration = mTransitionAcceleration.m_transitionAcceleration + Time.deltaTime * 0.2f;
			Mathf.Clamp(this.m_transitionAcceleration, 0f, 1f);
			Vector3 mTarget = this.m_target.transform.position - this.m_cameraTransform.position;
			float mFollowRadius = mTarget.magnitude - this.m_followRadius;
			mTarget.Normalize();
			Vector3 mFollowRate = (((mTarget + (mTarget * mFollowRadius)) * this.m_followRate) * this.m_transitionAcceleration) * Time.smoothDeltaTime;
			Vector3 vector3 = this.m_target.transform.position;
			float mFollowHeight = vector3.y + this.m_followHeight;
			float single = mFollowRate.y;
			Vector3 mCameraTransform = this.m_cameraTransform.position;
			mFollowRate.y = single + (mFollowHeight - mCameraTransform.y) * this.m_followHeightRate * this.m_transitionAcceleration * Time.smoothDeltaTime;
			Transform transforms = this.m_cameraTransform;
			transforms.position = transforms.position + mFollowRate;
			float mCameraTransform1 = this.m_cameraTransform.rotation.eulerAngles.y;
			float mFollowAngle = this.m_followAngle - mCameraTransform1;
			if (this.m_continuallyOrbit)
			{
				this.m_cameraTransform.RotateAround(this.m_target.transform.position, Vector3.up, this.m_orbitRate * Time.smoothDeltaTime);
			}
			else
			{
				this.m_cameraTransform.RotateAround(this.m_target.transform.position, Vector3.up, mFollowAngle * this.m_followRotationRate * Time.smoothDeltaTime);
			}
			Quaternion quaternion = Quaternion.LookRotation(this.m_target.transform.position - this.m_cameraTransform.position);
			this.m_cameraTransform.rotation = Quaternion.Slerp(this.m_cameraTransform.rotation, quaternion, Time.deltaTime * this.m_transitionAcceleration * this.m_lookAtRate * 5f);
		}
	}
}