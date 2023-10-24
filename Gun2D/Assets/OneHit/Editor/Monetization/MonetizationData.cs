using com.adjust.sdk;
using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace OneHit.Editor
{

    //[CreateAssetMenu(fileName = "MonetizationData", menuName = "hehe")]
    public class MonetizationData : ScriptableObject
    {
        [Header("Android")]
        [GUIColor(0,1,0)]
        public string AndroidIronSourceKey;
        [GUIColor(0, 1, 0)]
        public string AndroidAdmobAppID;
        [GUIColor(0, 1, 0)]
        public string AndroidAppOpenAdsID;

        [Header("iOS"), Space(5)]
        [GUIColor(1, 1, 0)]
        public string iOSIronSourceKey;
        [GUIColor(1, 1, 0)]
        public string iOSAdmobAppID;
        [GUIColor(1, 1, 0)]
        public string iOSAppOpenAdsID;

        [Header("Facebook"), Space(5)]
        [GUIColor(0, 1, 1)]
        public string FacebookAppID;
        [GUIColor(0, 1, 1)]
        public string FacebookClientID;

        [Header("Adjust"), Space(5)]
        [GUIColor(0.5f, 0.8f, 1)]
        public string AdjustAppToken;

        #region function
        private const string dir = "Assets/OneHit/Resources";
        internal static MonetizationData CreateInstance()
        {
            Directory.CreateDirectory(dir);
            var instance = ScriptableObject.CreateInstance<MonetizationData>();
            string assetPath = Path.Combine(dir, "MonetizationData.asset");
            AssetDatabase.CreateAsset(instance, assetPath);
            AssetDatabase.SaveAssets();
            return instance;
        }
        [InfoBox("<size=16>Tải file csv từ sheet của bên mkt về Folder <color=yellow>Assets</color> và đặt tên là <color=cyan>IdAds.csv</color>  </size>", InfoMessageType.Info)]
        [InfoBox("<size=16>Chú ý: vẫn phải điền các ID trong <color=cyan> Google Mobile Ads Setting và Facebook Setting </color> (do SDK private thuộc tính)</size>", InfoMessageType.Warning)]
        [Button("Load From File Csv")]
        public void LoadFromFileCsv()
        {
            try
            {
                AndroidIronSourceKey = CSVReader.ReadCSVData(18, 4);
                AndroidAdmobAppID = CSVReader.ReadCSVData(19, 4);
                AndroidAppOpenAdsID = CSVReader.ReadCSVData(20, 4);

                iOSIronSourceKey = CSVReader.ReadCSVData(56, 4);
                iOSAdmobAppID = CSVReader.ReadCSVData(57, 4);
                iOSAppOpenAdsID = CSVReader.ReadCSVData(58, 4);

                FacebookAppID = CSVReader.ReadCSVData(23, 4);
                FacebookClientID = CSVReader.ReadCSVData(24, 4);

                AdjustAppToken = CSVReader.ReadCSVData(35, 3);
            }
            catch
            {
                EditorUtility.DisplayDialog("Not Exist File", "Không tồn tại file, hãy kiểm tra lại đặt đúng tên và đúng thư mục chưa", "OK");
            }
            LoadIntoGame();
        }
        //[Button("Load Into Game")]
        private void LoadIntoGame()
        {
            try
            {
                LoadIntoIronSource();
                LoadIntoAdmob();
                LoadIntoAdjust();
            }
            catch
            {
                return;
            }
        }
        private void LoadIntoIronSource()
        {
            var ISMS = Resources.Load<IronSourceMediationSettings>("IronSourceMediationSettings");
            if (ISMS != null)
            {
                ISMS.AndroidAppKey = AndroidIronSourceKey.Trim();
                ISMS.IOSAppKey = iOSIronSourceKey.Trim();
            }
            else
                Debug.LogError("IronSourceMediationSettings Null");
            var ISMNS = Resources.Load<IronSourceMediatedNetworkSettings>("IronSourceMediatedNetworkSettings");
            if (ISMNS != null)
            {
                ISMNS.EnableAdmob = true;
                ISMNS.AdmobAndroidAppId = AndroidAdmobAppID.Trim();
                ISMNS.AdmobIOSAppId = iOSAdmobAppID.Trim();
            }
            else
                Debug.LogError(" IronSourceMediatedNetworkSettings Null");

            try
            {
                FindObjectOfType<IronSourceControl>()._androidAppKey = AndroidIronSourceKey.Trim();
                FindObjectOfType<IronSourceControl>()._iosAppKey = iOSIronSourceKey.Trim();
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Do not have IronSourceControl", "Please open scene has MasterControl", "OK");
                Debug.LogError(e);
                throw;
            }
        }
        public void LoadIntoAdmob()
        {
            try
            {
                FindObjectOfType<AdmobControl>().androidAppOpenAdID = AndroidAppOpenAdsID.Trim();
                FindObjectOfType<AdmobControl>().iosAppOpenAdID = iOSAppOpenAdsID.Trim();
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Do not have AdmobControl", "Please open scene has MasterControl", "OK");
                Debug.LogError(e);
                throw;
            }
        }
        public void LoadIntoAdjust()
        {
            try
            {
                FindObjectOfType<Adjust>().appToken = AdjustAppToken.Trim();
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Do not have Adjust", "Please open scene has MasterControl", "OK");
                Debug.LogError(e);
                throw;
            }
        }
#endregion
        #region helper
        [Button("Go to Google Mobile Ads Settings")]
        public void OpenGoogleMobileAdsSettings()
        {
            EditorApplication.ExecuteMenuItem("Assets/Google Mobile Ads/Settings...");
        }
        #endregion

    }
}