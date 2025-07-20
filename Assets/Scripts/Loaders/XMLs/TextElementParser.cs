using System.Xml.Linq;
using Core;
using UI.Utils;

namespace Loaders.XMLs
{
    public class TextElementParser : IXElementParser
    {
        public void PopulateScenario(XElement scenarioElement, ScenarioEntry scenario)
        {
            if (!XmlHelper.HasChild(scenarioElement, "text"))
            {
                return;
            }

            var textElement = scenarioElement.Element("text");
            var text = XmlHelper.GetStringAttribute(textElement, "string");
            if (!string.IsNullOrWhiteSpace(text))
            {
                scenario.Text = text;
            }
        }
    }
}