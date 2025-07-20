using System.Xml.Linq;
using Core;

namespace Loaders.XMLs
{
    public interface IXElementParser
    {
        /// <summary>
        /// 入力した Scenario 要素をパースし、ScenarioEntry に入力します。
        /// </summary>
        /// <param name="scenarioElement"></param>
        /// <param name="scenario"></param>
        void PopulateScenario(XElement scenarioElement, ScenarioEntry scenario);
    }
}