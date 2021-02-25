using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.SoundEngine;

namespace LyricPlayer.UI.Overlay.Renderers
{
    public interface IElementBasedRender
    {
        RenderElement RootElement { set; get; }

        void Destroy(Graphics gfx);

        void Setup(Graphics gfx);

        void Render(DrawGraphicsEventArgs renderArgs);

        void TrackChanged(TrackLyric trackLyric);

        void Init(ISoundEngine soundEngine, System.Drawing.Point size);
    }
}
