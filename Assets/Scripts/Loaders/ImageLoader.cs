using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Loaders
{
    public static class ImageLoader
    {
        /// <summary>
        /// Loads an image file and returns it as a Texture2D.
        /// You can choose to keep or remove the alpha (transparency) channel.
        /// </summary>
        /// <param name="filePath">The path to the image file.</param>
        /// <param name="keepAlpha">
        /// If true, keeps the alpha channel. If false, removes it to save memory.
        /// </param>
        /// <returns>The loaded Texture2D, or null if loading fails.</returns>
        public static async Task<Texture2D> LoadTexture(string filePath, bool keepAlpha = true)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Image file not found: {filePath}");
                return null;
            }

            var imageData = await File.ReadAllBytesAsync(filePath);

            // サイズは適当、後でロード時に上書きされる
            var texture = keepAlpha
                ? new Texture2D(2, 2)
                : new Texture2D(2, 2, TextureFormat.RGB24, true);

            if (texture.LoadImage(imageData)) // 読み込み成功したら
            {
                // 高品質で設定
                texture.filterMode = FilterMode.Trilinear;           // 滑らか補間（or Trilinear）
                texture.wrapMode = TextureWrapMode.Clamp;           // UIなら繰り返し不要
                texture.anisoLevel = 1;                             // UI用途なら基本1でOK（斜め表示多いなら2以上）
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
    }
}