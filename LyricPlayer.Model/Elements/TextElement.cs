using GameOverlay.Drawing;
using Newtonsoft.Json;

namespace LyricPlayer.Model.Elements
{
    public class TextElement : RenderElement
    {
        public TextElement(Lyric lyric) : base(lyric)
        {
            Text = lyric.Text;
        }
        public TextElement() : base() { }

        [JsonIgnore]
        public string Text { set; get; }
        public float FontSize { set; get; }
        public string FontName { set; get; }

        public Color TextColor { set; get; }
        public Color BackGroundColor { set; get; }

        public bool Italic { set; get; }
        public bool Bold { set; get; }
        public bool WordWrap { set; get; }
        public bool AutoSize { set; get; } = true;       
    }
}
