using DG.Tweening;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils 
{ 
    public static class TransformExtensions
    {
        /// <summary>
        /// Destroy all the children from transform
        /// </summary>
        /// <param name="transform"></param>
        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static void SetPositionAndRotation(this Transform obj, Transform newPosition)
        {
            obj.SetPositionAndRotation(newPosition.position, newPosition.rotation);
        }

        public static Transform GetLastChild(this Transform target)
        {
            return target.GetChild(target.transform.childCount - 1);
        }

        /// <summary>
        /// Destroy all the children from RectTransform
        /// </summary>
        /// <param name="transform"></param>
        public static void DestroyChildren(this RectTransform transform)
        {
            foreach (RectTransform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static void Copy(this Transform owner, Transform reference)
        {
            owner.position = reference.position;
            owner.rotation = reference.rotation;
            owner.localScale = reference.localScale;
        }

        public static void Copy(this RectTransform owner, RectTransform reference)
        {
            owner.position = reference.position;
            owner.rotation = reference.rotation;
            owner.localScale = reference.localScale;
            owner.sizeDelta = reference.sizeDelta;
        }

        public static Sequence DOTransform(this Transform owner, Transform destination, float duration, Ease ease)
        {
            var sequence = DOTween.Sequence();

            sequence.Join(owner.DOMove(destination.position, duration).SetEase(ease));
            sequence.Join(owner.DORotate(destination.rotation.eulerAngles, duration).SetEase(ease));
            sequence.Join(owner.DOScale(destination.localScale, duration).SetEase(ease));

            return sequence;
        }

        public static Sequence DOTransform(this RectTransform owner, RectTransform destination, float duration, Ease ease)
        {
            var sequence = DOTween.Sequence();

            sequence.Join(owner.DOMove(destination.position, duration).SetEase(ease));
            sequence.Join(owner.DORotate(destination.rotation.eulerAngles, duration).SetEase(ease));
            sequence.Join(owner.DOScale(destination.localScale, duration).SetEase(ease));
            sequence.Join(owner.DOSizeDelta(destination.sizeDelta, duration).SetEase(ease));

            return sequence;
        }
    }
}