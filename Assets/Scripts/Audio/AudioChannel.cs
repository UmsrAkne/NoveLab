using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class AudioChannel
    {
        public AudioSource Source { get; set; }

        public int Index { get; private set; }

        // 論理的な再生状態フラグ（Play直後の true を保証する）
        public bool IsPlayingInternal { get; private set; }

        public Action<int> OnStarted;
        public Action<int> OnFinished;

        // 再生開始直後の不安定な物理フラグを無視するためのフレームカウンター
        private int frameWait;

        public AudioChannel(int index, AudioSource source)
        {
            Index = index;
            Source = source;
        }

        public void Play(AudioClip clip, float volume, float pan)
        {
            if (clip == null)
            {
                return;
            }

            // 既に再生中なら停止する。
            // このケースでも終了イベントは飛ぶ。
            if (IsPlayingInternal)
            {
                Stop();
            }

            Source.clip = clip;
            Source.volume = volume;
            Source.panStereo = pan;
            Source.Play();

            // 論理フラグを即座に立て、イベントを即時実行
            IsPlayingInternal = true;
            frameWait = 5; // Unityのオーディオエンジン準備時間は待機してやりすごす。
            OnStarted?.Invoke(Index);
        }

        public void Stop()
        {
            if (!IsPlayingInternal)
            {
                return;
            }

            Source.Stop();
            Finish();
        }

        public void UpdateCheck()
        {
            if (!IsPlayingInternal)
            {
                return;
            }

            // 再生直後の数フレームは物理チェックをスキップ
            if (frameWait > 0)
            {
                frameWait--;
                return;
            }

            // 物理的に停止（自然終了）していたら終了処理
            if (!Source.isPlaying)
            {
                Finish();
            }
        }

        private void Finish()
        {
            IsPlayingInternal = false;
            frameWait = 0;
            OnFinished?.Invoke(Index);
        }
    }
}