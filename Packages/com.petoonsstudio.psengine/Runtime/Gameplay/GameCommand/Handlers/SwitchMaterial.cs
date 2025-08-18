using System;
using UnityEngine;


namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SwitchMaterial : GameCommandReceiver
    {
        [SerializeField] private Renderer m_Target;
        [SerializeField] private Material[] m_Materials;
        
        private int count;

        public override void Execute()
        {
            count++;
            m_Target.material = m_Materials[count % m_Materials.Length];

            EndAction();
        }
    }
}
