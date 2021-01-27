using GameOverlay.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.UI.Overlay.Elements
{
    class TextElement : RenderElement
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
