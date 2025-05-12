namespace UI.TypeWriter
{
    public class TypewriterEngine
    {
        private readonly float interval = 0.05f; // 1文字表示ごとの間隔（秒）
        private readonly ITextDisplayTarget target;
        private string fullText;
        private int currentIndex;
        private float elapsed;

        public TypewriterEngine(ITextDisplayTarget target)
        {
            this.target = target;
        }

        public void SetText(string text)
        {
            fullText = text;
            currentIndex = 0;
            elapsed = 0f;
        }

        public void Update(float deltaTime)
        {
            if (currentIndex >= fullText.Length)
            {
                return;
            }

            elapsed += deltaTime;

            while (elapsed >= interval)
            {
                elapsed -= interval;
                target.Text += fullText[currentIndex];
                currentIndex++;

                if (currentIndex >= fullText.Length)
                {
                    break;
                }
            }
        }

        public bool IsFinished => currentIndex >= fullText.Length;
    }
}