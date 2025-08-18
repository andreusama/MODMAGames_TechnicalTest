using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XClosedCaptionProperties
    {
        internal XClosedCaptionProperties(Interop.XClosedCaptionProperties interopStruct)
        {
            this.BackgroundColor = new XColor(interopStruct.BackgroundColor);
            this.FontColor = new XColor(interopStruct.FontColor);
            this.WindowColor = new XColor(interopStruct.WindowColor);
            this.FontEdgeAttribute = interopStruct.FontEdgeAttribute;
            this.FontStyle = interopStruct.FontStyle;
            this.FontScale = interopStruct.FontScale;
            this.Enabled = interopStruct.Enabled.Value;
        }
    
        public XColor BackgroundColor { get; }
        public XColor FontColor { get; }
        public XColor WindowColor { get; }
        public XClosedCaptionFontEdgeAttribute FontEdgeAttribute { get; }
        public XClosedCaptionFontStyle FontStyle { get; }
        public float FontScale { get; }
        public bool Enabled { get; }
    }
}
