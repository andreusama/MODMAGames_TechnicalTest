using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class IntegerValuesAttribute : PropertyAttribute
    {
        public readonly string[] Values;

        public IntegerValuesAttribute(int[] values)
        {
            Values = new string[values.Length];

            for(int i = 0; i < values.Length; i++)
            {
                Values[i] = values[i].ToString();
            }
        }
    }
}