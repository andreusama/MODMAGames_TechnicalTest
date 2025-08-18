using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblPreferredColor
    {
        internal XblPreferredColor(Interop.XblPreferredColor interopPreferredColor)
        {
            this.PrimaryColor = Converters.ByteArrayToString(interopPreferredColor.primaryColor);
            this.SecondaryColor = Converters.ByteArrayToString(interopPreferredColor.secondaryColor);
            this.TertiaryColor = Converters.ByteArrayToString(interopPreferredColor.tertiaryColor);
        }

        public string PrimaryColor { get; }
        public string SecondaryColor { get; }
        public string TertiaryColor { get; }
    }
}
