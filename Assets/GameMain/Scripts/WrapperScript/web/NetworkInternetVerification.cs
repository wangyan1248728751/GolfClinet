using System;
using UnityEngine;

public class NetworkInternetVerification
{
	private GameObject netGameObject;

	private InternetReachabilityVerifier _verifier;

	private GameObject spriteRedX;

	private static NetworkInternetVerification singleton;

	public static NetworkInternetVerification instance
	{
		get
		{
			if (NetworkInternetVerification.singleton == null)
			{
				NetworkInternetVerification.singleton = new NetworkInternetVerification();
			}
			return NetworkInternetVerification.singleton;
		}
	}

	static NetworkInternetVerification()
	{
	}

	private NetworkInternetVerification()
	{
		this.CreateNetIRVObject();
	}

	private void CreateNetIRVObject()
	{
		this.netGameObject = new GameObject()
		{
			name = "InternetVerificationObject"
		};
		this._verifier = this.netGameObject.AddComponent<InternetReachabilityVerifier>();
	}

	private bool NetworksPresent()
	{
		return (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ? true : Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork);
	}

	public void Reset()
	{
		Debug.Log("*** RESET IVR ***");
		this._verifier.Stop();
		this._verifier.StartCheckAvailability();
	}

	private void SetRedXActive(bool isActive)
	{
		if (this.spriteRedX != null)
		{
			this.spriteRedX.SetActive(isActive);
		}
	}

	public NetworkInternetVerification.NET_STATES VerifyInternetAvailability()
	{
		//if (!this.NetworksPresent())
		//{
		//	this.SetRedXActive(true);
		//	return NetworkInternetVerification.NET_STATES.NETWORKS_UNAVAILABLE;
		//}
		//if (InternetReachabilityVerifier.Instance.status == InternetReachabilityVerifier.Status.NetVerified)
		//{
		//	this.SetRedXActive(false);
			return NetworkInternetVerification.NET_STATES.VERIFIED_ONLINE;
		//}
		//if (InternetReachabilityVerifier.Instance.status == InternetReachabilityVerifier.Status.Offline)
		//{
		//	this.SetRedXActive(true);
		//	return NetworkInternetVerification.NET_STATES.VERIFIED_OFFLINE;
		//}
		//this.SetRedXActive(true);
		//return NetworkInternetVerification.NET_STATES.WAITING;
	}

	public enum NET_STATES
	{
		WAITING,
		VERIFIED_ONLINE,
		VERIFIED_OFFLINE,
		NETWORKS_UNAVAILABLE
	}
}