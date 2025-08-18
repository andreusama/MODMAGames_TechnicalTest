using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class EnGUIPanel : MonoBehaviour
    {
        [SerializeField] protected EnGUIContent m_Content;
        [SerializeField] protected EnGUIManager.PreviousBehaviour m_PreviousBehaviour = EnGUIManager.PreviousBehaviour.DoNothing;
        [SerializeField] protected EnGUIManager.ContentBehaviour m_SelfBehaviour = EnGUIManager.ContentBehaviour.Open;

        public delegate void PanelDelegate();

        public event PanelDelegate OnPanelAwake;
        public event PanelDelegate OnPanelDestroy;

        protected bool m_IsAddressable;

        public EnGUIContent Content => m_Content;

        void OnEnable()
        {
            m_Content.OnDisableContent += DestroyPanel;
        }

        void OnDisable()
        {
            m_Content.OnDisableContent -= DestroyPanel;
        }

        public virtual void OpenPanel()
        {
            OpenPanel(EventSystem.current);
        }

        public void ClosePanel(bool immediately = false)
        {
            EnGUIManager.Instance.CloseAllUntilFind(m_Content, immediately);
        }

        protected virtual void OpenPanel(EventSystem system)
        {
            EnGUIManager.Instance.PushContent(m_Content, m_PreviousBehaviour, system, m_SelfBehaviour);
            OnPanelAwake?.Invoke();
        }

        protected virtual void DestroyPanel()
        {
            OnPanelDestroy?.Invoke();

            if (m_IsAddressable)
                Addressables.ReleaseInstance(gameObject);
            else
                Destroy(gameObject);
        }

        public static async Task<EnGUIPanel> CreatePanel(AssetReferenceGameObject reference, Transform parent)
        {
            var operation = Addressables.InstantiateAsync(reference, parent);

            await operation.Task;

            var panel = operation.Result.GetComponent<EnGUIPanel>();

            panel.m_IsAddressable = true;
            panel.OpenPanel();

            return panel;
        }

        public static async Task<EnGUIPanel> CreatePanel(AssetReferenceGameObject reference, Transform parent, EventSystem eventSystem)
        {
            var operation = Addressables.InstantiateAsync(reference, parent);

            await operation.Task;

            var panel = operation.Result.GetComponent<EnGUIPanel>();

            panel.m_IsAddressable = true;
            panel.OpenPanel(eventSystem);

            return panel;
        }

        public static EnGUIPanel CreatePanel(GameObject prefab, Transform parent)
        {
            var panel = Instantiate(prefab, parent).GetComponent<EnGUIPanel>();

            panel.m_IsAddressable = false;
            panel.OpenPanel();

            return panel;
        }

        public static EnGUIPanel CreatePanel(GameObject prefab, Transform parent, EventSystem eventSystem)
        {
            var panel = Instantiate(prefab, parent).GetComponent<EnGUIPanel>();

            panel.m_IsAddressable = false;
            panel.OpenPanel(eventSystem);

            return panel;
        }
    }
}