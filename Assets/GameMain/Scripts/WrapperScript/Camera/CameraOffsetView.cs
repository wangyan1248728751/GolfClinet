using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CameraOffsetView : MonoBehaviour
{
	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private Button _cancelButton;

	[SerializeField]
	private Button _resetButton;

	[SerializeField]
	private Button _applyButton;

	public Action<float> OnOffsetChanged;

	public Action<float> OnApplyOffset;

	private float _prevOffsetSliderVal;

	public CameraOffsetView()
	{
	}

	private void Awake()
	{
		this._cancelButton.onClick.AddListener(new UnityAction(this.OnCancelClick));
		this._resetButton.onClick.AddListener(new UnityAction(this.OnResetClick));
		this._applyButton.onClick.AddListener(new UnityAction(this.OnApplyClick));
		this._slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderValueChanged));
	}

	private void OnApplyClick()
	{
		if (this.OnApplyOffset != null)
		{
			this.OnApplyOffset(this._slider.@value);
		}
		base.gameObject.SetActive(false);
	}

	private void OnCancelClick()
	{
		this._slider.@value = this._prevOffsetSliderVal;
		if (this.OnOffsetChanged != null)
		{
			this.OnOffsetChanged(this._slider.@value);
		}
		base.gameObject.SetActive(false);
	}

	private void OnResetClick()
	{
		this._slider.@value = 0.5f;
		if (this.OnOffsetChanged != null)
		{
			this.OnOffsetChanged(this._slider.@value);
		}
	}

	private void OnSliderValueChanged(float val)
	{
		if (this.OnOffsetChanged != null)
		{
			this.OnOffsetChanged(val);
		}
	}

	public void Show(float sliderValue)
	{
		this._prevOffsetSliderVal = sliderValue;
		this._slider.@value = sliderValue;
		base.gameObject.SetActive(true);
	}
}