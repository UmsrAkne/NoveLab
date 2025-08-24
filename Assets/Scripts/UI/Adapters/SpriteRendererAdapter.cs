using System.Collections.Generic;
using Core;
using UnityEngine;

namespace UI.Adapters
{
    public class SpriteRendererAdapter : MonoBehaviour, IDisplayImage
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private SpriteMask spriteMask;

        private Texture2D texture2D;

        private Vector2 basePos;
        private Vector2 shakeOffset;

        private readonly Dictionary<string, IUIAnimation> animations = new();

        public void SetTexture(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }

            if (spriteRenderer == null)
            {
                Debug.LogWarning("SpriteRenderer is not assigned!");
                return;
            }

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1);
            spriteRenderer.sprite = sprite;
            texture2D = texture;
            if (spriteMask != null)
            {
                spriteMask.sprite = sprite;
            }
        }

        public void SetLayerIndex(int index)
        {
            spriteRenderer.sortingOrder = index;
        }

        public Texture2D GetTexture() => texture2D;

        public GameObject GameObject => gameObject;

        public int LayerIndex => spriteRenderer.sortingOrder;

        public void SetAlpha(float alpha)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        public void SetScale(float scale)
        {
            transform.localScale = new Vector3(scale, scale, 1f); // Z=1で十分
        }

        public void RegisterAnimation(string key, IUIAnimation anime)
        {
            if (animations.ContainsKey(key))
            {
                Debug.LogWarning($"Animation with key '{key}' already registered. Overwriting.");
            }

            animations[key] = anime;
            anime.OnCompleted += () =>
            {
                if (animations.ContainsKey(key))
                {
                    animations.Remove(key);
                    Debug.Log($"Animation '{key}' completed and removed from registry.");
                }
            };
        }

        public void PlayAnimations()
        {
            foreach (var keyValuePair in animations)
            {
                keyValuePair.Value.Start();
            }
        }

        public int SortingOrder
        {
            get => spriteRenderer.sortingOrder;
            set => spriteRenderer.sortingOrder = value;
        }

        public void SetBasePosition(Vector2 pos)
        {
            basePos = pos;
            UpdateFinalPosition();
        }

        public void SetOffsetPosition(Vector2 offset)
        {
            shakeOffset = offset;
            UpdateFinalPosition();
        }

        private void UpdateFinalPosition()
        {
            transform.localPosition = basePos + shakeOffset;
        }
    }
}