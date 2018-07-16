using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Golf
{
    public class PromotionalVideoComponent : GameFrameworkComponent
    {

        int videoTimerEventId = 0;

        public void RemoveTimer()
        {
            TimerManager.RemoveTimerEvent(videoTimerEventId);
        }

        public void StartTimer()
        {
            //Debug.Log("准备播放!");
            videoTimerEventId = TimerManager.CreateTimerEvent(StartVideo, 10, false);
        }

        void StartVideo()
        {
            //GameEntry.UI.OpenUIForm(UIFormId.VideoForm);
        }

    }
}
