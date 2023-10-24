using System;
using UnityEngine;
using com.adjust.sdk;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace OneHit
{
    public enum AdsState
    {
        AdsProduction,
        AdsTest,
        UnlockAll
    }

    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance { get; private set; }

        [Space] public AdsState adsState;

        [Space, Header("Check when ad loaded")]
        [ReadOnly] public bool readyShowBanner;
        [ReadOnly] public bool readyShowInterstitial;
        [ReadOnly] public bool readyShowRewardedVideo;
        [ReadOnly] public bool allowShowOpenAd = true;

        [Space, Header("Time wait to reload if ad not ready")]
        public float interstitialReloadWaitTime = 0.5f;
        public float rewardedVideoReloadWaitTime = 1f;
        public float appOpenAdReloadWaitTime = 2f;

        [Space, Header("Time between 2 consecutive commercials")]
        public float consecutiveInterAdShowTimeLimit = 20f;


        private IronSourceControl _ironSource;
        private AdmobControl _admob;

        private Action<bool> _interstitialCallback;
        private Action<bool> _rewardedVideoCallback;
        private Action<bool> _appOpenAdCallback;


        public bool AdsUnavailable => !PrefInfo.IsUsingAd()
                                   || !InternetConnection.HasInternet()
                                   || adsState == AdsState.UnlockAll;



        #region =============== INITIALIZATION ===============

        private void Awake()
        {
            Instance = this;
            _ironSource = GetComponentInChildren<IronSourceControl>();
            _admob = GetComponentInChildren<AdmobControl>();
        }

        public void Init()
        {
            Debug.LogWarning("<color=orange> ADS MANAGER: Start init </color>");
            _ironSource.Init(this);
            _admob.Init(this);

            // set time to show ads
            SetTimeShowAds();
            Debug.Log($"<color=cyan> TimeToShowAds: {GetTimeShowAds()} </color>");
        }

        #endregion



        #region =============== BANNER VIEW ===============

        public void LoadBanner()
        {
            if (AdsUnavailable) return;

            Debug.LogWarning("ADS MANAGER: Load Banner");
            _ironSource.LoadBanner();
        }

        public async void ShowBanner()
        {
            if (AdsUnavailable) return;

            if (!readyShowBanner)
            {
                Debug.LogWarning("ADS MANAGER: Reload Banner");
                _ironSource.LoadBanner();

                Debug.LogWarning("ADS MANAGER: Wait until Banner ready");
                await new WaitUntil(() => readyShowBanner);
            }
            Debug.LogWarning("ADS MANAGER: Show Banner");
            _ironSource.ShowBanner();

            

        }

        public void HideBanner()
        {
            Debug.LogWarning("ADS MANAGER: Hide Banner");
            _ironSource.HideBanner();
        }

        public void OnBannerLoaded()
        {
            Debug.LogWarning("ADS MANAGER: On Banner Loaded");
            readyShowBanner = true;

            

        }

        public void OnBannerFailedToLoad()
        {
            Debug.LogError("ADS MANAGER: On Banner Failed To Load");
            readyShowBanner = false;
            this.LoadBanner();
        }

        #endregion



        #region =============== INTERSTITIAL ===============

        public void LoadInterstitial()
        {
            if (AdsUnavailable) return;

            Debug.LogWarning("ADS MANAGER: Load Interstitial");
            _ironSource.LoadInterstitial();
        }

        public async void ShowInterstitial(Action<bool> callback)
        {
            _interstitialCallback = callback;

#if UNITY_EDITOR
            Debug.Log("<color=lime> ADS MANAGER: Skip Interstitial in UNITY_EDITOR </color>");
            this.OnInterstitialCallback(true);
            return;
#endif

            if (adsState == AdsState.UnlockAll || !PrefInfo.IsUsingAd())
            {
                Debug.LogWarning("ADS MANAGER: Skip Interstitial (unlock all or not using ad)");
                this.OnInterstitialCallback(true);
                return;
            }
            if (!InternetConnection.HasInternet() || !IsAllowShowInterstitial())
            {
                Debug.LogWarning("ADS MANAGER: Skip Interstitial (no internet or not allow show)");
                this.OnInterstitialCallback(false);
                return;
            }

            if (!readyShowInterstitial)
            {
                Debug.LogWarning("ADS MANAGER: Reload Interstitial");
                _ironSource.LoadInterstitial();

                Debug.LogWarning($"ADS MANAGER: Wait until Interstitial ready (no more {interstitialReloadWaitTime}s)");
                float waitTimeLoadAd = 0f;
                while (!readyShowInterstitial && waitTimeLoadAd < interstitialReloadWaitTime)
                {
                    waitTimeLoadAd += 0.1f;
                    await UniTask.Delay(100);
                }
            }

            if (readyShowInterstitial)
            {
                Debug.LogWarning("ADS MANAGER: Ready show Interstitial");
                _ironSource.ShowInterstitial();
            }
            else
            {
                Debug.LogError("ADS MANAGER: Not ready show Interstitial");
                this.OnInterstitialCallback(false);
            }
        }

        public void OnInterstitialReady()
        {
            Debug.LogWarning("ADS MANAGER: On Interstitial Ready");
            readyShowInterstitial = true;
        }

        public void OnInterstitialLoadFailed()
        {
            Debug.LogWarning("ADS MANAGER: On Interstitial Load Failed");
            readyShowInterstitial = false;
            this.LoadInterstitial();
        }

        public void OnInterstitialOpen()
        {
            Debug.LogWarning("ADS MANAGER: On Interstitial Open");
            //SoundManager.Instance.ChangePitch(0);
            Time.timeScale = 0;

            allowShowOpenAd = false;
            readyShowInterstitial = false;
        }

        public void OnInterstitalClose()
        {
            Debug.LogWarning("ADS MANAGER: On Interstital Close");
            //SoundManager.Instance.ChangePitch(1);
            Time.timeScale = 1;

            allowShowOpenAd = true;

            PrefInfo.SetTimeShowAds();
            this.LoadInterstitial();
        }

        public void OnInterstitialShowSucceeded()
        {
            Debug.LogWarning("ADS MANAGER: On Interstitial Show Succeeded");
            FirebaseManager.Instance.LogEvent(FirebaseEvent.ADS_INTERSTITIAL);

            this.OnInterstitialCallback(true);
            this.LoadInterstitial();
        }

        public void OnInterstitialShowFailed()
        {
            Debug.LogWarning("ADS MANAGER: On Interstitial Show Failed");
            this.OnInterstitialCallback(false);
            this.LoadInterstitial();
        }

        public void OnInterstitialCallback(bool showSuccess)
        {
            Debug.LogWarning($"ADS MANAGER: On Interstitial Callback ({showSuccess})");
            _interstitialCallback?.Invoke(showSuccess);
            _interstitialCallback = null;
        }

        #endregion



        #region =============== REWARDED VIDEO ===============

        public void LoadRewardedVideo()
        {
            if (AdsUnavailable) return;

            Debug.LogWarning("ADS MANAGER: Load Rewarded Video");
            _ironSource.LoadRewardedVideo();
        }

        public async void ShowRewardedVideo(Action<bool> callback)
        {
            _rewardedVideoCallback = callback;

            if (InternetConnection.Instance.isTestNoInternet)
            {
                Debug.LogError("ADS MANAGER: is test no internet");
                this.OnRewardedVideoCallback(false);
                return;
            }

#if UNITY_EDITOR
            Debug.Log("<color=lime> ADS MANAGER: Skip Rewarded Video in UNITY_EDITOR </color>");
            this.OnRewardedVideoCallback(true);
            return;
#endif

            if (adsState == AdsState.UnlockAll || !PrefInfo.IsUsingAd())
            {
                Debug.Log(" ADS MANAGER: Skip Rewarded Video (unlock all or not using ad)");
                this.OnRewardedVideoCallback(true);
                return;
            }
            if (!InternetConnection.HasInternet())
            {
                Debug.LogError("ADS MANAGER: Skip Rewarded Video (no internet)");
                this.OnRewardedVideoCallback(false);
                return;
            }

            if (!readyShowRewardedVideo)
            {
                Debug.LogWarning("ADS MANAGER: Reload Rewarded Video");
                _ironSource.LoadRewardedVideo();

                Debug.LogWarning($"ADS MANAGER: Wait until Rewarded Video ready (no more {rewardedVideoReloadWaitTime}s)");
                float waitTimeLoadAd = 0f;
                while (!readyShowRewardedVideo && waitTimeLoadAd < rewardedVideoReloadWaitTime)
                {
                    waitTimeLoadAd += 0.1f;
                    await UniTask.Delay(100);
                }
            }

            if (readyShowRewardedVideo)
            {
                Debug.LogWarning("ADS MANAGER: Ready show Rewarded Video");
                _ironSource.ShowRewardedVideo();
            }
            else
            {
                Debug.LogError("ADS MANAGER: Not ready show Rewarded Video");
                this.OnRewardedVideoCallback(false);
            }
        }

        public void OnRewardedVideoAvailable()
        {
            Debug.LogWarning("ADS MANAGER: On Rewarded Video Available");
            readyShowRewardedVideo = true;
        }

        public void OnRewardedVideoUnavailable()
        {
            Debug.LogWarning("ADS MANAGER: On Rewarded Video Unavailable");
            readyShowRewardedVideo = false;
            this.LoadRewardedVideo();
        }

        public void OnRewardedVideoOpen()
        {
            Debug.LogWarning("ADS MANAGER: On Rewarded Video Open");
            //SoundManager.Instance.ChangePitch(0);
            Time.timeScale = 0;

            allowShowOpenAd = false;
            readyShowRewardedVideo = false;
        }

        public void OnRewardedVideoClose()
        {
            Debug.LogWarning("ADS MANAGER: On Rewarded Video Close");
            //SoundManager.Instance.ChangePitch(1);
            Time.timeScale = 1;

            allowShowOpenAd = true;

            PrefInfo.SetTimeShowAds();
            this.LoadRewardedVideo();
        }

        public void OnRewardedVideoShowSucceeded()
        {
            Debug.LogWarning("ADS MANAGER: On Rewarded Video Show Succeeded");
            FirebaseManager.Instance.LogEvent(FirebaseEvent.ADS_REWARD);

            this.OnRewardedVideoCallback(true);
            this.LoadRewardedVideo();
        }

        public void OnRewardedVideoShowFailed()
        {
            Debug.LogWarning("ADS MANAGER: On Rewarded Video Show Failed");
            this.OnRewardedVideoCallback(false);
            this.LoadRewardedVideo();
        }

        public void OnRewardedVideoCallback(bool showSuccess)
        {
            Debug.LogWarning($"ADS MANAGER: On Rewarded Callback ({showSuccess})");

            if (!showSuccess) VideoNotReadyPanel.Instance.Enable();

            _rewardedVideoCallback?.Invoke(showSuccess);
            _rewardedVideoCallback = null;
        }

        #endregion



        #region =============== APP OPEN AD ===============

        public void LoadAppOpenAd()
        {
            if (AdsUnavailable) return;

            Debug.LogWarning("ADS MANAGER: Load app open ad");
            _admob.LoadAppOpenAd();
        }

        public async void ShowAppOpenAd(Action<bool> callback)
        {
            _appOpenAdCallback = callback;

            if (adsState == AdsState.UnlockAll || !PrefInfo.IsUsingAd())
            {
                Debug.LogWarning("ADS MANAGER: Skip app open ad (unlock all or ads removed)");
                this.OnAppOpenAdCallback(true);
                return;
            }
            if (!allowShowOpenAd || !InternetConnection.HasInternet())
            {
                Debug.LogWarning("ADS MANAGER: Skip app open ad (not allow show or no internet)");
                this.OnAppOpenAdCallback(false);
                return;
            }

            if (!_admob.IsOpenAdAvailable)
            {
                Debug.LogWarning("ADS MANAGER: Reload app open ad");
                _admob.LoadAppOpenAd();

                Debug.LogWarning($"ADS MANAGER: Wait until app open ad ready (no more {appOpenAdReloadWaitTime}s)");
                float waitTimeLoadAd = 0f;
                while (!_admob.IsOpenAdAvailable && waitTimeLoadAd < appOpenAdReloadWaitTime)
                {
                    waitTimeLoadAd += 0.1f;
                    await UniTask.Delay(100);
                }
            }

            if (_admob.IsOpenAdAvailable)
            {
                Debug.LogWarning("ADS MANAGER: Ready show app open ad");
                _admob.ShowAppOpenAd();
            }
            else
            {
                Debug.LogError("ADS MANAGER: Not ready show app open ad");
                this.OnAppOpenAdCallback(false);
            }
        }

        public void OnAppOpenAdCallback(bool showSuccess)
        {
            Debug.LogWarning($"ADS MANAGER: On app open ad callback ({showSuccess})");
            _appOpenAdCallback?.Invoke(showSuccess);
            _appOpenAdCallback = null;
        }

        public void OnAppOpenAdOpen()
        {
            Debug.LogWarning("ADS MANAGER: On App Open Ad Open");
            allowShowOpenAd = false;
        }

        public void OnAppOpenAdClose()
        {
            Debug.LogWarning("ADS MANAGER: On App Open Ad Close");
            FirebaseManager.Instance.LogEvent(FirebaseEvent.OPEN_AD);
            PrefInfo.SetTimeShowAds();

            allowShowOpenAd = true;

            this.OnAppOpenAdCallback(true);
            this.LoadAppOpenAd();
        }

        public void OnAppOpenAdFailed()
        {
            Debug.LogWarning("ADS MANAGER: On App Open Ad Failed");
            PrefInfo.SetTimeShowAds();

            allowShowOpenAd = true;

            this.OnAppOpenAdCallback(false);
            this.LoadAppOpenAd();
        }

        public void OnAppOpenAdPaid(AdValue adValue)
        {
            Debug.LogWarning("ADS MANAGER: On App Open Ad Paid");

            double revenue = adValue.Value / 1000000f;
            var imp = new[] {
                new Parameter("ad_platform", "Admob"),
                new Parameter("ad_source", "Admob"),
                new Parameter("ad_unit_name", "open_ads"),
                new Parameter("ad_format", "open_ads"),
                new Parameter("value", revenue),
                new Parameter("currrency", adValue.CurrencyCode)
             };
            FirebaseAnalytics.LogEvent("ad_impression", imp);

            AdjustAdRevenue adjustEvent = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
            //most important is calling setRevenue with two parameters
            adjustEvent.setRevenue(revenue, adValue.CurrencyCode);
            //Sent event to Adjust server
            Adjust.trackAdRevenue(adjustEvent);
        }

        #endregion



        public static bool IsAllowShowOpenAd()
        {
            DateTime lastTimeShowAd = DateTime.Parse(PlayerPrefs.GetString("TimeToShowAds", new DateTime(1990, 1, 1).ToString()));
            double time = (DateTime.Now - lastTimeShowAd).TotalSeconds;
            if (time < 10) Debug.LogError("Not allow show open ads");
            return time >= 10; //! không show open ad trong vòng 10s
        }

        public bool IsAllowShowInterstitial()
        {
            DateTime lastTimeShowAd = DateTime.Parse(PlayerPrefs.GetString("TimeToShowAds", new DateTime(1990, 1, 1).ToString()));
            double time = (DateTime.Now - lastTimeShowAd).TotalSeconds;
            return time >= consecutiveInterAdShowTimeLimit; //! không show inter ads trong vòng 20s
        }

        //-------------------------------------------------------------------------------------


        public static string GetTimeShowAds()
        {
            return PlayerPrefs.GetString("TimeToShowAds", new DateTime(1990, 1, 1).ToString());
        }

        public static void SetTimeShowAds()
        {
            PlayerPrefs.SetString("TimeToShowAds", DateTime.Now.ToString());
        }
    }
}