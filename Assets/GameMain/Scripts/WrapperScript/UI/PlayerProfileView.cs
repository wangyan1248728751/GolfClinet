using Data;
using Security;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerProfileView : MonoBehaviour
{
	private const string LogOut = "LOG OUT";

	private const string LogIn = "LOG IN";

	private const string DefaultName = "UNREGISTERED\nUSER";

	private const string DefaultLocation = "LOCATION";

	private const string SyncInProgressString = "DATA SYNCING...";

	private const string SyncIsDoneString = "DATA SYNCED";

	private const string SyncErrorString = "DATA SYNC ERROR";

	[SerializeField]
	private Text _logOutButtonLabel;

	[SerializeField]
	private Text _syncStatusLabel;

	[SerializeField]
	private Text _userNameLabel;

	[SerializeField]
	private Slider _handToggle;

	[SerializeField]
	private Slider _distanceUnitsToggle;

	[SerializeField]
	private Slider _speedUnitsToggle;

	[SerializeField]
	//private LoginScreenView _loginScreen;

	private Animator _animator;

	public GameObject SyncInProgress;

	public GameObject SyncNotDone;

	public GameObject SyncError;

	public GameObject SyncDone;

	private float _syncStateChangeTime;

	private SynchronizationState _lastState;

	private bool _syncStateUpdated;

	private const float StateChangeDelay = 6f;

	public PlayerProfileView()
	{
	}

	private void Awake()
	{
		//this._animator = base.GetComponent<Animator>();
		//this._handToggle.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => {
		//	this._handToggle.@value = (this._handToggle.@value < 1f ? 1f : 0f);
		//	this.OnHandToggleChanged();
		//});
		//this._distanceUnitsToggle.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => {
		//	this._distanceUnitsToggle.@value = (this._distanceUnitsToggle.@value < 1f ? 1f : 0f);
		//	this.DistanceUnitsToggleChanged();
		//});
		//this._speedUnitsToggle.GetComponent<EventTrigger>().triggers[0].callback.AddListener((BaseEventData arg0) => {
		//	this._speedUnitsToggle.@value = (this._speedUnitsToggle.@value < 1f ? 1f : 0f);
		//	this.SpeedUnitsToggleChanged();
		//});
		SecurityWrapperService.Instance.OnConnected += new Action(this.OnDeviceConnected);
	}

	private void ChangeSyncValue(SynchronizationState syncState)
	{
		this.SyncInProgress.gameObject.SetActive(false);
		this.SyncNotDone.gameObject.SetActive(false);
		this.SyncDone.gameObject.SetActive(false);
		this.SyncError.gameObject.SetActive(false);
		switch (syncState)
		{
			case SynchronizationState.Waiting:
			case SynchronizationState.Done:
				{
					this.SyncDone.gameObject.SetActive(true);
					this._syncStatusLabel.text = "DATA SYNCED";
					break;
				}
			case SynchronizationState.Pushing:
			case SynchronizationState.InProgress:
			case SynchronizationState.Pulling:
				{
					this.SyncInProgress.gameObject.SetActive(true);
					this._syncStatusLabel.text = "DATA SYNCING...";
					break;
				}
			case SynchronizationState.NotPossible:
				{
					this.SyncNotDone.gameObject.SetActive(true);
					break;
				}
			case SynchronizationState.Error:
				{
					this.SyncError.gameObject.SetActive(true);
					this._syncStatusLabel.text = "DATA SYNC ERROR";
					break;
				}
		}
	}

	private void DistanceUnitsToggleChanged()
	{
		UnitsConverter.Instance.IsMetricsEnabled = this._distanceUnitsToggle.@value <= 0f;
	}

	private void OnDestroy()
	{
		SecurityWrapperService.Instance.OnConnected -= new Action(this.OnDeviceConnected);
	}

	private void OnDeviceConnected()
	{
		SecurityWrapperService.Instance.SetHandedness(ApplicationDataManager.instance.GetPlayerHandednessIsLefty());
	}

	private void OnHandToggleChanged()
	{
		this.UpdateHand(this._handToggle.@value <= 0f);
	}

	public void OnLogOutButtonClick()
	{
		this._animator.Play("ProfilePanelIsHidden");
		base.Invoke("ShowLoginScreenAndLogout", 0.05f);
	}

	public void OpenUrlClicked()
	{
	}

	public void Show()
	{
		bool playerHandednessIsLefty = ApplicationDataManager.instance.GetPlayerHandednessIsLefty();
		if (LoginManager.IsUserLoggedIn)
		{
			this._logOutButtonLabel.text = "LOG OUT";
			if (LoginManager.IsUserLoggedIn)
			{
				string firstName = LoginManager.UserData.FirstName;
				string lastName = LoginManager.UserData.LastName;
				if (firstName == null || firstName.Trim() == string.Empty)
				{
					firstName = LoginManager.UserData.UserName;
					if (firstName == null || firstName.Trim() == string.Empty)
					{
						Debug.Log("User name returned blank.");
						firstName = "Error getting name from DB.";
					}
				}
				this._userNameLabel.text = string.Concat(firstName.ToUpper(), " ", (!string.IsNullOrEmpty(lastName) ? lastName.ToUpper() : string.Empty));
				if (ApplicationDataManager.instance.WeatherLocationText.Trim() == string.Empty)
				{
				}
				this._handToggle.@value = (!playerHandednessIsLefty ? 1f : 0f);
				this._distanceUnitsToggle.@value = (!UnitsConverter.Instance.IsMetricsEnabled ? 1f : 0f);
				this._speedUnitsToggle.@value = (!UnitsConverter.Instance.IsKPHEnabled ? 1f : 0f);
			}
		}
		else
		{
			this._logOutButtonLabel.text = "LOG IN";
			this._userNameLabel.text = "UNREGISTERED\nUSER";
			this._handToggle.@value = (!playerHandednessIsLefty ? 1f : 0f);
			this._distanceUnitsToggle.@value = (!UnitsConverter.Instance.IsMetricsEnabled ? 1f : 0f);
			this._speedUnitsToggle.@value = (!UnitsConverter.Instance.IsKPHEnabled ? 1f : 0f);
		}
	}

	private void ShowLoginScreenAndLogout()
	{
		//this._loginScreen.GetComponent<Animator>().Play("Show");
		//this._loginScreen.LogOut();
	}

	private void SpeedUnitsToggleChanged()
	{
		UnitsConverter.Instance.IsKPHEnabled = this._speedUnitsToggle.@value <= 0f;
	}

	private void Start()
	{
		//SynchronizationState state = SynchronizationManager.State;
		//SynchronizationState synchronizationState = state;
		//this._lastState = state;
		//this.ChangeSyncValue(synchronizationState);
	}

	private void Update()
	{
		this.UpdateSyncValue();
	}

	private void UpdateHand(bool isLeftHanded)
	{
		if (ApplicationDataManager.instance != null)
		{
			ApplicationDataManager.instance.SetPlayerHandednessIsLefty(isLeftHanded, true);
		}
		SecurityWrapperService.Instance.SetHandedness(isLeftHanded);
	}

	private void UpdateSyncValue()
	{
		//if (this._lastState != SynchronizationManager.State)
		//{
		//	this._lastState = SynchronizationManager.State;
		//	this._syncStateChangeTime = Time.unscaledTime;
		//	this._syncStateUpdated = false;
		//}
		//if (!this._syncStateUpdated && Time.unscaledTime - this._syncStateChangeTime > 6f)
		//{
		//	this.ChangeSyncValue(SynchronizationManager.State);
		//	this._syncStateUpdated = true;
		//}
	}
}