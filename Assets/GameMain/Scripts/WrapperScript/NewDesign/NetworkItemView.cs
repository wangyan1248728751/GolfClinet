using SkyTrakWrapper;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SkyTrak.NewDesign
{
	public class NetworkItemView : MonoBehaviour
	{
		[SerializeField]
		private Text _networkName;

		[SerializeField]
		private Text _securityType;

		[SerializeField]
		private Image _networkStrengthSprite;

		//private UIEventListener _itemButton;

		private ManualNetworkParams _networkParams;

		public Action<ManualNetworkParams> OnClick;

		public ManualNetworkParams NetworkParams
		{
			get
			{
				return this._networkParams;
			}
		}

		public NetworkItemView()
		{
		}

		public void OnItemClick()
		{
			if (this.OnClick != null)
			{
				this.OnClick(this._networkParams);
			}
		}

		public void UpdateItem(ManualNetworkParams networkParams)
		{
			this._networkParams = networkParams;
			this._networkName.text = this._networkParams.NetParams.SSID;
			this._securityType.text = this._networkParams.NetParams.securityType;
			this._networkStrengthSprite.fillAmount = this._networkParams.NetParams.signalLevel / 100f;
		}
	}
}