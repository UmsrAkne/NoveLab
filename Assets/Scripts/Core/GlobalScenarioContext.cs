using System.Collections.Generic;
using ScenarioModel;

namespace Core
{
    public class GlobalScenarioContext
    {
        public string ScenarioDirectoryPath { get; set; } = string.Empty;

        public SceneSetting SceneSetting { get; set; } = new ();

        public List<ScenarioEntry> Scenarios { get; set; } = new ();

        public bool IsLoaded { get; set; }
    }
}