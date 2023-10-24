using System;
using UnityEngine;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using GoogleMobileAds.Common;

namespace OneHit
{
    public class AdmobControl : MonoBehaviour
    {
        [Space, GUIColor(0.25f, 0.9f, 0.5f),ReadOnly]
        [SerializeField] public string androidAppOpenAdID;
        [Space, GUIColor(1, 0.9f, 0.1f),ReadOnly]
        [SerializeField] public string iosAppOpenAdID;

        [Space]
        [SerializeField] private bool testAppOpenAd;

        [SerializeField, ReadOnly]
        private string appOpenAdTestID = "ca-app-pub-3940256099942544/3419835294";

        private AdsManager _adsManager;

        private AppOpenAd _appOpenAd;

        private DateTime _expireTime; // thời gian hết hạn của app open ad

        // check xem có phải lần đầu vào game không, khi đó OnAppStateChanged() nhưng sẽ không show OpenAd
        private bool _isFirstTimeOpenApp = true;

        public bool IsOpenAdAvailable => _appOpenAd != null
                                      && _appOpenAd.CanShowAd()
                                      && DateTime.Now < _expireTime;



        #region =============== HANDLE ===============

        public void Init(AdsManager adsManager)
        {
            Debug.LogWarning("Admob: Start init!");
            _adsManager = adsManager;

            if (testAppOpenAd)
            {
                Debug.LogWarning("Admob: Is using Android app open ad test");
                androidAppOpenAdID = appOpenAdTestID;
            }

            MobileAds.Initialize((initStatus) =>
            {
                Debug.LogWarning("Admob: Init complete!");
                _adsManager.LoadAppOpenAd();
            });
        }


        public void LoadAppOpenAd()
        {
            // Clean up the old ad before loading a new one.
            if (_appOpenAd != null)
            {
                Debug.LogWarning("Admob: Destroy current app open ad");
                _appOpenAd.Destroy();
                _appOpenAd = null;
            }

            Debug.LogWarning("Admob: Loading the app open ad");

            string key = "";
#if UNITY_IOS
            key = iosAppOpenAdID;
#else
            key = androidAppOpenAdID;
#endif
            AppOpenAd.Load(key, ScreenOrientation.Portrait, new AdRequest.Builder().Build(), (ad, error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Admob: App open ad failed to load an ad " + "with error: " + error);
                    _isFirstTimeOpenApp = false;
                    return;
                }

                // App open ad is loaded.
                Debug.LogWarning("Admob: App open ad loaded with response: " + ad.GetResponseInfo());
                _appOpenAd = ad;

                // App open ads can be preloaded for up to 4 hours.
                _expireTime = DateTime.Now + TimeSpan.FromHours(4);

                this.RegisterAppOpenAdEvents();
            });
        }

        public void ShowAppOpenAd()
        {
            Debug.LogWarning("Admob: Show app open ad");
            _appOpenAd.Show();
        }

#endregion



        #region  =============== APP OPEN AD EVENTS ===============

        private void RegisterAppOpenAdEvents()
        {
            _appOpenAd.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            _appOpenAd.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            _appOpenAd.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            _appOpenAd.OnAdImpressionRecorded += OnAdImpressionRecorded;
            _appOpenAd.OnAdClicked += OnAdClicked;
            _appOpenAd.OnAdPaid += OnAdPaid;
        }

        // Raised when an ad opened full screen content
        private void OnAdFullScreenContentOpened()
        {
            Debug.Log("Admob: App open ad full screen content opened");
            _adsManager.OnAppOpenAdOpen();
        }

        // Raised when the ad closed full screen content
        private void OnAdFullScreenContentClosed()
        {
            Debug.Log("Admob: App open ad full screen content closed.");
            _adsManager.OnAppOpenAdClose();
            _isFirstTimeOpenApp = false;
        }

        // Raised when the ad failed to open full screen content.
        private void OnAdFullScreenContentFailed(AdError error)
        {
            Debug.LogError("Admob: App open ad failed to open full screen content with error " + error.GetMessage());
            _adsManager.OnAppOpenAdFailed();
            _isFirstTimeOpenApp = false;
        }

        // Raised when a click is recorded for an ad
        private void OnAdClicked()
        {
            Debug.Log("Admob: App open ad was clicked");
        }

        // Raised when an impression is recorded for an ad
        private void OnAdImpressionRecorded()
        {
            Debug.Log("Admob: App open ad recorded an impression");
        }

        // Raised when the ad is estimated to have earned money
        private void OnAdPaid(AdValue adValue)
        {
            Debug.Log($"Admob: App open ad paid value: {adValue.Value}, {adValue.CurrencyCode})");
            _adsManager.OnAppOpenAdPaid(adValue);
        }

        #endregion



        #region  =============== APP STATE CHANGED ===============

        private void Awake()
        {
            // Use the AppStateEventNotifier to listen to application open/close events.
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }

        private void OnDestroy()
        {
            // Always unlisten to events when complete.
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }

        private void OnAppStateChanged(AppState state)
        {
            Debug.LogWarning("Admob: App State changed to: " + state);

            // if the app is Foregrounded and the ad is available, show it.
            if (state == AppState.Foreground)
            {
                if (_isFirstTimeOpenApp) return;
                _adsManager.ShowAppOpenAd(null);
            }
        }

        #endregion
    }
}