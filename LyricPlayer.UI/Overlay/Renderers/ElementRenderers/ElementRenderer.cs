using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal abstract class ElementRenderer<T> : IElementRenderer where T : Elements.RenderElement
    {
        public void Render(T element, DrawGraphicsEventArgs renderArgs)
        {
            var gfx = renderArgs.Graphics;
            gfx.ClipRegionStart(
                element.RenderArea.Left,
                element.RenderArea.Top,
                element.RenderArea.Right - element.Padding.Right,
                element.RenderArea.Bottom - element.Padding.Bottom
                );
            gfx.TransformStart(TransformationMatrix.Rotation(element.Rotation));

            InternalRender(element, renderArgs);

            gfx.TransformEnd();
            gfx.ClipRegionEnd();
        }

        protected abstract void InternalRender(T element, DrawGraphicsEventArgs renderArgs);
        public abstract void Destroy(Graphics gfx);
        public abstract void Setup(Graphics gfx);
    }
}
