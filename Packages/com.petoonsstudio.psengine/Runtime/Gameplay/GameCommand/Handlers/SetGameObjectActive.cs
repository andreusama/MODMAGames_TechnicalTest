using System;
using UnityEngine;


namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SetGameObjectActive : GameCommandReceiver
    {
        [Serializable]
        public struct Action
        {
            public GameObject target;
            public bool isEnabled;
        }

        [SerializeField] private Action[] m_Targets;

        public override void Execute()
        {
            foreach (var g in m_Targets)
                g.target.SetActive(g.isEnabled);

            EndAction();
        }
    }
}
