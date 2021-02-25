using Newtonsoft.Json;
using System.Drawing;

namespace LyricPlayer.Model.Elements
{
    public class TextElement : BasicElement
    {
        public TextElement() { }
        public TextElement(string text) : base() { Text = text; }
        public string Text { set; get; }
        public float FontSize { set; get; } = 40;
        public string FontName { set; get; } = "Times New Roman";

        public TextHorizontalAlignment HorizontalAlignment { set; get; }
        public TextVerticalAlignment VerticalAlignment { set; get; }

        public Color TextColor { set; get; } = Color.White;

        public bool Italic { set; get; }
        public bool Bold { set; get; } = true;
        public bool WordWrap { set; get; }
        public bool AutoSize { set; get; } = true;
        /// <summary>
        /// between 0 and 1
        /// </summary>
        public float LineWidthBreakTreshold { set; get; } = 0.85f;
    }
}
