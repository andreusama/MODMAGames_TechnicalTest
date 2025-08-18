#if UNITY_EDITOR
using CrossSceneReference;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

public class GuidReferenceDrawer : ObjectDrawer<GuidReference>
{
    GUIContent sceneLabel = new GUIContent("Containing Scene", "The target object is expected in this scene asset.");
    GUIContent clearButtonGUI = new GUIContent("Clear", "Remove Cross Scene Reference");

    public override GuidReference OnGUI(GUIContent content, GuidReference instance)
    {
        if(instance == null) { return instance; }

        System.Guid currentGuid;
        GameObject currentGO = null;

        byte[] byteArray = instance.SerializedGuid;

        currentGuid = new System.Guid(byteArray);
        currentGO = GuidManager.ResolveGuid(currentGuid);
        GuidComponent currentGuidComponent = currentGO != null ? currentGO.GetComponent<GuidComponent>() : null;

        GuidComponent component = null;

        if (currentGuid != System.Guid.Empty && currentGuidComponent == null)
        {
            bool guiEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.LabelField(new GUIContent(instance.CachedName, "Target GameObject is not currently loaded."), EditorStyles.objectField);
            GUI.enabled = guiEnabled;

            if (GUILayout.Button(clearButtonGUI, EditorStyles.miniButton))
            {

                ClearSerializedGUID(instance);
            }
        }
        else
        {
            component = EditorGUILayout.ObjectField(currentGuidComponent, typeof(GuidComponent), true) as GuidComponent;
        }

        if (currentGuidComponent != null && component == null)
        {
            ClearSerializedGUID(instance);
        }

        if (component != null)
        {
            instance.CachedName = component.name;
            string scenePath = component.gameObject.scene.path;
            instance.CachedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            if (component != currentGuidComponent)
            {
                instance.SerializedGuid = component.GetGuid().ToByteArray();
            }
        }

        EditorGUI.indentLevel++;
        bool cachedGUIState = GUI.enabled;
        GUI.enabled = false;
        EditorGUILayout.ObjectField(sceneLabel, instance.CachedScene, typeof(SceneAsset), false);
        GUI.enabled = cachedGUIState;
        EditorGUI.indentLevel--;

        return instance;
    }

    private void ClearSerializedGUID(GuidReference guidReference)
    {
        guidReference.CachedName = string.Empty;
        guidReference.CachedScene = null;

        for (int i = 0; i < guidReference.SerializedGuid.Length; ++i)
        {
            guidReference.SerializedGuid[i] = 0;
        }
    }
}
#endif
