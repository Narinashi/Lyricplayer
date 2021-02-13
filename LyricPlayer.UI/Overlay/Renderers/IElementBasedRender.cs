using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.LyricEngine;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.UI.Overlay.Renderers.ElementRenderers;

namespace LyricPlayer.UI.Overlay.Renderers
{
    internal interface IElementBasedRender : IElementRenderer
    {
        RenderElement RootElement { set; get; }

        void Render(DrawGraphicsEventArgs renderArgs);

        void LyricChanged(TrackLyric trackLyric, Lyric currentLyric);

        void Init(ILyricEngine lyricEngine, Point size, Point location);
    }
}
