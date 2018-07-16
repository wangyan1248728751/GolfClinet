using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Golf
{
    public class HeadImage : MonoBehaviour
    {
        public Image image;
        // Use this for initialization
        void OnEnable()
        {
            StartCoroutine(LoadImage(GameEntry.GameData.WxAvator));
        }
        IEnumerator LoadImage(string path)
        {
            WWW www = new WWW(path);
            yield return www;
            Texture2D texture = www.texture;
            Sprite sprites = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprites;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}