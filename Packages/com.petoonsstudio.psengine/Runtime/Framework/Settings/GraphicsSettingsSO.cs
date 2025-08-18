using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    [CreateAssetMenu(fileName = "GraphicSettingsSO", menuName = "Petoons Studio/PSEngine/Framework/Settings/Graphics Settings SO")]
    public class GraphicsSettingsSO : ScriptableObject
    {
        [SerializeReference, SubclassSelector] public IGraphicsSettings Settings;

        public virtual void LoadSettings(IGraphicsSettings settings)
        {
            if (settings != null)
            {
                Settings = settings;
                Settings.ApplySettings();
            }
        }
    }
}