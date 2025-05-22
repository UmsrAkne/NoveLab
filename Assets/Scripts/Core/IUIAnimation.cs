using System;

namespace Core
{
    public interface IUIAnimation
    {
        bool IsPlaying { get; }

        void Start();

        void Stop();

        event Action OnCompleted;
    }
}