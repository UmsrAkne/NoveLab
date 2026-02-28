using System.Collections.Generic;

namespace ScenarioModel
{
    public class ScenarioEntry
    {
        public string Text { get; set; } = string.Empty;

        public List<ImageOrder> ImageOrders { get; set; } = new ();

        public List<AudioOrder> VoiceOrders { get; set; } = new ();

        public List<AudioOrder> SeOrders { get; set; } = new ();

        public List<AudioOrder> BgvOrders { get; set; } = new ();

        public AudioOrder BgmOrder { get; set; }

        public Chapter Chapter { get; set; }

        public List<AnimationSpec> Animations { get; set; } = new ();

        public int ScenarioId { get; set; }
    }
}