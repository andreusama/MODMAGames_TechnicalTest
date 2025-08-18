using PetoonsStudio.PSEngine.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Tools
{
    public class InputActionAssetSetUp : EditorWindow
    {
        private static InputActionAsset m_InputActionAsset;
        private static EditorWindow m_Window;

        private const string HELPBOX_TOOL_TEXT = "Click setup to add nedded schemes for all platforms, if the schema is already present, it won't be added or modified. This is important as scheme are case sensitive and must be our pre defined names.";

        [MenuItem("Window/Petoons Studio/PSEngine/Input/Setup Input Asset", priority = ToolsUtils.INPUT_CATEGORY)]
        public static void OpenSetupInputActionWindow()
        {
            m_Window = GetWindow<InputActionAssetSetUp>();
            m_Window.titleContent = new GUIContent("Set up InputActionAsset");
            m_Window.minSize = new Vector2(425, 130);
            m_InputActionAsset = FindFirstInputActionAsset();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(5f);
            m_InputActionAsset = (InputActionAsset)EditorGUILayout.ObjectField(m_InputActionAsset, typeof(InputActionAsset), false);

            EditorGUILayout.Space(5f);

            if (GUILayout.Button("Set up"))
            {
                AddControlSchema(m_InputActionAsset, InputManager.INPUT_SCHEME_SWITCH);
                AddControlSchema(m_InputActionAsset, InputManager.INPUT_SCHEME_PC_GAMEPAD);
                AddControlSchema(m_InputActionAsset, InputManager.INPUT_SCHEME_PC_KEYBOARD);
                AddControlSchema(m_InputActionAsset, InputManager.INPUT_SCHEME_PS4);
                AddControlSchema(m_InputActionAsset, InputManager.INPUT_SCHEME_PS5);
                AddControlSchema(m_InputActionAsset, InputManager.INPUT_SCHEME_XBOX);
            }
            EditorGUILayout.HelpBox(HELPBOX_TOOL_TEXT, MessageType.Info);
        }

        private static void AddControlSchema(InputActionAsset actionAsset, string schemeName)
        {
            if (actionAsset.FindControlScheme(schemeName) == null)
            {
                actionAsset.AddControlScheme(new InputControlScheme(schemeName));
                Debug.Log($"<color=green>Schema: {schemeName} added.</color>");
            }
            else
            {
                Debug.Log($"<color=yellow>Schema: {schemeName} already exist!</color>");
                if (string.Compare(actionAsset.FindControlScheme(schemeName).Value.name, schemeName, false) != 0)
                {
                    Debug.LogError($"<color=red>ENSURE THAT THE NAME IS THE SAME: </color>Asset wrong scheme name:{actionAsset.FindControlScheme(schemeName).Value.name}/Correct scheme name:{schemeName}");
                }
            }
        }

        private static InputActionAsset FindFirstInputActionAsset()
        {
            var asset = AssetDatabase.FindAssets("t:InputActionAsset");
            string myAssetPath = AssetDatabase.GUIDToAssetPath(asset[0]);
            return AssetDatabase.LoadAssetAtPath<InputActionAsset>(myAssetPath);
        }
    }

}
