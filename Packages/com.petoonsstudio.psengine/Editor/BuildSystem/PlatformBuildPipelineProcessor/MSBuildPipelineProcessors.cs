using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class MSBuildPipelineProcessors : IPreBuildPipelineProcessor, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private const string MsixvcOutputDirectory = "MSIXVC";
        private const string Win32OutputDirectory = "Win32";
        private static string buildOutputFolderPath;
        private static string buildWin32OutputFolderPath;
        private static string buildMsixvcOutputFolderPath;

        public virtual void OnPreBuildPipeline()
        {
#if MICROSOFT_GAME_CORE
            //It uses GdkBuild.cs from GDK plugin for MicrosoftStore on ChooseOutputFolder() function in order to avoid changing inernal functions to be public
            Debug.Log("[PETOONS_BUILDER] Preparing build for MS");
            PreBuildPipeline();
#endif
        }

        public virtual void OnPostprocessBuild(BuildReport report)
        {
#if MICROSOFT_GAME_CORE
            Debug.Log("[PETOONS_BUILDER] Executing Post MS build steps");
            if (PetoonsMasterBuilder.CreatePackage)
                GdkBuild.PostBuild(buildWin32OutputFolderPath, buildMsixvcOutputFolderPath, PetoonsMasterBuilder.SubmissionEncryption);
#endif
        }

#if MICROSOFT_GAME_CORE
        protected virtual void PreBuildPipeline()
        {
            //Create necessay setup of BuildOptions in order to execute GdkBuild.PreBuild()
            PetoonsMasterBuilder.BuildOptions.locationPathName = new FileInfo(PetoonsMasterBuilder.BuildOptions.locationPathName).Directory.FullName;
            buildOutputFolderPath = PetoonsMasterBuilder.BuildOptions.locationPathName.Replace("/", "\\");
            buildWin32OutputFolderPath = buildOutputFolderPath + "\\" + Win32OutputDirectory;
            buildMsixvcOutputFolderPath = buildOutputFolderPath + "\\" + MsixvcOutputDirectory;
            Directory.CreateDirectory(buildWin32OutputFolderPath);
            Directory.CreateDirectory(buildMsixvcOutputFolderPath);
            GdkBuild.PreBuild(buildWin32OutputFolderPath);

            //Clean Buildoptions after PreBuild() in order to work for next Build steps
            PetoonsMasterBuilder.BuildOptions.locationPathName = buildWin32OutputFolderPath + "/" + PlayerSettings.productName + ".exe";
        }
#endif
    }
}
