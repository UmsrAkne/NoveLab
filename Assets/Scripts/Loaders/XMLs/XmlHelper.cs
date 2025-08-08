using System.Xml.Linq;

namespace Loaders.XMLs
{
    public static class XmlHelper
    {
        public static string GetStringAttribute(XElement element, string attrName)
        {
            return HasAttribute(element, attrName)
                ? element.Attribute(attrName)!.Value
                : string.Empty;
        }

        public static float GetFloatAttribute(XElement element, string attrName, float defaultValue = default)
        {
            if (!HasAttribute(element, attrName))
            {
                return defaultValue;
            }

            return float.TryParse(element.Attribute(attrName)!.Value, out var result)
                ? result
                : defaultValue;
        }

        public static int GetIntAttribute(XElement element, string attrName, int defaultValue = default)
        {
            if (!HasAttribute(element, attrName))
            {
                return defaultValue;
            }

            return int.TryParse(element.Attribute(attrName)!.Value, out var result)
                ? result
                : defaultValue;
        }

        public static bool GetBoolAttribute(XElement element, string attributeName, bool defaultValue = false)
        {
            var attr = element?.Attribute(attributeName);
            if (attr == null)
            {
                return defaultValue;
            }

            return bool.TryParse(attr.Value, out var result) ? result : defaultValue;
        }

        public static bool HasChild(XElement element, string childName)
        {
            return element.Element(childName) != null;
        }

        private static bool HasAttribute(XElement element, string attributeName)
        {
            return element.Attribute(attributeName) != null;
        }
    }
}