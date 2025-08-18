using System;
using UnityEngine;


namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SetRendererActive : GameCommandReceiver
    {
        [Serializable]
        public struct Action
        {
            public Renderer target;
            public bool isEnabled;
        }

        [SerializeField] private Action[] m_Targets;

        public override void Execute()
        {
            foreach (var g in m_Targets)
                g.target.enabled = g.isEnabled;

            EndAction();
        }
    }
}