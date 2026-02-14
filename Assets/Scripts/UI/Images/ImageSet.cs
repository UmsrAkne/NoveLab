using System;
using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UI.Adapters;
using UI.Animations;
using UnityEngine;

namespace UI.Images
{
    public class ImageSet : MonoBehaviour, IDisplayImage, ICrossFadeCapable
    {
        private readonly Dictionary<string, IUIAnimation> animations = new();
        private bool isEyeAActive = true;
        private bool isMouthAActive = true;
        private int sortingBase;
        private CancellationTokenSource fadeCts;
        private Vector2 basePos;
        private Vector2 shakeOffset;
        private bool isBaseAActive = true;

        [SerializeField]
        private SpriteRendererAdapter baseImageA;

        [SerializeField]
        private SpriteRendererAdapter baseImageB;

        public GameObject GameObject => gameObject;

        public async UniTask CrossFadeExpression(Texture2D newTexture, float duration = 0.5f)
        {
            // 前回フェードキャンセル
            if (fadeCts is { IsCancellationRequested: false, })
            {
                fadeCts.Cancel();
                fadeCts.Dispose();
            }

            fadeCts = new CancellationTokenSource();
            var token = fadeCts.Token;

            var (oldBase, newBase) = isBaseAActive ? (baseImageA, baseImageB) : (baseImageB, baseImageA);

            // 強制完了
            oldBase.SetAlpha(1f);

            // 新画像セット
            newBase.SetTexture(newTexture);
            newBase.SetAlpha(0f);
            newBase.SetLayerIndex(oldBase.LayerIndex + 1);

            ApplySortingOrder();

            var fade = new FadeIn(newBase) { Duration = duration, };

            var tcs = new UniTaskCompletionSource();
            fade.OnCompleted += () => tcs.TrySetResult();

            fade.Start();

            try
            {
                await using (token.Register(fade.Stop))
                {
                    await tcs.Task;
                }
            }
            catch (OperationCanceledException)
            {
                fade.Stop();
                return;
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            // 完了後
            oldBase.SetAlpha(0f);
            isBaseAActive = !isBaseAActive;

            ApplySortingOrder();
        }

        public void SetAlpha(float alpha)
        {
            baseImageA.SetAlpha(alpha);
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
            baseImageA.SetTexture(baseTexture);
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

        public int SortingOrder
        {
            get => sortingBase;
            set
            {
                if (sortingBase == value)
                {
                    return;
                }

                sortingBase = value;
                ApplySortingOrder();
            }
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

        private void ApplySortingOrder()
        {
            SetAdapterOrder(baseImageA);
        }

        private void SetAdapterOrder(SpriteRendererAdapter adapter)
        {
            if (adapter == null)
            {
                return;
            }

            adapter.SortingOrder = sortingBase + adapter.LayerIndex;
        }
    }
}