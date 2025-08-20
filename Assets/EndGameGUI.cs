using EnGUI;
using Game.Framework;
using RTLTMPro;
using UnityEngine;

public class EndGameGUI : EnGUIContent
{
    [SerializeField]
    private RTLTextMeshPro m_ResultText;

    private SceneGroup m_SceneGroupToLoad;

    public void Retry(bool win, SceneGroup sceneGroup)
    {
        m_ResultText.text = win ? "You Win!" : "You Lose!";
        m_SceneGroupToLoad = sceneGroup;
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
