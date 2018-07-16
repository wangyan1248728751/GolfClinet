using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Golf
{
    public class MapTouch : MonoBehaviour, IDragHandler,IPointerClickHandler
    {
        Vector2 inputPos0;
        Vector2 inputPos1;
        float lastDistance;    //上一次移动的距离
        /*TODO: 三种状态: 
                        点击 暂时先不做
                        拖动 移动距离x缩放尺寸 边缘判定及回弹
                        缩放 缩放中心=>lerp(当前地图中心,双指缩放距离中点,t)
                            缩放大小=>
                            以上都没做完
        */
        public void OnDrag(PointerEventData eventData)
        {
            //print("input Count"+ Input.touchCount);
            //if (Input.GetMouseButton(0))
            //{
            //    GameEntry.Map._mapCameraManager.Move(eventData.delta);
            //    Debug.Log(eventData.delta);
            //}
			//if (Input.touchCount == 1)
			//	GameEntry.Map._mapCameraManager.Move(eventData.delta);
            if (Input.touchCount == 2)
                OnScale();
        }
        Vector2 m_screenPos = new Vector2();
        private void Update()
        {
            if (Input.touchCount==1)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                    m_screenPos = Input.touches[0].position;   //记录手指刚触碰的位置
                if (Input.touches[0].phase == TouchPhase.Moved) //手指在屏幕上移动，移动摄像机
                {
                    GameEntry.Map._mapCameraManager.Move(new Vector2(Input.touches[0].deltaPosition.x * Time.deltaTime, Input.touches[0].deltaPosition.y * Time.deltaTime));
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
		{
			Vector3 vec = new Vector3(eventData.position.x, eventData.position.y, 0);
			vec = GameEntry.Map._mapCameraManager.mapCamera.ScreenToWorldPoint(vec);
			vec.y = 5;
#if UNITY_EDITOR
			golfballHitData data = new golfballHitData();
			data.endPoint = vec;
			GameEntry.HitBall.CheckGolfballDestination(ref data);
			GameEntry.GameData.currGolfball.Add(data);
			GameEntry.Map._islandManager.Shine();
			//Debug.Log("得分地块:" + a);
			//GameEntry.Map._islandManager.Shine(a);
#endif
		}

        /// <summary>
        /// 缩放输入
        /// </summary>
        void OnScale()
        {
            inputPos0 = Input.GetTouch(0).position;
            inputPos1 = Input.GetTouch(1).position;

            if (Input.GetTouch(1).phase == TouchPhase.Began)
            {
                lastDistance = Vector3.Distance(inputPos0, inputPos1);
            }
            float scaleSize = Vector3.Distance(inputPos0, inputPos1) - lastDistance;
            //print("scaleSize:" + scaleSize + " lastDistance:" + lastDistance);
            lastDistance = Vector3.Distance(inputPos0, inputPos1);

            GameEntry.Map._mapCameraManager.Scale(scaleSize);
        }
        /*public float scaleSize;
        void Update()
        {
            GameEntry.Map._mapCameraManager.Scale(scaleSize);
        }*/

    }
}