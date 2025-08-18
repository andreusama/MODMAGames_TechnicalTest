using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class SendMessage : GameCommandReceiver
    {
        [SerializeField] private string m_Message;
        [SerializeField] private GameObject m_Receiver;

        public override void Execute()
        {
            m_Receiver.SendMessage(m_Message);

            EndAction();
        }
    }
}