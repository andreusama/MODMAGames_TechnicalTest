using EnGUI;
using Game.Framework;
using RTLTMPro;
using UnityEngine;

public class EndGameGUI : EnGUIContent
{
    [SerializeField]
    private RTLTextMeshPro m_ResultText;

    public SceneGroup m_SceneGroupToLoad;

    public void Initialize(bool win)
    {
        m_ResultText.text = win ? "You Win!" : "You Lose!";
    }

    public void OnRetryButtonClicked()
    {
        if (m_SceneGroupToLoad != null)
        {
            SceneLoaderManager.Instance.LoadSceneGroup(m_SceneGroupToLoad);
        }
        else
        {
            Debug.LogWarning("No scene group assigned for retry.");
        }
    }
}
