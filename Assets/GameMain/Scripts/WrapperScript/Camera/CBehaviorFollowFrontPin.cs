using System;
using UnityEngine;

public class CBehaviorFollowFrontPin : IBehaviorInterface
{
	private GameObject m_SecondTarget;

	public CBehaviorFollowFrontPin(Transform cameraTransform) : base(cameraTransform)
	{
	}

	public override void Init(GameObject target)
	{
		if (target != null && this.m_SecondTarget != null)
		{
			this.m_target = target;
		}
	}

	public void SetOffsetVector(Vector3 Offsetv)
	{
	}

	public void SetOrbitRate(float rate)
	{
	}

	public void SetSecondTarget(GameObject target)
	{
		this.m_SecondTarget = target;
	}

	public void ShouldContinuallyOrbit(bool shouldOrbit)
	{
	}

	public override void Update()
	{
		if (this.m_target != null && this.m_SecondTarget != null)
		{
			CBehaviorFollowFrontPin mTransitionAcceleration = this;
			mTransitionAcceleration.m_transitionAcceleration = mTransitionAcceleration.m_transitionAcceleration + Time.deltaTime * 0.2f;
			Mathf.Clamp(this.m_transitionAcceleration, 0f, 1f);
			Vector3 mTarget = this.m_target.transform.position - this.m_cameraTransform.position;
			float single = mTarget.magnitude;
			mTarget.Normalize();
			Vector3 mFollowRate = (((mTarget + (mTarget * single)) * this.m_followRate) * this.m_transitionAcceleration) * Time.smoothDeltaTime;
			Vector3 vector3 = this.m_target.transform.position;
			float mFollowHeight = vector3.y + this.m_followHeight;
			float single1 = mFollowRate.y;
			Vector3 mCameraTransform = this.m_cameraTransform.position;
			mFollowRate.y = single1 + (mFollowHeight - mCameraTransform.y) * this.m_followHeightRate * this.m_transitionAcceleration * Time.smoothDeltaTime;
			Transform transforms = this.m_cameraTransform;
			transforms.position = transforms.position + mFollowRate;
			Vector3 mCameraTransform1 = this.m_cameraTransform.forward * -0.5f;
			this.m_cameraTransform.position = this.m_cameraTransform.position + mCameraTransform1;
			Quaternion quaternion = Quaternion.LookRotation(this.m_SecondTarget.transform.position - this.m_cameraTransform.position);
			this.m_cameraTransform.rotation = Quaternion.Slerp(this.m_cameraTransform.rotation, quaternion, Time.deltaTime * this.m_transitionAcceleration * this.m_lookAtRate * 1f);
		}
	}
}