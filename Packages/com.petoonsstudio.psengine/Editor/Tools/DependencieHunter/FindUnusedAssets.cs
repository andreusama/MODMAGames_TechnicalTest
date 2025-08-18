using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Tools
{
    public class FindUnusedAssets
    {
        private List<string> m_UnusedAssets = new List<string>();
        private DependenciesHunterInformation m_Information;
        private Vector2 m_ScrollPosition;
        private Vector2 m_ScrollIgnorePatternsPosition;
        private Vector2 m_ScrollMatchPatternsPosition;
        private int m_ItemsPerPage = 200;
        private int m_CurrentPage = 1;
        private bool m_ShowMatchPatternsFoldout = false;
        private bool m_ShowIgnorePatternsFoldout = false;
        private bool m_UnusedAssetsFoldout = false;
        private List<string> m_MatchPatterns = new List<string>();
        private List<string> m_IgnorePatterns = new List<string>();

        public FindUnusedAssets(DependenciesHunterInformation information)
        {
            PopulateDefaultIgnorePatterns();
            UpdateUnusedAssets(information);
        }

        public void PopulateDefaultIgnorePatterns()
        {
            m_IgnorePatterns = new List<string>
            {
                @"/Resources/",
                @"/Editor/",
                @"/Editor Default Resources/",
                @"/ThirdParty/",
                @"ProjectSettings/",
                @"Packages/",
                @"\.asmdef$",
                @"link\.xml$",
                @"\.csv$",
                @"\.md$",
                @"\.json$",
                @"\.xml$",
                @"\.txt$",
                @"\.cs$",
                @"\.a$",
                @"\.prx$",
                @"\.prefs$",
                @"\.cginc$",
                @"\.dll$",
                @"\.so$",
                @"\.framework$",
                @"\.dylib$",
                @"\.Config$",
                @"\.cpp$",
                @"\.XML$",
                @"\.exe$",
                @"\.zip$",
                @"\.mm$"
            };
        }
        
        public void UpdateUnusedAssets(DependenciesHunterInformation information)
        {
            m_UnusedAssets = GetUnusedAssets(information);
        }

        public void DrawGUI()
        {
            string num = (m_UnusedAssets != null) ? m_UnusedAssets.Count.ToString() : "0";
            EditorGUILayout.LabelField("Unused assets: " + $"{num} ");

            GUIUtilities.HorizontalLine();

            DrawCollapsableList(m_MatchPatterns, ref m_ShowMatchPatternsFoldout, "Match Patterns", ref m_ScrollMatchPatternsPosition, DrawMatchPatternsClear);

            DrawCollapsableList(m_IgnorePatterns, ref m_ShowIgnorePatternsFoldout, "Ignore Patterns", ref m_ScrollIgnorePatternsPosition, DrawIgnorePatternsDefault);

            GUIUtilities.HorizontalLine();

            if (m_UnusedAssets != null && m_UnusedAssets.Count > 0)
                DrawUnusedAssets(m_UnusedAssets);
            else
                DrawNoUnusedAssets();
        }

#region DRAW
        private void DrawMatchPatternsClear()
        {
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                m_MatchPatterns.Clear();
            }
        }

        private void DrawIgnorePatternsDefault()
        {
            if (GUILayout.Button(new GUIContent("Default", "Set the list to default list of ignore patterns."), EditorStyles.toolbarButton))
            {
                PopulateDefaultIgnorePatterns();
            }
        }
        private void DrawCollapsableList(List<string> m_List,ref bool colapse, string colapseText, ref Vector2 scroll, Action resetAction)
        {
            colapse = EditorGUILayout.Foldout(colapse, colapseText);
            if (colapse)
            {
                EditorGUILayout.BeginVertical("Box");
                scroll = EditorGUILayout.BeginScrollView(scroll);
                EditorGUI.indentLevel++;
                for (int i = 0; i < m_List.Count; i++)
                {
                    m_List[i] = EditorGUILayout.TextField(m_List[i].ToString());
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndScrollView();
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                resetAction?.Invoke();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", EditorStyles.toolbarButton))
                {
                    m_List.Add("");
                }
                if (GUILayout.Button("-", EditorStyles.toolbarButton))
                {
                    if (m_List.Count > 0)
                        m_List.RemoveAt(m_List.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawNoUnusedAssets()
        {
            EditorGUILayout.HelpBox("There are no unused assets.", MessageType.Info);
        }

        private void DrawUnusedAssets(List<string> unusedAssetsGUID)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_UnusedAssetsFoldout = EditorGUILayout.Foldout(m_UnusedAssetsFoldout, "UnusedAssets");
            if (m_UnusedAssetsFoldout)
            {
                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
                for (int i = (m_CurrentPage - 1) * m_ItemsPerPage; i < unusedAssetsGUID.Count && i < (m_CurrentPage - 1) * m_ItemsPerPage + m_ItemsPerPage; i++)
                {
                    DrawUnusedAsset(unusedAssetsGUID[i]);

                }
                EditorGUILayout.EndScrollView();
            }
            
            EditorGUILayout.EndVertical();

            GUIUtilities.HorizontalLine();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("<"))
            {
                if(m_CurrentPage > 1)
                    m_CurrentPage--;
            }
            GUILayout.Label($"{(m_CurrentPage - 1) * m_ItemsPerPage}-{m_CurrentPage * m_ItemsPerPage}", GUILayout.Width(100));
            if (GUILayout.Button(">"))
            {
                if(m_CurrentPage * m_ItemsPerPage < m_UnusedAssets.Count)
                    m_CurrentPage++;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawUnusedAsset(string assetGUID)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            GUIContent assetContent = GetAssetContent(assetGUID, assetPath);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(assetContent, GUILayout.MinWidth(300f), GUILayout.Height(18f)))
            {
                Selection.objects = new[] { AssetDatabase.LoadMainAssetAtPath(assetPath) };
            }
            EditorGUILayout.EndHorizontal();
        }
#endregion

        private GUIContent GetAssetContent(string assetGUID, string assetPath)
        {
            GUIContent content = new GUIContent();
            var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            content = EditorGUIUtility.ObjectContent(null, type);
            content.text = Path.GetFileName(assetPath);
            return content;
        }

        private List<string> GetUnusedAssets(DependenciesHunterInformation information)
        {
            m_Information = information;
            var finalList = m_Information.MapListCache.FindAll(x =>
            {
                var path = AssetDatabase.GUIDToAssetPath(x.Guid);

                //Filter match patterns if are there
                var match = IsMatchForOutput(path, m_MatchPatterns);
                return x.Dependencies.Count <= 0 
                    && IsValidForOutput(path, m_IgnorePatterns)
                    && !AssetDatabase.IsValidFolder(path)
                    && match;
            }).ToList();
            var returnList = finalList.Select(x => 
            {
                return x.Guid;
            }).ToList();

            return returnList;
        }

        private bool IsValidForOutput(string path, List<string> ignoreInOutputPatterns)
        {
            return ignoreInOutputPatterns.All(pattern
                => string.IsNullOrEmpty(pattern) || !Regex.Match(path, pattern).Success);
        }

        private bool IsMatchForOutput(string path, List<string> ignoreInOutputPatterns)
        {
            return ignoreInOutputPatterns.All(pattern
                => Regex.Match(path, pattern).Success);
        }
    }
}
