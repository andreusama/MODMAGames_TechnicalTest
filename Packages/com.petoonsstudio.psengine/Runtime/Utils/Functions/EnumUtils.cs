using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class EnumUtils
    {
        public static bool IsSetFilledWithAllEnumValues<T>(ISet<T> set) where T : Enum
        {
            return set.Count >= Enum.GetValues(typeof(T)).Length;
        }

        public static T RandomEnumWithExclusions<T>(ISet<T> exclusions) where T : Enum
        {
            System.Array enumValuesArray = System.Enum.GetValues(typeof(T));
            List<T> enumValuesList = ((T[])enumValuesArray).ToList();
            List<T> valuesWithExclusions = enumValuesList.Except(exclusions).ToList();

            if (valuesWithExclusions.Count == 0)
            {
                Debug.LogError("Cannot generate a random enum value because there's too much exclusions");
                return enumValuesList[0];
            }

            return valuesWithExclusions[UnityEngine.Random.Range(0, valuesWithExclusions.Count)];
        }

        public static T RandomEnum<T>(Array enumValues = null) where T : Enum
        {
            if (enumValues == null) enumValues = Enum.GetValues(typeof(T));
            int random = UnityEngine.Random.Range(0, enumValues.Length);
            return (T)enumValues.GetValue(random);
        }
    }
}
