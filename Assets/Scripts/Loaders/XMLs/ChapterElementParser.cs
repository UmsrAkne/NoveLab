using System.Xml.Linq;
using ScenarioModel;
using UI.Utils;

namespace Loaders.XMLs
{
    public class ChapterElementParser : IXElementParser
    {
        private readonly string elementName = "chapter";

        public void PopulateScenario(XElement scenarioElement, ScenarioEntry scenario)
        {
            if (!XmlHelper.HasChild(scenarioElement, elementName))
            {
                return;
            }

            var chapterElement = scenarioElement.Element(elementName);
            scenario.Chapter = new Chapter
            {
                Name = XmlHelper.GetStringAttribute(chapterElement, "name"),
            };
        }
    }
}