using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.DataTable;

namespace Golf
{
	public class UserInfoForm : UGuiForm
	{
		[SerializeField]
		GameObject mask;
		[SerializeField]
		GameObject baseItem;
		[SerializeField]
		GameObject exitBtn;
		[SerializeField]
		GameObject DetailView;
		[SerializeField]
		Image detailImage;
		[SerializeField]
		GameObject exitDViewBtn;
		[SerializeField]
		GameObject deleteGoodsBtn;
        [SerializeField]
        GameObject QRCode;
        [SerializeField]
        GameObject ExitBtn;

		//[SerializeField]
		Text _id;
		[SerializeField]
		Text _name;
		[SerializeField]
		InputField ReName;
		[SerializeField]
		GameObject changeNameBtn;
		[SerializeField]
		GameObject rightBtn;

		public List<Goods> goodsDatas = new List<Goods>();
		public List<BagBaseItem> items = new List<BagBaseItem>();

		protected internal override void OnInit(object userData)
		{
			base.OnInit(userData);
			mask.AddClick(CloseForm);
			exitBtn.AddClick(CloseForm);
            ExitBtn.AddClick(CloseDetailView);

			changeNameBtn.AddClick(ChangeNameOnclick);
			rightBtn.AddClick(ChangeNameRightOnclick);
			GameEntry.Event.Subscribe(HitBallEndEventArgs.EventId, HitBallEnd);
            
        }

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            GameEntry.UI.OpenUIForm(UIFormId.MessageForm);
            _name.text = GameEntry.GameData.WxName;
            GetUserBag();
            //AddGoods(272);
            //DeleteGoods(272);
        }

        protected internal override void OnClose(object userData)
        {
            GameEntry.Event.Unsubscribe(HitBallEndEventArgs.EventId, HitBallEnd);
            base.OnClose(userData);
        }

        void GetUserBag()
        {
            m2s_getacticlebyuid msg = new m2s_getacticlebyuid();

            M2SInfo m2sInfo = new M2SInfo(msg, GetUserBagSuccess, WebRequestFail);
            GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
        }

        void ChangeNameOnclick()
		{
			_name.gameObject.SetActive(false);
			changeNameBtn.SetActive(false);
			ReName.gameObject.SetActive(true);

		}

		void ChangeNameRightOnclick()
		{
			//改名网络链接
			ChangeUserName();
			_name.gameObject.SetActive(true);
			changeNameBtn.SetActive(true);
			ReName.gameObject.SetActive(false);
			_name.text = ReName.text;
		}

		void AddGoods(int goodsId)
		{
			m2s_exchangeshop msg = new m2s_exchangeshop();
			msg.shopid = goodsId;
			M2SInfo m2sInfo = new M2SInfo(msg, AddGoodsSuccess, WebRequestFail);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		void DeleteGoods(int goodsId)
		{
			DetailView.SetActive(true);
			m2s_updateacticlenum msg = new m2s_updateacticlenum();
			msg.id = 0;//固定传0
			msg.acticle = new Goods();
			msg.acticle.sid = goodsId;
			msg.acticle.uid = GameEntry.WebRequestToServerComponent.userid;
			msg.acticle.num = -1;//固定传-1
			msg.acticle.enable = 0;//固定传0
            QRCode.SetActive(false);
            DetailView.SetActive(false);
            M2SInfo m2sInfo = new M2SInfo(msg, DeleteGoodsSuccess, WebRequestFail);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		/// <summary>
		/// 改名
		/// </summary>
		void ChangeUserName()
		{
			m2s_updatenickname msg = new m2s_updatenickname();
			msg.nickname = ReName.text;
			M2SInfo m2sInfo = new M2SInfo(msg, ChangeUserNameSuccess, WebRequestFail);
			GameEntry.WebRequestToServerComponent.SendJsonMsg(m2sInfo);
		}

		void RefreshItems()
		{
			foreach(BagBaseItem item in items)
			{
				Destroy(item.gameObject);
			}
			items.Clear();

			for(int i = 0; i<goodsDatas.Count;i++)
			{
				GameObject go = Instantiate(baseItem, baseItem.transform.parent.transform);
				go.SetActive(true);
				BagBaseItem item = go.GetComponent<BagBaseItem>();
				item.Init(goodsDatas[i]);
				go.AddClick(OpenDetailView,i);
				items.Add(item);
			}

		}

		void OpenDetailView(int goodsDataIndex)
		{
			//Debug.Log("index:"+ goodsDataIndex);
			DetailView.SetActive(true);
			detailImage.sprite = items[goodsDataIndex].image.sprite;
            exitDViewBtn.AddClick(DeleteGoods,items[goodsDataIndex].goodsData.sid);
            deleteGoodsBtn.AddClick(viewQR);
		}
       
        void viewQR()
        {
         QRCode.SetActive(true);
        }
   
		void CloseForm()
		{
			Close();
		}
		void CloseDetailView()
		{
			DetailView.SetActive(false);
           
        }

		void HitBallEnd(object sender, GameEventArgs e)
		{
			//HitBallEndEventArgs ne = (HitBallEndEventArgs)e;

			//if(GameEntry.GameData.currGolfball[GameEntry.GameData.currGolfball.Count-1].rewardMapId !=0)
			//{
			//	GameEntry.GameData.webRewardMap[]
			//}
		}

		void GetUserBagSuccess(object obj)
		{
			m2c_getacticlebyuid msg = (m2c_getacticlebyuid)obj;
			goodsDatas = msg.goodsList;
			RefreshItems();
		}

		void AddGoodsSuccess(object obj)
		{

		}

		void DeleteGoodsSuccess(object obj)
		{
			GameEntry.Event.Fire(this, new ShowMessageEventArgs("核销成功!"));
			DetailView.SetActive(false);
			GetUserBag();
		}

		private void ChangeUserNameSuccess(object obj)
		{
			GameEntry.Event.Fire(this, new ShowMessageEventArgs("更名成功!"));
			GameEntry.GameData.userData.name = ReName.text;
		}



		private void WebRequestFail(object obj)
		{
			Debug.Log("Refresh RankListDatas Fail");
			GameEntry.Event.Fire(this, new ShowMessageEventArgs("网络异常!"));
			Close();
		}
	}

	public class Goods
	{
		/// <summary>
		/// 服务器数据库表的id
		/// </summary>
		public int id;
		/// <summary>
		/// 用户id
		/// </summary>
		public string uid;
		/// <summary>
		/// 商品id
		/// </summary>
		public int sid;
		/// <summary>
		/// 数量
		/// </summary>
		public int num;
		/// <summary>
		/// 图片地址
		/// </summary>
		public string url;
		public int enable;
	}


}
