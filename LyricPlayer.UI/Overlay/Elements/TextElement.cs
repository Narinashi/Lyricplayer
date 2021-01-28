using GameOverlay.Drawing;

namespace LyricPlayer.UI.Overlay.Elements
{
    internal class TextElement : RenderElement
    {
        public string Text { set; get; }
        public float FontSize { set; get; }
        public string FontName { set; get; }

        public Color TextColor { set; get; }
        public Color BackGroundColor { set; get; }

        public bool Italic { set; get; }
        public bool Bold { set; get; }
        public bool WordWrap { set; get; }
    }
}
