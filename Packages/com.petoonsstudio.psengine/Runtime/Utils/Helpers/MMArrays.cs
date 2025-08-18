using System.Collections.Generic;

namespace PetoonsStudio.PSEngine.Utils
{
    public class MMArrays
    {
        public static bool Contains<T>(T item, T[] mask)
        {
            foreach (var element in mask)
            {
                if (EqualityComparer<T>.Default.Equals(element, item)) return true;
            }
            return false;
        }
    }
}