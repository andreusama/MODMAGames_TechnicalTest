using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Credits
{
    [Serializable]
    public class CreditsSectionUI : MonoBehaviour
    {
        public TextMeshProUGUI Title;
        public List<CreditsMentionUI> Mentions = new List<CreditsMentionUI>();
    }
}
