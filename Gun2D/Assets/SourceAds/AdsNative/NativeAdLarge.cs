using System;
using System.Collections.Generic;
using System.Linq;
using com.adjust.sdk;
using UnityEngine;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine.UI;


public class NativeAdLarge : NativeAdComponent
{
    //public bool isAllowShow;
    private int playCount;
    
    public override void TryShow()
    {
        if (!PrefInfo.IsUsingAd()) return;
        
        if (ADReadyToShow)
        {
            gameObject.SetActive(true);
        }
        RequestNativeAdHandle();

        playCount = 0;
    }
    
    private void OnEnable()
    {
        //this.RemoveListener(EventID.Play, OnPlay);
        //this.RegisterListener(EventID.Play, OnPlay);
    }

    private void OnDestroy()
    {
        //this.RemoveListener(EventID.Play, OnPlay);
    }

    private void OnPlay(object o)
    {
        playCount++;
        if (playCount % 2 == 0)
            RequestNativeAdHandle();
    }

    public override void AdLoadedHandle()
    {
        Debug.Log("Large ad loaded");
        if (gameObject != null)
        {
            if (transform.parent.gameObject.activeSelf) gameObject.SetActive(true);
        }
    }

    public override void AdFailedLoadHandle()
    {
        
    }
    
    
}