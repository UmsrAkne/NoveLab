using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Adapters
{
    public class UnityImageAdapter : MonoBehaviour, IDisplayImage
    {
        [SerializeField]
        private Image image;

        public void SetTexture(Texture2D texture)
        {
            if (image == null)
            {
                Debug.LogWarning("Image is not assigned!");
                return;
            }

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
            image.SetNativeSize();
        }

        public void SetAlpha(float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }

        public void SetPosition(Vector2 anchoredPosition)
        {
            image.rectTransform.anchoredPosition = anchoredPosition;
        }

        public void SetScale(float scale)
        {
            image.rectTransform.localScale = new Vector3(scale, scale, 1);
        }
    }
}