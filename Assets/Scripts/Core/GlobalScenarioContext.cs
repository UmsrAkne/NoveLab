using System.Collections.Generic;
using Scenes.Scenario;

namespace Core
{
    public class GlobalScenarioContext
    {
        public string ScenarioDirectoryPath { get; set; } = string.Empty;

        public List<ScenarioEntry> Scenarios { get; set; } = new ();

        public bool IsLoaded { get; set; }
    }
}