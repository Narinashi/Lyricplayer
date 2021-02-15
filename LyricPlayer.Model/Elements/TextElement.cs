using GameOverlay.Drawing;
using Newtonsoft.Json;

namespace LyricPlayer.Model.Elements
{
    public class TextElement : RenderElement
    {
        public TextElement(Lyric lyric) : base(lyric)
        {
        }
        public TextElement() : base() { }
        public TextElement(string text) : base() { Text = text; }
        public string Text { set; get; }
        public float FontSize { set; get; } = 16;
        public string FontName { set; get; } = "Times New Roman";

        public Color TextColor { set; get; } = new Color(1f,1f,1f,1f);
        public Color BackGroundColor { set; get; }

        public bool Italic { set; get; }
        public bool Bold { set; get; }
        public bool WordWrap { set; get; }
        public bool AutoSize { set; get; } = true;
        /// <summary>
        /// between 0 and 1
        /// </summary>
        public float LineWidthBreakTreshold { set; get; } = 0.85f;
    }
}
