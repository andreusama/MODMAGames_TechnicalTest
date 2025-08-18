using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public interface IMessage : IComparable<IMessage>
    {
        public enum InterruptionType { None, Current, All }
        bool WaitIfCutsceneRunning { get; }
        InterruptionType Interruption { get; }
        int Priority { get; }
        float Duration { get; }
    }
}