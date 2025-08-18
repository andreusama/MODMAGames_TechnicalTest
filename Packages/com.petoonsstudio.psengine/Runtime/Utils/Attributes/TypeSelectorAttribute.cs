using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeSelectorAttribute : PropertyAttribute
    {
        public Type InterfaceType { get; }

        public TypeSelectorAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}
