using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class ImageExtensions
    {
        public static void SetAlpha(this Image image, float value)
        {
            var color = image.color;
            color.a = value;
            image.color = color;
        }
    }
}
