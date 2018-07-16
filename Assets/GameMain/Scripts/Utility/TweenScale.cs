using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TweenScale : MonoBehaviour {

	[Range(0, 5)]
	public float durationTime = 0.3f;//缩放总时间
	[Range(0, 2)]
	public float scaleRate = 0.9f;//缩放倍率
	public bool isLoop = false;	//是否循环
	RectTransform rectTrs;
	private void Awake()
	{
		rectTrs = transform.GetComponent<RectTransform>();
	}

	public void TweenRectTransformScale()
	{
		rectTrs.localScale = Vector3.one;
		if (isLoop)
		{
			rectTrs.DOScale(scaleRate, durationTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
		}
		else
		{
			rectTrs.DOScale(scaleRate, durationTime / 2).SetEase(Ease.Linear).OnComplete(() =>
			{
				rectTrs.DOScale(Vector3.one, durationTime / 2).SetEase(Ease.Linear);
			});
		}
	}

	public void TweenScaleOnDown()
	{
		rectTrs.DOScale(scaleRate, durationTime / 2).SetEase(Ease.Linear);
	}

	public void TweenScaleOnUp()
	{
		rectTrs.DOScale(Vector3.one, durationTime / 2).SetEase(Ease.Linear);
	}
}
