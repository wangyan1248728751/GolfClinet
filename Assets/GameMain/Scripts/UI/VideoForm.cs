using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.Video;
using UnityEngine.UI;

namespace Golf
{
    public class VideoForm : UGuiForm
    {

        [SerializeField]
        private Image videoImage;
        [SerializeField]
        private VideoPlayer video;

        internal protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            video.loopPointReached += VideoEnd;
        }

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            //Debug.Log("开始播放!");
            video.targetCamera = GameEntry.Scene.MainCamera;
            videoImage.raycastTarget = true;
            video.Play();
        }

        public void StopVideo()
        {
            video.Stop();
            VideoEnd(video);
        }

        void VideoEnd(VideoPlayer _video)
        {
            _video.targetCamera = null;
            videoImage.raycastTarget = false;
            Close();
        }
    }
}
