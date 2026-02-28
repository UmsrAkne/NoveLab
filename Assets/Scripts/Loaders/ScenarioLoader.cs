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
                new BgmElementParser(),
                new ChapterElementParser(),
                new AnimeElementParser(),
            };

            var scenarios = doc.Root?.Elements("scenario")
                .Where(x => !x.Elements("ignore").Any())
                .ToList();

            if (scenarios == null || scenarios.Count == 0)
            {
                return new List<ScenarioEntry>();
            }

            // start位置取得
            var startIndex = scenarios.FindIndex(x => x.Elements("start").Any());
            if (startIndex < 0)
            {
                startIndex = 0;
            }

            // end位置取得
            var endIndex = scenarios.FindIndex(x => x.Elements("end").Any());
            if (endIndex < 0)
            {
                endIndex = scenarios.Count - 1;
            }

            // start > end なら安全のため空
            if (startIndex > endIndex)
            {
                return new List<ScenarioEntry>();
            }

            return scenarios
                .Skip(startIndex)
                .Take(endIndex - startIndex + 1)
                .Select((x, index)=>
                {
                    var scenarioEntry = new ScenarioEntry()
                    {
                        ScenarioId = index,
                    };

                    elementParsers.ForEach(p => p.PopulateScenario(x, scenarioEntry));
                    return scenarioEntry;
                })
                .ToList();
        }

        public List<ScenarioEntry> Load(string xmlFilePath)
        {
            return Load(XDocument.Load(xmlFilePath));
        }
    }
}