using System;
using UnityEngine;


namespace PetoonsStudio.PSEngine.Gameplay
{
    public class ParticleSystemEmit : GameCommandReceiver
    {
        [SerializeField] private ParticleSystem[] m_ParticleSystems;
        [SerializeField] private int m_Count;

        public override void Execute()
        {
            foreach (var ps in m_ParticleSystems)
            {
                ps.Emit(m_Count);
            }

            EndAction();
        }
    }
}
