using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal interface IElementRenderer
    {
        void Destroy(Graphics gfx);

        void Setup(Graphics gfx);
    }
}
