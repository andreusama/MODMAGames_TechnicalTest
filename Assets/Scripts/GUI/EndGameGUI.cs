using EnGUI;
using Game.Framework;
using RTLTMPro;
using UnityEngine;

public class EndGameGUI : EnGUIContent
{
    [SerializeField]
    private RTLTextMeshPro m_ResultText;

    public SceneGroup SceneGroupToLoad;

    public void Initialize(bool win)
    {
        m_ResultText.text = win ? "You Win!" : "You Lose!";
    }

    public void OnRetryButtonClicked()
    {
        if (SceneGroupToLoad != null)
        {
            SceneLoaderManager.Instance.LoadSceneGroup(SceneGroupToLoad);
        }
        else
        {
            Debug.LogWarning("No scene group assigned for retry.");
        }
    }
}
