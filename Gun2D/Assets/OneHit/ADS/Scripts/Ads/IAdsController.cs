using System;

namespace OneHit
{
    public interface IAdsController
    {
        void Init(AdsManager adsManager);

        void LoadBanner();
        void ShowBanner();
        void HideBanner();

        void LoadInterstitial();
        void ShowInterstitial();

        void LoadRewardedVideo();
        void ShowRewardedVideo();

        void LoadAppOpenAd();
        void ShowAppOpenAd();
    }
}