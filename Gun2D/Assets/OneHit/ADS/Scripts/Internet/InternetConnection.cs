using System;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

namespace OneHit
{
    public class InternetConnection : MonoBehaviour
    {
        public static InternetConnection Instance { get; private set; }

        [Space, InfoBox("Bắt buộc phải có kết nối internet mới có thể chơi")]
        [SerializeField] private bool requireInternet;

        [ShowIf("requireInternet", true)]
        [ShowInInspector, ReadOnly] private bool canCheckInternet;

        [ShowIf("requireInternet", true)]
        [SerializeField] private float timePerCheckInternet = 1f;


        [InfoBox("True: auto show NoInternetPanel when show rewarded ad\n" +
                 "False: auto skip rewarded ad in editor")]
        [Space(30)] public bool isTestNoInternet;


        private void Awake() => Instance = this;

        private void Start()
        {
            LogInternetStatus();
            StartCoroutine(WaitCheckInternet());
        }

        private IEnumerator WaitCheckInternet()
        {
            // not check internet if not require
            if (!requireInternet) yield break;

            // wait loading scene complete
            yield return new WaitUntil(() => canCheckInternet);

            Debug.LogWarning("<color=cyan> Internet: Start check internet </color>");
            var waitForSeconds = new WaitForSeconds(timePerCheckInternet);

            while (true)
            {
                yield return waitForSeconds;

                if (!HasInternet())
                {
                    NoInternetPanel.Instance.Enable();
                    yield return new WaitUntil(HasInternet);
                    NoInternetPanel.Instance.Disable();
                }
            }
        }

        private void LogInternetStatus()
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                Debug.LogWarning("<color=lime> Internet: Network is available through wifi! </color>");

            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                Debug.LogWarning("<color=lime> Internet: Network is available through mobile data! </color>");

            if (Application.internetReachability == NetworkReachability.NotReachable)
                Debug.LogError("<color=red> Internet: Network not available! </color>");
        }

        public void CheckInternetAfterLoading()
        {
            canCheckInternet = true;
        }

        public void OpenWifiSetting()
        {
            try
            {
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
                intent.Call<AndroidJavaObject>("setAction", "android.settings.WIFI_SETTINGS");

                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intent);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static bool HasInternet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}