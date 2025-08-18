using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MonoScriptAttribute : PropertyAttribute
    {
        public System.Type type;
    }
}
