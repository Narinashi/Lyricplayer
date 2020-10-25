using GameOverlay.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.UI.Overlay
{
    public class LyricHolder
    {
        public string TextToDraw { set; get; }
        public string FontName { set; get; }
        public float FontSize { set; get; }

        public Point CurrentLocation { set; get; }
        public Point DestinationLocation { set; get; }
        public Point RenderSize { set; get; }
        public Color ForeColor { set; get; }
        public Color BackgroundColor { set; get; }

        /// <summary>
        /// in Milliseconds
        /// </summary>
        public int Duration { set; get; }

        public bool LocationSet { set; get; }
        public bool IsCurrent { set; get; }
    }
}
