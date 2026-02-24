using System.Xml.Linq;
using ScenarioModel;

namespace Loaders.XMLs
{
    public class VoiceElementParser : IXElementParser
    {
        public void PopulateScenario(XElement scenarioElement, ScenarioEntry scenario)
        {
            if (!XmlHelper.HasChild(scenarioElement, "voice"))
            {
                return;
            }

            var voiceElements = scenarioElement.Elements("voice");
            foreach (var element in voiceElements)
            {
                var vc = ConvertToAudioOrder(element);
                if (string.IsNullOrWhiteSpace(vc.FileName))
                {
                    continue;
                }

                scenario.VoiceOrders.Add(vc);
            }
        }

        private AudioOrder ConvertToAudioOrder(XElement voiceElement)
        {
            var order = new AudioOrder
            {
                AudioType = AudioType.Voice,
                FileName = XmlHelper.GetStringAttribute(voiceElement, "fileName"),
                Volume = XmlHelper.GetFloatAttribute(voiceElement, "volume", 1.0f),
                Delay = XmlHelper.GetFloatAttribute(voiceElement, "delay"),
                Pan = XmlHelper.GetFloatAttribute(voiceElement, "pan"),
                RepeatCount = XmlHelper.GetIntAttribute(voiceElement, "repeatCount"),
                ChannelIndex = XmlHelper.GetIntAttribute(voiceElement, "channel"),
            };

            return order;
        }
    }
}