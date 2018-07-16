using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class InternetReachabilityVerifier : MonoBehaviour
{
	public InternetReachabilityVerifier.CaptivePortalDetectionMethod captivePortalDetectionMethod;

	public string customMethodURL = string.Empty;

	public string customMethodExpectedData = "OK";

	public InternetReachabilityVerifier.CustomMethodVerifierDelegate customMethodVerifierDelegate;

	private InternetReachabilityVerifier.Status _status;

	private string _lastError = string.Empty;

	private static InternetReachabilityVerifier _instance;

	private static RuntimePlatform[] methodGoogle204Supported;

	private const InternetReachabilityVerifier.CaptivePortalDetectionMethod fallbackMethodIfNoDefaultByPlatform = InternetReachabilityVerifier.CaptivePortalDetectionMethod.MicrosoftNCSI;

	private WaitForSeconds defaultReachabilityCheckPeriod = new WaitForSeconds(5f);

	private WaitForSeconds netVerificationMismatchRetryTime = new WaitForSeconds(7f);

	private bool netActivityRunning;

	public static InternetReachabilityVerifier Instance
	{
		get
		{
			return InternetReachabilityVerifier._instance;
		}
	}

	public string lastError
	{
		get
		{
			return this._lastError;
		}
		set
		{
			this._lastError = value;
		}
	}

	public InternetReachabilityVerifier.Status status
	{
		get
		{
			return this._status;
		}
		set
		{
			this._status = value;
			if (this.statusChangedDelegate != null)
			{
				this.statusChangedDelegate(value);
			}
		}
	}

	static InternetReachabilityVerifier()
	{
		InternetReachabilityVerifier._instance = null;
		//InternetReachabilityVerifier.methodGoogle204Supported = new RuntimePlatform[] { typeof(< PrivateImplementationDetails >).GetField("$field-A521C39CEF3283C4ED1AFD0A913B091FAFDDFD6D").FieldHandle };
	}

	public InternetReachabilityVerifier()
	{
	}

	private void Awake()
	{
		if (InternetReachabilityVerifier._instance != null)
		{
			DestroyImmediate(base.gameObject);
			return;
		}
		InternetReachabilityVerifier._instance = this;
		DontDestroyOnLoad(base.gameObject);
	}

	private bool checkCaptivePortalDetectionResult(InternetReachabilityVerifier.CaptivePortalDetectionMethod cpdm, WWW www)
	{
		if (www == null)
		{
			return false;
		}
		if (www.error != null && www.error.Length > 0)
		{
			return false;
		}
		switch (cpdm)
		{
			case InternetReachabilityVerifier.CaptivePortalDetectionMethod.Google204:
				{
					Dictionary<string, string> strs = www.responseHeaders;
					string empty = string.Empty;
					if (strs.ContainsKey("STATUS"))
					{
						empty = strs["STATUS"];
					}
					else if (strs.ContainsKey("NULL"))
					{
						empty = strs["NULL"];
					}
					if (empty.Length > 0)
					{
						if (empty.IndexOf("204 No Content") >= 0)
						{
							return true;
						}
					}
					else if (www.size == 0)
					{
						return true;
					}
					break;
				}
			case InternetReachabilityVerifier.CaptivePortalDetectionMethod.GoogleBlank:
				{
					if (www.size == 0)
					{
						return true;
					}
					break;
				}
			case InternetReachabilityVerifier.CaptivePortalDetectionMethod.MicrosoftNCSI:
				{
					if (www.text.StartsWith("Microsoft NCSI"))
					{
						return true;
					}
					break;
				}
			case InternetReachabilityVerifier.CaptivePortalDetectionMethod.Apple:
				{
					if (www.text.IndexOf("<BODY>Success</BODY>") < 50)
					{
						return true;
					}
					break;
				}
			case InternetReachabilityVerifier.CaptivePortalDetectionMethod.Ubuntu:
				{
					if (www.text.IndexOf("Lorem ipsum dolor sit amet") == 109)
					{
						return true;
					}
					break;
				}
			case InternetReachabilityVerifier.CaptivePortalDetectionMethod.Custom:
				{
					if (this.customMethodVerifierDelegate != null)
					{
						return this.customMethodVerifierDelegate(www, this.customMethodExpectedData);
					}
					if (www.text.StartsWith(this.customMethodExpectedData))
					{
						return true;
					}
					break;
				}
		}
		return false;
	}

	private string getCaptivePortalDetectionURL(InternetReachabilityVerifier.CaptivePortalDetectionMethod cpdm)
	{
		if (cpdm == InternetReachabilityVerifier.CaptivePortalDetectionMethod.Custom)
		{
			return this.customMethodURL;
		}
		if (cpdm == InternetReachabilityVerifier.CaptivePortalDetectionMethod.Google204)
		{
			return "http://clients3.google.com/generate_204";
		}
		if (cpdm == InternetReachabilityVerifier.CaptivePortalDetectionMethod.MicrosoftNCSI)
		{
			return "http://www.msftncsi.com/ncsi.txt";
		}
		if (cpdm == InternetReachabilityVerifier.CaptivePortalDetectionMethod.GoogleBlank)
		{
			return "http://www.google.com/blank.html";
		}
		if (cpdm == InternetReachabilityVerifier.CaptivePortalDetectionMethod.Apple)
		{
			return "http://www.google.com/blank.html";
		}
		if (cpdm == InternetReachabilityVerifier.CaptivePortalDetectionMethod.Ubuntu)
		{
			return "http://start.ubuntu.com/connectivity-check";
		}
		return string.Empty;
	}

	[DebuggerHidden]
	private IEnumerator netActivity()
	{
		//InternetReachabilityVerifier.< netActivity > c__Iterator0 variable = null;
		//return variable;
		yield return 0;
	}

	public void setNetActivityTimes(float defaultReachabilityCheckPeriodSeconds, float netVerificationErrorRetryTimeSeconds, float netVerificationMismatchRetryTimeSeconds)
	{
		this.defaultReachabilityCheckPeriod = new WaitForSeconds(defaultReachabilityCheckPeriodSeconds);
		this.netVerificationMismatchRetryTime = new WaitForSeconds(netVerificationMismatchRetryTimeSeconds);
	}

	private void Start()
	{
		if (this.captivePortalDetectionMethod == InternetReachabilityVerifier.CaptivePortalDetectionMethod.DefaultByPlatform)
		{
			this.captivePortalDetectionMethod = InternetReachabilityVerifier.CaptivePortalDetectionMethod.MicrosoftNCSI;
			if (this.captivePortalDetectionMethod == InternetReachabilityVerifier.CaptivePortalDetectionMethod.DefaultByPlatform)
			{
				this.captivePortalDetectionMethod = InternetReachabilityVerifier.CaptivePortalDetectionMethod.MicrosoftNCSI;
			}
		}
		if (this.captivePortalDetectionMethod == InternetReachabilityVerifier.CaptivePortalDetectionMethod.Google204 && Array.IndexOf<RuntimePlatform>(InternetReachabilityVerifier.methodGoogle204Supported, Application.platform) < 0)
		{
			this.captivePortalDetectionMethod = InternetReachabilityVerifier.CaptivePortalDetectionMethod.GoogleBlank;
		}
		if (this.captivePortalDetectionMethod != InternetReachabilityVerifier.CaptivePortalDetectionMethod.Custom || this.customMethodURL.Length != 0)
		{
			base.StartCoroutine(this.netActivity());
			return;
		}
		UnityEngine.Debug.LogError("IRV - Custom method is selected but URL is empty, cannot start! (disabling component)", this);
		base.enabled = false;
	}

	public void StartCheckAvailability()
	{
		base.StartCoroutine(this.netActivity());
	}

	public void Stop()
	{
		base.StopAllCoroutines();
		this._status = InternetReachabilityVerifier.Status.Offline;
	}

	public event InternetReachabilityVerifier.StatusChangedDelegate statusChangedDelegate;

	public enum CaptivePortalDetectionMethod
	{
		DefaultByPlatform,
		Google204,
		GoogleBlank,
		MicrosoftNCSI,
		Apple,
		Ubuntu,
		Custom
	}

	public delegate bool CustomMethodVerifierDelegate(WWW www, string customMethodExpectedData);

	public enum Status
	{
		Offline,
		PendingVerification,
		Error,
		Mismatch,
		NetVerified
	}

	public delegate void StatusChangedDelegate(InternetReachabilityVerifier.Status newStatus);
}