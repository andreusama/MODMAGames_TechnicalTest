using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public abstract class SendOnCollision : GameCommandSender
    {
        [SerializeField] 
        protected LayerMask m_SourceLayers;
        [SerializeField, TagSelector] 
        protected List<string> m_SourceTags = new List<string>();

        public bool LayerMatch(int layer) => 0 != (m_SourceLayers.value & 1 << layer);
        public bool TagMatch(string tag) => m_SourceTags.Count <= 0 || m_SourceTags.Contains(tag);
        public bool TagAndLayerMatch(GameObject obj) => LayerMatch(obj.layer) && TagMatch(obj.tag);
    }
}
