using System.Xml.Linq;
using Scenes.Scenario;
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

            var text = XmlHelper.GetStringAttribute(scenarioElement, "string");
            if (!string.IsNullOrWhiteSpace(text))
            {
                scenario.Text = text;
            }
        }
    }
}