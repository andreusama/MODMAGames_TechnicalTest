using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// Inherit from this class if you wanna create a new resolver
    /// </summary>
    public abstract class ObjectReplacerResolver
    {
        public abstract IList<ObjectReplacerResolverData> Data { get; protected set; }
        /// <summary>
        /// Resolves the new gameobject from the replacers.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="replacers"></param>
        /// <returns></returns>
        public abstract ObjectReplacerResolverData ResolveReplacement(GameObject gameObject);
        /// <summary>
        /// Draw the Resolver user interface, here you will find the necessary information to solve the replacement.
        /// </summary>
        /// <returns></returns>
        public abstract VisualElement DrawResolverUI();
        public abstract VisualElement DrawReplacersUI();

        public virtual void BindReplacerItem(VisualElement v, int i)
        {
            ObjectField objectField = v.Q<ObjectField>(ObjectReplacerResolverData.REPLACEMENT_NAME);
            objectField.value = Data[i].Replacement;
            objectField.RegisterValueChangedCallback((o) => Data[i].Replacement = o.newValue);
        }
        public virtual VisualElement MakeReplacerItem()
        {
            VisualElement root = new VisualElement();

            ObjectField objectField = new ObjectField();
            objectField.name = ObjectReplacerResolverData.REPLACEMENT_NAME;
            objectField.objectType = typeof(GameObject);
            objectField.allowSceneObjects = false;
            root.Add(objectField);

            return root;
        }
        public abstract void ReplacerItemAdded(IEnumerable<int> indices);

        public virtual void DrawHandles(SceneView sceneView)
        {
            foreach (var selection in Selection.gameObjects)
            {
                DrawSelectedHandle(sceneView, selection);
            }
        }

        public virtual void DrawSelectedHandle(SceneView sceneView, GameObject selectedGameObject)
        {
            if (Data.Count > 0)
            {
                ObjectReplacerResolverData replacement = ResolveReplacement(selectedGameObject);
                Color previewColor = new Color(0f, 1f, 0f, 0.15f);

                if(replacement == null && Data[Data.Count-1].Replacement != null)
                {
                    replacement = Data[Data.Count - 1];
                    previewColor = new Color(1f, 0f, 0f, 0.15f); 
                }

                if (replacement != null && replacement.Replacement != null)
                {
                    GameObject x = replacement.Replacement as GameObject;
                    Mesh tempMesh = x.GetComponent<MeshFilter>().sharedMesh;
                    Material mat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
                    mat.color = previewColor;
                    Graphics.DrawMesh(tempMesh,selectedGameObject.transform.position,selectedGameObject.transform.rotation, mat,0);
                }
            }
        }
    }
}
