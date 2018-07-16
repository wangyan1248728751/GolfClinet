using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Golf;

public class GameCameraStateMachine : StateMachineBehaviour {

	//OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GameCameraManager.Instance.GameCameraState = GameCameraManager.GameCameraStateEnum.normal;
		animator.SetInteger("CameraState", (int)GameCameraManager.Instance.GameCameraState);
	}

}
