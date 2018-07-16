using System;
using UnityEngine;

public class CBehavior45Degrees : IBehaviorInterface
{
	private float initialXPos = 175f;

	private float initialYPos = 60f;

	private float initialZPos = -11f;

	private float initialXLook = 25f;

	private float initialYLook = 325f;

	private float initialZLook;

	private Vector3 m_initialPosition;

	private Vector3 m_initialLook;

	private Vector3 m_lerpStartPosition;

	private Vector3 m_worldLook;

	public CBehavior45Degrees(Transform cameraTransform) : base(cameraTransform)
	{
		this.m_initialPosition = new Vector3(this.initialXPos, this.initialYPos, this.initialZPos);
		this.m_worldLook = new Vector3(this.initialXLook, this.initialYLook, this.initialZLook);
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
			this.m_cameraTransform.rotation = Quaternion.Euler(this.m_worldLook);
			if (this.m_lookAtTarget)
			{
				this.m_cameraTransform.LookAt(this.m_target.transform);
			}
		}
	}
}