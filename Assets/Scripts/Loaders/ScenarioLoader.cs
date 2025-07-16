using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Loaders.XMLs;
using Scenes.Scenario;

namespace Loaders
{
    public class ScenarioLoader
    {
        public List<ScenarioEntry> Load(string xmlFilePath)
        {
            var doc = XDocument.Load(xmlFilePath);

            var elementParsers = new List<IXElementParser>
            {
                new TextElementParser(),
            };

            return doc.Root?.Elements("scenario").Select(x =>
            {
                var scenarioEntry = new ScenarioEntry();
                elementParsers.ForEach(p => p.PopulateScenario(x, scenarioEntry));
                return scenarioEntry;
            }).ToList();
        }
    }
}