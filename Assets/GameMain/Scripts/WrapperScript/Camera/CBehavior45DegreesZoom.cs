using System;
using UnityEngine;

public class CBehavior45DegreesZoom : IBehaviorInterface
{
	private Vector3 m_initialPosition;

	private Vector3 m_CamRotation;

	public CBehavior45DegreesZoom(Transform cameraTransform) : base(cameraTransform)
	{
		this.m_initialPosition = new Vector3(162f, 76f, 158f);
		this.m_CamRotation = new Vector3(61f, 342f, 355f);
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
			this.m_cameraTransform.position = this.m_initialPosition;
			this.m_cameraTransform.rotation = Quaternion.Euler(this.m_CamRotation);
			if (this.m_lookAtTarget)
			{
				this.m_cameraTransform.LookAt(this.m_target.transform);
			}
		}
	}
}