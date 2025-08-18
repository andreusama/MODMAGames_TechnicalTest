using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class MyCustomBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    //Required in order to test the VrSetup dialog
    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_2021_2_OR_NEWER
        UnityEditor.PS5.PlayerSettings.supportsVR = true;
#endif
    }
}
