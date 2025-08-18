using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [CustomPropertyDrawer(typeof(TypeSelectorAttribute))]
    public class TypeSelectorAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Obt�n el tipo de la interfaz desde el atributo
            Type interfaceType = GetInterfaceType(property);

            // Obt�n todos los tipos en el proyecto que implementan la interfaz
            Type[] implementingTypes = GetImplementingTypes(interfaceType);

            // Obt�n los nombres de los tipos para el desplegable
            string[] typeNames = GetImplementingTypeNames(implementingTypes);

            // Obt�n el �ndice del tipo seleccionado actualmente
            int selectedIndex = GetSelectedIndex(property, implementingTypes);

            // Dibuja el desplegable
            int newSelectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, typeNames);

            // Guarda el tipo seleccionado en la propiedad
            if (newSelectedIndex != selectedIndex)
            {
                property.stringValue = implementingTypes[newSelectedIndex].AssemblyQualifiedName;
            }

            EditorGUI.EndProperty();
        }

        private Type GetInterfaceType(SerializedProperty property)
        {
            // Obt�n el tipo de la interfaz desde el atributo
            TypeSelectorAttribute typeEnumAttribute = attribute as TypeSelectorAttribute;
            return typeEnumAttribute.InterfaceType;
        }

        private Type[] GetImplementingTypes(Type interfaceType)
        {
            // Obtiene todos los tipos en el proyecto
            Type[] allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .ToArray();

            // Filtra los tipos que implementan la interfaz
            return allTypes.Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToArray();
        }

        private string[] GetImplementingTypeNames(Type[] implementingTypes)
        {
            // Obtiene los nombres de los tipos para el desplegable
            return implementingTypes.Select(x => x.Name).ToArray();
        }

        private int GetSelectedIndex(SerializedProperty property, Type[] implementingTypes)
        {
            // Obtiene el nombre completo del tipo seleccionado actualmente
            string typeFullName = property.stringValue;

            // Busca el �ndice del tipo seleccionado actualmente
            for (int i = 0; i < implementingTypes.Length; i++)
            {
                if (implementingTypes[i].AssemblyQualifiedName == typeFullName)
                {
                    return i;
                }
            }

            // Si no se encuentra el tipo, establece el �ndice en 0 (primer elemento)
            return 0;
        }
    }
}
