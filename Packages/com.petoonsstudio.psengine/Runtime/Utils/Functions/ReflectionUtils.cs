using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public class ReflectionUtils : MonoBehaviour
    {
        public static List<Type> GetClassChildren(Type parentClass, bool includeParent = false)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => parentClass.IsAssignableFrom(p) && (includeParent || p != parentClass));

            return types.ToList();
        }

        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default;
            PropertyInfo[] properties = type.GetProperties(flags);
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    try
                    {
                        property.SetValue(copy, property.GetValue(original, null), null);
                    }
                    catch { }
                    // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it,
                    // so I didn't catch anything specific.
                }
            }

            FieldInfo[] fields = type.GetFields(flags);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }

        // AudioSources need special treatment, thus the generic one does not work for them
        // Source: https://discussions.unity.com/t/how-to-get-a-component-from-an-object-and-add-it-to-another-copy-components-at-runtime/80939/7
        public static AudioSource CopyAudioSource(AudioSource original, GameObject destination)
        {
            AudioSource copy = destination.AddComponent<AudioSource>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default;
            PropertyInfo[] properties = typeof(AudioSource).GetProperties(flags);
            foreach (var property in properties)
            {
                if (property.CanWrite && property.Name != "minVolume" && property.Name != "maxVolume" && property.Name != "rolloffFactor")
                {
                    try
                    {
                        property.SetValue(copy, property.GetValue(original, null), null);
                    }
                    catch { }
                    // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it,
                    // so I didn't catch anything specific.
                }
            }

            FieldInfo[] fields = typeof(AudioSource).GetFields(flags);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }

            AnimationCurve animationCurve = original.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            copy.SetCustomCurve(AudioSourceCurveType.CustomRolloff, animationCurve);

            return copy;
        }
    }
}