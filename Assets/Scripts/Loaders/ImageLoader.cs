using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Loaders
{
    public static class ImageLoader
    {
        private readonly static Dictionary<string, Texture2D> TextureCache = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Loads an image file and returns it as a Texture2D.
        /// Uses cache if available. You can choose to keep or remove the alpha (transparency) channel.
        /// </summary>
        /// <param name="filePath">The path to the image file.</param>
        /// <param name="keepAlpha">
        /// If true, keeps the alpha channel. If false, removes it to save memory.
        /// </param>
        /// <returns>The loaded Texture2D, or null if loading fails.</returns>
        public static async Task<Texture2D> LoadTexture(string filePath, bool keepAlpha = true)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Debug.LogError("File path is null or empty.");
                return null;
            }

            // キャッシュがあれば再利用
            if (TextureCache.TryGetValue(filePath, out var cachedTexture))
            {
                return cachedTexture;
            }

            if (!File.Exists(filePath))
            {
                Debug.LogError($"Image file not found: {filePath}");
                return null;
            }

            var imageData = await File.ReadAllBytesAsync(filePath);

            var texture = keepAlpha
                ? new Texture2D(2, 2, TextureFormat.RGBA32, false)
                : new Texture2D(2, 2, TextureFormat.RGB24, true);

            if (texture.LoadImage(imageData))
            {
                texture.filterMode = FilterMode.Trilinear;
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.anisoLevel = 1;

                // キャッシュに登録
                TextureCache[filePath] = texture;

                return texture;
            }

            Debug.LogError("Failed to load image");
            return null;
        }

        /// <summary>
        /// Returns a new list of paths reordered by proximity to the given index.
        /// Starts with the item at the initial index, then alternates between items
        /// to the right and left of it.
        /// </summary>
        /// <param name="paths">The original list of paths.</param>
        /// <param name="initialIndex">The index to start from.</param>
        /// <returns>A new list reordered by proximity to the initial index.</returns>
        public static List<string> ReorderByProximity(List<string> paths, int initialIndex)
        {
            if (paths is not { Count: > 1, } || initialIndex >= paths.Count)
            {
                return paths;
            }

            var results = new List<string> { paths[initialIndex], };

            for (var i = 1; i < paths.Count; i++)
            {
                var right = i + initialIndex;
                if (paths.Count > right)
                {
                    results.Add(paths[right]);
                }

                var left = initialIndex - i;
                if (left >= 0)
                {
                    results.Add(paths[left]);
                }
            }

            return results;
        }

        public static void ClearCache()
        {
            TextureCache.Clear();
        }
    }
}