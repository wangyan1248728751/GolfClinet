namespace Golf
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        public static ConfigComponent Config
        {
            get;
            private set;
        }

        public static PromotionalVideoComponent PromotionalVideo
        {
            get;
            private set;
        }

        public static GameDataComponent GameData
        {
            get;
            private set;
        }

        public static GameCoreComponent GameCore
        {
            get;
            private set;
        }

        public static MapComponent Map
        {
            get;
            private set;
        }

        public static HitBallComponent HitBall
        {
            get;
            private set;
        }

		public static WebRequestToServerComponent WebRequestToServerComponent
		{
			get;
			private set;
		}



		private static void InitCustomComponents()
        {
            Config = UnityGameFramework.Runtime.GameEntry.GetComponent<ConfigComponent>();
            PromotionalVideo = UnityGameFramework.Runtime.GameEntry.GetComponent<PromotionalVideoComponent>();
            GameData = UnityGameFramework.Runtime.GameEntry.GetComponent<GameDataComponent>();
            GameCore = UnityGameFramework.Runtime.GameEntry.GetComponent<GameCoreComponent>();
            Map = UnityGameFramework.Runtime.GameEntry.GetComponent<MapComponent>();
            HitBall = UnityGameFramework.Runtime.GameEntry.GetComponent<HitBallComponent>();
			WebRequestToServerComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<WebRequestToServerComponent>();
		}
    }
}
