using System;
using UnityEngine;

public class CBehaviorStaticCutPin : IBehaviorInterface
{
	private Vector3 m_initialPosition;

	private Vector3 m_lerpStartPosition;

	private GameObject m_SecondTarget;

	public CBehaviorStaticCutPin(Transform cameraTransform) : base(cameraTransform)
	{
		this.m_initialPosition = cameraTransform.position;
	}

	public override void Init(GameObject target)
	{
		if (target != null && this.m_SecondTarget != null)
		{
			this.m_target = target;
		}
	}

	public void ResetCameraAnchorPosition(Transform newTransform)
	{
		this.m_initialPosition = newTransform.position;
	}

	public void SetInitialPosition(Vector3 initialPosition)
	{
		this.m_initialPosition = initialPosition;
	}

	public void SetSecondTarget(GameObject target)
	{
		this.m_SecondTarget = target;
	}

	public override void Update()
	{
		if (this.m_target != null && this.m_SecondTarget != null)
		{
			Vector3 mInitialPosition = this.m_initialPosition;
			Vector3 mSecondTarget = this.m_SecondTarget.transform.position;
			Vector3 vector3 = mInitialPosition - mSecondTarget;
			vector3.y = 0f;
			float single = vector3.magnitude;
			vector3.Normalize();
			vector3 = vector3 * (single + 6f);
			vector3.y = 3f;
			this.m_cameraTransform.position = new Vector3(mSecondTarget.x, 0f, mSecondTarget.z) + vector3;
			this.m_cameraTransform.LookAt(this.m_target.transform);
		}
	}
}