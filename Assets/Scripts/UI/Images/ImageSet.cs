using System.Collections.Generic;
using Core;
using UI.Adapters;
using UnityEngine;

namespace UI.Images
{
    public class ImageSet : MonoBehaviour, IDisplayImage
    {
        [SerializeField]
        private CanvasGroup canvasGroup;
        private readonly Dictionary<string, IUIAnimation> animations = new();

        [SerializeField]
        private SpriteRendererAdapter baseImage;

        [SerializeField]
        private SpriteRendererAdapter eye;

        [SerializeField]
        private SpriteRendererAdapter mouth;

        public GameObject GameObject => gameObject;

        public void SetExpression(Texture2D eyeTex, Texture2D mouthTex)
        {
            eye.SetTexture(eyeTex);
            mouth.SetTexture(mouthTex);
        }

        public void SetTextures(Texture2D baseTexture, Texture2D eyeTex, Texture2D mouthTex)
        {
            baseImage.SetTexture(baseTexture);
            baseImage.SetLayerIndex(0);
            eye.SetTexture(eyeTex);
            eye.SetLayerIndex(1);
            mouth.SetTexture(mouthTex);
            mouth.SetLayerIndex(2);
        }

        public void SetAlpha(float alpha)
        {
            baseImage.SetAlpha(alpha);
            eye.SetAlpha(alpha);
            mouth.SetAlpha(alpha);
        }

        public void SetScale(float scale)
        {
            var rect = GameObject.transform as RectTransform;
            if (rect != null)
            {
                rect.localScale = new Vector3(scale, scale, 1f);
            }
        }

        public void SetTexture(Texture2D baseTexture)
        {
            baseImage.SetTexture(baseTexture);
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
                if (!animations.ContainsKey(key))
                {
                    return;
                }

                animations.Remove(key);
                Debug.Log($"Animation '{key}' completed and removed from registry.");
            };
        }

        public void PlayAnimations()
        {
            foreach (var keyValuePair in animations)
            {
                keyValuePair.Value.Start();
            }
        }

        public void SetBasePosition(Vector2 pos)
        {
        }

        public void SetOffsetPosition(Vector2 offset)
        {
        }
    }
}