using System.Collections;
using TMPro;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
    public class XboxGamerTag : MonoBehaviour
    {
#if UNITY_GAMECORE || UNITY_EDITOR

        [SerializeField]
        private TextMeshProUGUI m_GamerTag;

#if UNITY_GAMECORE
        private void Start()
        {
            StartCoroutine(GetTag());
        }

        private IEnumerator GetTag()
        {
            yield return new WaitWhile(() => GameCoreManager.Instance.XboxUser.XboxUserHandle == null);

            m_GamerTag.text = GameCoreUserController.GetGamerTag(GameCoreManager.Instance.XboxUser.XboxUserHandle);
        }
#endif
#endif
    }
}
