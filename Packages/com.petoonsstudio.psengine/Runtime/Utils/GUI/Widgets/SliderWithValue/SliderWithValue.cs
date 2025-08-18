using TMPro;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.EnGUI
{
    public class SliderWithValue : Slider
    {
        public TextMeshProUGUI ValueText;
        public float Multiplier = 10f;
        public string Format = "0";

        protected override void Start()
        {
            base.Start();
            onValueChanged.AddListener(ChangeValueText);
        }
        public override void SetValueWithoutNotify(float input)
        {
            base.SetValueWithoutNotify(input);
            input *= Multiplier;
            ValueText.text = input.ToString(Format);
        }
        private void ChangeValueText(float value)
        {
            value *= Multiplier;
            ValueText.text = value.ToString(Format);
        }
    }
}
