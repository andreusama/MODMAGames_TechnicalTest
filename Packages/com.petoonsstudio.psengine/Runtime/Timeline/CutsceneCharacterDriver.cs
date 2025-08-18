using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [RequireComponent(typeof(Cutscene))]
    public class CutsceneCharacterDriver : MonoBehaviour
    {
        [SerializeField, Self] private Cutscene m_Cutscene;

        private List<GameObject> m_DrivenObjects;

        void Awake()
        {
            m_DrivenObjects = new List<GameObject>();
        }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        void OnEnable()
        {
            m_Cutscene.OnCutsceneStart.AddListener(CutsceneState);
            m_Cutscene.OnCutsceneStop.AddListener(GameplayState);
        }

        void OnDisable()
        {
            m_Cutscene.OnCutsceneStart.RemoveListener(CutsceneState);
            m_Cutscene.OnCutsceneStop.RemoveListener(GameplayState);
        }

        private void CutsceneState()
        {
            TimelineAsset timelineAsset = (TimelineAsset)m_Cutscene.Director.playableAsset;
            var trackAssets = timelineAsset.outputs;

            foreach (var binding in trackAssets)
            {
                var obj = m_Cutscene.Director.GetGenericBinding(binding.sourceObject) as GameObject;
                if (obj != null && !m_DrivenObjects.Contains(obj))
                {
                    m_DrivenObjects.Add(obj);
                    var responders = obj.GetComponentsInChildren<ICutsceneResponder>();
                    foreach (var responder in responders)
                        responder.SetCutsceneState();
                }
            }
        }

        private void GameplayState()
        {
            foreach (var driven in m_DrivenObjects)
            {
                var responders = driven.GetComponentsInChildren<ICutsceneResponder>();
                foreach (var responder in responders)
                    responder.SetGameplayState();
            }
        }
    }
}