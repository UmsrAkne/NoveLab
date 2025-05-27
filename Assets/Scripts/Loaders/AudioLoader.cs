using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Loaders
{
    public class AudioLoader : MonoBehaviour
    {
        // キャッシュ辞書
        private readonly Dictionary<string, AudioClip> audioClipCache = new (StringComparer.OrdinalIgnoreCase);

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
            clip.name = Path.GetFileNameWithoutExtension(filePath); // 任意

            // キャッシュに追加
            var fileName = Path.GetFileName(filePath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            if (!string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                audioClipCache[fileName] = clip;
                audioClipCache[fileNameWithoutExtension] = clip;
            }

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