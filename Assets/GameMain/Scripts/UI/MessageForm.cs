using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using UnityGameFramework;
using UnityEngine.UI;
using GameFramework.DataTable;
using GameFramework.Event;
using DG.Tweening;
using UnityGameFramework.Runtime;
using System;

namespace Golf
{
    public class MessageForm : UGuiForm
    {
		[SerializeField]
		Text message = null;
        [SerializeField]
        Image BgLeft;
        [SerializeField]
        Image BgRight;

        Tweener myTweener;
        Tweener myTweener1;
        Tweener myTweener2;

        internal protected override void OnInit(object userData)
        {
            base.OnInit(userData);
			EventComponent _eventComponent = GameEntry.Event;
			_eventComponent.Subscribe(ShowMessageEventArgs.EventId, ShowMessage);
            message.color = Color.clear;
            BgLeft.color = Color.clear;
            BgRight.color = Color.clear;
        }

		public void ShowMessage(object sender, GameEventArgs e)
		{
			ShowMessageEventArgs ne = e as ShowMessageEventArgs;
			print(ne.Message);
            message.text = ne.Message;

			myTweener.Kill();
            myTweener1.Kill();
            myTweener2.Kill();

            if (string.IsNullOrEmpty(ne.Message))
            {
                message.color = Color.clear;
                BgLeft.color = Color.clear;
                BgRight.color = Color.clear;
                return;
            }

            message.color = Color.white;
            BgLeft.color = Color.white;
            BgRight.color = Color.white;

			myTweener = message.DOColor(Color.white, 1.5f).OnComplete(() =>
			{
				myTweener = message.DOColor(Color.clear, 0.5f);
			});
            myTweener1 = BgLeft.DOColor(Color.white, 1.5f).OnComplete(() =>
            {
                myTweener1 = BgLeft.DOColor(Color.clear, 0.5f);
            });
            myTweener2 = BgRight.DOColor(Color.white, 1.5f).OnComplete(() =>
            {
                myTweener2 = BgRight.DOColor(Color.clear, 0.5f);
            });
		}
	}

    public class ShowMessageEventArgs : GameEventArgs
    {
		public static readonly int EventId = typeof(ShowMessageEventArgs).GetHashCode();

		public ShowMessageEventArgs(string str)
        {
            _message = str;
        }
        string _message;

        public override int Id
        {
            get
            {
				return EventId;
			}
        }

        public string Message
        {
            get{
                return _message;
            }
        }

		public override void Clear()
		{

		}
	}


}
