using Newtonsoft.Json;
using PetoonsStudio.PSEngine.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class TemplateProjectWindow : EditorWindow
{
    private const string TEMPLATE_ROOT = "package";
    private const string TEMPLATE_PROJECT_DATA = "ProjectData~";
    private const string TEMPLATE_ASSETS = "Assets";
    private const string TEMPLATE_PROJECT_SETTINGS = "ProjectSettings";
    private const string TEMPLATE_PACKAGES = "Packages";
    private const string TEMPLATE_LIBRARY = "Library";

    private readonly string UnityEditorApplicationProjectTemplatesPath = Path.Combine(
        Path.GetDirectoryName(EditorApplication.applicationPath),
        "Data",
        "Resources",
        "PackageManager",
        "ProjectTemplates"
    );

    private readonly string[] m_LibraryDirectories =
    {
        "Artifacts"
    };

    private readonly string[] m_LibraryFiles =
    {
        "ArtifactDB",
        "SourceAssetDB"
    };

    private string m_WorkSpaceDirectory;

    private string m_TargetPath;
    private string m_TemplateName;
    private string m_TemplateDisplayName;
    private string m_TemplateDescription;
    private string m_TemplateDefaultScene;
    private string m_TemplateVersion;
    private SceneAsset m_TemplateDefaultSceneAsset;

    private bool m_ReplaceTemplate = true;

    public string TemplateDataPath => Path.Combine(m_TargetPath, TEMPLATE_PROJECT_DATA);
    public string TemplateAssetsPath => Path.Combine(TemplateDataPath, TEMPLATE_ASSETS);
    public string TemplateSettingsPath => Path.Combine(TemplateDataPath, TEMPLATE_PROJECT_SETTINGS);
    public string TemplatePackagesPath => Path.Combine(TemplateDataPath, TEMPLATE_PACKAGES);
    public string TemplateLibraryPath => Path.Combine(TemplateDataPath, TEMPLATE_LIBRARY);

    private string ProjectSettings => Application.dataPath.Replace("/Assets", "/ProjectSettings");
    private string ProjectPackages => Application.dataPath.Replace("/Assets", "/Packages");
    private string ProjectLibrary => Application.dataPath.Replace("/Assets", "/Library");

    [MenuItem("Window/Petoons Studio/PSEngine/Editor/Template Project Window", priority = ToolsUtils.EDITOR_CATEGORY)]
    private static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<TemplateProjectWindow>();
        var icon = EditorGUIUtility.Load("saveas@2x") as Texture2D;
        window.titleContent = new GUIContent("Save Project As Template", icon);
        window.Show();
    }
    private void OnGUI()
    {
        string projectFolder = Application.dataPath.Replace("/Assets", string.Empty);
        if (GUILayout.Button("Select Target Folder"))
        {
            m_WorkSpaceDirectory = EditorUtility.SaveFolderPanel("Choose target folder", projectFolder, "ProjectTemplate");
            m_TargetPath = Path.Combine(m_WorkSpaceDirectory, TEMPLATE_ROOT);
            SetTemplateDataFromPackageJson();
        }

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            m_WorkSpaceDirectory = EditorGUILayout.TextField("Path:", m_WorkSpaceDirectory);
            if (check.changed)
            {
                m_TargetPath = Path.Combine(m_WorkSpaceDirectory, TEMPLATE_ROOT);
                SetTemplateDataFromPackageJson();
            }
        }

        m_TemplateName = EditorGUILayout.TextField("Name:", m_TemplateName);
        m_TemplateDisplayName = EditorGUILayout.TextField("Display name:", m_TemplateDisplayName);
        m_TemplateDescription = EditorGUILayout.TextField("Description:", m_TemplateDescription);
        DefaultSceneGUI();
        m_TemplateVersion = EditorGUILayout.TextField("Version:", m_TemplateVersion);
        m_ReplaceTemplate = EditorGUILayout.Toggle("Replace template:", m_ReplaceTemplate);

        if (GUILayout.Button("Create"))
        {
            DeletePrevious();

            AssetDatabase.SaveAssets();
            CreateTemplate();
        }
    }
    private void DefaultSceneGUI()
    {
        m_TemplateDefaultSceneAsset = (SceneAsset)EditorGUILayout.ObjectField("Default scene asset:", m_TemplateDefaultSceneAsset, typeof(SceneAsset), false);
        if (m_TemplateDefaultSceneAsset != null)
        {
            m_TemplateDefaultScene = AssetDatabase.GetAssetPath(m_TemplateDefaultSceneAsset);
        }
        else
        {
            m_TemplateDefaultScene = null;
        }

        using (new EditorGUI.DisabledGroupScope(true))
        {
            EditorGUILayout.TextField("Default scene:", m_TemplateDefaultScene);
        }
    }

    private void CreateTemplate()
    {
        try
        {
            EditorUtility.DisplayProgressBar("Template Project", "Starting", 0f);
            Directory.CreateDirectory(m_TargetPath);

            EditorUtility.DisplayProgressBar("Template Project", "Copying Assets", 0.1f);
            Directory.CreateDirectory(TemplateAssetsPath);
            string s = Path.Combine(m_TargetPath, TEMPLATE_PROJECT_DATA);
            CopyDirectory(Application.dataPath, TemplateAssetsPath, true);

            EditorUtility.DisplayProgressBar("Template Project", "Copying Projectt Settings", 0.3f);
            Directory.CreateDirectory(TemplateSettingsPath);
            CopyDirectory(ProjectSettings, TemplateSettingsPath, true);
            DeleteProjectVersionTxt();

            EditorUtility.DisplayProgressBar("Template Project", "Copying Packages", 0.4f);
            Directory.CreateDirectory(TemplatePackagesPath);
            CopyDirectory(ProjectPackages, TemplatePackagesPath, true);

            EditorUtility.DisplayProgressBar("Template Project", "Copying Library", 0.6f);
            CopyLibrary();

            EditorUtility.DisplayProgressBar("Template Project", "Creating Manifest", 0.8f);
            CreatePackageManifest();

            EditorUtility.DisplayProgressBar("Template Project", "Compressing Template", 0.9f);
            Compress();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
    private void Compress()
    {
        string outputName = $"{m_TemplateName}-{m_TemplateVersion}.tgz";
        string outputPath = Path.Combine(m_WorkSpaceDirectory, outputName);
        string arguments = $"czf {outputName} {TEMPLATE_ROOT}";

        Process compressionProces = new Process();
        compressionProces.StartInfo.WorkingDirectory = m_WorkSpaceDirectory;
        compressionProces.StartInfo.Arguments = arguments;
        compressionProces.StartInfo.FileName = "tar";
        compressionProces.StartInfo.CreateNoWindow = true;
        compressionProces.Start();

        compressionProces.WaitForExit();
    }
    private void CopyLibrary()
    {
        Directory.CreateDirectory(TemplateLibraryPath);

        foreach (var directory in m_LibraryDirectories)
        {
            var sourcePath = Path.Combine(ProjectLibrary, directory);
            var destPath = Path.Combine(TemplateLibraryPath, directory);
            Directory.CreateDirectory(destPath);
            CopyDirectory(sourcePath, destPath, true);
        }

        foreach (var file in m_LibraryFiles)
        {
            var path = Path.Combine(ProjectLibrary, file);
            File.Copy(path, Path.Combine(TemplateLibraryPath, file));
        }
    }
    private void DeleteProjectVersionTxt()
    {
        var projectVersionTxtPath = Path.Combine($"{m_TargetPath}/ProjectData~/", "ProjectSettings", "ProjectVersion.txt");

        if (File.Exists(projectVersionTxtPath))
        {
            File.Delete(projectVersionTxtPath);
        }
        else
        {
            UnityEngine.Debug.LogErrorFormat("File ProjectVersion.txt does not exist at path: {0}", projectVersionTxtPath);
        }
    }
    private void CreatePackageManifest()
    {
        TemplateData templateData = new TemplateData();
        templateData.name = m_TemplateName;
        templateData.displayName = m_TemplateDisplayName;
        templateData.description = m_TemplateDescription;
        templateData.defaultScene = m_TemplateDefaultScene;
        templateData.version = m_TemplateVersion;

        string data = JsonUtility.ToJson(templateData);

        File.WriteAllText($"{m_TargetPath}/package.json", data);
    }
    private void SetTemplateDataFromPackageJson()
    {
        var packageJsonPath = Path.Combine(m_TargetPath, "package.json");
        if (File.Exists(packageJsonPath))
        {
            var packageJson = File.ReadAllText(packageJsonPath);
            var templateData = JsonUtility.FromJson<TemplateData>(packageJson);
            m_TemplateName = templateData.name;
            m_TemplateDisplayName = templateData.displayName;
            m_TemplateDescription = templateData.description;
            m_TemplateDefaultScene = templateData.defaultScene;
            m_TemplateVersion = templateData.version;
            SetDefaultSceneAssetFromPath();
        }
    }
    private void SetDefaultSceneAssetFromPath()
    {
        if (m_TemplateDefaultScene != String.Empty)
        {
            m_TemplateDefaultSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(m_TemplateDefaultScene);
            UnityEngine.Debug.AssertFormat(m_TemplateDefaultSceneAsset != null, "Failed to load scene asset at path from package.json, path: {0}", m_TemplateDefaultScene);
        }
        else
        {
            m_TemplateDefaultSceneAsset = null;
        }
    }
    private void DeletePrevious()
    {
        if (!Directory.Exists(m_TargetPath)) return;

        var directories = Directory.GetDirectories(m_TargetPath);

        foreach (var directory in directories)
        {
            Directory.Delete(directory, true);
        }

        var files = Directory.GetFiles(m_TargetPath);

        foreach (var file in files)
        {
            File.Delete(file);
        }
    }
    private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        DirectoryInfo[] dirs = dir.GetDirectories();
        Directory.CreateDirectory(destinationDir);
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    [Serializable]
    private class TemplateData
    {
#pragma warning disable 0649
        public string name = "com.unity.template.petoonstemplate";
        public string displayName = "Petoons Template";
        public string description = "Petoons Template";
        public string defaultScene = "Assets/_Scenes/Boot.unity";
        public string version = "1.0.0";
#pragma warning restore 0649
    }
}