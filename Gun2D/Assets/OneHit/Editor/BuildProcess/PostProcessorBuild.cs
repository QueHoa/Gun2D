using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace OneHit.Utility
{
    public class PostProcessorBuild : UnityEditor.Build.IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 1; } }
        GoogleDriveURL link;
        public void OnPostprocessBuild(BuildReport report)
        {
            link = Resources.Load<GoogleDriveURL>("GoogleDriveURL");
            string fileExtension = System.IO.Path.GetExtension(report.summary.outputPath);
            OpenLink(fileExtension);


        }
        void OpenLink(string type)
        {
            if (type == ".aab")
            {
                Application.OpenURL(link.aabLink);
            }
            else if (type == ".apk")
            {
                Application.OpenURL(link.apkLink);
            }
        }

       
    }

}