using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Golf
{
	public class GameCameraManager : MonoBehaviour
	{

		private static GameCameraManager instance;
		public static GameCameraManager Instance
		{
			get
			{
				return instance;
			}
		}

		public Transform tee;
		public Transform cameraTrans;
		public Camera _camera;

		private Vector3 rawPos;
		[SerializeField]
		private Animator animator;
		public GameCameraStateEnum GameCameraState = GameCameraStateEnum.normal;


		void Awake()
		{
			instance = this;
			rawPos = _camera.transform.localPosition;
		}

		public void ResetGameCamera()
		{
			ResetAnimator();
			ResetPos();
		}

		public void ResetAnimator()
		{
			//Opening Cinematic
			animator.enabled = true;
			GameCameraState = GameCameraStateEnum.OpeningCinematic;
			animator.SetInteger("CameraState", (int)GameCameraState);
		}

		public void ResetPos()
		{
			GameEntry.Map._islandManager.ShineEnd();
            //StartCoroutine(closeBInfo());
            //if (GameEntry.UI.HasUIForm(UIFormId.BallInfoForm))
            //    GameEntry.UI.GetUIForm(UIFormId.BallInfoForm).Close(true);
            tee.localPosition = new Vector3((GameEntry.GameData.teePosX - 0.5f) * 100, -0.01f, 0);
			cameraTrans.transform.SetParent(tee);
			cameraTrans.transform.localPosition = Vector3.zero;
			cameraTrans.LookAt(new Vector3(0, 0, 75));
			_camera.transform.rotation = Quaternion.Euler(25, 0, 0);
			_camera.transform.localPosition = new Vector3(0, 5, -4);
		}

        IEnumerator closeBInfo()
        {
            yield return new WaitForSeconds(3.5f);
            if (GameEntry.UI.HasUIForm(UIFormId.BallInfoForm))
                GameEntry.UI.GetUIForm(UIFormId.BallInfoForm).Close(true);
        }

		public void GameCameraFollowBall(golfballHitData currBall)
		//public void GameCameraFollowBall(CBall currBall)
		{
			//0,5,-4
			animator.enabled = false;

			//cameraTrans.SetParent(currBall.transform);
			//cameraTrans.transform.localPosition = Vector3.zero;
			//_camera.transform.localPosition = new Vector3(0, 5, -4);


			_camera.transform.localRotation = Quaternion.Euler(new Vector3(25, 0, 0));

			float height;
			if (currBall.maxHeight > 5)
				height = currBall.maxHeight;
			else
				height = 5;
			_camera.transform.DOLocalMove(_camera.transform.localPosition + new Vector3(0, height * 1.5f, -height * 1.2f), currBall.time * 6 / 10f).OnComplete(() =>
				{


					_camera.transform.DOLocalMove(currBall.endPoint + new Vector3(0, 15, -20), currBall.time * 6 / 10f);
				//_camera.transform.DOLocalMove(new Vector3(0, height * 1.5f, -6), currBall.time / 2f).OnComplete(() =>
				//{
				//	_camera.transform.DOLocalMove(currBall.endPoint + new Vector3(0, 15, -20), currBall.time / 2f);
				//});
			});
			_camera.transform.DOLocalRotate(new Vector3(35, 0, 0), currBall.time / 1f).SetEase(Ease.InQuad);
		}

		public enum GameCameraStateEnum
		{
			normal,
			OpeningCinematic,
		}

	}
}