using PetoonsStudio.PSEngine.Utils;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class SortingLayerGroupPlayableBehaviour : PlayableBehaviour
    {
        public SortingLayerGroupTrack SortingTrack { get; set; }

        [SortingLayerSelector]
        public int Layer;
        public int OrderLayer;

        private SortingGroup m_BoundGameObject;
        private int m_BoundGameObjectInitialLayer;
        private int m_BoundGameObjectInitialOrderLayer;

        private bool m_BoundGameObjectRestored;

        public override void OnPlayableCreate(Playable playable)
        {
            m_BoundGameObject = SortingTrack.GetSortingGroupBinding();

            if (m_BoundGameObject == null)
                return;

            m_BoundGameObjectInitialLayer = m_BoundGameObject.sortingLayerID;
            m_BoundGameObjectInitialOrderLayer = m_BoundGameObject.sortingOrder;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_BoundGameObject == null)
                return;

            var timelineTime = playable.GetGraph().GetRootPlayable(0).GetTime();
            if (timelineTime + info.deltaTime >= SortingTrack.end)
                RestoreBoundGameObject();
            else
            {
                m_BoundGameObject.sortingLayerID = SortingLayer.layers[Layer].id;
                m_BoundGameObject.sortingOrder = OrderLayer;
            }
        }

        public override void OnGraphStart(Playable playable) => m_BoundGameObjectRestored = false;

        private void RestoreBoundGameObject()
        {
            if (m_BoundGameObject == null)
                return;

            if (m_BoundGameObjectRestored)
                return;

            switch (SortingTrack.postPlaybackState)
            {
                case SortingLayerGroupTrack.PostPlaybackState.Revert:
                    m_BoundGameObject.sortingLayerID = m_BoundGameObjectInitialLayer;
                    m_BoundGameObject.sortingOrder = m_BoundGameObjectInitialOrderLayer;
                    break;
                case SortingLayerGroupTrack.PostPlaybackState.LeaveAsIs:
                    m_BoundGameObject.sortingLayerID = SortingLayer.layers[Layer].id;
                    m_BoundGameObject.sortingOrder = OrderLayer;
                    break;
                default:
                    break;
            }

            m_BoundGameObjectRestored = true;
        }
    }
}

