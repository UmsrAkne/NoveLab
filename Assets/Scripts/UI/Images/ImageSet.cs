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
    public class ImageSet : MonoBehaviour, IDisplayImage
    {
        private readonly Dictionary<string, IUIAnimation> animations = new();
        private bool isEyeAActive = true;
        private bool isMouthAActive = true;
        private int sortingBase;
        private CancellationTokenSource fadeCts;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private SpriteRendererAdapter eyeA;

        [SerializeField]
        private SpriteRendererAdapter eyeB;

        [SerializeField]
        private SpriteRendererAdapter mouthA;

        [SerializeField]
        private SpriteRendererAdapter mouthB;

        [SerializeField]
        private SpriteRendererAdapter baseImage;

        public GameObject GameObject => gameObject;

        public async UniTask CrossFadeExpression(Texture2D newEye, Texture2D newMouth, float duration = 0.5f)
        {
            // 前回のフェードを強制完了
            if (fadeCts is { IsCancellationRequested: false, })
            {
                fadeCts.Cancel(); // トークンでキャンセル
                fadeCts.Dispose();
            }

            fadeCts = new CancellationTokenSource();
            var token = fadeCts.Token;

            var (eyeOld, eyeNew) = isEyeAActive ? (eyeA, eyeB) : (eyeB, eyeA);
            var (mouthOld, mouthNew) = isMouthAActive ? (mouthA, mouthB) : (mouthB, mouthA);

            // 強制完了（途中までだった旧スロットを強制表示
            eyeOld.SetAlpha(1f);
            mouthOld.SetAlpha(1f);

            // 新しいテクスチャ＋透明
            eyeNew.SetTexture(newEye);
            eyeNew.SetAlpha(0f);
            mouthNew.SetTexture(newMouth);
            mouthNew.SetAlpha(0f);

            // 表示順調整（必要なら）
            eyeNew.SetLayerIndex(eyeOld.LayerIndex + 1);
            mouthNew.SetLayerIndex(mouthOld.LayerIndex + 1);

            ApplySortingOrder();

            // 新フェード開始（キャンセル可能なように）
            var eyeFade = new FadeIn(eyeNew) { Duration = duration, };
            var mouthFade = new FadeIn(mouthNew) { Duration = duration, };

            async UniTask SafeFade(FadeIn fade)
            {
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
                    fade.Stop(); // 念のため停止
                }
            }

            await UniTask.WhenAll(SafeFade(eyeFade), SafeFade(mouthFade));

            // 強制完了した可能性もあるので再確認
            if (token.IsCancellationRequested)
            {
                return;
            }

            // 完了後
            eyeOld.SetAlpha(0f);
            mouthOld.SetAlpha(0f);
            isEyeAActive = !isEyeAActive;
            isMouthAActive = !isMouthAActive;
            ApplySortingOrder();
        }

        public void SetTextures(Texture2D baseTexture, Texture2D eyeTex, Texture2D mouthTex)
        {
            if (baseTexture != null)
            {
                baseImage.SetTexture(baseTexture);
                baseImage.SetLayerIndex(0);
            }

            if (eyeTex != null)
            {
                eyeA.SetTexture(eyeTex);
                eyeA.SetLayerIndex(1);
            }

            if (mouthTex != null)
            {
                mouthA.SetTexture(mouthTex);
                mouthA.SetLayerIndex(2);
            }
        }

        public void SetAlpha(float alpha)
        {
            baseImage.SetAlpha(alpha);
            eyeA.SetAlpha(alpha);
            mouthA.SetAlpha(alpha);
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
        }

        public void SetOffsetPosition(Vector2 offset)
        {
        }

        private void ApplySortingOrder()
        {
            SetAdapterOrder(baseImage);
            SetAdapterOrder(eyeA);
            SetAdapterOrder(eyeB);
            SetAdapterOrder(mouthA);
            SetAdapterOrder(mouthB);
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