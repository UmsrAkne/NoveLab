using System;

namespace UI.Animations
{
    public interface IUIAnimation
    {
        bool IsPlaying { get; }

        void Start();

        void Stop();

        event Action OnCompleted;
    }
}