using System;
using System.Collections.Generic;

namespace ScenarioModel
{
    public sealed class AnimationSpec : IOrder
    {
        public string Name { get; set; } = string.Empty;

        public Dictionary<string, string> Attrs { get; set; } = new (StringComparer.OrdinalIgnoreCase);

        public List<AnimationSpec> Children { get; set; } = new ();

        public List<string> ResourceFileNames => GetResourceFileNames(this);

        private static List<string> GetResourceFileNames(AnimationSpec spec)
        {
            var attrs = new List<string>();

            if (spec.Name.Equals("chain", StringComparison.OrdinalIgnoreCase))
            {
                var list = new List<string>();
                foreach (var child in spec.Children)
                {
                    list.AddRange(GetResourceFileNames(child));
                }

                return list;
            }

            if (spec.Name.Equals("image", StringComparison.OrdinalIgnoreCase))
            {
                var patterns = new List<string>() {"a", "b", "c", "d", };

                foreach (var pattern in patterns)
                {
                    spec.Attrs.TryGetValue(pattern, out var value);
                    attrs.Add(value ?? string.Empty);
                }
            }

            if (spec.Name.Equals("draw", StringComparison.OrdinalIgnoreCase))
            {
                var patterns = new List<string>() {"a", "b", "c", "d", };

                foreach (var pattern in patterns)
                {
                    spec.Attrs.TryGetValue(pattern, out var value);
                    attrs.Add(value ?? string.Empty);
                }
            }

            return attrs;
        }
    }
}