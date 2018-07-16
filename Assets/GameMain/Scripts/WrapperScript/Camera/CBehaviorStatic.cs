using System;
using UnityEngine;

public class CBehaviorStatic : IBehaviorInterface
{
	private Vector3 m_initialPosition;

	private Vector3 m_lerpStartPosition;

	private Vector3 m_offset;

	private Vector3 m_rotationOffset;

	public CBehaviorStatic(Transform cameraTransform) : base(cameraTransform)
	{
		this.m_initialPosition = cameraTransform.position;
	}

	public override void Init(GameObject target)
	{
		if (target != null)
		{
			this.m_target = target;
		}
	}

	public void ResetCameraAnchorPosition(Transform newTransform)
	{
		this.m_initialPosition = newTransform.position;
	}

	public override void SetOffset(Vector3 posOffset, Vector3 rotationOffset)
	{
		this.m_offset = posOffset;
		this.m_rotationOffset = rotationOffset;
	}

	public override void Update()
	{
		if (this.m_target != null)
		{
			this.m_cameraTransform.position = this.m_initialPosition + this.m_offset;
			this.m_cameraTransform.forward = Vector3.forward;
			Transform mCameraTransform = this.m_cameraTransform;
			Quaternion quaternion = this.m_cameraTransform.rotation;
			mCameraTransform.rotation = Quaternion.Euler(quaternion.eulerAngles + this.m_rotationOffset);
		}
	}
}