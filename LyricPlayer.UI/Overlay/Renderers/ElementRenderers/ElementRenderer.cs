using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model.Elements;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal abstract class ElementRenderer<T> : IElementRenderer where T :RenderElement
    {
        public void Render(T element, DrawGraphicsEventArgs renderArgs)
        {
            var gfx = renderArgs.Graphics;
            gfx.ClipRegionStart(element.AbsoluteRenderArea);
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
