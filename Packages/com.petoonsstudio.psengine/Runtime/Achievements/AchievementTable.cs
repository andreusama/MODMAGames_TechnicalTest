using PetoonsStudio.PSEngine.Utils;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Achievements
{
    [CreateAssetMenu(fileName = "Achievement Table", menuName = "Petoons Studio/Achievement Table")]
    public class AchievementTable : AddressablesDatabase<AchievementTable, Achievement>
    {
#if UNITY_EDITOR
        /// <summary>
        /// Match table key with achievement ID
        /// </summary>
        private void OnValidate()
        {
            foreach (var item in Values)
            {
                if (item.Value.editorAsset != null)
                {
                    Achievement achievement = AssetDatabase.LoadAssetAtPath<Achievement>(AssetDatabase.GUIDToAssetPath(item.Value.AssetGUID));
                    achievement.ID = item.Key;
                }
            }
        }
#endif
    }
}