using Core;
using UI.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Adapters
{
    public class SpriteAdapter : MonoBehaviour, IDisplayImage
    {
        [SerializeField]
        private Image image;

        private Texture2D texture2D;
        private FadeIn fadeIn;
        private Slide slide;

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
            texture2D = texture;
        }

        public GameObject GameObject => gameObject;

        public Texture2D GetTexture()
        {
            return texture2D;
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
            var rt = image.rectTransform;
            var originalSize = image.sprite.rect.size;
            rt.sizeDelta = originalSize * scale;
        }

        public void FadeIn(float duration)
        {
            fadeIn.Duration = duration;
            fadeIn.Start();
        }

        public void Slide(float duration, float angle, float distance)
        {
            slide.Duration = duration;
            slide.Angle = angle;
            slide.Distance = distance;
            slide.Start();
        }

        private void Awake()
        {
            fadeIn = new FadeIn(this);
            slide = new Slide(this);
        }
    }
}