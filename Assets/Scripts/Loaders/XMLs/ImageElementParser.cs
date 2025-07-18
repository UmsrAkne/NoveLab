using System.Linq;
using System.Xml.Linq;
using Scenes.Scenario;
using UI.Images;
using UI.Utils;

namespace Loaders.XMLs
{
    public class ImageElementParser
    {
        public void PopulateScenario(XElement scenarioElement, ScenarioEntry scenario)
        {
            if (!XmlHelper.HasChild(scenarioElement, "image"))
            {
                return;
            }

            var imageElements = scenarioElement.Elements("image");
            foreach (var element in imageElements)
            {
                scenario.ImageOrders.Add(ConvertToImageOrder(element));
            }
        }

        private ImageOrder ConvertToImageOrder(XElement imageElement)
        {
            var order = new ImageOrder();

            // a〜d属性を順に ImageNames に追加
            foreach (var key in new[] { "a", "b", "c", "d", })
            {
                var name = XmlHelper.GetStringAttribute(imageElement, key);
                order.ImageNames.Add(!string.IsNullOrWhiteSpace(name) ? name : string.Empty);
            }

            order.X = XmlHelper.GetFloatAttribute(imageElement, "x");
            order.Y = XmlHelper.GetFloatAttribute(imageElement, "y");
            order.Scale = XmlHelper.GetFloatAttribute(imageElement, "scale", 1.0f);
            order.Duration = XmlHelper.GetFloatAttribute(imageElement, "duration", 1.0f);
            order.Delay = XmlHelper.GetFloatAttribute(imageElement, "delay");
            order.TargetLayerIndex = XmlHelper.GetIntAttribute(imageElement, "targetLayerIndex");
            order.IsOverwrite = XmlHelper.GetBoolAttribute(imageElement, "isOverwrite");

            return order;
        }
    }
}