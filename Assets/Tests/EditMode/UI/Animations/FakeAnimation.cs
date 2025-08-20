using System;
using Core;

namespace Tests.EditMode.UI.Animations
{
    public class FakeAnimation : IUIAnimation
    {
        public bool IsPlaying { get; private set; }
        public string GroupId { get; set; } = string.Empty;

        public int TargetLayerIndex { get; set; }

        public event Action OnCompleted;

        public void Start()
        {
            IsPlaying = true;
            // テスト側から明示的に OnCompleted を呼ぶまで待機
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void Complete()
        {
            IsPlaying = false;
            OnCompleted?.Invoke();
        }
    }
}