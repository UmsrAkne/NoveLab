using UnityEngine;

namespace Core
{
    public interface IDisplayImage
    {
        public void SetAlpha(float alpha);

        public void SetScale(float scale);

        public void SetTexture(Texture2D texture);

        public GameObject GameObject { get; }

        public void RegisterAnimation(string key, IUIAnimation animation);

        public void PlayAnimations();

        public int SortingOrder { get; set; }

        void SetBasePosition(Vector2 pos);

        void SetOffsetPosition(Vector2 offset);
    }
}