using System;
using System.Xml.Linq;
using ScenarioModel;
using UI.Utils;

namespace Loaders.XMLs
{
    public class BgmElementParser : IXElementParser
    {
        public void PopulateScenario(XElement scenarioElement, ScenarioEntry scenario)
        {
            if (!XmlHelper.HasChild(scenarioElement, "bgm"))
            {
                return;
            }

            var bgmElement = scenarioElement.Element("bgm");
            scenario.BgmOrder = ConvertToAudioOrder(bgmElement);
        }

        /// <summary>
        /// Converts a given <c>XElement</c> representing a BGM element into an AudioOrder instance.
        /// </summary>
        /// <param name="bgmElement">The XML element expected to have the tag name "bgm".</param>
        /// <returns>
        /// An AudioOrder object populated with attributes extracted from the XML element.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the provided <paramref name="bgmElement"/> does not have the tag name "bgm".
        /// </exception>
        public AudioOrder ConvertToAudioOrder(XElement bgmElement)
        {
            if (bgmElement.Name != "bgm")
            {
                throw new ArgumentException();
            }

            var order = new AudioOrder
            {
                AudioType = AudioType.Bgm,
                FileName = XmlHelper.GetStringAttribute(bgmElement, "fileName"),
                Volume = XmlHelper.GetFloatAttribute(bgmElement, "volume", 1.0f),
                Delay = XmlHelper.GetFloatAttribute(bgmElement, "delay"),
            };

            return order;
        }
    }
}