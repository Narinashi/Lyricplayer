using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.LyricEngine;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.UI.Overlay.Renderers.ElementRenderers;

namespace LyricPlayer.UI.Overlay.Renderers
{
    public interface IElementBasedRender 
    {
        RenderElement RootElement { set; get; }

        void Destroy(Graphics gfx);

        void Setup(Graphics gfx);

        void Render(DrawGraphicsEventArgs renderArgs);

        void LyricChanged(TrackLyric trackLyric);

        void Init(ILyricEngine lyricEngine, System.Drawing.Point size);
    }
}
