using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        //The name of the bool field that will be in control
        public string ConditionalSourceField = "";
        //TRUE = Hide in inspector / FALSE = Disable in inspector 
        public bool Comparison = false;

        public ConditionalHideAttribute(string conditionalSourceField)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.Comparison = false;
        }

        public ConditionalHideAttribute(string conditionalSourceField, bool comparison)
        {
            this.ConditionalSourceField = conditionalSourceField;
            this.Comparison = comparison;
        }
    }
}