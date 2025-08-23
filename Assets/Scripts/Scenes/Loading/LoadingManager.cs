using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Core;
using Cysharp.Threading.Tasks;
using Loaders;
using ScenarioModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Scenes.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public static GlobalScenarioContext GlobalScenarioContext { get; private set; } = new ();

        [SerializeField] private TextMeshProUGUI errorMessageText;

        [SerializeField] private AudioLoader audioLoader;

        [SerializeField] private LogDumper logDumper;

        private readonly string scenarioFileName = "scenario.xml";
        private readonly string settingFileName = "setting.xml";

        private async void Start()
        {
            var path = GlobalScenarioContext.ScenarioDirectoryPath;
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.Log("シナリオディレクトリのパスが設定されていません。デバッグ用のシーンを設定します。");
                path = Path.Combine(new DirectoryInfo("scenes").FullName, "99999999_9999");
                GlobalScenarioContext.ScenarioDirectoryPath = path;
            }

            var scenarioFilePath = $"{path}/texts/{scenarioFileName}";
            var settingFilePath = $"{path}/texts/{settingFileName}";

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(scenarioFilePath))
            {
                AppendMessageLine($"{scenarioFileName} が見つかりません。");
                AppendMessageLine($"scenario directory path: {path}");
                AppendMessageLine($"scenario file path: {scenarioFilePath}");
                return;
            }

            if (!File.Exists(settingFilePath))
            {
                AppendMessageLine($"{settingFileName} が見つかりません。");
                return;
            }

            var settingLoader = new SettingLoader();
            try
            {
                GlobalScenarioContext.SceneSetting = settingLoader.Load(settingFilePath);
                AppendMessageLine($"{settingFileName} の読み込みに成功しました");
            }
            catch (XmlException e)
            {
                AppendMessageLine("Error:");
                AppendMessageLine($"Line number: {e.LineNumber}");
                AppendMessageLine(e.Message);
                throw;
            }

            var scenarioLoader = new ScenarioLoader();
            try
            {
                GlobalScenarioContext.Scenarios = scenarioLoader.Load(scenarioFilePath);
                AppendMessageLine($"{scenarioFileName} の読み込みに成功しました。");
            }
            catch (XmlException e)
            {
                AppendMessageLine("Error:");
                AppendMessageLine($"Line number: {e.LineNumber}");
                AppendMessageLine(e.Message);
                throw;
            }

            if (!GlobalScenarioContext.IsLoaded)
            {
                var voiceTask = LoadAssets(
                    Path.Combine(GlobalScenarioContext.ScenarioDirectoryPath, "voices"),
                    ".ogg",
                    s => s.VoiceOrders, // IEnumerable<IOrder>
                    GlobalScenarioContext.Voices,
                    async p => await audioLoader.LoadAudioClipAsync(p)
                );

                var bgvTask = LoadAssets(
                    Path.Combine(GlobalScenarioContext.ScenarioDirectoryPath, "bgvs"),
                    ".ogg",
                    s => s.BgvOrders,
                    GlobalScenarioContext.Bgvs,
                    async p => await audioLoader.LoadAudioClipAsync(p)
                );

                var imageTask = LoadAssets(
                    Path.Combine(GlobalScenarioContext.ScenarioDirectoryPath, "images"),
                    ".png",
                    s => s.ImageOrders,
                    GlobalScenarioContext.Images,
                    async p => await ImageLoader.LoadTexture(p)
                );

                var animationImageTask = LoadAssets(
                    Path.Combine(GlobalScenarioContext.ScenarioDirectoryPath, "images"),
                    ".png",
                    s => s.Animations,
                    GlobalScenarioContext.Images,
                    async p => await ImageLoader.LoadTexture(p)
                );

                var seTask = LoadAssets(
                    new DirectoryInfo("commonResource/ses").FullName,
                    ".ogg",
                    s => s.SeOrders,
                    GlobalScenarioContext.Ses,
                    async p => await audioLoader.LoadAudioClipAsync(p)
                );

                var bgmDirectory = new DirectoryInfo("commonResource/bgms").FullName;

                var bgmTask = LoadAssets(
                    bgmDirectory,
                    ".ogg",
                    s => new List<IOrder>() { s.BgmOrder, },
                    GlobalScenarioContext.BGMs,
                    async p => await audioLoader.LoadAudioClipAsync(p)
                );

                var defaultBgmTask = LoadDefaultSceneBGMAsync(bgmDirectory, ".ogg");

                await UniTask.WhenAll(voiceTask, bgvTask, seTask, imageTask, animationImageTask, bgmTask, defaultBgmTask);
            }

            GlobalScenarioContext.IsLoaded = true;
            SceneManager.LoadScene("ScenarioScene");
        }

        private void AppendMessageLine(string msg)
        {
            errorMessageText.text += msg + "\n";
        }

        private async UniTask LoadAssets<TAsset>(
            string resourceDirectoryPath,
            string extension,
            Func<ScenarioEntry, IEnumerable<IOrder>> orderSelector,
            IDictionary<string, TAsset> targetDictionary,
            Func<string, UniTask<TAsset>> loader)
        {
            var directory = new DirectoryInfo(resourceDirectoryPath);

            // シナリオ上の全リソースファイル名を取得（単数/複数両対応）
            var fileNames = GlobalScenarioContext.Scenarios
                .SelectMany(orderSelector)
                .Where(o => o != null)
                .SelectMany(o => o.ResourceFileNames)
                .Where(fn => !string.IsNullOrWhiteSpace(fn))
                .Distinct();

            var loadedCount = 0;

            foreach (var fn in fileNames)
            {
                var fullName = PathNormalizer.NormalizeFilePath(Path.Combine(directory.FullName, fn), extension);
                logDumper.Log($"start load: {fullName}");

                if (targetDictionary.ContainsKey(fullName))
                {
                    logDumper.Log($"{fullName} は既にロード済みのためスキップします。");
                    continue;
                }

                try
                {
                    var asset = await loader(fullName);

                    RegisterAssetWithMultipleKeys(targetDictionary, fullName, asset);
                    logDumper.Log($"{fullName} をロードしました。");
                    loadedCount++;
                }
                catch (Exception ex)
                {
                    logDumper.Log($"ファイルのロード中にエラーが発生しました: {ex.Message}");
                    logDumper.Log($"対象ファイル: {fullName}");
                    logDumper.Log($"スタックトレース: {ex.StackTrace}");
                    throw;
                }
            }

            logDumper.Log($"{directory.Name} ファイルのロードが完了しました。({loadedCount} 件)");
        }

        private async UniTask LoadDefaultSceneBGMAsync(string directory, string extension)
        {
            var bgmOrder = GlobalScenarioContext.SceneSetting.BgmOrder;
            if (bgmOrder == null || string.IsNullOrWhiteSpace(bgmOrder.FileName))
            {
                return;
            }

            var fullPath = PathNormalizer.NormalizeFilePath(
                Path.Combine(directory, bgmOrder.FileName), extension);

            var clip = await audioLoader.LoadAudioClipAsync(fullPath);
            RegisterAssetWithMultipleKeys(GlobalScenarioContext.BGMs, fullPath, clip);
        }

        private void RegisterAssetWithMultipleKeys<TAsset>(IDictionary<string, TAsset> targetDictionary, string fullName, TAsset asset)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullName);
            var fileName = Path.GetFileName(fullName);

            targetDictionary.TryAdd(fullName, asset);
            targetDictionary.TryAdd(fileNameWithoutExtension, asset);
            targetDictionary.TryAdd(fileName, asset);
        }
    }
}