using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Golf
{
    public class MapCameraManager : MonoBehaviour
    {
		private static MapCameraManager instance;
		public static MapCameraManager Instance
		{
			get
			{
				return instance;
			}
		}

        public Camera mapCamera;
		
        Vector3 rawPos;
        float rawCameraSize;

		

        void Awake()
        {
			instance = this;
			//init MapCamera
            rawPos = mapCamera.transform.position;
            rawCameraSize = mapCamera.orthographicSize;
            moveTargetPos = rawPos;
            scaleTargetSize = rawCameraSize;
		}

        void Update()
        {
			UpdateMapCamera();
		}

        public void ReSetMapCamera()
        {
			mapCamera.transform.position = rawPos;
			mapCamera.orthographicSize = rawCameraSize;
        }


		public void UpdateMapCamera()
		{
			if (Vector3.Distance(moveTargetPos, mapCamera.transform.position) > 0.5f)
			{
				moveLerpTime = moveSpeed / Vector3.Distance(moveTargetPos, mapCamera.transform.position);
				mapCamera.transform.position = Vector3.Lerp(mapCamera.transform.position, moveTargetPos, moveLerpTime);
			}
			if (scaleTargetSize > 0.5f)
			{
				scaleLerpTime = scaleSpeed / Mathf.Abs(scaleTargetSize - mapCamera.orthographicSize);
				mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, scaleTargetSize, scaleLerpTime);
			}
		}


        float moveSpeed = 10;   //差值移动速度
        public Vector3 moveTargetPos;  //移动目标点
        float moveLerpTime;     //差值移动时间

        /// <summary>
        /// 移动摄像机坐标.
        /// </summary>
        public void Move(Vector2 vec)
        {
            moveTargetPos += new Vector3(-vec.x, 0, -vec.y);
            float horMoveArea = rawCameraSize - mapCamera.orthographicSize;
            float verMoveArea = rawCameraSize - mapCamera.orthographicSize;
            if (Mathf.Abs(moveTargetPos.x) > horMoveArea)
            {
                if (moveTargetPos.x > 0)
                    moveTargetPos.x = horMoveArea;
                else
                    moveTargetPos.x = -horMoveArea;
            }
            if (Mathf.Abs(moveTargetPos.z - rawPos.z) > verMoveArea)
            {
                if (moveTargetPos.z > rawPos.z)
                    moveTargetPos.z = verMoveArea + rawPos.z;
                else
                    moveTargetPos.z = -verMoveArea + rawPos.z;
            }
        }

        float scaleSpeed = 10;  //缩放速度
        public float scaleTargetSize;  //缩放目标大小
        float scaleLerpTime;     //差值缩放时间

        /// <summary>
        /// 设置摄像机缩放.
        /// </summary>
        public void Scale(float size)
        {
            scaleTargetSize -= size;
            if (scaleTargetSize > 115)
                scaleTargetSize = 115;
            if (scaleTargetSize < 30)
                scaleTargetSize = 30;
        }

    }
}