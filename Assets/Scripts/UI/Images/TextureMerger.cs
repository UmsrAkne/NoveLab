using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Images
{
    public class TextureMerger : MonoBehaviour
    {

        private static Material blitMaterial;
        private readonly Dictionary<TextureKey, Texture2D> textureCache = new ();

        public Texture2D Merge(Texture2D baseTex, Texture2D overlay1, Texture2D overlay2)
        {
            var key = new TextureKey(baseTex, overlay1, overlay2);
            if (textureCache.TryGetValue(key, out var cached))
            {
                Debug.Log($"TextureCache hit: {key}");
                return cached;
            }

            var rt = RenderTexture.GetTemporary(baseTex.width, baseTex.height, 0);
            RenderTexture.active = rt;

            GL.Clear(true, true, Color.clear);

            if (blitMaterial == null)
            {
                blitMaterial = new Material(Shader.Find("Sprites/Default"));
            }

            var mat = blitMaterial;

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

            textureCache.TryAdd(key, result);

            return result;
        }

        private struct TextureKey : IEquatable<TextureKey>
        {
            private readonly Texture2D baseTexture;
            private readonly Texture2D overlay1;
            private readonly Texture2D overlay2;

            public TextureKey(Texture2D b, Texture2D o1, Texture2D o2)
            {
                baseTexture = b;
                overlay1 = o1;
                overlay2 = o2;
            }

            public bool Equals(TextureKey other)
            {
                return baseTexture == other.baseTexture
                       && overlay1 == other.overlay1
                       && overlay2 == other.overlay2;
            }

            public override bool Equals(object obj)
            {
                return obj is TextureKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(baseTexture, overlay1, overlay2);
            }
        }
    }
}