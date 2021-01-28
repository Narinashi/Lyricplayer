using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Models;
using LyricPlayer.UI.Overlay.Elements;

namespace LyricPlayer.UI.Overlay.Renderers
{
    internal class ElementBasedRenderer : IElementBasedRender
    {
        public RenderElement RootElement { get; set; }

        public void LyricChanged(TrackLyric trackLyric, Lyric currentLyric)
        {
        }

        public void Render(DrawGraphicsEventArgs renderArgs)
        {
        }

        public void Init(Point size, Point location)
        {
            RootElement = new RenderElement
            {
                Size = size,
                Position = location,
            };
        }
        public void Setup(Graphics gfx)
        {
            if (RootElement == null)
                RootElement = new RenderElement
                { Size = DisplayTools.GetPhysicalDisplaySize().ToOverlaySize() };
        }
        public void Destroy(Graphics gfx)
        {
            RootElement?.Dispose();
            RootElement = null;
        }
    }
}
