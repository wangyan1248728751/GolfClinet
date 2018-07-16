using System;
using System.Collections.Generic;
using UnityEngine;

public class CBehaviorDynamic : IBehaviorInterface
{
	private Vector3 m_initialPosition;

	private Vector3 m_lerpStartPosition;

	private Vector3 CamOffset = new Vector3(20f, 10f, -10f);

	private Vector3 lastGoodStartLocation;

	private Vector3 lastGoodBallLanding;

	private Vector3 lastGoodBallResting;

	public CBehaviorDynamic(Transform cameraTransform) : base(cameraTransform)
	{
		this.m_initialPosition = cameraTransform.position;
	}

	public override void Init(GameObject target)
	{
		if (target != null)
		{
			this.m_target = target;
			this.m_initialPosition = this.m_target.transform.position;
		}
	}

	public void ResetCameraAnchorPosition(Transform newTransform)
	{
		this.m_initialPosition = newTransform.position;
	}

	public override void Update()
	{
		Vector3 vector3;
		Vector3 vector31;
		if (this.m_target != null)
		{
			CBall component = this.m_target.GetComponent<CBall>();
			if (component == null)
			{
				this.m_cameraTransform.position = this.m_initialPosition + this.CamOffset;
				this.m_cameraTransform.LookAt(this.m_target.transform.position);
			}
			else
			{
				BallTrajectory trajectory = component.GetTrajectory();
				if (trajectory != null && trajectory.m_points.Count != 0)
				{
					BallTrajectory.Point item = component.GetTrajectory().m_points[0];
					this.lastGoodStartLocation = item.pos;
					BallTrajectory.Point point = component.GetTrajectory().m_points[component.GetTrajectory().firstBounceFrame];
					this.lastGoodBallLanding = point.pos;
					this.lastGoodBallResting = component.GetTrajectory().GetRestingPosition();
				}
				if ((this.lastGoodBallLanding - this.lastGoodStartLocation).magnitude >= (this.lastGoodBallResting - this.lastGoodStartLocation).magnitude)
				{
					vector3 = this.lastGoodBallResting;
					vector31 = this.lastGoodBallLanding;
				}
				else
				{
					vector3 = this.lastGoodBallLanding;
					vector31 = this.lastGoodBallResting;
				}
				Vector3 vector32 = vector31 - vector3;
				float single = vector32.magnitude / 2f;
				vector32.Normalize();
				Vector3 vector33 = vector3 + (vector32 * single);
				Vector3 vector34 = Vector3.Cross(Vector3.up, vector32);
				if (vector33.x > this.lastGoodStartLocation.x)
				{
					vector34 *= -1f;
				}
				vector34 *= 2f;
				vector34.y = 0.5f;
				this.m_cameraTransform.position = vector33 + vector34;
				this.m_cameraTransform.LookAt(this.m_target.transform);
			}
		}
	}
}