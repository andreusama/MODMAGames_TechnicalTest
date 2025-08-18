using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PetoonsStudio.PSEngine.QuestSystem
{
    public interface IQuestGoalIcon
    {
        public AssetReferenceSprite ObjectiveIcon { get; }
    }
}