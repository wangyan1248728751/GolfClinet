using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

namespace Golf
{
    public class IMsgToClient
    {
        public string resultCode;
        public string resultDesc;
    }
    public interface IMsgBase
    {
        string msgName { set; get; }
        string uid { set; get; }
        string rid { set; get; }
        string session { set; get; }
        void MsgAnalysis(JsonData res,Action<object> action);
    }
    public class M2SInfo
    {
        public M2SInfo(IMsgBase ibase, Action<object> SuccessAc, Action<object> FailureAc,bool ignoreConnect=false)
        {
            msg = ibase;
            ignoreConnectStatue = ignoreConnect;
            SuccessAction = SuccessAc;
            FailureAction = FailureAc;
        }
        public IMsgBase msg;
        public bool ignoreConnectStatue = false;//不显示连接状态
        public Action<object> SuccessAction { set; get; }
        public Action<object> FailureAction { set; get; }
    }
}