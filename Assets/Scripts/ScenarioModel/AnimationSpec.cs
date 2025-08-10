using System;
using System.Collections.Generic;

namespace ScenarioModel
{
    public sealed class AnimationSpec
    {
        public string Name { get; set; } = string.Empty;

        public Dictionary<string, string> Attrs { get; set; } = new (StringComparer.OrdinalIgnoreCase);

        public List<AnimationSpec> Children { get; set; } = new ();
    }
}