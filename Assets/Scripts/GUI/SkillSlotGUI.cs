using UnityEngine;
using UnityEngine.UI;

public class SkillSlotGUI : MonoBehaviour
{
    private ICooldownSource m_Source;

    [Header("UI")]
    [SerializeField] private Image m_CooldownFillImage; // Image hija (Filled)
    [SerializeField] private bool m_AutoFindChildImage = true;
    [SerializeField] private bool m_InvertFill; // opcional: 0->1 en vez de 1->0

    private void Awake()
    {
        if (m_CooldownFillImage == null && m_AutoFindChildImage)
        {
            var images = GetComponentsInChildren<Image>(true);
            foreach (var img in images)
            {
                if (img.gameObject != gameObject)
                {
                    m_CooldownFillImage = img;
                    break;
                }
            }
        }
        if (m_CooldownFillImage != null && m_CooldownFillImage.type != Image.Type.Filled)
            m_CooldownFillImage.type = Image.Type.Filled;

        SetFill(0f);
    }

    public void SetSource(ICooldownSource source) => m_Source = source;
    public void SetSource(MonoBehaviour sourceBehaviour)
    {
        m_Source = sourceBehaviour as ICooldownSource;
    }

    private void Update()
    {
        if (m_CooldownFillImage == null) return;

        float p = (m_Source != null) ? m_Source.CooldownProgress01 : 0f;
        SetFill(m_InvertFill ? 1f - p : p);
    }

    private void SetFill(float v01)
    {
        m_CooldownFillImage.fillAmount = Mathf.Clamp01(v01);
    }
}
