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
            var scenarioLoader = new ScenarioLoader();
            var scenarioFilePath = $"{GlobalScenarioContext.ScenarioDirectoryPath}/texts/scenario.xml";
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