using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class CameraAnglePopup : Dropdown
{
	private Button _adjustOffsetButton;

	private GameObject _adjustOffsetCheckmark;

	public Action OnAdjustOffsetButton;

	private bool _isCheckmarkActive;

	private bool _isButtonActive;

	public CameraAnglePopup()
	{
	}

	public void ChangeAvailabilityAdjustOffsetButton(bool enable)
	{
		this._isButtonActive = enable;
	}

	protected override GameObject CreateDropdownList(GameObject template)
	{
		GameObject gameObject = base.CreateDropdownList(template);
		this._adjustOffsetButton = gameObject.transform.Find("CameraOffsetButton").GetComponent<Button>();
		this._adjustOffsetButton.onClick.AddListener(new UnityAction(this.OnAdjustOffsetButtonClick));
		this._adjustOffsetButton.interactable = this._isButtonActive;
		this._adjustOffsetCheckmark = this._adjustOffsetButton.transform.Find("Item Checkmark").gameObject;
		this._adjustOffsetCheckmark.SetActive(this._isCheckmarkActive);
		return gameObject;
	}

	private void OnAdjustOffsetButtonClick()
	{
		base.Hide();
		if (this.OnAdjustOffsetButton != null)
		{
			this.OnAdjustOffsetButton();
		}
	}

	public void SetBottomButtonMark(bool visible)
	{
		this._isCheckmarkActive = visible;
	}
}