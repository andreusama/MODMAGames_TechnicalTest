using System;
using UnityEngine;


namespace PetoonsStudio.PSEngine.Gameplay
{
    public class PlayAnimation : GameCommandReceiver
    {
        [SerializeField] private Animation[] m_Animations;

        public override void Execute()
        {
            foreach (var a in m_Animations)
                a.Play();

            EndAction();
        }
    }
}
