using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{

    public class SimpleRotator : SimpleTransformer
    {
        [SerializeField] private Vector3 m_Axis = Vector3.forward;
        [SerializeField] private float m_Start = 0;
        [SerializeField] private float m_End = 90;


        public override void PerformTransform(float position)
        {
            var curvePosition = m_AccelCurve.Evaluate(position);
            var q = Quaternion.AngleAxis(Mathf.Lerp(m_Start, m_End, curvePosition), m_Axis);
            transform.localRotation = q;
        }
    }
}
