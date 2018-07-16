using System.Collections.Generic;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Golf
{
    public class ProcedureMain : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

		public bool closeGame = false;

        public void GotoMenu()
        {
            
        }

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);

        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);

        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.GameCore.GameInit();
            GameEntry.GameCore.GameStart();
			GameEntry.UI.OpenUIForm(UIFormId.UserForm, this);
		}
       
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
			closeGame = false;
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
			if(closeGame)
			{
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.Menu);
                ChangeState<ProcedureChangeScene>(procedureOwner);
                ChangeState<ProcedureFunction>(procedureOwner);
            }
        }
    }
}
