using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    /// <summary>
    /// Interface used only for showing information about a Goal (as a short description) 
    /// for things like NodeCanvas, PropertyDrawers, etc.
    /// </summary>
    public interface IQuestGoalInfo
    {
#if UNITY_EDITOR
        public string Info { get; }
#endif
    } 
}
