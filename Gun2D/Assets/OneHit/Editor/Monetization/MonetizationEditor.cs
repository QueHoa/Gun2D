using OneHit;
using OneHit.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GoogleMobileAds.Editor;
using com.adjust.sdk;

namespace OneHit.Editor
{
    public class MonetizationEditor : EditorWindow
    {
        [MenuItem("OneHit/Monetization")]
        public static void OpenDataInspector()
        {
           MonetizationData  data = Resources.Load<MonetizationData>("MonetizationData");
            if (data != null)
            {
               
            }
            else data = MonetizationData.CreateInstance();
            Selection.activeObject = data;
        }
    }
}
