using System;
using System.Collections.Generic;
using ScenarioModel;
using UnityEngine;

namespace Core
{
    public class GlobalScenarioContext
    {
        public string ScenarioDirectoryPath { get; set; } = string.Empty;

        public SceneSetting SceneSetting { get; set; } = new ();

        public List<ScenarioEntry> Scenarios { get; set; } = new ();

        public bool IsLoaded { get; set; }

        public Dictionary<string, AudioClip> Voices { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, Texture2D> Images { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}