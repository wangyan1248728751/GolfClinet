namespace Golf
{
    /// <summary>
    /// 界面编号。
    /// </summary>
    public enum UIFormId
    {
        Undefined = 0,

        /// <summary>
        /// 弹出框。
        /// </summary>
        DialogForm = 1,

        /// <summary>
        /// 宣传视频.
        /// </summary>
        VideoForm = 80,

        /// <summary>
        /// 登陆.
        /// </summary>
        LoginForm = 90,

		/// <summary>
		/// 注册.
		/// </summary>
		RegisterForm = 91,

		OldLoginForm = 92,

		/// <summary>
		/// 二维码登陆.
		/// </summary>
		QRCodeLoginForm = 93,

		/// <summary>
		/// 主菜单。
		/// </summary>
		MenuForm = 100,

        /// <summary>
        /// 设置。
        /// </summary>
        SettingForm = 101,

        /// <summary>
        /// 关于。
        /// </summary>
        AboutForm = 102,

        /// <summary>
        /// 用户信息.
        /// </summary>
        UserForm = 110,

		/// <summary>
		/// 用户背包界面
		/// </summary>
		UserInfoForm = 111,

		/// <summary>
		/// 单球信息界面
		/// </summary>
		BallInfoForm = 115,
		/// <summary>
		/// 结算界面
		/// </summary>
		ResultForm = 120,

		/// <summary>
		/// 排行榜
		/// </summary>
		LeaderboardForm = 130,

		/// <summary>
		/// 消息
		/// </summary>
		MessageForm = 200,

        /// <summary>
        /// 登录后
        /// </summary>
        FunctionForm=300,
    }
}
