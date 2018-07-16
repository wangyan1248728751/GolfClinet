using System.Collections;
using System.Collections.Generic;
using GameFramework.DataTable;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace Golf
{
    public class IslandManager : MonoBehaviour
    {
        public List<GameObject> islandList;
        public List<Vector3> islandRawPos;
        public List<IslandScoreBase> islandScoreList;
        public List<GameObject> Cylinder;
        public Material defaultMaterial;
        public Material redCy;
        public Material blueCy;

		public Shader LoadingShader;
		public Shader TextureShader;

		int MapLayerId = 14;
		int MapObjectLayerId = 15;

		private void Start()
		{
			int listIndex;
			int mapIndex;
			//读表 加载
			IDataTable<DRRewardMap> dtRewardMap = GameEntry.GameData.dtRewardMap;
			if(GameEntry.GameData.onLine)
			{
				//在线模式
				foreach ( DRRewardMap _drRewardMap in GameEntry.GameData.webRewardMap.Values)
				{
					listIndex = _drRewardMap.Id / 1000 - 1;
					mapIndex = _drRewardMap.Id % 1000 - 1;

					if (listIndex >= islandScoreList.Count || mapIndex >= islandScoreList[listIndex]._renderer.Length)
						continue;

					string scoreColor;
					if (string.IsNullOrEmpty(_drRewardMap.ScoreColor))
						scoreColor = _drRewardMap.Score.ToString();
					else
						scoreColor = string.Format("<color=#{0}>{1}</color>", _drRewardMap.ScoreColor, _drRewardMap.Score.ToString());

					//islandScoreList[listIndex]._text[mapIndex].text = scoreColor;
					islandScoreList[listIndex]._renderer[mapIndex].name = _drRewardMap.Id.ToString();
                    if (!string.IsNullOrEmpty(_drRewardMap.ImageUrl))
                        StartCoroutine(GetImage(islandScoreList[listIndex]._renderer[mapIndex], _drRewardMap.ImageUrl));
                }
			}
			else
			{
				//离线模式
				foreach (DRRewardMap _drRewardMap in dtRewardMap)
				{
					listIndex = _drRewardMap.Id / 1000 - 1;
					mapIndex = _drRewardMap.Id % 1000 - 1;

					if (listIndex >= islandScoreList.Count || mapIndex >= islandScoreList[listIndex]._renderer.Length)
						continue;

					string scoreColor;
					if (string.IsNullOrEmpty(_drRewardMap.ScoreColor))
						scoreColor = _drRewardMap.Score.ToString();
					else
						scoreColor = string.Format("<color=#{0}>{1}</color>", _drRewardMap.ScoreColor, _drRewardMap.Score.ToString());

					//islandScoreList[listIndex]._text[mapIndex].text = scoreColor;
					islandScoreList[listIndex]._renderer[mapIndex].name = _drRewardMap.Id.ToString();
                    //if (!string.IsNullOrEmpty(_drRewardMap.ImageUrl))
                    //    StartCoroutine(GetImage(islandScoreList[listIndex]._renderer[mapIndex], _drRewardMap.ImageUrl));

                }
            }
			
			foreach (GameObject go in islandList)
			{
				islandRawPos.Add(go.transform.localPosition);
			}
		}

		public void Shine()
		{
			var id = GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1].rewardMapId;
			if (id == 0)
				return;
			islandScoreList[id / 1000 - 1]._renderer[id % 1000 - 1].enabled = true;
			islandScoreList[id / 1000 - 1]._renderer[id % 1000 - 1].gameObject.layer = MapLayerId;
            if (GameEntry.GameData.onLine)
            {

			GameObject shineText = Instantiate(islandScoreList[id / 1000 - 1]._text[id % 1000 - 1].gameObject);
			shineText.transform.position = islandScoreList[id / 1000 - 1]._text[id % 1000 - 1].gameObject.transform.position;
			shineText.layer = MapLayerId;
			shineText.transform.LookAt(GameEntry.Map._gameCameraManager._camera.transform);
			shineText.transform.localEulerAngles = new Vector3(-shineText.transform.localEulerAngles.x, shineText.transform.localEulerAngles.y + 180, shineText.transform.localEulerAngles.z);
			TextMeshPro text = shineText.GetComponent<TextMeshPro>();
			shineText.SetActive(false);
			text.enabled = true;
			shineText.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

			text.DOColor(new Color(text.color.a, text.color.r, text.color.b, 1), 1).OnComplete(() =>
			{
				shineText.SetActive(true);
				shineText.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 1.5f);
				shineText.transform.DOLocalMoveY(4, 1.5f).OnComplete(() =>
				{
					text.DOColor(new Color(text.color.a, text.color.r, text.color.b, 0), 2).OnComplete(() =>
					{
						Destroy(shineText);
					});
				});
			});
            }

            if (!GameEntry.GameData.onLine)
            {
                if (GameEntry.GameData.currGolfball.Count % 2 == 1)
                {
                    Cylinder[id / 1000 - 1].GetComponent<Renderer>().material = redCy;
                }
                else
                {
                    Cylinder[id / 1000 - 1].GetComponent<Renderer>().material = blueCy;
                }
                Cylinder[id / 1000 - 1].SetActive(true);
            }
            else
            {

			    islandScoreList[id / 1000 - 1]._tx[id % 1000 - 1].SetActive(true);
			    islandScoreList[id / 1000 - 1]._tx[id % 1000 - 1].gameObject.transform.SetLocalPositionY(0.01f);
            }
		}

		public void ShineEnd()
		{
			if (GameEntry.GameData.currGolfball.Count == 0)
				return;
			var id = GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1].rewardMapId;
			if (id == 0)
				return;
			//islandScoreList[id / 1000 - 1]._renderer[id % 1000 - 1].gameObject.SetActive(false);
			islandScoreList[id / 1000 - 1]._renderer[id % 1000 - 1].gameObject.layer = MapObjectLayerId;
			islandScoreList[id / 1000 - 1]._text[id % 1000 - 1].gameObject.layer = MapObjectLayerId;
			islandScoreList[id / 1000 - 1]._tx[id % 1000 - 1].SetActive(false);
		}

        /// <summary>
        /// 随机变更岛的坐标
        /// </summary>
        public void RandomIslandPos()
        {
			if (GameEntry.GameData.onLine)
			{
				for (int i = 0; i < GameEntry.GameData.webPos.Count; i++)
				//foreach(Vector2 vec2 in GameEntry.GameData.webPos)
				{
					islandList[i].transform.localPosition = new Vector3(GameEntry.GameData.webPos[i].x, islandList[i].transform.localPosition.y, GameEntry.GameData.webPos[i].y);

				}
			}
			else
			{
				List<Vector3> small = new List<Vector3>();
				List<Vector3> medium = new List<Vector3>();
				List<Vector3> big = new List<Vector3>();

				for (int i = 0; i < islandRawPos.Count; i++)
				{
					if (i < 3)
						small.Add(islandRawPos[i]);
					if (i >= 3 && i < 6)
						medium.Add(islandRawPos[i]);
					if (i >= 6 && i < 8)
						big.Add(islandRawPos[i]);
				}

				for (int i = 0; i < islandList.Count; i++)
				{
					if (i < 3)
					{
						int randomIndex = Random.Range(0, small.Count);
						islandList[i].transform.localPosition = RandomPos(small[randomIndex], 3f);
						small.RemoveAt(randomIndex);
					}
					if (i >= 3 && i < 6)
					{
						int randomIndex = Random.Range(0, medium.Count);
						islandList[i].transform.localPosition = RandomPos(medium[randomIndex], 5f);
						medium.RemoveAt(randomIndex);
					}
					if (i >= 6 && i < 8)
					{
						int randomIndex = Random.Range(0, big.Count);
						islandList[i].transform.localPosition = RandomPos(big[randomIndex], 7f);
						big.RemoveAt(randomIndex);
					}


				}
			}
        }

        Vector3 RandomPos(Vector3 rawPos,float offset)
        {
            Vector2 randomVec = Random.insideUnitCircle * offset;
            rawPos.x += randomVec.x;
            rawPos.z += randomVec.y;
            return rawPos;
        }

		public IEnumerator GetImage(Renderer _renderer, string url)
		{
			_renderer.material = new Material(LoadingShader);
			_renderer.material.color = Color.black;
			WWW www = new WWW("http://" + url);
			yield return www;
			if (string.IsNullOrEmpty(www.error))
			{
				Material textureMaterial = new Material(TextureShader);
				Texture2D tex = www.texture;
				textureMaterial.mainTexture = tex;
				_renderer.material = textureMaterial;
			}
		}



    }
}