using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.BuildSystem
{
#if UNITY_PS4
    public class PS4BuildPipelineProcessors : IPreBuildPipelineProcessor
    {
        private const string k_ParamElementName = "param";
        private const string k_AttributeKeyName = "key";
        private const string k_AttributeCategoryName = "CATEGORY";
        private const string k_AppCategoryPatch = "gp";
        private const string k_AppCategoryApplication = "gd";

        public void OnPreBuildPipeline()
        {
            CheckCurrentSFXType();
        }

        /// <summary>
        /// This function ensures that Unity build won't dfaile
        /// </summary>
        private void CheckCurrentSFXType()
        {
            var parameterPath = PlayerSettings.PS4.paramSfxPath;

            if (!File.Exists(parameterPath) || string.IsNullOrEmpty(parameterPath)) return;

            XDocument paramDoc = XDocument.Load(parameterPath);
            try
            {
                XElement xElement = GetDescendantParamElement(paramDoc, k_AttributeCategoryName);
                
                if(xElement == null)
                    return;

                if(xElement.Value == k_AppCategoryPatch)
                {
                    PlayerSettings.PS4.category = PlayerSettings.PS4.PS4AppCategory.Patch;
                }
                else if (xElement.Value == k_AppCategoryApplication)
                {
                    PlayerSettings.PS4.category = PlayerSettings.PS4.PS4AppCategory.Application;
                }
            }
            catch (Exception)
            {
                Debug.LogError("Invalid or corrupt params for PS4.");
                return;
            }
        }

        private XElement GetDescendantParamElement(XDocument document, string keyValue)
        {
            return document.Descendants(k_ParamElementName).FirstOrDefault(el => el.Attribute(k_AttributeKeyName)?.Value == keyValue);
        }
    }
#endif
}
