using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.ObjectPool;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.Event;

namespace Golf
{
    public class HitBallComponent : GameFrameworkComponent
    {
        [SerializeField]
        private FlightPathItem _FlightPathItemTemplate = null;

        private IObjectPool<FlightPathObject> _FlightPathObjectPool = null;
        private List<FlightPathItem> _FlightPathItems = null;
        private int m_InstancePoolCapacity = 10;
		public CBall currBall;
        public List<string> blueisland = new List<string>();
        public List<string> redisland = new List<string>();

        private void Start()
        {
            _FlightPathObjectPool = GameEntry.ObjectPool.CreateSingleSpawnObjectPool<FlightPathObject>("FlightPath", m_InstancePoolCapacity);
            _FlightPathItems = new List<FlightPathItem>();
        }

        public void Init()
        {
			_FlightPathObjectPool.AutoReleaseInterval = 10;
			_FlightPathObjectPool.Capacity = 3;
		}

		public void PrepareHit()
		{
			CSimulationManager.instance.ResetScene();
			Security.SecurityWrapperService.Instance.ArmBox();
			currBall = CSimulationManager.instance.GetNewBall();
			currBall.transform.SetParent(GameCameraManager.Instance.tee.transform);
			currBall.transform.localPosition = new Vector3(0, 0.1f, 0);
		}


		public void HitGolfBall(ref golfballHitData data)
		{
			CheckGolfballDestination(ref data);

			if (data.endPoint.z > 20)
				//GameCameraManager.Instance.GameCameraFollowBall(currBall);
				GameCameraManager.Instance.GameCameraFollowBall(data);
		}

		public void BallEnd(CBall ball)
		{
			if (GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1].score != 0)
				ball.tx_luodi.gameObject.SetActive(true);
			else
				ball.tx_luoshui.gameObject.SetActive(true);

            if (GameEntry.GameData.currGolfball.Count < GameEntry.GameData.MaxGolfballCount && GameEntry.GameData.onLine)
            {
                PrepareHit();
            }
            else if((GameEntry.HitBall.redisland.Count != 8 && GameEntry.HitBall.blueisland.Count != 8) && !GameEntry.GameData.onLine)
            {
                PrepareHit();
            }

			ShowFlightPath(GameEntry.GameData.pathBtnState);
		}

        /// <summary>
        /// 落点检测.
        /// </summary>
        public void CheckGolfballDestination(ref golfballHitData data)
        {
            bool hitIsland = false;
            bool hitscore = false;
            int rewardMapId = 0;
			GameObject rewardMap = null;
			Vector3 rayStartPos = new Vector3(data.endPoint.x, data.endPoint.y + 10, data.endPoint.z);
            Ray ray = new Ray(rayStartPos, -transform.up);
            //检测分数
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("MapObject")))
            {
                
				// 如果射线与平面碰撞，打印碰撞物体信息  
				Debug.Log("碰撞对象: " + hit.collider.name);
				if (hit.collider.tag == "score")
                {
                    hitscore = true;
                    rewardMapId = int.Parse(hit.collider.name);
					rewardMap = hit.collider.gameObject;
                }
            }

            //检测地块
            RaycastHit hit2;
            if (Physics.Raycast(ray, out hit2, Mathf.Infinity, 1 << LayerMask.NameToLayer("Map")))
            {
                
                // 如果射线与平面碰撞，打印碰撞物体信息  
                Debug.Log("碰撞对象: " + hit2.collider.name);

				if (hit2.collider.tag == "island")
                {
                    hitIsland = true;
                    if (!GameEntry.GameData.onLine && hitscore)
                    {
                        if (GameEntry.GameData.currGolfball.Count % 2 != 1)
                        {
                            Debug.Log(GameEntry.GameData.currGolfball.Count);
                            if (!redisland.Contains(hit2.collider.name))
                            {
                                redisland.Add(hit2.collider.name);
                            }
                            if (blueisland != null)
                            {
                                for (int i = 0; i < blueisland.Count; i++)
                                {
                                    if (blueisland[i]==hit2.collider.name)
                                    {
                                        blueisland.Remove(hit2.collider.name);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!blueisland.Contains(hit2.collider.name))
                            {
                                blueisland.Add(hit2.collider.name);
                            }
                            if (redisland != null)
                            {
                                for (int i = 0; i < redisland.Count; i++)
                                {
                                    if (redisland[i]==hit2.collider.name)
                                    {
                                        redisland.Remove(hit2.collider.name);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (hitIsland)
			{
				data.rewardMapId = rewardMapId;
				data.rewardMap = rewardMap;
			}
        }

        /// <summary>
        /// 显示球的飞行路径
        /// </summary>
        public void ShowFlightPath(pathBtnEnum pathBtnState)
        {
            hidePath();
            foreach(golfballHitData data in GetPathData(pathBtnState))
            {
                DrawPath(data.endPoint);
                data.ball.gameObject.SetActive(true);
            }
        }

        private List<golfballHitData> GetPathData(pathBtnEnum pathBtnState)
        {
            List<golfballHitData> data = new List<golfballHitData>();
            switch (pathBtnState)
            {
                //case pathBtnEnum.none:
                //    break;

                case pathBtnEnum.one:
                    if (GameEntry.GameData.currGolfball.Count > 0)
                        data.Add(GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1]);
                    break;

                case pathBtnEnum.more:
                    if (GameEntry.GameData.currGolfball.Count > 3)
                    {
                        for (int i = 0; i < 3; i++)
                            data.Add(GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1 - i]);
                    }
                    else
                        data = GameEntry.GameData.currGolfball;
                    break;
            }
            return data;
        }

        private void DrawPath(Vector3 pos)
        {
            FlightPathItem item = CreateFlightPathItem();
			item.lineEnd.transform.position = new Vector3(pos.x, pos.y + 10, pos.z);
			//item.lineEnd.transform.position = pos;
			//item.GetComponent<LineRenderer>().SetPosition(0, CSimulationManager.instance.m_ballTemplate.transform.position);
			//item.GetComponent<LineRenderer>().SetPosition(1,pos);
			_FlightPathItems.Add(item);
        }

        private FlightPathItem CreateFlightPathItem()
        {
            FlightPathItem pathItem = null;
            FlightPathObject flightPathObject = _FlightPathObjectPool.Spawn();
            if (flightPathObject != null)
            {
                pathItem = (FlightPathItem)flightPathObject.Target;
            }
            else
            {
                pathItem = Instantiate(_FlightPathItemTemplate);
                _FlightPathObjectPool.Register(new FlightPathObject(pathItem), true);
            }

            return pathItem;
        }

        public void hidePath()
        {
            foreach (FlightPathItem item in _FlightPathItems)
                _FlightPathObjectPool.Unspawn(item);
            _FlightPathItems.Clear();
            foreach (golfballHitData data in GameEntry.GameData.currGolfball)
                data.ball.gameObject.SetActive(false);
        }
	}

    public enum pathBtnEnum
    {
        //none,
        one,
        more,
    }
}
