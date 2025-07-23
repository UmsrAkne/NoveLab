using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Loaders.XMLs;
using ScenarioModel;

namespace Loaders
{
    public class ScenarioLoader
    {
        public List<ScenarioEntry> Load(XDocument doc)
        {
            var elementParsers = new List<IXElementParser>
            {
                new TextElementParser(),
                new ImageElementParser(),
                new VoiceElementParser(),
                new SeElementParser(),
                new BgvElementParser(),
            };

            return doc.Root?.Elements("scenario").Select(x =>
            {
                var scenarioEntry = new ScenarioEntry();
                elementParsers.ForEach(p => p.PopulateScenario(x, scenarioEntry));
                return scenarioEntry;
            }).ToList();
        }

        public List<ScenarioEntry> Load(string xmlFilePath)
        {
            return Load(XDocument.Load(xmlFilePath));
        }
    }
}