using System.Collections.Generic;

namespace ScenarioModel
{
    public class AudioOrder
    {
        public AudioType AudioType { get; set; }

        public int ChannelIndex { get; set; }

        public string FileName { get; set; } = string.Empty;

        public List<string> FileNames { get; private set; } = new ();

        public float Volume { get; set; } = 1.0f;

        public float Delay { get; set; } = 0f;

        public float Pan { get; set; } = 0f;

        public int RepeatCount { get; set; } = 0;
    }
}