using UnityEngine;
using UnityEngine.UI;

namespace UI.Utils
{
    public class SpriteUtil : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        /// <summary>
        /// Creates a 1x1 black sprite using a dynamically generated texture.
        /// This is useful for placeholder or solid-color UI backgrounds without relying on external image assets.
        /// </summary>
        /// <returns>A Sprite filled with solid black color.</returns>
        private static Sprite CreateBlackSprite()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.black);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }

        private void Awake()
        {
            if (image.sprite == null)
            {
                image.sprite = CreateBlackSprite();
            }
        }
    }
}