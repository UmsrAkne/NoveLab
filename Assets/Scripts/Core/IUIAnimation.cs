using System;

namespace Core
{
    public interface IUIAnimation
    {
        bool IsPlaying { get; }

        void Start();

        void Stop();

        string GroupId { get; set; }

        int TargetLayerIndex { get; set; }

        event Action OnCompleted;
    }
}