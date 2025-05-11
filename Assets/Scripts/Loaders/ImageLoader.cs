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
    }
}