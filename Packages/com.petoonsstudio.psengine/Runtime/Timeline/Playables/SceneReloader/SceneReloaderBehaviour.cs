using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class SceneReloaderBehaviour : PlayableBehaviour
    {
        public void ReloadScene(GameObject sceneGameObject)
        {
            SceneManager.LoadSceneAsync(sceneGameObject.scene.buildIndex);
        }
    }

}