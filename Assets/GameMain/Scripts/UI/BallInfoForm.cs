using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.DataTable;
using System.Collections;

namespace Golf
{
	public class BallInfoForm : UGuiForm
	{
		[SerializeField]
		Text distance;
		[SerializeField]
		Text maxHeight;
		[SerializeField]
		Text speed;
		[SerializeField]
		Text launchAngle;
		[SerializeField]
		Text horizontalAngle;
		[SerializeField]
		Text backSpin;
		[SerializeField]
		Text sideSpin;
        [SerializeField]
        Text km;
        [SerializeField]
        Text yd;
        [SerializeField]
        Image color;

        float m_time = 0;

		protected internal override void OnOpen(object userData)
		{
			base.OnOpen(userData);
			if (GameEntry.GameData.currGolfball.Count == 0)
				OnClose(true);
            m_time = 0;
			golfballHitData data = GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1];

            InvokeRepeating("repeat", 0f, 0.01f);
            distance.text = new Vector2(data.endPoint.x, data.endPoint.z).magnitude.ToString("F1");
            yd.text = "yd";
            maxHeight.text = data.maxHeight.ToString("F1") + "m";
			speed.text = data.speed.ToString("F1");
            km.text = "km/h";
			launchAngle.text = data.launchAngle.ToString("F1") + "°";
			horizontalAngle.text = data.horizontalAngle.ToString("F1") + "°";

			backSpin.text = data.backSpin.ToString("F0")+"\n" + "rpm";
			sideSpin.text = data.sideSpin.ToString("F0") + "rpm";
		}

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            golfballHitData data = GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1];
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            m_time += Time.deltaTime;
           
            
            distance.text = Mathf.Lerp(0, new Vector2(data.endPoint.x, data.endPoint.z).magnitude, 1/data.time * m_time).ToString("F1");
        }
        int i=0;
        void repeat()
        {
            golfballHitData data = GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count - 1];
            if (i<=data.speed) 
            {
                speed.text = i++.ToString();
                color.fillAmount = Mathf.Lerp(0, data.speed / 100, i/ data.speed);
            }
            if (i> data.speed)
            {
                CancelInvoke();
                i = 0;
            }
        }

	}
}
