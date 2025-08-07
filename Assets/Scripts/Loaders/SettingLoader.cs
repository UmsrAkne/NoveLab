using System.Xml.Linq;
using Loaders.XMLs;
using ScenarioModel;

namespace Loaders
{
    public class SettingLoader
    {
        public SceneSetting Load(XDocument doc)
        {
            var rootElement = doc.Root;
            return ToSceneSetting(rootElement);
        }

        public SceneSetting Load(string xmlFilePath)
        {
            return Load(XDocument.Load(xmlFilePath));
        }

        public SceneSetting ToSceneSetting(XElement element)
        {
            var setting = new SceneSetting();

            var bgmElement = element.Element("bgm");
            if (bgmElement != null)
            {
                setting.BgmOrder = new BgmElementParser().ConvertToAudioOrder(bgmElement);
            }

            if (XmlHelper.HasChild(element, "voice"))
            {
                setting.VoiceVolume = XmlHelper.GetFloatAttribute(element.Element("voice"), "volume", 1.0f);
            }

            if (XmlHelper.HasChild(element, "se"))
            {
                setting.SeVolume = XmlHelper.GetFloatAttribute(element.Element("se"), "volume", 1.0f);
            }

            if (XmlHelper.HasChild(element, "bgv"))
            {
                setting.BgvVolume = XmlHelper.GetFloatAttribute(element.Element("bgv"), "volume", 1.0f);
            }

            if (XmlHelper.HasChild(element, "window"))
            {
                setting.WindowWidth = XmlHelper.GetIntAttribute(element.Element("window"), "width", 1280);
            }

            return setting;
        }
    }
}