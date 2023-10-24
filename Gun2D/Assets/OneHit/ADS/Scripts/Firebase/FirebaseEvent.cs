namespace OneHit
{
    public static class FirebaseEvent
    {
        #region FIXED - Đã đặt trong source code ADS (không cần thêm ở đâu nữa)
        public static readonly string OPEN_AD = "OPEN_AD";
        public static readonly string ADS_REWARD = "ADS_REWARD";
        public static readonly string ADS_INTERSTITIAL = "ADS_INTERSTITIAL";
        public static readonly string PURCHASE_SUCCESS_NOADS = "PURCHASE_SUCCESS_NOADS";
        #endregion

        //-----TRACKING: Ở những nơi cần log event, goi FirebaseManager.Instance.LogEvent(FirebaseEvent.TEN_EVENT);

        public static readonly string TOTAL_COLLECT_COIN = "TOTAL_COLLECT_COIN";
        public static readonly string TOTAL_UNLOCK_BY_REWARD = "TOTAL_UNLOCK_BY_REWARD";
        public static readonly string TOTAL_UNLOCK_BY_COIN = "TOTAL_UNLOCK_BY_COIN";

        public static readonly string OPEN_SETTING = "OPEN_SETTING";
        public static readonly string OPEN_COLLECT_COIN = "OPEN_COLLECT_COIN";

        public static readonly string PLAY_BACK_HOME = "PLAY_BACK_HOME";
        public static readonly string END_GAME = "END_GAME";
        public static readonly string START_PLAY = "START_PLAY";
        public static readonly string REPLAY = "REPLAY";

        public static readonly string END_OTHER_STAGE = "END_OTHER_STAGE";
        public static readonly string END_BACK_HOME = "END_BACK_HOME";
    }
}