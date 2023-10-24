using UnityEngine;
using com.adjust.sdk;
using Firebase.Analytics;
using Sirenix.OdinInspector;

namespace OneHit
{
    public class IronSourceControl : MonoBehaviour, IAdsController
    {
        [Space]
        [GUIColor(0.25f, 0.9f, 0.5f), ReadOnly] public string _androidAppKey;
        [Space]
        [GUIColor(1, 0.9f, 0.1f), ReadOnly] public string _iosAppKey;

        [Space]
        [SerializeField, ReadOnly] private string uniqueUserId = "demoUserUnity";
        [SerializeField, ReadOnly] private string INTERSTITIAL_INSTANCE_ID = "0";
        [SerializeField, ReadOnly] private string REWARDED_INSTANCE_ID = "0";

        private AdsManager _adsManager;

        #region =============== INITIALIZATION ===============
        public void Init(AdsManager adsManager)
        {
            Debug.LogWarning("IronSource: Start init!!!");
            _adsManager = adsManager;

            // SDK init
            Debug.Log($"IronSource: IronSource.Agent.init({_androidAppKey})");

#if UNITY_IOS
            IronSource.Agent.init(_iosAppKey);
#else
            IronSource.Agent.init(_androidAppKey);
#endif

            IronSource.Agent.setConsent(true);
            IronSource.Agent.shouldTrackNetworkState(true);
            IronSource.Agent.validateIntegration();
            IronSource.Agent.setMetaData("do_not_sell", "false"); // Setup new
            IronSource.Agent.setMetaData("Facebook_IS_CacheFlag", "IMAGE"); // Facebook

            IronSourceEvents.onSdkInitializationCompletedEvent += OnSdkInitializationCompletedEvent;
            IronSourceEvents.onImpressionDataReadyEvent += OnImpressionDataReadyEvent;

            this.RegisterBannerEvents();
            this.RegisterInterstitialEvents();
            this.RegisterRewardedVideoEvents();
        }

        private void OnSdkInitializationCompletedEvent()
        {
            Debug.LogWarning("IronSource: Init complete!");
            _adsManager.LoadBanner();
            _adsManager.LoadInterstitial();
            _adsManager.LoadRewardedVideo();
        }

        #endregion



        #region =============== BANNER VIEW ===============

        public void LoadBanner()
        {
            Debug.Log("IronSource: IronSource.Agent.loadBanner");
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        }

        public void ShowBanner()
        {
            Debug.Log("IronSource: IronSource.Agent.displayBanner");
            IronSource.Agent.displayBanner();
        }

        public void HideBanner()
        {
            Debug.Log("IronSource: IronSource.Agent.hideBanner");
            IronSource.Agent.hideBanner();
        }

        /************* Banner AdInfo Delegates *************/
        private void RegisterBannerEvents()
        {
            IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
            IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;
        }

        // Invoked once the banner has loaded
        private void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got BannerOnAdLoadedEvent");
            _adsManager.OnBannerLoaded();
        }

        // Invoked when the banner loading process has failed.
        private void BannerOnAdLoadFailedEvent(IronSourceError error)
        {
            Debug.LogError("IronSource: I got BannerOnAdLoadFailedEvent: " + error.getCode() + ", description : " + error.getDescription());
            _adsManager.OnBannerFailedToLoad();
        }

        // Invoked when end user clicks on the banner ad
        private void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got BannerOnAdClickedEvent");
        }

        // Notifies the presentation of a full screen content following user click
        private void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got BannerOnAdScreenPresentedEvent");
        }

        // Notifies the presented screen has been dismissed
        private void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got BannerOnAdScreenDismissedEvent");
        }

        // Invoked when the user leaves the app
        private void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got BannerOnAdLeftApplicationEvent");
        }

        #endregion



        #region =============== INTERSTITIAL ===============

        public void LoadInterstitial()
        {
            Debug.Log("IronSource: IronSource.Agent.loadInterstitial");
            IronSource.Agent.loadInterstitial();
        }

        public void ShowInterstitial()
        {
            Debug.LogWarning("IronSource: ShowInterstitial");

            if (IronSource.Agent.isInterstitialReady())
            {
                Debug.Log("IronSource: IronSource.Agent.showInterstitial");
                IronSource.Agent.showInterstitial();
                _adsManager.OnInterstitialOpen();
            }
            else
            {
                Debug.LogError("IronSource: Interstitial not ready");
                _adsManager.OnInterstitialCallback(false);
            }
        }

        /************* Interstitial AdInfo Delegates *************/
        private void RegisterInterstitialEvents()
        {
            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
            IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        }

        // Invoked when the interstitial ad was loaded succesfully.
        private void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got InterstitialOnAdReadyEvent");
            _adsManager.OnInterstitialReady();
        }

        // Invoked when the initialization process has failed.
        private void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
        {
            Debug.LogError($"IronSource: I got InterstitialOnAdLoadFailed (code: {ironSourceError.getCode()}, description: {ironSourceError.getDescription()})");
            _adsManager.OnInterstitialLoadFailed();
        }

        // Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
        private void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got InterstitialOnAdOpenedEvent");
        }

        // Invoked when the interstitial ad closed and the user went back to the application screen.
        private void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got InterstitialOnAdClosedEvent");
            _adsManager.OnInterstitalClose();
        }

        // Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
        // This callback is not supported by all networks, and we recommend using it only if it's supported by all networks you included in your build.
        private void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got InterstitialOnAdShowSucceededEvent");
            _adsManager.OnInterstitialShowSucceeded();
        }

        // Invoked when the ad failed to show.
        private void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            Debug.LogError($"IronSource: I got InterstitialOnAdShowFailedEvent (code: {ironSourceError.getCode()}, description: {ironSourceError.getDescription()})");
            _adsManager.OnInterstitialShowFailed();
        }

        // Invoked when end user clicked on the interstitial ad
        private void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got InterstitialOnAdClickedEvent");
        }

        #endregion



        #region =============== REWARDED VIDEO ===============

        public void LoadRewardedVideo()
        {
            Debug.Log("IronSource: IronSource.Agent.loadRewardedVideo");
            IronSource.Agent.loadRewardedVideo();
        }

        public void ShowRewardedVideo()
        {
            Debug.LogWarning("IronSource: ShowRewardedVideo");

            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                Debug.Log("IronSource: IronSource.Agent.showRewardedVideo");
                IronSource.Agent.showRewardedVideo();
                _adsManager.OnRewardedVideoOpen();
            }
            else
            {
                Debug.LogError("IronSource: RewardedVideo not ready");
                _adsManager.OnRewardedVideoCallback(false);
            }
        }

        /************* RewardedVideo AdInfo Delegates *************/
        private void RegisterRewardedVideoEvents()
        {
            IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
        }

        // Indicates that there’s an available ad.
        // The adInfo object includes information about the ad that was loaded successfully
        private void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got RewardedVideoOnAdAvailable");
            _adsManager.OnRewardedVideoAvailable();
        }

        // Indicates that no ads are available to be displayed
        private void RewardedVideoOnAdUnavailable()
        {
            Debug.Log("IronSource: I got RewardedVideoOnAdUnavailable");
            _adsManager.OnRewardedVideoUnavailable();
        }

        // The Rewarded Video ad view has opened. Your activity will loose focus.
        private void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got RewardedVideoOnAdOpenedEvent");
        }

        // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
        private void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource: I got RewardedVideoOnAdClosedEvent");
            _adsManager.OnRewardedVideoClose();
        }

        // The user completed to watch the video, and should be rewarded.
        // The placement parameter will include the reward data.
        // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
        private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            Debug.Log($"IronSource: I got RewardedVideoOnAdRewardedEvent (name = {placement.getRewardName()}, amount = {placement.getRewardAmount()})");
            _adsManager.OnRewardedVideoShowSucceeded();
        }

        // The rewarded video ad was failed to show.
        private void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            Debug.LogError($"IronSource: I got RewardedVideoOnAdShowFailedEvent (code: {ironSourceError.getCode()}, description: {ironSourceError.getDescription()})");
            _adsManager.OnRewardedVideoShowFailed();
        }

        // Invoked when the video ad was clicked.
        // This callback is not supported by all networks, and we recommend using it only if
        // it’s supported by all networks you included in your build.
        private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            Debug.Log($"IronSource: I got RewardedVideoOnAdRewardedEvent (name = {placement.getRewardName()}, amount = {placement.getRewardAmount()})");
        }

        #endregion



        #region =============== EVENT ADREVENUE IRONSOURCE ===============

        private void OnImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            SendRevenueToFirebase(impressionData);
            SendRevenueToAdjust(impressionData);
        }

        private void SendRevenueToFirebase(IronSourceImpressionData impressionData)
        {
            Debug.Log("IronSource:  ImpressionSuccessEvent impressionData = " + impressionData);
            if (impressionData == null) return;
            Parameter[] adParameters =
            {
            new Parameter("ad_platform", "ironSource"),
            new Parameter("ad_source", impressionData.adNetwork),
            new Parameter("ad_unit_name", impressionData.adUnit),
            new Parameter("ad_format", impressionData.instanceName),
            new Parameter("currency", "USD"),
            new Parameter("value", impressionData.revenue ?? 0)
        };
            FirebaseAnalytics.LogEvent("ad_impression", adParameters);
        }

        private void SendRevenueToAdjust(IronSourceImpressionData impressionData)
        {
            if (impressionData == null) return;
            double revenue = impressionData.revenue ?? 0;
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceIronSource);
            adjustAdRevenue.setRevenue(revenue, "USD");
            // optional fields
            adjustAdRevenue.setAdRevenueNetwork(impressionData.adNetwork);
            adjustAdRevenue.setAdRevenueUnit(impressionData.adUnit);
            adjustAdRevenue.setAdRevenuePlacement(impressionData.placement);
            // track Adjust ad revenue
            Adjust.trackAdRevenue(adjustAdRevenue);
        }

        #endregion


        public void LoadAppOpenAd() { }

        public void ShowAppOpenAd() { }

        private void OnApplicationPause(bool isPaused)
        {
            Debug.Log($"IronSource: IronSource.Agent.onApplicationPause({isPaused})");
            IronSource.Agent.onApplicationPause(isPaused);
        }
    }
}