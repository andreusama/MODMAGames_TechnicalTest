using PetoonsStudio.PSEngine.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_SWITCH
using UnityEngine.InputSystem.Switch;
using nn.hid;
#endif

namespace PetoonsStudio.PSEngine.Input
{
    public struct BindingIconData
    {
        public Sprite IconSprite;
        public string BindingKeyText;
        public bool IsKeyboardBinding;
    }

    public struct NewProviderEvent
    {
        public IIconProvider NewProvider;

        public NewProviderEvent(IIconProvider newProvider)
        {
            NewProvider = newProvider;
        }
    }

    [CreateAssetMenu(fileName = "IconServiceProvider", menuName = "Petoons Studio/PSEngine/Input/Icons/Icon Service Provider")]
    public class IconServiceProvider : SingletonScriptableObject<IconServiceProvider>
    {
        [Header("Common gamepads")]
        public AssetReferenceT<GamepadIconProvider> PSIconProvider;
        public AssetReferenceT<GamepadIconProvider> PS5IconProvider;
        public AssetReferenceT<GamepadIconProvider> XboxIconProvider;

        [Header("Switch Icon Providers")]
        public AssetReferenceT<GamepadIconProvider> SwitchHandheldIconProvider;
        public AssetReferenceT<GamepadIconProvider> SwitchJoyconIconProvider;
        public AssetReferenceT<GamepadIconProvider> SwitchProIconProvider;

        [Header("Keyboard")]
        public AssetReferenceT<KeyboardIconProvider> KeyboardIconProvider;

        private IIconProvider m_CurrentIconProvider;

        public bool ProviderRequestDone { get; set; }

        public TMP_SpriteAsset GetTextSpriteAsset()
        {
            if (!ProviderRequestDone)
                return null;

            return m_CurrentIconProvider.IconTextSprites;
        }

        public void UpdateDeviceProvider(InputDevice device)
        {
            ProviderRequestDone = false;

#if UNITY_SWITCH
            if (device is NPad)
            {
                switch ((device as NPad).styleMask)
                {
                    case NPad.NpadStyles.JoyLeft:
                    case NPad.NpadStyles.JoyRight:
                        Addressables.LoadAssetAsync<GamepadIconProvider>(SwitchJoyconIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
                        break;
                    default:
                        Addressables.LoadAssetAsync<GamepadIconProvider>(SwitchHandheldIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
                        break;
                }
            }
    #if UNITY_EDITOR
            else
            {
                Addressables.LoadAssetAsync<GamepadIconProvider>(SwitchHandheldIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
            }
    #endif
#elif UNITY_XBOXONE || UNITY_GAMECORE
            Addressables.LoadAssetAsync<GamepadIconProvider>(XboxIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
#elif UNITY_PS4
            Addressables.LoadAssetAsync<GamepadIconProvider>(PSIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
#elif UNITY_PS5
            Addressables.LoadAssetAsync<GamepadIconProvider>(PS5IconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
#else
            if (device.description.ToString().Contains("Keyboard"))
            {
                Addressables.LoadAssetAsync<KeyboardIconProvider>(KeyboardIconProvider).Completed += handle => UpdateIconProvider<KeyboardIconProvider>(handle);
            }
            else if (device.description.ToString().Contains("Generic") || device.description.ToString().Contains("XInput"))
            {
                Addressables.LoadAssetAsync<GamepadIconProvider>(XboxIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
            }
            else if (device.description.ToString().Contains("Sony") || device.description.ToString().Contains("PlayStation"))
            {
                if (device.name.Contains("DualSense"))
                {
                    Addressables.LoadAssetAsync<GamepadIconProvider>(PS5IconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
                }
                else
                {
                    Addressables.LoadAssetAsync<GamepadIconProvider>(PSIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
                }
            }
            else if (device.description.ToString().Contains("Nintendo"))
            {
                Addressables.LoadAssetAsync<GamepadIconProvider>(SwitchProIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
            }
            else if (device.name.Contains("AirConsole"))
            {
                m_CurrentIconProvider = null;
            }
            else
            {
                Addressables.LoadAssetAsync<GamepadIconProvider>(XboxIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
            }
#endif
        }

        private void UpdateIconProvider<T>(AsyncOperationHandle<T> handle) where T : IIconProvider
        {
            m_CurrentIconProvider = handle.Result;
            m_CurrentIconProvider.Initialize();

            ProviderRequestDone = true;

            PSEventManager.TriggerEvent(new NewProviderEvent(m_CurrentIconProvider));
        }

        public Sprite RequestSprite(string mapType, string controlPath)
        {
            if (string.IsNullOrEmpty(controlPath) || string.IsNullOrEmpty(mapType))
            {
                return null;
            }

            if (m_CurrentIconProvider == null)
            {
                return null;
            }

            return m_CurrentIconProvider.GetSprite(mapType, controlPath);
        }

        public void LoadDefaultProvider()
        {
            ProviderRequestDone = false;

#if UNITY_SWITCH
            Addressables.LoadAssetAsync<GamepadIconProvider>(SwitchHandheldIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
#elif UNITY_XBOXONE || UNITY_GAMECORE
            Addressables.LoadAssetAsync<GamepadIconProvider>(XboxIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
#elif UNITY_PS4
            Addressables.LoadAssetAsync<GamepadIconProvider>(PSIconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
#elif UNITY_PS5
            Addressables.LoadAssetAsync<GamepadIconProvider>(PS5IconProvider).Completed += handle => UpdateIconProvider<GamepadIconProvider>(handle);
#else
            Addressables.LoadAssetAsync<KeyboardIconProvider>(KeyboardIconProvider).Completed += handle => UpdateIconProvider<KeyboardIconProvider>(handle);
#endif
        }

        /// <summary>
        /// Get Binding Icon based on Action Binding
        /// </summary>
        /// <param name="action"></param>
        /// <param name="isRemappingSprite"></param>
        /// <returns></returns>
        public BindingIconData GetBindingIcon(InputBinding binding, string mapKey)
        {
            string controlPath = InputManager.GetBindingControlPath(binding);

            return GetIconData(controlPath, mapKey);
        }

        /// <summary>
        /// Get icon data from path + map
        /// </summary>
        /// <param name="controlPath"></param>
        /// <param name="mapKey"></param>
        /// <returns></returns>
        private BindingIconData GetIconData(string controlPath, string mapKey)
        {
            BindingIconData data = new BindingIconData();

            if (m_CurrentIconProvider is KeyboardIconProvider)
            {
                KeyboardIconProvider keyBoardProvider = m_CurrentIconProvider as KeyboardIconProvider;

                if (!keyBoardProvider.IsSpecialKey(controlPath, mapKey))
                {
                    data.BindingKeyText = InputControlPath.ToHumanReadableString(controlPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    data.BindingKeyText = data.BindingKeyText.FirstCharToUpper();
                }
            }

            data.IconSprite = RequestSprite(mapKey, controlPath);

#if UNITY_STANDALONE
            data.IsKeyboardBinding = InputManager.Instance.CurrentDevice == null ? true : InputManager.Instance.CurrentDevice is Keyboard;
#else
            data.IsKeyboardBinding = false;
#endif
            return data;
        }
    }
}