using UnityEngine;
using Game.Framework;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    [Header("Scene Group to Load")]
    public SceneGroup SampleSceneGroup;

    private void Start()
    {
        StartCoroutine(LoadSampleSceneGroup());
    }

    private IEnumerator LoadSampleSceneGroup()
    {
        // Start scene loading (non-additive, with loading screen and memory release)
        SceneLoaderManager.Instance.LoadSceneGroup(SampleSceneGroup, additive: false, loadingScene: true, releaseMemory: true);

        yield break;
    }
}
