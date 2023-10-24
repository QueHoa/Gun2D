using System;
using Firebase;
using UnityEngine;
using System.Collections;
using Firebase.Analytics;
using Firebase.RemoteConfig;
using Sirenix.OdinInspector;

namespace OneHit
{
    public class FirebaseManager : MonoBehaviour
    {
        public static FirebaseManager Instance { get; private set; }

        [ShowInInspector, ReadOnly] private bool isInitialized;
        [ShowInInspector, ReadOnly] private bool fetchDone;
        private LastFetchStatus result;

        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            this.FetchData();
        }

        public void Init()
        {
            Debug.Log("<color=orange> Firebase: init </color>");

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                Debug.Log("<color=lime> Firebase: initialized </color>");
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                    this.InitializeFirebase();
                else
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            });
        }

        private void InitializeFirebase()
        {
            Debug.Log("<color=cyan> Firebase: Enabling data collection </color>");
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            isInitialized = true;
        }

        private void FetchData()
        {
            StartCoroutine(WaitForFetch());
            StartCoroutine(WaitForFetchDone());
        }

        private IEnumerator WaitForFetch()
        {
            yield return new WaitUntil(() => isInitialized);
            this.Fetch();
        }

        private IEnumerator WaitForFetchDone()
        {
            yield return new WaitUntil(() => fetchDone);

            if (result.Equals(LastFetchStatus.Success))
            {
                try
                {
                    FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    ConfigValue value = FirebaseRemoteConfig.DefaultInstance.GetValue("AdSetting_time_iap");
                    Debug.Log("AdSetting_time_iap: " + value.StringValue);
                    if (!string.IsNullOrEmpty(value.StringValue))
                    {
                        PlayerPrefs.SetString("AdSetting_time_iap", value.StringValue);
                    }

                    ConfigValue value1 = FirebaseRemoteConfig.DefaultInstance.GetValue("AdSetting_time_reward");
                    Debug.Log("AdSetting_time_reward: " + value1.StringValue);
                    if (!string.IsNullOrEmpty(value1.StringValue))
                    {
                        PlayerPrefs.SetInt("AdSetting_time_reward", int.Parse(value1.StringValue));
                    }

                    ConfigValue value2 = FirebaseRemoteConfig.DefaultInstance.GetValue("AdSetting_time_normal");
                    Debug.Log("AdSetting_time_normal: " + value2.StringValue);
                    if (!string.IsNullOrEmpty(value2.StringValue))
                    {
                        PlayerPrefs.SetString("AdSetting_time_normal", value2.StringValue);
                    }

                    ConfigValue value3 = FirebaseRemoteConfig.DefaultInstance.GetValue("AdSetting_play");
                    Debug.Log("AdSetting_play: " + value3.StringValue);
                    if (!string.IsNullOrEmpty(value3.StringValue))
                    {
                        PlayerPrefs.SetString("AdSetting_play", value3.StringValue);
                    }

                    ConfigValue value4 = FirebaseRemoteConfig.DefaultInstance.GetValue("AdSetting_level");
                    Debug.Log("AdSetting_level: " + value4.StringValue);
                    if (!string.IsNullOrEmpty(value4.StringValue))
                    {
                        PlayerPrefs.SetString("AdSetting_level", value4.StringValue);
                    }

                    ConfigValue value5 = FirebaseRemoteConfig.DefaultInstance.GetValue("level_show_rate");
                    Debug.Log("level_show_rate: " + value5.StringValue);
                    if (!string.IsNullOrEmpty(value5.StringValue))
                    {
                        PlayerPrefs.SetInt("level_show_rate", int.Parse(value5.StringValue));
                    }

                    ConfigValue ads_play = FirebaseRemoteConfig.DefaultInstance.GetValue("AdSetting_play_gateplay");
                    Debug.Log("AdSetting_play_gateplay: " + ads_play.StringValue);
                    if (!string.IsNullOrEmpty(ads_play.StringValue))
                    {
                        PlayerPrefs.SetString("AdSetting_play_gateplay", ads_play.StringValue);
                    }

                    ConfigValue ads_time = FirebaseRemoteConfig.DefaultInstance.GetValue("AdSetting_time_gateplay");
                    Debug.Log("AdSetting_time_gateplay: " + ads_time.StringValue);
                    if (!string.IsNullOrEmpty(ads_time.StringValue))
                    {
                        PlayerPrefs.SetString("AdSetting_time_gateplay", ads_time.StringValue);
                    }

                    //if (MasterControl.Instance != null)
                    //{
                    //    MasterControl.Instance.adsController.LoadRule();
                    //}
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            else if (result.Equals(LastFetchStatus.Failure))
            {
                Debug.LogError("<color=red> Firebase: Faile to load </color>");
            }
            else
            {
                Debug.LogWarning("<color=orange> Firebase: pending </color>");
            }
        }

        private void Fetch()
        {
            Debug.Log("<color=cyan> Fetching data... </color>");
            try
            {
                FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWith(task2 =>
                {
                    ConfigInfo info = FirebaseRemoteConfig.DefaultInstance.Info;
                    result = info.LastFetchStatus;
                    if (info.LastFetchStatus.Equals(LastFetchStatus.Success))
                    {
                        fetchDone = true;
                    }
                    else if (info.LastFetchStatus.Equals(LastFetchStatus.Failure))
                    {
                        Debug.LogError("Firebase: fail to load");
                    }
                    else
                    {
                        Debug.LogWarning("Firebase: pending");
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogError("<color=red> LOI 5108410284: " + e.ToString() + "</color>");
            }
        }

        public void LogEvent(string eventName)
        {
            StartCoroutine(DoLogEvent(eventName));
        }

        private IEnumerator DoLogEvent(string eventName)
        {
            yield return new WaitForSeconds(1);
            float timer = 4f;
            while (!isInitialized && timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer < 0) break;
                yield return null;
            }
            try
            {
                Debug.Log($"<color=orange> Firebase Log: </color> <color=cyan> {eventName} </color>");
                FirebaseAnalytics.LogEvent(eventName);
            }
            catch (FirebaseException e)
            {
                Debug.LogError(e);
            }
        }
    }
}