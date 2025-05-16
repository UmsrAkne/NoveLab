using UnityEngine;

namespace Core
{
    public interface IDisplayImage
    {
        public void SetAlpha(float alpha);

        public void SetPosition(Vector2 anchoredPosition);

        public void SetScale(float scale);

        public void SetTexture(Texture2D texture);

        public GameObject GameObject { get; }

        public void FadeIn(float duration);

        public void Slide(float duration, float angle, float distance);
    }
}