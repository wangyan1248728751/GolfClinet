using System.Collections;
using System.Collections.Generic;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Golf
{
    public class ProcedureFunction : ProcedureBase
    {
       
        public bool Loginsucess = false;
        public bool quit = false;
        private FunctionForm _functionForm=null;
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            Loginsucess = false;
            quit = false;
            GameEntry.UI.OpenUIForm(UIFormId.FunctionForm, this);
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (Loginsucess)
            {
                //ChangeState<ProcedureMenu>(procedureOwner);
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.Main);
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
            if (quit)
            {
                ChangeState<ProcedureMenu>(procedureOwner);
            }
        }
        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            _functionForm = ne.UIForm.Logic as FunctionForm;
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (_functionForm != null)
            {
                _functionForm.Close(true);
                _functionForm = null;
            }
        }

    }
}
