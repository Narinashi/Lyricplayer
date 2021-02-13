using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.LyricEngine;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;

namespace LyricPlayer.UI.Overlay.Renderers
{
    internal class ElementBasedRenderer : IElementBasedRender
    {
        public RenderElement RootElement { get; set; }
        TrackLyric CurrentPlayingTrack { set; get; }
        ILyricEngine LyricEngine { set; get; }

        public void Setup(Graphics gfx)
        {
            if (RootElement == null)
                RootElement = new RenderElement { Size = new Point(1, 1) };
        }

        public void LyricChanged(TrackLyric trackLyric, Lyric currentLyric)
        {
            RootElement.ChildElements.Clear();
            RootElement.ChildElements.Add(currentLyric.Element);
        }

        public void Render(DrawGraphicsEventArgs renderArgs)
        {

        }

        public void Init(ILyricEngine lyricEngine, Point size, Point location)
        {
            LyricEngine = lyricEngine;
            RootElement = new RenderElement
            {
                Size = size,
                Location = location,
            };
        }

        public void Destroy(Graphics gfx)
        {
            RootElement?.Dispose();
            RootElement = null;
        }
    }
}
