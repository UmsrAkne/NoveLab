using System.Collections.Generic;
using UI.Images;

namespace Scenes.Scenario
{
    public class ScenarioEntry
    {
        public string Text { get; set; }

        public List<ImageOrder> ImageOrders { get; set; } = new ();
    }
}