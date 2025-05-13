using UnityEngine;

namespace Core
{
    public interface IDisplayImage
    {
        public void SetAlpha(float alpha);

        public void SetPosition(Vector2 anchoredPosition);

        public void SetScale(float scale);
    }
}