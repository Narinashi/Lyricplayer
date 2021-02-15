using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model.Elements;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal interface IElementRenderer
    {
        void Destroy(Graphics gfx);

        void Render<T>(T element, DrawGraphicsEventArgs drawEventArgs) where T : RenderElement;

        void Setup(Graphics gfx);
    }
}
 