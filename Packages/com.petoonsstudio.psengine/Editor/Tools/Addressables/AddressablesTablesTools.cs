using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Localization.Plugins.Google;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public static class AddressablesTablesTools
    {
        #region PRELOAD TABLES
        [MenuItem(ToolsUtils.LOCALIZATION_TOOLS_MENU + "Preload All Tables ON", priority = ToolsUtils.LOCALIZATION_CATEGORY)]
        public static void PreloadAllTablesON()
        {
            PreloadAllTables(true);
        }

        [MenuItem(ToolsUtils.LOCALIZATION_TOOLS_MENU + "Preload All Tables OFF", priority = ToolsUtils.LOCALIZATION_CATEGORY)]
        public static void PreloadAllTablesOFF()
        {
            PreloadAllTables(false);
        }

        public static void PreloadAllTables(bool preload)
        {
            int success = 0;
            int fail = 0;

            string title = "Preload All Table ";
            title += preload ? "ON" : "OFF";

            EditorUtility.DisplayProgressBar(title, "Fetching Tables", 0f);

            var collections = FindLocalizationTableCollections();

            EditorUtility.ClearProgressBar();

            string changes = $"{title} Changes: \n";

            for (int i = 0; i < collections.Length; i++)
            {
                var table = collections[i];
                EditorUtility.DisplayProgressBar(title, $"Current Table: {table.name} ({i}/{collections.Length})", i / collections.Length);
                try
                {
                    table.SetPreloadTableFlag(preload);
                    changes += $"{i}. {table.TableCollectionName} => SUCESS \n";
                    success++;
                }
                catch
                {
                    changes += $"{i}. {table.TableCollectionName} => FAIL \n";
                    fail++;
                }
            }

            EditorUtility.ClearProgressBar();

            Debug.Log(changes);

            EditorUtility.DisplayDialog(title, $"Setted all tables succesfull: {success}, with {fail} errors.", "Sugoi.");
        }

        private static void SetTablePreload(LocalizationTableCollection table, bool preload)
        {
        }
        #endregion

        #region PULL ALL SHEETS
        [MenuItem(ToolsUtils.LOCALIZATION_TOOLS_MENU + "Pull All StringTables", priority = ToolsUtils.LOCALIZATION_CATEGORY)]
        public static void UseTool()
        {
            PullAllTables();
        }

        private static void PullAllTables()
        {
            int success = 0;
            int fail = 0;

            EditorUtility.DisplayProgressBar("Pulling All Tables", "Fetching Tables", 0f);

            var collections = FindStringTableCollections();

            EditorUtility.ClearProgressBar();

            for (int i = 0; i < collections.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Pulling All Tables", $"Current Table: {collections[i].name} ({i}/{collections.Length})", i / collections.Length);
                try
                {
                    PullSheets(collections[i]);
                    success++;
                }
                catch
                {
                    fail++;
                }
            }

            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayDialog("Finished pulling all tables", $"Pulled {success} tables successfully, with {fail} errors.", "Sugoi.");
        }

        private static void PullSheets(StringTableCollection tableCollection)
        {
            var extension = tableCollection.Extensions[0] as GoogleSheetsExtension;

            var sheets = new GoogleSheets(extension.SheetsServiceProvider);
            sheets.SpreadSheetId = extension.SpreadsheetId;

            sheets.PullIntoStringTableCollection(extension.SheetId, tableCollection, extension.Columns);
        }
        #endregion

        private static LocalizationTableCollection[] FindLocalizationTableCollections()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(LocalizationTableCollection).ToString());
            LocalizationTableCollection[] collections = new LocalizationTableCollection[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                collections[i] = AssetDatabase.LoadAssetAtPath(path, typeof(LocalizationTableCollection)) as LocalizationTableCollection;
            }

            return collections;
        }

        private static StringTableCollection[] FindStringTableCollections()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(StringTableCollection).ToString());
            StringTableCollection[] collections = new StringTableCollection[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                collections[i] = AssetDatabase.LoadAssetAtPath(path, typeof(StringTableCollection)) as StringTableCollection;
            }

            return collections;
        }
    }

}