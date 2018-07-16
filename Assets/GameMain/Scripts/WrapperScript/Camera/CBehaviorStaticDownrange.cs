using System;
using UnityEngine;

public class CBehaviorStaticDownrange : IBehaviorInterface
{
	private Vector3 m_initialPosition;

	private Vector3 m_lerpStartPosition;

	private Vector3 CamOffset = new Vector3(0f, 5f, 10f);

	private Vector3 Ball = new Vector3(118f, 0.06f, 0f);

	public CBehaviorStaticDownrange(Transform cameraTransform) : base(cameraTransform)
	{
		this.m_initialPosition = new Vector3(118.5f, 20f, 360f);
	}

	public override void Init(GameObject target)
	{
		if (target != null)
		{
			this.m_target = target;
		}
	}

	public void SetInitialPosition(Vector3 initialPosition)
	{
		this.m_initialPosition = initialPosition + this.CamOffset;
	}

	public override void Update()
	{
		if (this.m_target != null)
		{
			Vector3 mCameraTransform = this.m_cameraTransform.forward * -0.9f;
			this.m_cameraTransform.position = this.m_initialPosition + mCameraTransform;
			Quaternion quaternion = Quaternion.LookRotation(this.Ball - this.m_cameraTransform.position);
			this.m_cameraTransform.rotation = Quaternion.Slerp(this.m_cameraTransform.rotation, quaternion, Time.deltaTime * this.m_transitionAcceleration * this.m_lookAtRate * 1f);
			this.m_cameraTransform.LookAt(this.m_target.transform.position);
		}
	}
}