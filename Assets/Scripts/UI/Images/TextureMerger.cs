using UnityEngine;

namespace UI.Images
{
    public class TextureMerger : MonoBehaviour
    {
        public Texture2D Merge(Texture2D baseTex, Texture2D overlay1, Texture2D overlay2)
        {
            var rt = new RenderTexture(baseTex.width, baseTex.height, 0);
            RenderTexture.active = rt;

            GL.Clear(true, true, Color.clear);

            var mat = new Material(Shader.Find("Sprites/Default"));

            // 1. ベース
            Graphics.Blit(baseTex, rt);

            // 2. overlay1
            if (overlay1 != null)
            {
                Graphics.Blit(overlay1, rt, mat);
            }

            // 3. overlay2
            if (overlay2 != null)
            {
                Graphics.Blit(overlay2, rt, mat);
            }

            var result = new Texture2D(baseTex.width, baseTex.height, TextureFormat.RGBA32, false);
            result.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            result.Apply();

            RenderTexture.active = null;
            rt.Release();

            if (Application.isPlaying)
            {
                Destroy(mat);
            }
            else
            {
                DestroyImmediate(mat);
            }

            return result;
        }
    }
}