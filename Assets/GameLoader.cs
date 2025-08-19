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
        // Lanza la carga de la escena (no aditiva, con pantalla de carga y liberando memoria)
        SceneLoaderManager.Instance.LoadSceneGroup(SampleSceneGroup, additive: false, loadingScene: true, releaseMemory: true);

        yield break;
    }
}
