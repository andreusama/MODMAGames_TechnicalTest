using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace PetoonsStudio.PSEngine.Tools
{
    public static class PetoonsProcessorUtils
    {
        public static string SPLIT_REGEX = @"\[.*?\]";
        public static string OPEN_KEY = "[";
        public static string CLOSE_KEY = "]";
        public static bool ContainsKey(AssetImporter importer, string importerKey)
        {
            return importer.userData.Contains(importerKey);
        }

        public static bool AlreadyImported(AssetImporter importer, string importedKey)
        {
            return importer.userData.Contains(importedKey);
        } 

        public static void AddUserData(AssetImporter importer, string data)
        {
            if (string.IsNullOrEmpty(importer.userData))
            {
                importer.userData = OPEN_KEY + data + CLOSE_KEY;
            }
            else
            {
                importer.userData += OPEN_KEY + data + CLOSE_KEY;
            }
        }

        public static string GetImporterUserData(AssetImporter importer, string userDataKey)
        {
            string data = importer.userData;
            string[] result = Regex.Matches(data, @"\[.*?\]").Cast<Match>().Select(m => m.Value).ToArray();
            foreach (var dataSegment in result)
            {
                if (dataSegment.Contains(userDataKey))
                    return dataSegment.Replace(OPEN_KEY,"").Replace(CLOSE_KEY,"");
            }
            return null;
        }

        internal static string CreateDirectoryRecursive(string currentDirectory)
        {
            var directories = currentDirectory.Split('/');
            var lasteDirectoryName = directories[directories.Length - 1];
            var splittedDirectories = directories.SkipLast(1);
            var parentDirectory = string.Join('/', splittedDirectories);
            if (!AssetDatabase.IsValidFolder(parentDirectory))
            {
                CreateDirectoryRecursive(parentDirectory);
            }

            AssetDatabase.CreateFolder(parentDirectory, lasteDirectoryName);
            return currentDirectory;
        }
    }
}
