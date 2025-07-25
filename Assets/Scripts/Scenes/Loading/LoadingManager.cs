using System.IO;
using System.Xml;
using Core;
using Loaders;
using TMPro;
using UnityEngine;

namespace Scenes.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public static GlobalScenarioContext GlobalScenarioContext { get; private set; } = new ();

        [SerializeField] private TextMeshProUGUI errorMessageText;

        private void Start()
        {
            var path = GlobalScenarioContext.ScenarioDirectoryPath;
            var scenarioFilePath = $"{path}/texts/scenario.xml";
            var settingFilePath = $"{path}/texts/settings.xml";

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(scenarioFilePath))
            {
                AppendMessageLine("scenario.xml が見つかりません。");
                AppendMessageLine($"scenario directory path: {path}");
                AppendMessageLine($"scenario file path: {scenarioFilePath}");
                return;
            }

            if (!File.Exists(settingFilePath))
            {
                AppendMessageLine("setting.xml が見つかりません。");
                return;
            }

            var settingLoader = new SettingLoader();
            try
            {
                GlobalScenarioContext.SceneSetting = settingLoader.Load(settingFilePath);
                AppendMessageLine("setting.xml の読み込みに成功しました");
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
                AppendMessageLine("scenario.xml の読み込みに成功しました。");
            }
            catch (XmlException e)
            {
                AppendMessageLine("Error:");
                AppendMessageLine($"Line number: {e.LineNumber}");
                AppendMessageLine(e.Message);
                throw;
            }

            GlobalScenarioContext.IsLoaded = true;
        }

        private void AppendMessageLine(string msg)
        {
            errorMessageText.text += msg + "\n";
        }
    }
}