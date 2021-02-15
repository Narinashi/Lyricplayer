using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model.Elements;
using System;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal abstract class ElementRenderer : IElementRenderer, IDisposable
    {
        public void Render<T>(T element, DrawGraphicsEventArgs renderArgs) where T : RenderElement
        {
            PrepareToRender(element, renderArgs);

            var gfx = renderArgs.Graphics;
            gfx.ClearScene(new Color(20,20,20,0.35f));
            gfx.ClipRegionStart(element.AbsoluteRenderArea);
            gfx.TransformStart(TransformationMatrix.Rotation(element.Rotation));

            InternalRender(element, renderArgs);

            gfx.TransformEnd();
            gfx.ClipRegionEnd();

            foreach(var child in element.ChildElements)
            {
                var type = child.GetType();
                if (RendererResolver.Renderers.ContainsKey(type))
                    RendererResolver.Renderers[type].Render(child, renderArgs);
            }
        }

        private void PrepareToRender<T>(T element, DrawGraphicsEventArgs renderArgs) where T : RenderElement
        { InternalRenderPreparation(element, renderArgs); }

        protected abstract void InternalRenderPreparation(RenderElement element, DrawGraphicsEventArgs renderArgs);
        protected abstract void InternalRender(RenderElement element, DrawGraphicsEventArgs renderArgs);

        public abstract void Destroy(Graphics gfx);
        public abstract void Setup(Graphics gfx);
        public abstract void Dispose();

    }

    internal abstract class ElementRenderer<T> : ElementRenderer where T : RenderElement
    {
        

        protected override sealed void InternalRender(RenderElement element, DrawGraphicsEventArgs renderArgs)
        {
            InternalRender((T)element, renderArgs);
        }

        protected override sealed void InternalRenderPreparation(RenderElement element, DrawGraphicsEventArgs renderArgs)
        {
            InternalRenderPreparation((T)element, renderArgs);
        }
        protected abstract void InternalRender(T element, DrawGraphicsEventArgs renderArgs);
        protected virtual void InternalRenderPreparation(T element, DrawGraphicsEventArgs renderArgs) { }
    }
}
