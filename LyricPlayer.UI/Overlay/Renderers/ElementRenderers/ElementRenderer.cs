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
            gfx.ClipRegionStart(element.AbsoluteRenderArea.ToOverlayRectangle());
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
        {
            ComputeElementSizeAndLocation(element);
            InternalRenderPreparation(element, renderArgs);
        }
        private void ComputeElementSizeAndLocation(RenderElement element)
        {
            if (element.ParentElement == null) return;
            switch (element.Dock)
            {
                case ElementDock.Fill:
                    element.Size = element.ParentElement.Size;
                    element.Location = new System.Drawing.Point(0,0);
                    break;
                case ElementDock.Top:
                    element.Size = new System.Drawing.Point(element.ParentElement.Size.X, element.Size.Y);
                    element.Location = new System.Drawing.Point(0,0);
                    break;
                case ElementDock.Left:
                    element.Size = new System.Drawing.Point(element.Size.X, element.ParentElement.Size.Y);
                    element.Location = new System.Drawing.Point(0, 0);
                    break;
                case ElementDock.Bottom:
                    element.Size = new System.Drawing.Point(element.ParentElement.Size.X, element.Size.Y);
                    element.Location = new System.Drawing.Point(0, element.ParentElement.Size.Y - element.Size.Y);
                    break;
                case ElementDock.Right:
                    element.Size = new System.Drawing.Point(element.Size.X, element.ParentElement.Size.Y);
                    element.Location = new System.Drawing.Point(element.ParentElement.Size.X - element.Size.X, 0);
                    break;
            }
        }

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
