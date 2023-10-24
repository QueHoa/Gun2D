using System;
using System.Collections.Generic;
using System.Linq;
using com.adjust.sdk;
using UnityEngine;
using GoogleMobileAds.Api;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using OneHit;

public abstract class NativeAdComponent : MonoBehaviour
{
    private bool _isInit;
    private bool _isRequesting;
    protected bool ADReadyToShow;
    public RectTransform rt;
    

    #region Component

    [Header("Key")]
    public string NativeAdIdAndroid;
    public string NativeAdIdIos;
    private string idAds;
    NativeAd nativeAd;
    
    [Space]
    [Header("Text")]
    public TextMeshProUGUI adHeadline;
    public TextMeshProUGUI adCallToAction;
    public TextMeshProUGUI adBodyText;
    public TextMeshProUGUI ratingAndPrice;

    [Space]
    [Header("Image")]
    public Image adMainTexture;
    public RawImage adIconTexture;
    public RawImage infoIcon;
    
    private List<Texture2D> adTextures = new List<Texture2D>();
    private List<GameObject> adImages = new List<GameObject>();
    
    [Space]
    [Header("Waiting")]
    public GameObject adLoaded;
    public GameObject adLoading;
    #endregion

    private void OnDisable()
    {
        //this.RemoveListener(EventID.OpenPopup, OnPopupOpened);
        //this.RemoveListener(EventID.ClosePopup, OnPopupClosed);
    }

    private void OnEnable()
    {
        //this.RegisterListener(EventID.OpenPopup, OnPopupOpened);
        //this.RegisterListener(EventID.ClosePopup, OnPopupClosed);
    }

    protected virtual void Start()
    {
        
        if (!PrefInfo.IsUsingAd())
        {
            gameObject.SetActive(false);
            return;
        }

        if (adLoading != null)
        {
            adLoading.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        if (adLoaded != null) adLoaded.SetActive(false);
        MobileAds.Initialize(initStatus =>
        {
            _isInit = true;
#if UNITY_EDITOR
            ADReadyToShow = true;
#endif
        });
        onAdLoaded += AdLoadedHandle;
        onAdFailedToLoad += AdFailedLoadHandle;
        
    }

    private void OnPopupOpened(object obj)
    {
        SetClickable(false);
    }

    private void OnPopupClosed(object obj)
    {
        SetClickable(true);
    }

    public abstract void TryShow();
    public abstract void AdLoadedHandle();
    public abstract void AdFailedLoadHandle();
    private Action onAdLoaded;
    private Action onAdFailedToLoad;


    public void RequestNativeAdHandle()
    {
        //if (!GameController.Instance.CheckInternet()) return;
        if (_isRequesting) return;
        _isRequesting = true;
        Debug.Log(gameObject.name + " Request " + Time.time);
        RequestNativeAd();
    }

    private void RequestNativeAd()
    {
        idAds = NativeAdIdAndroid;
#if UNITY_IOS
        idAds = NativeAdIdIos;
#endif
        AdLoader adLoader = new AdLoader.Builder(idAds)
                    .ForNativeAd()
                    .Build();
        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
        adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        adLoader.LoadAd(new AdRequest.Builder().Build());
// #if UNITY_EDITOR
//         _isRequesting = false;
//         this.PostEvent(EventID.RequestComplete);
// #endif
    }

    private void OnPaidEvent(object sender, AdValueEventArgs impressionData)
    {
        if (impressionData == null)
        {
            Debug.Log("impress data null");
            return;
        }
        double revenue = impressionData.AdValue.Value / 1000000f;
        Debug.Log ("unity-script: onPaidEvent: " + impressionData);
        var imp = new[]
        {
            new Firebase.Analytics.Parameter("ad_platform", "Admob"),
            new Firebase.Analytics.Parameter("ad_source", "Admob"),
            new Firebase.Analytics.Parameter("ad_unit_name", "native_ads"),
            new Firebase.Analytics.Parameter("ad_format", "native_ads"),
            new Firebase.Analytics.Parameter("value", revenue),
            new Firebase.Analytics.Parameter("currency", impressionData.AdValue.CurrencyCode)
        };
        
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", imp);
        
        AdjustAdRevenue adjustEvent = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
        //most important is calling setRevenue with two parameters
        adjustEvent.setRevenue(revenue, impressionData.AdValue.CurrencyCode);
        //Sent event to Adjust server
        Adjust.trackAdRevenue(adjustEvent);
    } 
    
    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Native ad failed to load: " + args.LoadAdError.GetMessage());
        _isRequesting = false;
        //RequestNativeAd();
        //Util.Delay(1f, () => NativeAdController.Register(RequestNativeAd));
        onAdFailedToLoad?.Invoke();
        //this.PostEvent(EventID.RequestComplete);
    }

    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        ADReadyToShow = true;
        _isRequesting = false;

        if (gameObject != null)
        {
            if (gameObject.activeSelf) FirebaseManager.Instance.LogEvent("ADS_NATIVE");
        }

        nativeAd?.Destroy();
        nativeAd = args.nativeAd;
        nativeAd.OnPaidEvent += OnPaidEvent;
        
        #region Set Data To Component
        
        //set textures and details
        if (adIconTexture != null)
        {
            adIconTexture.texture = nativeAd.GetIconTexture();

            if (!nativeAd.RegisterIconImageGameObject(adIconTexture.gameObject))
            {
                Debug.Log("error registering icon");
            }
        }

        if (infoIcon != null)
        {
            infoIcon.texture = nativeAd.GetAdChoicesLogoTexture();
            infoIcon.SetNativeSize();

            if (!nativeAd.RegisterAdChoicesLogoGameObject(infoIcon.gameObject))
            {
                Debug.Log("error registering ad choices");
            }
        }

        if (adHeadline != null)
        {
            adHeadline.text = nativeAd.GetHeadlineText();
            if (!nativeAd.RegisterHeadlineTextGameObject(adHeadline.gameObject))
            {
                Debug.Log("error registering headline");
            }
        }

        if (adBodyText != null)
        {
            adBodyText.text = nativeAd.GetBodyText();
            adBodyText.text = Compact(adBodyText.text);
            if (!nativeAd.RegisterBodyTextGameObject(adBodyText.gameObject))
            {
                Debug.Log("error registering body");
            }
        }

        if (ratingAndPrice != null)
        {
            ratingAndPrice.text = nativeAd.GetStarRating() + "    " + nativeAd.GetPrice();
            if (Math.Abs(nativeAd.GetStarRating() - (-1f)) < 0.1f)
            {
                ratingAndPrice.text = "";
            }

            if (!nativeAd.RegisterPriceGameObject(ratingAndPrice.gameObject))
            {
                Debug.Log("error registering price");
            }
        }

        if (adCallToAction != null)
        {
            adCallToAction.text = nativeAd.GetCallToActionText();
            if (!nativeAd.RegisterCallToActionGameObject(adCallToAction.gameObject))
            {
                Debug.Log("error registering call to action");
            }
        }

        
        adTextures = nativeAd.GetImageTextures();
        if (adTextures != null)
        {
            var tex = adTextures.Count == 0 ? nativeAd.GetIconTexture() : adTextures[0];
            if (adMainTexture != null)
            {
                adImages.Add(adMainTexture.gameObject);
                adImages[0].GetComponent<Image>().sprite = Sprite.Create(tex,
                    new Rect(0.0f, 0.0f, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f), 100.0f);

                if (nativeAd.RegisterImageGameObjects(adImages) == 0)
                {
                    Debug.Log("error registering image");
                }
            }
        }
        
        #endregion
                 
        onAdLoaded?.Invoke();
        
        if (adLoaded != null) adLoaded.SetActive(true);
        if (adLoading != null) adLoading.SetActive(false);
        
        //this.PostEvent(EventID.RequestComplete);
    }
    

    private string Compact(string s)
    {
        try
        {
            if (s.Length < 75) return s;
            s = s.Substring(0, 75);
            s += "...";

            return s;
        }
        catch (Exception e)
        {
            return "";
        }
    }

    protected bool IsVisible()
    {
//        Debug.Log("Is");
        return rt.IsVisibleFrom(Camera.main);
    }

    public bool CheckRequest()
    {
        if (IsVisible() && !_isRequesting && _isInit)
        {
            return true;
        }

        return false;
    }

    public void SetClickable(bool status)
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = status;
        }
    }
}

public static class RendererExtensions
{
    /// <summary>
    /// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
    /// </summary>
    /// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);
 
        int visibleCorners = 0;
        Vector3 tempScreenSpaceCorner; // Cached
        for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
        {
            tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]); // Transform world space position of corner to screen space
            if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
            {
                visibleCorners++;
            }
        }
        return visibleCorners;
    }
 
    /// <summary>
    /// Determines if this RectTransform is fully visible from the specified camera.
    /// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    /// </summary>
    /// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
    }
 
    /// <summary>
    /// Determines if this RectTransform is at least partially visible from the specified camera.
    /// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    /// </summary>
    /// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
    }
}
