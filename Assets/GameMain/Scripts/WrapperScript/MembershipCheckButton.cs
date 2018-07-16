using Security;
using SkyTrakWrapper;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MembershipCheckButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private STSWMMSFeatureFlagType _membershipFlag;

	private Button _button;

	private bool IsMMSReady
	{
		get
		{
			return (SecurityWrapperService.Instance == null ? false : SecurityWrapperService.Instance.IsMMSReady);
		}
	}

	public MembershipCheckButton()
	{
	}

	private void Awake()
	{
		this._button = base.GetComponent<Button>();
		MembershipManager.OnMMSUpdated += new Action(this.OnMmsUpdated);
	}

	private bool CheckAccess()
	{
		if (!this.IsMMSReady)
		{
			return false;
		}
		return MembershipManager.GetAccess(this._membershipFlag);
	}

	private void OnDestroy()
	{
		MembershipManager.OnMMSUpdated -= new Action(this.OnMmsUpdated);
	}

	private void OnMmsUpdated()
	{
		this._button.interactable = this.CheckAccess();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!this.CheckAccess())
		{
			//MonoSingleton<UIData>.Singleton.UpgradeRequiredWindowData.Show(base.gameObject, null, null, 3f);
		}
	}

	private void Start()
	{
		this.OnMmsUpdated();
	}
}