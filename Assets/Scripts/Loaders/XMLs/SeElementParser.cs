using System.Xml.Linq;
using ScenarioModel;

namespace Loaders.XMLs
{
    public class SeElementParser : IXElementParser
    {
        public void PopulateScenario(XElement scenarioElement, ScenarioEntry scenario)
        {
            if (!XmlHelper.HasChild(scenarioElement, "se"))
            {
                return;
            }

            var seElements = scenarioElement.Elements("se");
            foreach (var element in seElements)
            {
                scenario.SeOrders.Add(ConvertToAudioOrder(element));
            }
        }

        private AudioOrder ConvertToAudioOrder(XElement seElement)
        {
            var order = new AudioOrder
            {
                AudioType = AudioType.Se,
                FileName = XmlHelper.GetStringAttribute(seElement, "fileName"),
                Volume = XmlHelper.GetFloatAttribute(seElement, "volume", 1.0f),
                Delay = XmlHelper.GetFloatAttribute(seElement, "delay"),
                Pan = XmlHelper.GetFloatAttribute(seElement, "pan"),
                RepeatCount = XmlHelper.GetIntAttribute(seElement, "repeatCount"),
                ChannelIndex = XmlHelper.GetIntAttribute(seElement, "channel"),
            };

            return order;
        }
    }
}