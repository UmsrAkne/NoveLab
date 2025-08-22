using System;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Animations
{
    public class ReplaceExpression : IUIAnimation
    {
        private readonly ICrossFadeCapable imageSet;
        private readonly Texture2D newEye;
        private readonly Texture2D newMouth;

        public float Duration { get; set; } = 0.5f;

        public bool IsPlaying { get; private set; }

        public string GroupId { get; set; }

        public int TargetLayerIndex { get; set; }

        public event Action OnCompleted;

        public ReplaceExpression(ICrossFadeCapable imageSet, Texture2D newEye, Texture2D newMouth)
        {
            this.imageSet = imageSet;
            this.newEye = newEye;
            this.newMouth = newMouth;
        }

        public void Start()
        {
            RunAsync().Forget(); // ← Forget()で fire-and-forget 明示的に
            return;

            async UniTaskVoid RunAsync()
            {
                IsPlaying = true;
                await imageSet.CrossFadeExpression(newEye, newMouth, Duration);
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