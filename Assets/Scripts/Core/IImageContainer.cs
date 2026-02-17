using UnityEngine;

namespace Core
{
    public interface IImageContainer
    {
        void AddImage(IDisplayImage image);

        IDisplayImage GetFront();

        public SpriteRenderer GetEffectRenderer();

        public void SetEffectAlpha(float alpha);
    }
}