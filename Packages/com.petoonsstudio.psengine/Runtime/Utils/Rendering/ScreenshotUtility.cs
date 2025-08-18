using System.IO;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class ScreenshotUtility
    {
        public static RenderTexture CreateScreenshotTexture(int width, int height)
        {
            return new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        }

        public static void CaptureToTexture(Camera source, RenderTexture targetTexture, LayerMask renderMask)
        {
            if (source == null)
                source = Camera.main;

            LayerMask originalRenderMask = source.cullingMask;

            PrepareCamera(ref source, renderMask);
            RenderToTexture(ref source, targetTexture);
            CleanUp(ref source, originalRenderMask);
        }

        public static void SaveRenderTextureToDisk(RenderTexture texture, string path, string filename)
        {
            RenderTexture.active = texture;
            Texture2D tex2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            tex2D.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            RenderTexture.active = null;

            byte[] bytes = tex2D.EncodeToPNG();

#if UNITY_EDITOR || UNITY_STANDALONE
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.WriteAllBytes(path + filename + ".png", bytes);
#endif
            //TODO Localize this per platform  or move it

            Object.Destroy(tex2D);
        }

        public static Texture2D LoadTextureFromDisk(string path)
        {
            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(path))
            {
                FileData = File.ReadAllBytes(path);
                Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }

        public static Sprite LoadSpriteFromDisk(string path, float pixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
        {
            Texture2D texture = LoadTextureFromDisk(path);
            if (texture == null) return null;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), pixelsPerUnit, 0, spriteType);
        }

        private static void PrepareCamera(ref Camera camera, LayerMask renderMask)
        {
            camera.cullingMask = renderMask;
        }

        private static void RenderToTexture(ref Camera camera, RenderTexture renderTarget)
        {
            camera.targetTexture = renderTarget;
            camera.Render();
            camera.targetTexture = null;
        }

        private static void CleanUp(ref Camera camera, LayerMask originalRenderMask)
        {
            camera.cullingMask = originalRenderMask;
        }
    }
}
