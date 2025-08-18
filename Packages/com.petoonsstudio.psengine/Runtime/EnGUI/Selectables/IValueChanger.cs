using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public interface IValueChanger
    {
        public UnityEvent<string> OnValueChange { get; set; }
    }
}

