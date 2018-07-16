using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Golf
{
    public class MapComponent : GameFrameworkComponent
    {

        public IslandManager _islandManager = null;
		public GameCameraManager _gameCameraManager = null;
        public MapCameraManager _mapCameraManager = null;

        public void Init()
        {
            _islandManager = FindObjectOfType<IslandManager>();
			_gameCameraManager = FindObjectOfType<GameCameraManager>();
            _mapCameraManager = FindObjectOfType<MapCameraManager>();
        }

        public void ResetMap()
        {
            //TODO:清理物体 

            //岛屿 
            _islandManager.RandomIslandPos();
			//相机
			ResetCameras();
            for (int i = 0; i < _islandManager.Cylinder.Count; i++)
            {
                _islandManager.Cylinder[i].SetActive(false);
            }
            GameEntry.HitBall.redisland.Clear();
            GameEntry.HitBall.blueisland.Clear();
			//TODO:分数物体重置
		}

		public void ResetCameras()
		{
			_gameCameraManager.ResetGameCamera();

			_mapCameraManager.ReSetMapCamera();
		}


    }
}
