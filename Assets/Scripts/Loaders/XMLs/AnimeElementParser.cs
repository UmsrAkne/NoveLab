using System;
using System.Linq;
using System.Xml.Linq;
using ScenarioModel;

namespace Loaders.XMLs
{
    public class AnimeElementParser : IXElementParser
    {
        public void PopulateScenario(XElement scenarioEl, ScenarioEntry scenario)
        {
            scenario.Animations.AddRange(
                scenarioEl.Elements("anime").Select(e => ParseAnime(e, false)));
        }

        private AnimationSpec ParseAnime(XElement el, bool insideChain)
        {
            var name = (string)el.Attribute("name")
                       ?? throw new InvalidOperationException("<anime> に name がありません");

            if (name.Equals("chain", StringComparison.OrdinalIgnoreCase))
            {
                if (insideChain)
                {
                    throw new InvalidOperationException("chain の入れ子にできません");
                }

                return new AnimationSpec
                {
                    Name = "chain",
                    Attrs = el.Attributes().Where(a => a.Name.LocalName != "name")
                        .ToDictionary(a => a.Name.LocalName, a => a.Value, StringComparer.OrdinalIgnoreCase),
                    Children = el.Elements("anime").Select(c => ParseAnime(c, true)).ToList(),
                };
            }
            else
            {
                return new AnimationSpec
                {
                    Name = name,
                    Attrs = el.Attributes().Where(a => a.Name.LocalName != "name")
                        .ToDictionary(a => a.Name.LocalName, a => a.Value, StringComparer.OrdinalIgnoreCase),
                };
            }
        }
    }
}