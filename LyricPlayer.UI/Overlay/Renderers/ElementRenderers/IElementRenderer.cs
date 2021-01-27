using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal interface IElementRenderer
    {
        void Destroy(Graphics gfx);

        void Setup(Graphics gfx);
    }

    internal interface IElementRenderer<T> : IElementRenderer where T : Elements.RenderElement
    {
        void Render(T element, DrawGraphicsEventArgs renderArgs);
    }
}
