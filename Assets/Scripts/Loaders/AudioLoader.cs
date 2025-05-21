using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Loaders
{
    public class AudioLoader : MonoBehaviour
    {
        // キャッシュ辞書
        private readonly Dictionary<string, AudioClip> audioClipCache = new ();

        /// <summary>
        /// 指定したファイルパスの ogg ファイルをキャッシュとして読み込み
        /// </summary>
        public async UniTask<AudioClip> LoadAudioClipAsync(string filePath)
        {
            if (audioClipCache.TryGetValue(filePath, out var cachedClip))
            {
                return cachedClip;
            }

            var url = "file://" + filePath;

            using var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS);

            await request.SendWebRequest().ToUniTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load audio: {request.error}");
                return null;
            }

            var clip = DownloadHandlerAudioClip.GetContent(request);
            clip.name = System.IO.Path.GetFileNameWithoutExtension(filePath); // 任意

            // キャッシュに追加
            audioClipCache[filePath] = clip;
            return clip;
        }

        /// <summary>
        /// キャッシュ済みの AudioClip を取得（未ロードなら null）
        /// </summary>
        public AudioClip GetCachedClip(string filePath)
        {
            audioClipCache.TryGetValue(filePath, out var clip);
            return clip;
        }
    }
}