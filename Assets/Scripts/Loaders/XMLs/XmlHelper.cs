using System.Xml;
using System.Xml.Linq;

namespace UI.Utils
{
    public static class XmlHelper
    {
        public static int GetIntAttribute(XmlElement elem, string attrName, int defaultValue = default)
        {
            if (elem.HasAttribute(attrName) && int.TryParse(elem.GetAttribute(attrName), out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static float GetFloatAttribute(XmlElement elem, string attrName, float defaultValue = default)
        {
            if (elem.HasAttribute(attrName) && float.TryParse(elem.GetAttribute(attrName), out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static bool GetBoolAttribute(XmlElement elem, string attrName, bool defaultValue = default)
        {
            if (elem.HasAttribute(attrName) && bool.TryParse(elem.GetAttribute(attrName), out var value))
            {
                return value;
            }

            return defaultValue;
        }

        public static string GetStringAttribute(XElement element, string attrName)
        {
            return HasAttribute(element, attrName)
                ? element.Attribute(attrName)!.Value
                : string.Empty;
        }

        public static bool HasChild(XElement element, string childName)
        {
            return element.Element(childName) != null;
        }

        public static bool HasAttribute(XElement element, string attributeName)
        {
            return element.Attribute(attributeName) != null;
        }
    }
}