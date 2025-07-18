using System.Collections.Generic;
using Core;

namespace Scenes.Scenario
{
    public class ScenarioEntry
    {
        public string Text { get; set; }

        public List<IImageOrder> ImageOrders { get; set; } = new ();
    }
}