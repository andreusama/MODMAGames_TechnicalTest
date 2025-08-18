using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// Attribute that require implementation of the provided interface.
    /// </summary>
    public class InterfaceTypeAttribute : PropertyAttribute
    {
		public Type type;

		/// <summary>
		/// Creates a new InterfaceType attribute.
		/// </summary>
		/// <param name="type">The type of interface which is allowed.</param>
		public InterfaceTypeAttribute(Type type)
		{
			this.type = type;
		}
	}
}