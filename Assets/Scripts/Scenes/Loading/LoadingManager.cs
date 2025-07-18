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

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(scenarioFilePath))
            {
                errorMessageText.text += "scenario.xml が見つかりません。\n";
                errorMessageText.text += $"scenario directory path: {path}\n";
                errorMessageText.text += $"scenario file path: {scenarioFilePath}\n";
                return;
            }

            var scenarioLoader = new ScenarioLoader();
            try
            {
                GlobalScenarioContext.Scenarios = scenarioLoader.Load(scenarioFilePath);
                errorMessageText.text = "scenario.xml の読み込みに成功しました。";
            }
            catch (XmlException e)
            {
                errorMessageText.text += $"Error:\n";
                errorMessageText.text += $"Line number: {e.LineNumber}\n";
                errorMessageText.text = e.Message;
                throw;
            }

            GlobalScenarioContext.IsLoaded = true;
        }
    }
}