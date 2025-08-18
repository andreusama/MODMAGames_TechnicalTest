using System;
using UnityEngine;


namespace PetoonsStudio.PSEngine.Gameplay
{
    public class ToggleGameObjectActive : GameCommandReceiver
    {
        [SerializeField] private GameObject[] m_Targets;

        public override void Execute()
        {
            foreach (var g in m_Targets)
                g.SetActive(!g.activeSelf);

            EndAction();
        }
    }
}
