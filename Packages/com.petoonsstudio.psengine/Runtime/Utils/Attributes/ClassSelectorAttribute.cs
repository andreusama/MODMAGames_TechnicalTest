using System;
using UnityEngine;


namespace PetoonsStudio.PSEngine.Utils
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ClassSelectorAttribute : PropertyAttribute
    {
        public Type SelectableType;
        public bool IncludeParent = false;
        public bool IncludeNone = true;

        public ClassSelectorAttribute(Type selectableType)
        {
            SelectableType = selectableType;
        }

        public ClassSelectorAttribute(Type selectableType, bool includeParent)
        {
            SelectableType = selectableType;
            IncludeParent = includeParent;
        }

        public ClassSelectorAttribute(Type selectableType, bool includeParent, bool includeNone)
        {
            SelectableType = selectableType;
            IncludeParent = includeParent;
            IncludeNone = includeNone;
        }
    }
}