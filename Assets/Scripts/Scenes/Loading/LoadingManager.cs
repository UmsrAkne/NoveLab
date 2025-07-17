using Core;
using Loaders;
using UnityEngine;

namespace Scenes.Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public static GlobalScenarioContext GlobalScenarioContext { get; private set; } = new ();

        private void Start()
        {
            var scenarioLoader = new ScenarioLoader();
            var scenarioFilePath = $"{GlobalScenarioContext.ScenarioDirectoryPath}/texts/scenario.xml";
            GlobalScenarioContext.Scenarios = scenarioLoader.Load(scenarioFilePath);
            GlobalScenarioContext.IsLoaded = true;
        }
    }
}