using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;

namespace LyricPlayer.UI.Overlay.Renderers
{
    public interface ILyricOverlayRenderer
    {
        string RendererKey { get; }
        string FontName { set; get; }
        float FontSize { set; get; }
        float MainLineFontSize { set; get; }
        int DisplayingLyricLinesCount { set; get; }
        int InterLineSpace { set; get; }
        GraphicsWindow OverlayParent { set; get; }
        Color FontColor { set; get; }
        Color BackgroundColor { set; get; }

        /// <summary>
        /// called every frame
        /// </summary>
        void Render(DrawGraphicsEventArgs renderArgs);

        void LyricChanged(TrackLyric trackLyric, Lyric currentLyric);

        void Setup(Graphics gfx);

        void Destroy(Graphics gfx);
    }
}
