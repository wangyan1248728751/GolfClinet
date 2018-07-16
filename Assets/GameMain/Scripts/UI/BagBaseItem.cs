using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Golf
{
	public class BagBaseItem : MonoBehaviour {

		public Text count;
		public Image image;

		public Goods goodsData;

		public void Init(Goods data)
		{
			goodsData = data;
			count.text = "x" + data.num;
			if (gameObject.activeSelf)
				StartCoroutine(LoadImage("http://" + data.url));
		}

		IEnumerator LoadImage(string path)
		{
			WWW www = new WWW(path);
			yield return www;
			Texture2D texture = www.texture;
			Sprite sprites = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			image.sprite = sprites;
		}

	}
}
