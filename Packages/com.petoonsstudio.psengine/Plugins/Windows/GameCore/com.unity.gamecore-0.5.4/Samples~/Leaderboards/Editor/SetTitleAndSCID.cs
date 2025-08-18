using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_GAMECORE

namespace LeaderboardSample
{
    [InitializeOnLoad]
    public class SetTitleAndSCID : EditorWindow
    {
        [MenuItem("GameCoreSamples/Set Leaderboards TitleID and SCID")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SetTitleAndSCID));
        }

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
#if UNITY_GAMECORE_XBOXONE
            Settings = UnityEditor.GameCore.GameCoreXboxOneSettings.GetInstance();
#endif
#if UNITY_GAMECORE_SCARLETT
            Settings = UnityEditor.GameCore.GameCoreScarlettSettings.GetInstance();
#endif

            if (Settings.GameConfig.TitleId != TestTitleID || Settings.SCID != TestSCID)
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    ShowWindow();
                };
        }

        const string TestSandbox = "XDKS.1";
        const string TestTitleID = "73ECA03C";
        const string TestSCID = "00000000-0000-0000-0000-000073eca03c";

        void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Game Core Package Leaderboards Sample - Requirements");
            GUILayout.Space(10);
            GUILayout.Label("Make sure you set the Title ID, and SCID to the values below");
            GUILayout.Label("in Player Settings -> Publishing Settings and run on the " + TestSandbox + " sandbox.");
            GUILayout.Label("sandbox.");
            GUILayout.Space(30);
            GUILayout.Label("Title ID:");
            GUILayout.TextArea(TestTitleID);
            GUILayout.Label("SCID:");
            GUILayout.TextArea(TestSCID);
            GUILayout.Space(10);
            GUILayout.Label("And set the devkit sandbox to:");
            GUILayout.TextArea(TestSandbox);
            if (GUILayout.Button("Apply Values"))
            {
                Settings.GameConfig.TitleId = TestTitleID;
                Settings.SCID = TestSCID;

                Settings.ApplyAnyChanges();
            }
        }

#if UNITY_GAMECORE_XBOXONE
        static private UnityEditor.GameCore.GameCoreXboxOneSettings Settings;
#endif
#if UNITY_GAMECORE_SCARLETT
        static private UnityEditor.GameCore.GameCoreScarlettSettings Settings;
#endif
    }
}
#endif // UNITY_GAMECORE

