using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using PetoonsStudio.PSEngine.BuildSystem;

#if UNITY_GAMECORE
using UnityEditor.GameCore;
#endif

namespace PetoonsStudio.PSEngine.Tools
{
    public class PetoonsBuildWindow : EditorWindow
    {
        private bool m_BuildContent = false;

        private static string[] m_Directives;
        private static string NswDirective;
        private static string Ps4Directive;
        private static string Ps5Directive;

        private Vector2 m_BuildInformationScrollPosition;
        private static bool m_SummaryFoldout = true;
        private bool m_DirectiveFoldout = true;
        private static bool m_SDKFoldout = true;
        private static float m_SummaryMinWidth = 115f;

        private static bool m_AddressablesSettingsExist = false;

        private const string BUILD_ADDRESSABLES_WITH_PLAYER_BUILD = "Addressables.BuildAddressablesWithPlayerBuild";

        public static void Init()
        {
            PetoonsBuildWindow window = (PetoonsBuildWindow)EditorWindow.GetWindow(typeof(PetoonsBuildWindow));
            window.titleContent = new GUIContent("Build");
            Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            window.position = new Rect(mousePos.x, mousePos.y, window.position.width, window.position.height);
            window.Show();

            m_Directives = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            NswDirective = Environment.GetEnvironmentVariable("NINTENDO_SDK_ROOT");
            Ps4Directive = Environment.GetEnvironmentVariable("SCE_ORBIS_SDK_DIR");
            Ps5Directive = Environment.GetEnvironmentVariable("SCE_PROSPERO_SDK_DIR");

            m_AddressablesSettingsExist = AddressableAssetSettingsDefaultObject.SettingsExists;
            if (m_AddressablesSettingsExist)
            {
                SetupAddressableBuildSettings();
            }
        }

        public static void CloseWindow()
        {
            PetoonsBuildWindow window = (PetoonsBuildWindow)EditorWindow.GetWindow(typeof(PetoonsBuildWindow));
            window.Close();
        }

        void OnGUI()
        {
            EditorGUILayout.Space();

            DrawVersion();

            if (m_AddressablesSettingsExist)
                DrawAddressablesOptions();

            DrawAdditionalOptions();

            m_BuildInformationScrollPosition = EditorGUILayout.BeginScrollView(m_BuildInformationScrollPosition, GUILayout.ExpandHeight(false));

            DrawBuildSummary();

            DrawDirectives();

#if UNITY_SWITCH || UNITY_PS4 || UNITY_PS5
            DrawSDKs();
#endif
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("Build"))
            {
                PetoonsMasterBuilder.InternalBuildPipeline(m_BuildContent);
                CloseWindow();
            }

            if (GUILayout.Button("Cancel"))
            {
                CloseWindow();
            }
        }

        private static void DrawVersion()
        {
            EditorGUILayout.BeginVertical("Helpbox");
            PlayerSettings.bundleVersion = EditorGUILayout.TextField("New Version", PlayerSettings.bundleVersion);
            EditorGUILayout.EndVertical();
        }

        private void DrawAdditionalOptions()
        {
#if MICROSOFT_GAME_CORE
            EditorGUILayout.BeginVertical("Helpbox");
            EditorGUILayout.LabelField("Microsoft Store");
            EditorGUI.indentLevel++;
            PetoonsMasterBuilder.CreatePackage = EditorGUILayout.Toggle("Create Package", PetoonsMasterBuilder.CreatePackage);
            if (PetoonsMasterBuilder.CreatePackage)
            {
                PetoonsMasterBuilder.SubmissionEncryption = EditorGUILayout.Toggle("Submission Encription", PetoonsMasterBuilder.SubmissionEncryption);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
#endif
        }

        private void DrawAddressablesOptions()
        {
            EditorGUILayout.BeginVertical("Helpbox");
            m_BuildContent = EditorGUILayout.Toggle("Build Addressables", m_BuildContent);
            EditorGUILayout.EndVertical();
        }

        private static void DrawBuildSummary()
        {
            var path = PetoonsMasterBuilder.BuildOptions.locationPathName;
            EditorGUILayout.BeginVertical("Helpbox");
            m_SummaryFoldout = EditorGUILayout.Foldout(m_SummaryFoldout, "Summary");
            if (m_SummaryFoldout)
            {
                DrawSummaryField("Path", path);
                DriveInfo drive = GetDriveInfo(path);
                DrawSummaryField("Availabe space", GetSpaceContent(drive));
                DrawSummaryField("Target", PetoonsMasterBuilder.BuildOptions.target.ToString());
#if UNITY_STANDALONE
                DrawSubPlatformTarget();
#endif
                DrawSummaryField("Current Version", Application.version);
                DrawSummaryField("Options", (PetoonsMasterBuilder.BuildOptions.options.HasFlag(BuildOptions.Development) ? "Development" : "Non Development"));
#if !UNITY_STANDALONE
                DrawBuildType();
#endif
            }
            EditorGUILayout.EndVertical();
        }


        private void DrawDirectives()
        {
            if (m_Directives == null || m_Directives.Length <= 0 || (m_Directives.Length == 1 && m_Directives[0] == String.Empty))
                return;

            EditorGUILayout.BeginVertical("Helpbox");
            m_DirectiveFoldout = EditorGUILayout.Foldout(m_DirectiveFoldout, "Directives");
            if (m_DirectiveFoldout)
            {
                EditorGUI.indentLevel++;
                foreach (var directive in m_Directives)
                {
                    EditorGUILayout.LabelField(new GUIContent("-" + directive));
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSDKs()
        {
            EditorGUILayout.BeginVertical("Helpbox");
            m_SDKFoldout = EditorGUILayout.Foldout(m_SDKFoldout, "SDK's");
            if (m_SDKFoldout)
            {
#if UNITY_SWITCH
                if (!string.IsNullOrEmpty(NswDirective))
                    EditorGUILayout.LabelField("NSW: " + NswDirective);
#elif UNITY_PS4
            if (!string.IsNullOrEmpty(PlayerSettings.PS4.SdkOverride))
                EditorGUILayout.LabelField("PS4 override: " + PlayerSettings.PS4.SdkOverride);
            if (!string.IsNullOrEmpty(Ps4Directive))
                EditorGUILayout.LabelField("PS4 current: " + Ps4Directive);
#elif UNITY_PS5
            if (!string.IsNullOrEmpty(UnityEditor.PS5.PlayerSettings.sdkOverride))
                EditorGUILayout.LabelField("PS4 override: " + UnityEditor.PS5.PlayerSettings.sdkOverride);
            if (!string.IsNullOrEmpty(Ps5Directive))
                EditorGUILayout.LabelField("PS5 current: " + Ps5Directive);
#endif
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawSubPlatformTarget()
        {
#if STANDALONE_STEAM
            DrawSummaryField("Platform", "STEAM");
#elif STANDALONE_GOG
            DrawSummaryField("Platform", "GOG");
#elif STANDALONE_EPIC
            DrawSummaryField("Platform", "EPIC");
#elif MICROSOFT_GAME_CORE
            DrawSummaryField("Platform", "WS");
#else
            DrawSummaryField("Platform", "Standalone");
#endif
        }

        private static void DrawBuildType()
        {
#if UNITY_SWITCH
            DrawSummaryField("Build Type", EditorUserBuildSettings.switchCreateRomFile ? "NSP" : "Native");
#elif UNITY_PS4
            DrawSummaryField("Build Type", EditorUserBuildSettings.ps4BuildSubtarget.ToString());
#elif UNITY_PS5
            DrawSummaryField("Build Type", UnityEditor.PS5.PlayerSettings.buildSubtarget.ToString());
#elif UNITY_GAMECORE
#if UNITY_GAMECORE_XBOXONE
            DrawSummaryField("Build Type", GameCoreXboxOneSettings.GetInstance().DeploymentMethod.ToString());
            DrawSummaryField("Encryption", GameCoreXboxOneSettings.GetInstance().PackageEncryption.ToString());
#elif UNITY_GAMECORE_XBOXSERIES
            DrawSummaryField("Build Type", GameCoreScarlettSettings.GetInstance().DeploymentMethod.ToString());
            DrawSummaryField("Encryption", GameCoreScarlettSettings.GetInstance().PackageEncryption.ToString());
#endif
#endif
        }

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        private static DriveInfo GetDriveInfo(string path)
        {
            FileInfo f = new FileInfo(path);
            string driveRoot = Path.GetPathRoot(f.FullName);
            return new DriveInfo(driveRoot);
        }

        private static GUIContent GetSpaceContent(DriveInfo drive)
        {
            GUIContent availableSpaceContent = new GUIContent();
            if (drive.AvailableFreeSpace < 53687091200L)//50GB = 50 * 1024 * 1024 * 1024
            {
                availableSpaceContent = new GUIContent(EditorGUIUtility.IconContent("console.warnicon.sml"));
            }
            else if (drive.AvailableFreeSpace < 16106127360L)//15GB = 15 * 1024 * 1024 * 1024
            {
                availableSpaceContent = new GUIContent(EditorGUIUtility.IconContent("console.erroricon.sml"));
            }
            availableSpaceContent.text = $"{FormatBytes(drive.AvailableFreeSpace)} / {FormatBytes(drive.TotalSize)}";

            return availableSpaceContent;
        }

        private static void DrawSummaryField(string optionValue, string summaryValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(optionValue + ": ", EditorStyles.boldLabel, GUILayout.MaxWidth(m_SummaryMinWidth));
            EditorGUILayout.LabelField(summaryValue);
            EditorGUILayout.EndHorizontal();
        }
        private static void DrawSummaryField(string optionValue, GUIContent valueContent)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(optionValue + ": ", EditorStyles.boldLabel, GUILayout.MaxWidth(m_SummaryMinWidth));
            EditorGUILayout.LabelField(valueContent);
            EditorGUILayout.EndHorizontal();
        }


        private static void SetupAddressableBuildSettings()
        {
            EditorPrefs.SetBool(BUILD_ADDRESSABLES_WITH_PLAYER_BUILD, false);
            AddressableAssetSettingsDefaultObject.Settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.PreferencesValue;
        }
    }
}
