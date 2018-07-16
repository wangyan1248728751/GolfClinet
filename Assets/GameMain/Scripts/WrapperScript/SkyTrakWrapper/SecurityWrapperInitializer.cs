using Security;
using System;
using UnityEngine;

public class SecurityWrapperInitializer : MonoBehaviour
{
	private static SecurityWrapperInitializer _instance;

	private SecurityWrapperService _securityWrapperService;

	private void Awake()
	{
		if (SecurityWrapperInitializer._instance != null)
		{
			Destroy(base.gameObject);
			return;
		}
		DontDestroyOnLoad(base.gameObject);
		SecurityWrapperInitializer._instance = this;
		this._securityWrapperService = new SecurityWrapperService();
		if (this._securityWrapperService == null)
		{
			throw new Exception("SecurityWrapperService not initialized");
		}
		this._securityWrapperService.Init();
	}

	private bool FirstStartApplication = true;

	private void OnApplicationPause(bool pauseStatus)
	{
		if (this._securityWrapperService != null)
		{
			this._securityWrapperService.OnApplicationPause(pauseStatus);
		}
	}

	private void OnDisable()
	{
		if (this._securityWrapperService != null)
		{
			this._securityWrapperService.OnDisable();
		}
	}

	private void OnEnable()
	{
		if (this._securityWrapperService != null)
		{
			this._securityWrapperService.OnEnable();
		}
	}

	private void Start()
	{
		if (this._securityWrapperService != null)
		{
			this._securityWrapperService.Start();
		}
	}

	private void Update()
	{
		if (this._securityWrapperService != null)
		{
			this._securityWrapperService.Update();
		}
	}
}