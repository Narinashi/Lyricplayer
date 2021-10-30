using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.MusicPlayer;

namespace LyricPlayer.UI.Overlay.Renderers
{
    public interface IElementBasedRender
    {
        long Offset { set; get; }
        RenderElement RootElement { set; get; }

        void Destroy(Graphics gfx);

        void Setup(Graphics gfx);

        void Render(DrawGraphicsEventArgs renderArgs);

        void TrackChanged(TrackLyric trackLyric);

        void Init(AudioPlayer audioPlayer, System.Drawing.Point size);
    }
}
