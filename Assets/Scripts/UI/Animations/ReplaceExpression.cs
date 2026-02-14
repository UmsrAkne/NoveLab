using System;
using Core;
using Cysharp.Threading.Tasks;
using ScenarioModel;
using UnityEngine;

namespace UI.Animations
{
    public class ReplaceExpression : IUIAnimation
    {
        private readonly IImageContainer imageContainer;
        private readonly IImageSetFactory imageSetFactory;
        private readonly Texture2D newEye;
        private readonly Texture2D newMouth;

        public float Duration { get; set; } = 0.5f;

        public bool IsPlaying { get; private set; }

        public string GroupId { get; set; }

        public ImageOrder ImageOrder { get; set; }

        public int TargetLayerIndex { get; set; }

        public event Action OnCompleted;

        public ReplaceExpression(IImageContainer imageContainer, IImageSetFactory imageSetFactory)
        {
            this.imageContainer = imageContainer;
            this.imageSetFactory = imageSetFactory;
        }

        public void Start()
        {
            RunAsync().Forget(); // ← Forget()で fire-and-forget 明示的に
            return;

            async UniTaskVoid RunAsync()
            {
                IsPlaying = true;
                if (imageContainer.GetFront() is not ICrossFadeCapable imageSet)
                {
                    OnCompleted?.Invoke();
                    IsPlaying = false;
                    return;
                }

                var textures = imageSetFactory.GetTextures(ImageOrder);
                await imageSet.CrossFadeExpression(textures[0], Duration);
                IsPlaying = false;
                OnCompleted?.Invoke();
            }
        }

        public void Stop()
        {
            // CrossFadeExpression がトークンでキャンセルできる仕組みなので、
            // 必要であればここでキャンセルする処理を追加可能
            IsPlaying = false;
        }
    }
}