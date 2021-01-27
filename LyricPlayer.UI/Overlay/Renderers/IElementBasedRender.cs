using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Models;
using LyricPlayer.UI.Overlay.Renderers.ElementRenderers;

namespace LyricPlayer.UI.Overlay.Renderers
{
    internal interface IElementBasedRender : IElementRenderer
    {
        Elements.RenderElement RootElement { set; get; }

        void Render(DrawGraphicsEventArgs renderArgs);

        void LyricChanged(TrackLyric trackLyric, Lyric currentLyric);

        void Init(Point size, Point location);  
    }
}
