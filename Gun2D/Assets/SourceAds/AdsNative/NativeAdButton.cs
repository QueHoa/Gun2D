
using System;
using UnityEngine;

public class NativeAdButton : NativeAdComponent
{
    
    
    public override void TryShow()
    {
        if (!PrefInfo.IsUsingAd()) return;
        //gameObject.SetActive(ADReadyToShow);
        RequestNativeAdHandle();
    }

    public override void AdLoadedHandle()
    {
        
    }

    public override void AdFailedLoadHandle()
    {
        
    }


    
}