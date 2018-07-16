using System;
using UnityEngine;

public class CBehaviorFollowBehind : IBehaviorInterface
{
	public CBehaviorFollowBehind(Transform cameraTransform) : base(cameraTransform)
	{
	}

	public override void Init(GameObject target)
	{
		if (target != null)
		{
			this.m_target = target;
		}
	}

	public override void Update()
	{
		if (this.m_target != null)
		{
			CBehaviorFollowBehind mTransitionAcceleration = this;
			mTransitionAcceleration.m_transitionAcceleration = mTransitionAcceleration.m_transitionAcceleration + Time.deltaTime * 0f;
			Mathf.Clamp(this.m_transitionAcceleration, 0f, 1f);
			Vector3 mTarget = this.m_target.transform.position - this.m_cameraTransform.position;
			float mFollowRadius = mTarget.magnitude - this.m_followRadius;
			mTarget.Normalize();
			float single = Mathf.Min(this.m_followRate * this.m_transitionAcceleration * Time.deltaTime, 0.75f);
			Vector3 vector3 = (mTarget + (mTarget * mFollowRadius)) * 0.2f;
			Vector3 mTarget1 = this.m_target.transform.position;
			float mFollowHeight = mTarget1.y + this.m_followHeight;
			single = Mathf.Min(this.m_followHeightRate * this.m_transitionAcceleration * Time.deltaTime, 0.75f);
			float single1 = vector3.y;
			Vector3 mCameraTransform = this.m_cameraTransform.position;
			vector3.y = single1 + (mFollowHeight - mCameraTransform.y) * single;
			Transform transforms = this.m_cameraTransform;
			transforms.position = transforms.position + vector3;
			Vector3 mCameraTransform1 = this.m_cameraTransform.forward * -1f;
			this.m_cameraTransform.position = this.m_cameraTransform.position + mCameraTransform1;
			Quaternion quaternion = Quaternion.LookRotation(this.m_target.transform.position - this.m_cameraTransform.position);
			this.m_cameraTransform.rotation = Quaternion.Slerp(this.m_cameraTransform.rotation, quaternion, Time.deltaTime * this.m_transitionAcceleration * this.m_lookAtRate * 1f);
		}
	}
}