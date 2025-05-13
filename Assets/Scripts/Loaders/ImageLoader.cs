using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Loaders
{
    public static class ImageLoader
    {
        public static Texture2D LoadTexture(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Image file not found: {filePath}");
                return null;
            }

            var imageData = File.ReadAllBytes(filePath);
            var texture = new Texture2D(2, 2); // サイズは適当、後でロード時に上書きされる
            if (texture.LoadImage(imageData)) // 読み込み成功したら
            {
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