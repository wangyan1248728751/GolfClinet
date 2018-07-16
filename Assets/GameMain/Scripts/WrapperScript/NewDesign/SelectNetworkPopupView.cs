using SkyTrakWrapper;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SkyTrak.NewDesign
{
	public class SelectNetworkPopupView : MonoBehaviour
	{
		private readonly string[] _securityTypes = new string[] { "OPEN", "WEP", "WEP SHARED", "WPA", "WPA2 AES", "WPA2 Mixed", "WPA2 TKIP" };

		[SerializeField]
		private GameObject _scanningProcessImage;

		[SerializeField]
		private Transform _networkItemPlace;

		[SerializeField]
		private GameObject _networkItemPrefab;

		[SerializeField]
		private GameObject _manuallyAddNetworkButtonGO;

		[SerializeField]
		private GameObject _cancelScanButtonGO;

		[SerializeField]
		private InputField _networkNameInput;

		[SerializeField]
		private InputField _networkPasswordInput;

		[SerializeField]
		private Dropdown _securityTypeDropdown;

		[SerializeField]
		private Button _closeButton;

		private readonly List<NetworkItemView> _networkList = new List<NetworkItemView>();

		public Action<ManualNetworkParams> OnNetworkSelected;

		public Action OnScanCanceled;

		public SelectNetworkPopupView()
		{
		}

		private void AddNetworkItemView(ManualNetworkParams network)
		{
			NetworkItemView networkItemView = this.CheckIfNetworkAlreadyInList(network);
			if (networkItemView != null)
			{
				networkItemView.UpdateItem(network);
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this._networkItemPrefab, this._networkItemPlace);
			NetworkItemView component = gameObject.GetComponent<NetworkItemView>();
			component.UpdateItem(network);
			component.OnClick = new Action<ManualNetworkParams>(this.OnNetworkItemClick);
			gameObject.SetActive(true);
			this._networkList.Add(component);
		}

		private NetworkItemView CheckIfNetworkAlreadyInList(ManualNetworkParams network)
		{
			NetworkItemView networkItemView;
			List<NetworkItemView>.Enumerator enumerator = this._networkList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					NetworkItemView current = enumerator.Current;
					if (!current.NetworkParams.NetParams.SSID.Equals(network.NetParams.SSID) || !current.NetworkParams.NetParams.securityType.Equals(network.NetParams.securityType))
					{
						continue;
					}
					networkItemView = current;
					return networkItemView;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return networkItemView;
		}

		private void ClearPreviousResult()
		{
			foreach (NetworkItemView networkItemView in this._networkList)
			{
				UnityEngine.Object.Destroy(networkItemView.gameObject);
			}
			this._networkList.Clear();
		}

		public void OnAddNetworkClick()
		{
			if (string.IsNullOrEmpty(this._networkNameInput.text))
			{
				this._networkNameInput.GetComponent<Image>().color = new Color(1f, 0.7f, 0.7f, 1f);
				return;
			}
			RIPENetworkScanListParamsType rIPENetworkScanListParamsType = new RIPENetworkScanListParamsType()
			{
				SSID = this._networkNameInput.text,
				securityType = this._securityTypeDropdown.options[this._securityTypeDropdown.@value].text,
				signalLevel = 50f
			};
			RIPENetworkScanListParamsType rIPENetworkScanListParamsType1 = rIPENetworkScanListParamsType;
			ManualNetworkParams manualNetworkParam = new ManualNetworkParams()
			{
				NetParams = rIPENetworkScanListParamsType1,
				Password = this._networkPasswordInput.text
			};
			this.AddNetworkItemView(manualNetworkParam);
		}

		public void OnCancelScanButtonClick()
		{
			this.ShowHideScanningProcess(false);
			if (this.OnScanCanceled != null)
			{
				this.OnScanCanceled();
			}
		}

		private void OnCloseButtonClick()
		{
			this.Show(false);
		}

		private void OnNetworkItemClick(ManualNetworkParams netParams)
		{
			if (this.OnNetworkSelected != null)
			{
				this.OnNetworkSelected(netParams);
			}
			this.Show(false);
		}

		public void SetNetworkList(List<RIPENetworkScanListParamsType> networkList)
		{
			this.ShowHideScanningProcess(false);
			if (networkList == null)
			{
				return;
			}
			foreach (RIPENetworkScanListParamsType rIPENetworkScanListParamsType in networkList)
			{
				ManualNetworkParams manualNetworkParam = new ManualNetworkParams()
				{
					NetParams = rIPENetworkScanListParamsType,
					Password = null
				};
				this.AddNetworkItemView(manualNetworkParam);
			}
		}

		public void Show(bool show)
		{
			base.gameObject.SetActive(show);
		}

		private void ShowHideScanningProcess(bool show)
		{
			this._scanningProcessImage.SetActive(show);
			this._cancelScanButtonGO.gameObject.SetActive(show);
			this._manuallyAddNetworkButtonGO.SetActive(!show);
			this._closeButton.interactable = !show;
		}

		public void ShowScanNetwork()
		{
			this.Show(true);
			this.ClearPreviousResult();
			this.ShowHideScanningProcess(true);
		}

		private void Start()
		{
			List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
			string[] strArrays = this._securityTypes;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				optionDatas.Add(new Dropdown.OptionData(strArrays[i]));
			}
			this._securityTypeDropdown.options = optionDatas;
			this._closeButton.onClick.AddListener(new UnityAction(this.OnCloseButtonClick));
		}
	}
}