// -------------------------------------------------------------------------------------------------
// Assets/Editor/JenkinsBuild.cs
// -------------------------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

// ------------------------------------------------------------------------
// https://docs.unity3d.com/Manual/CommandLineArguments.html
// ------------------------------------------------------------------------

namespace PetoonsStudio.PSEngine.BuildSystem
{
    public class JenkinsBuilder
    {
        /// <summary>
        /// Called from Jenkins
        /// Launch a build
        /// </summary>
        public static void BuildCI(string[] scenes)
        {
            SystemLog("Build CI Requested");

            SetUserBuildSettings();

            var args = FindBuildArguments();

            BuildParameterCommand[] commands = BuildParameterProcessorUtils.ParseCommands(args);
            SystemLog($"Detected {commands.Length} commands");

            ApplyBuildParametersProcessors(commands);

            BuildReport buildReport = BuildPipeline.BuildPlayer(scenes, PetoonsMasterBuilder.BuildParameters.TargetDir, PetoonsMasterBuilder.BuildParameters.BuildTarget, PetoonsMasterBuilder.BuildParameters.BuildOptions); ;
            BuildSummary buildSummary = buildReport.summary;
            if (buildSummary.result == BuildResult.Succeeded)
            {
                SystemLog("Build Success: Time:" + buildSummary.totalTime + " Size:" + buildSummary.totalSize + " bytes");
            }
            else
            {
                SystemLog("Build Failed: Time:" + buildSummary.totalTime + " Total Errors: " + buildSummary.totalErrors);
                throw new Exception("Build Failed: Time:" + buildSummary.totalTime + " Total Errors: " + buildSummary.totalErrors);
            }
        }

        private static void ApplyBuildParametersProcessors(BuildParameterCommand[] commands)
        {
            var parametersProcessors = BuildParameterProcessorUtils.GetParameterProcessors(commands).OrderBy(processor => processor.Priority);

            foreach (var processor in parametersProcessors)
            {
                var processorSelectedCommand = commands.First(value => value.ID == processor.ID);
                var result = processor.ApplyBuildParameter(processorSelectedCommand.Value);

                HandleResult(result, processorSelectedCommand);
            }
        }

        private static void HandleResult(BuildProcessorResult result, BuildParameterCommand command)
        {
            switch (result)
            {
                case BuildProcessorResult.Critical:
                    throw new Exception($"ERROR: {command.ID} throw error with value: {command.Value}");
                case BuildProcessorResult.Warning:
                    SystemLog($"WARNING: {command.ID} return error with value: {command.Value}");
                    return;
                case BuildProcessorResult.Success:
                default:
                    return;
            }
        }

        private static void SetUserBuildSettings()
        {
            EditorUserBuildSettings.explicitArrayBoundsChecks = true;
            EditorUserBuildSettings.explicitDivideByZeroChecks = true;
            EditorUserBuildSettings.explicitNullChecks = true;
        }

        /// <summary>
        /// Called from Jenkins
        /// Add directives in order to do a domain reload
        /// </summary>
        public static void AddDirectives()
        {
            SystemLog("Directives Requested");
            var defineArgument = FindDirectiveArguments();

            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();

            for (int i = 0; i < defineArgument.Count; i++)
            {
                if (!allDefines.Contains(defineArgument[i]))
                {
                    SystemLog($"Add define {defineArgument[i]}");
                    CITools.AddDefine(defineArgument[i]);
                }
            }
        }

        /// <summary>
        /// Called from Jenkins
        /// Build Addressables for current platform
        /// </summary>
        public static void BuildAddressables()
        {
            SystemLog("Addressables Build Requested");

            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();

            SystemLog("Addressables Build Ended");
        }

        private static List<string> FindDirectiveArguments()
        {
            List<string> directivesArgs = new List<string>();
            // find: -executeMethod
            //   +1: Directives separated by ';'

            string[] args = System.Environment.GetCommandLineArgs();
            var execMethodArgPos = -1;
            bool allArgsFound = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-executeMethod")
                {
                    execMethodArgPos = i;
                }
                var realPos = execMethodArgPos == -1 ? -1 : i - execMethodArgPos - 2;

                switch (realPos)
                {
                    case 0:
                        SystemLog("Directives: " + args[i]);
                        var directives = args[i].Split(';');
                        for (int j = 0; j < directives.Length; j++)
                        {
                            directivesArgs.Add(directives[j]);
                        }
                        allArgsFound = true;
                        break;
                }
            }

            if (!allArgsFound)
                SystemLog("Incorrect Parameters for adding directives. Format: <Directive>;<Directive>;<Directive>");

            return directivesArgs;
        }

        private static string[] FindBuildArguments()
        {
            string[] args = Environment.GetCommandLineArgs();

            var execMethodArgPos = -1;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-executeMethod")
                {
                    execMethodArgPos = i;
                }
            }
            var array = args.Skip(execMethodArgPos + 2).Take(args.Length - (execMethodArgPos + 1));
            SystemLog($"Commands found: {array.Count()}");
            return array.ToArray();
        }

        public static void SystemLog(string message)
        {
            Console.WriteLine("[JENKINS_BUILDER] " + message);
        }
    }
}