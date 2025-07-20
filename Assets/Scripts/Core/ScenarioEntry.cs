using System.Collections.Generic;

namespace Core
{
    public class ScenarioEntry
    {
        public string Text { get; set; }

        public List<IImageOrder> ImageOrders { get; set; } = new ();
    }
}