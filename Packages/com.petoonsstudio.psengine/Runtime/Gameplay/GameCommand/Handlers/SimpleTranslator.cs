using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SimpleTranslator : SimpleTransformer
    {
        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private Vector3 m_Start = -Vector3.forward;
        [SerializeField] private Vector3 m_End = Vector3.forward;

        public override void PerformTransform(float position)
        {
            var curvePosition = m_AccelCurve.Evaluate(position);
            var pos = transform.TransformPoint(Vector3.Lerp(m_Start, m_End, curvePosition));
            Vector3 deltaPosition = pos - m_Rigidbody.position;
            if (Application.isEditor && !Application.isPlaying)
                m_Rigidbody.transform.position = pos;
            m_Rigidbody.MovePosition(pos);
        }
    }
}
