using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class BlendShapeController : MonoBehaviour
    {
        public float MinValue = 0f;
        public float MaxValue = 0.1f;

        public List<Transform> InputTransforms;

        public SkinnedMeshRenderer Renderer { get; private set; }

        private void OnEnable()
        {
            FetchRenderer();
        }

        private void Update()
        {
            for (int i = 0; i < InputTransforms.Count; i++)
            {
                if (InputTransforms[i]) ApplyWeight(i);
            }
        }

        private void ApplyWeight(int index)
        {
            Renderer.SetBlendShapeWeight(index, GetJointInput(index));
        }

        public float GetJointInput(int index)
        {
            return MMMaths.Remap(InputTransforms[index].localPosition.y, MinValue, MaxValue, 0f, 100f);
        }

        public void FetchRenderer()
        {
            Renderer = GetComponent<SkinnedMeshRenderer>();
        }
    }
}