using System.Collections.Generic;

namespace ScenarioModel
{
    public class ScenarioEntry
    {
        public string Text { get; set; }

        public List<ImageOrder> ImageOrders { get; set; } = new ();
    }
}