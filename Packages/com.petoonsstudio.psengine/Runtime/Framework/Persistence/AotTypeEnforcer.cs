using Newtonsoft.Json.Utilities;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    /// <summary>
    /// Ensures that the default constructor of the most used generic collections are compiled, like List, Dictionary and HashSet. This is to avoid IL2CPP stripping the constructors needed for reflection.
    /// This class does not need to be added to a script that's added to a GameObject. It just needs to be compiled. Inheriting from UnityEngine.MonoBehaviour will ensure to always be compiled.
    /// In order to add more types that are being stripped, overwrite the method EnforceTypes().
    /// </summary>
    public class AotTypeEnforcer : MonoBehaviour
    {
        private void Awake()
        {
            EnforceTypes();
        }

        /// <summary>
        /// Ensures the default constructors of the most used generic collections, like List and Dictionary, are compiled.
        /// You can overwrite this method to add more types that are being stripped. Use AotHelper.EnsureType<T>().
        /// </summary>
        protected virtual void EnforceTypes()
        {
            AotHelper.EnsureList<int>();
            AotHelper.EnsureDictionary<int, int>();
            //AotHelper.EnsureType<TypeBeingStripped>();
        }
    }
}
