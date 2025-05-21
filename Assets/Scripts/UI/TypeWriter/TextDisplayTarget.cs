using TMPro;

namespace UI.TypeWriter
{
    public class TextDisplayTarget : ITextDisplayTarget
    {
        private readonly TextMeshProUGUI textMeshPro;

        public TextDisplayTarget(TextMeshProUGUI textMeshPro)
        {
            this.textMeshPro = textMeshPro;
        }

        public string Text
        {
            get => textMeshPro.text;
            set => textMeshPro.text = value;
        }
    }
}