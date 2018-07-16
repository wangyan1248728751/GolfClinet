using GameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Golf
{
	public class ResultForm : UGuiForm
	{
		[SerializeField]
		Text totalScore;
		[SerializeField]
		List<GameObject> rankImages;
        [SerializeField]
        GameObject flag;


		protected internal override void OnOpen(object userData)
		{
			base.OnOpen(userData);

			GameEntry.GameData.needShowResultForm = false;

			int score = 0;
			foreach (golfballHitData data in GameEntry.GameData.currGolfball)
				score += data.score;
            if (GameEntry.GameData.onLine)
            {
			    totalScore.text = score.ToString();
            }
            else
            {
                totalScore.fontSize = 50;
                flag.SetActive(false);
                Debug.Log("红：" + GameEntry.HitBall.redisland.Count + "蓝：" + GameEntry.HitBall.blueisland.Count);
                if (GameEntry.HitBall.redisland.Count>GameEntry.HitBall.blueisland.Count)
                {
                    totalScore.text = "恭喜一号玩家取得胜利";
                }
                else if (GameEntry.HitBall.redisland.Count < GameEntry.HitBall.blueisland.Count)
                {
                    totalScore.text = "恭喜二号玩家取得胜利";
                }
                else
                {
                    totalScore.text = "旗鼓相当";
                }
            }

			foreach (GameObject go in rankImages)
				go.SetActive(false);

			if (score < 1000)
				rankImages[score / 200].SetActive(true);
			else
				rankImages[4].SetActive(true);
		}

		public void ResetGame()
		{
			GameEntry.GameCore.GameRestart();
			Close();
		}

	}
}
