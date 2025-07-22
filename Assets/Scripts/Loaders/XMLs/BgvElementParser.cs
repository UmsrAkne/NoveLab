using System;
using System.Linq;
using System.Xml.Linq;
using ScenarioModel;
using UI.Utils;

namespace Loaders.XMLs
{
    public class BgvElementParser
    {
        public void PopulateScenario(XElement scenarioElement, ScenarioEntry scenario)
        {
            var bgvElements = scenarioElement.Elements()
                .Where(e => e.Name == "bgv" || e.Name == "backgroundVoice");

            foreach (var element in bgvElements)
            {
                scenario.BgvOrders.Add(ConvertToAudioOrder(element));
            }
        }

        private AudioOrder ConvertToAudioOrder(XElement bgvElement)
        {
            var order = new AudioOrder
            {
                AudioType = AudioType.Bgv,
                Volume = XmlHelper.GetFloatAttribute(bgvElement, "volume", 1.0f),
                Pan = XmlHelper.GetFloatAttribute(bgvElement, "pan"),
                ChannelIndex = XmlHelper.GetIntAttribute(bgvElement, "channel"),
            };

            order.FileNames.AddRange(
                XmlHelper.GetStringAttribute(bgvElement, "names")
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => n.Trim()));

            return order;
        }
    }
}