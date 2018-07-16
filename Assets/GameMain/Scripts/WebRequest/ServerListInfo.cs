using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Golf
{
    public static  class ServerListInfo
    {

        private static List<serverMap> m_serverList;
        private static float m_userAgreementState=0;
        private static Dictionary<string, serverMap> m_serverDic;
        public static List<serverMap> ServerList
        {
            get
            {
                return m_serverList;
            }

            set
            {
                m_serverList = value;
            }
        }
        public static Dictionary<string, serverMap> ServerDic
        {
            get
            {
                return m_serverDic;
            }

            set
            {
                m_serverDic = value;
            }
        }

        public static float UserAgreementState
        {
            get
            {
                return m_userAgreementState;
            }

            set
            {
                m_userAgreementState = value;
            }
        }
    }
}