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
                var voiceTask = LoadVoices();
                var bgmTask = LoadBgm();
                var imageTask = LoadImages();
                await UniTask.WhenAll(voiceTask, imageTask, bgmTask);
            }

            GlobalScenarioContext.IsLoaded = true;
            SceneManager.LoadScene("ScenarioScene");
        }

        private void AppendMessageLine(string msg)
        {
            errorMessageText.text += msg + "\n";
        }

        private async UniTask LoadVoices()
        {
            logDumper.Log("Voice ファイルのロードを開始します。");
            var voiceFiles = Directory.GetFiles($"{GlobalScenarioContext.ScenarioDirectoryPath}/voices", "*.ogg") ;
            foreach (var vf in voiceFiles)
            {
                var a = await audioLoader.LoadAudioClipAsync(vf);

                var fullName = PathNormalizer.NormalizeFilePath(vf);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullName);
                var fileName = Path.GetFileName(fullName);

                GlobalScenarioContext.Voices.TryAdd(vf, a);
                GlobalScenarioContext.Voices.TryAdd(fileNameWithoutExtension, a);
                GlobalScenarioContext.Voices.TryAdd(fileName, a);

                logDumper.Log($"{fullName} をロードしました。");
            }

            logDumper.Log($"Voice ファイルのロードが完了しました。({voiceFiles.Length} 件)");
        }

        private async UniTask LoadImages()
        {
            logDumper.Log("Image ファイルのロードを開始します。");
            var imageFiles = Directory.GetFiles($"{GlobalScenarioContext.ScenarioDirectoryPath}/images", "*.png") ;
            foreach (var f in imageFiles)
            {
                var texture = await ImageLoader.LoadTexture(f);

                var fullName = PathNormalizer.NormalizeFilePath(f);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullName);
                var fileName = Path.GetFileName(fullName);

                GlobalScenarioContext.Images.TryAdd(f, texture);
                GlobalScenarioContext.Images.TryAdd(fileNameWithoutExtension, texture);
                GlobalScenarioContext.Images.TryAdd(fileName, texture);

                logDumper.Log($"{fullName} をロードしました。");
            }

            logDumper.Log($"Image ファイルのロードが完了しました。({imageFiles.Length} 件)");
        }

        private async UniTask LoadBgm()
        {
            logDumper.Log("BGM ファイルのロードを開始します。");

            var bgmDirectoryPath = new DirectoryInfo("commonResource/bgms").FullName;
            var bgmFileList = new List<string>();

            GlobalScenarioContext.SceneSetting.BgmOrder ??= new AudioOrder();

            if (GlobalScenarioContext.SceneSetting.BgmOrder.FileName == string.Empty)
            {
                var f = Directory.GetFiles(bgmDirectoryPath).First();
                GlobalScenarioContext.SceneSetting.BgmOrder.FileName = Path.GetFileName(f);
            }

            bgmFileList.Add(GlobalScenarioContext.SceneSetting.BgmOrder.FileName);

            var list = GlobalScenarioContext.Scenarios
                .Where(s => s.BgmOrder != null)
                .Select(s => s.BgmOrder.FileName);

            bgmFileList.AddRange(list);

            foreach (var bgmFileName in bgmFileList)
            {
                var fullName = PathNormalizer.NormalizeFilePath(Path.Combine(bgmDirectoryPath, bgmFileName), ".ogg");

                try
                {

                    var a = await audioLoader.LoadAudioClipAsync(fullName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullName);
                    var fileName = Path.GetFileName(fullName);

                    GlobalScenarioContext.BGMs.TryAdd(fullName, a);
                    GlobalScenarioContext.BGMs.TryAdd(fileNameWithoutExtension, a);
                    GlobalScenarioContext.BGMs.TryAdd(fileName, a);

                    logDumper.Log($"{fullName} をロードしました。");
                }
                catch (Exception ex)
                {
                    logDumper.Log($"BGM ファイルのロード中にエラーが発生しました: {ex.Message}");
                    logDumper.Log($"対象ファイル: {fullName}");
                    logDumper.Log($"スタックトレース: {ex.StackTrace}");
                    throw; // 再スローするかどうかは挙動に応じて
                }
            }

            logDumper.Log($"BGM ファイルのロードが完了しました。({bgmFileList.Count} 件)");
        }
    }
}