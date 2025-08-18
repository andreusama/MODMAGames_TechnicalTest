using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input
{
    [CreateAssetMenu(fileName = "BindingExclusionsList", menuName = "Petoons Studio/Input/Binding Exclusions List")]
    public class BindingList : ScriptableObject
    {
        public List<string> ExcludeList;
    }
}