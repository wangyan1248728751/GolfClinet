using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Golf
{
	/// <summary>
	/// 游客登录
	/// </summary>
	public class m2c_logintourist : IMsgToClient
	{
		public string name;
		public string pwd;
	}

	/// <summary>
	/// 登录成功，返回服务器信息
	/// </summary>
	public class m2c_login : IMsgToClient
	{
		public string ausession;
		/// <summary>
		/// 服务器列表
		/// </summary>
		public List<serverMap> smaps;
	}
	public class serverMap
	{
		public string load;
		public string port;
		public string ip;
		public string name;
		public string id;
		public string isnew;
		public string recommend;
	}
	/// <summary>
	/// 注册成功，返回服务器信息
	/// </summary>
	public class m2c_regist : IMsgToClient
	{
		public string ausession;
		/// <summary>
		/// 服务器列表
		/// </summary>
		public List<serverMap> smaps;
	}

	/// <summary>
	/// 选择服务
	/// </summary>
	public class m2c_getsession : IMsgToClient
	{
		public string id;
		public string session;
		public string auid;
	}

	/// <summary>
	/// 获取用户信息
	/// </summary>
	public class m2c_getuserinfo : IMsgToClient
	{
		public string id;
		public string nickname;
	}

	public class m2c_updatenickname : IMsgToClient
	{
	}

	/// <summary>
	/// 角色属性
	/// </summary>
	public class rolepromap
	{
		public string ptyid;
		public string ptyvalue;
		public string belong;
		public string ptyunit;
		public string name;
		public string id;
		public string rid;
		public string type;
	}
	/// <summary>
	/// 设置角色性别、昵称
	/// </summary>
	public class m2c_registroleinfo : IMsgToClient
	{
		public string uid;
		public string lastLogin;
		public string registtime;
		public string nickname;
		public string rid;
		public List<rolepromap> roleProMap;
	}
	public class m2c_addmessagesession : IMsgToClient
	{

	}

	public class m2c_getranklistbyuidbtid : IMsgToClient
	{
		public List<RankData> RankDatas;
	}
	public class m2c_updateranking : IMsgToClient
	{

	}

	/// <summary>
	/// 查询用户道具
	/// </summary>
	public class m2c_getacticlebyuid : IMsgToClient
	{
		public List<Goods> goodsList;
	}

	/// <summary>
	/// 核销道具
	/// </summary>
	public class m2c_updateacticlenum : IMsgToClient
	{

	}

	/// <summary>
	/// 增加道具
	/// </summary>
	public class m2c_exchangeshop : IMsgToClient
	{

	}

	/// <summary>
	/// 获取地块表
	/// </summary>
	public class m2c_getgolfislands : IMsgToClient
	{
		public Dictionary<int, DRRewardMap> webRewardMap;
	}

	/// <summary>
	/// 获取设备信息
	/// </summary>
	public class m2c_getgolfmachine : IMsgToClient
	{
		public int gmid = -100;
		public string callback;
	}
    public class m2c_forpaycode : IMsgToClient
    {
        
    }
    /// <summary>
    /// 击球点
    /// </summary>
    public class m2c_getkickoffposition : IMsgToClient
	{
		public float teePosX;
	}

	public class m2c_fortencentrequest : IMsgToClient
	{
		public string uid;
		public string auid;
		public string session;
        public string WxName;
        public string WxAvator;
	}
	public class m2c_deltencentrequest : IMsgToClient
	{

	}
    

}

