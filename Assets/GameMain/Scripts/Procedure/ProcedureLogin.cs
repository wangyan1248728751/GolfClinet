using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Golf
{
    public class ProcedureLogin : ProcedureBase
    {
        private iLogin _LoginForm = null;

        public bool LoginSuccess = false;
        public bool LoginExit = false;

        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        public bool enter = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            enter = false;
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            LoginExit = false;
            //LoginSuccess = false;

            GameEntry.WebRequestToServerComponent.ClearLoginMess();
            //GameEntry.UI.OpenUIForm(UIFormId.OldLoginForm, this);
            //GameEntry.UI.OpenUIForm(UIFormId.LoginForm, this);
            GameEntry.UI.OpenUIForm(UIFormId.QRCodeLoginForm, this);

        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (_LoginForm != null)
            {
                _LoginForm.CloseForm();
                _LoginForm = null;
            }
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            /*if (LoginSuccess)
            {
                //ChangeState<ProcedureMenu>(procedureOwner);
				procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.Main);
				ChangeState<ProcedureChangeScene>(procedureOwner);
			}*/

            if (LoginExit)
            {
                ChangeState<ProcedureMenu>(procedureOwner);
            }
            if (enter)
            {
                ChangeState<ProcedureFunction>(procedureOwner);

            }
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            _LoginForm = ne.UIForm.Logic as iLogin;
        }
    }

}
