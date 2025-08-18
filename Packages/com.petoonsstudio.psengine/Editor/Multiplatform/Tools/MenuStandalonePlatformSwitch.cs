using PetoonsStudio.PSEngine.BuildSystem;
using UnityEditor;

namespace PetoonsStudio.PSEngine.Tools
{
    public static class MenuStandalonePlatformSwitch
    {
        private static string[] Defines = new[] { "STANDALONE_STEAM", "STANDALONE_EPIC", "STANDALONE_GOG", "MICROSOFT_GAME_CORE" };
        private enum StandalonePlatforms : int
        {
            STEAM, EPIC, GOG, WS
        }

        [MenuItem(ToolsUtils.MULTIPLATFORM_TOOLS_MENU + "Standalone/Switch To Standalone")]
        public static void BackToStandalone()
        {
            DeleteStandalonePlatformDirectives();
        }

        [MenuItem(ToolsUtils.MULTIPLATFORM_TOOLS_MENU + "Standalone/Switch To Steam")]
        public static void SwitchToSteam()
        {
            DeleteStandalonePlatformDirectives();

            CITools.AddDefine(Defines[(int)StandalonePlatforms.STEAM]);
        }

        [MenuItem(ToolsUtils.MULTIPLATFORM_TOOLS_MENU + "Standalone/Switch To Epic")]
        public static void SwitchToEpic()
        {
            DeleteStandalonePlatformDirectives();

            CITools.AddDefine(Defines[(int)StandalonePlatforms.EPIC]);
        }

        [MenuItem(ToolsUtils.MULTIPLATFORM_TOOLS_MENU + "Standalone/Switch To GOG")]
        public static void SwitchToGOG()
        {
            DeleteStandalonePlatformDirectives();

            CITools.AddDefine(Defines[(int)StandalonePlatforms.GOG]);
        }

        [MenuItem(ToolsUtils.MULTIPLATFORM_TOOLS_MENU + "Standalone/Switch To WS")]
        public static void SwitchToWS()
        {
            DeleteStandalonePlatformDirectives();

            CITools.AddDefine(Defines[(int)StandalonePlatforms.WS]);
        }

        private static void DeleteStandalonePlatformDirectives()
        {
            foreach (var platform in Defines)
            {
                if (CITools.ExistDefine(platform))
                {
                    CITools.RemoveDefine(platform);
                }
            }
        }
    }
}
