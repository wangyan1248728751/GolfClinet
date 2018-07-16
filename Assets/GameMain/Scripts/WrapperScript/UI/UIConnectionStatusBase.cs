using Security;
using System;
using UnityEngine;

public class UIConnectionStatusBase : MonoBehaviour
{
	[SerializeField]
	protected Color notConnectedColor;

	[SerializeField]
	protected Color licenseCheckingColor;

	[SerializeField]
	protected Color connectedColor;

	private const float UpdateStatusInterval = 0.1f;

	private float _lastUpdateTime;

	private UIConnectionStatusBase.DeviceConnectionStatus _currentStatus;

	public UIConnectionStatusBase()
	{
	}

	private void Update()
	{
		if (Time.time - this._lastUpdateTime > 0.1f)
		{
			this.UpdateStatus();
			this._lastUpdateTime = Time.time;
		}
	}

	protected virtual void UpdateConnectedState()
	{
	}

	protected virtual void UpdateDiscoveringState()
	{
	}

	protected virtual void UpdateLicenseCheckingState()
	{
	}

	protected virtual void UpdateNotConnectedState()
	{
	}

	private void UpdateStatus()
	{
		if (SecurityWrapperService.Instance.IsDiscovering)
		{
			if (this._currentStatus != UIConnectionStatusBase.DeviceConnectionStatus.Discovering)
			{
				this.UpdateDiscoveringState();
				this._currentStatus = UIConnectionStatusBase.DeviceConnectionStatus.Discovering;
			}
		}
		else if (SecurityWrapperService.Instance.IsLicenseChecked)
		{
			if (this._currentStatus != UIConnectionStatusBase.DeviceConnectionStatus.Connected)
			{
				this.UpdateConnectedState();
				this._currentStatus = UIConnectionStatusBase.DeviceConnectionStatus.Connected;
			}
		}
		else if (SecurityWrapperService.Instance.IsLicenseChecking)
		{
			if (this._currentStatus != UIConnectionStatusBase.DeviceConnectionStatus.LicenseChecking)
			{
				this.UpdateLicenseCheckingState();
				this._currentStatus = UIConnectionStatusBase.DeviceConnectionStatus.LicenseChecking;
			}
		}
		else if (this._currentStatus != UIConnectionStatusBase.DeviceConnectionStatus.NotConnected)
		{
			this.UpdateNotConnectedState();
			this._currentStatus = UIConnectionStatusBase.DeviceConnectionStatus.NotConnected;
		}
	}

	private enum DeviceConnectionStatus
	{
		NotConnected,
		Discovering,
		LicenseChecking,
		Connected
	}
}