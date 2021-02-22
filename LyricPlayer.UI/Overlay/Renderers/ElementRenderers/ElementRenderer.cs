using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.UI.Overlay.EffectPlayers;
using System;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal abstract class ElementRenderer : IElementRenderer, IDisposable
    {
        public void Render<T>(T element, DrawGraphicsEventArgs renderArgs) where T : RenderElement
        {
            PrepareToRender(element, renderArgs);
            ApplyEffects(element, renderArgs);

            var gfx = renderArgs.Graphics;
            gfx.ClipRegionStart(element.AbsoluteRenderArea.ToOverlayRectangle());
            gfx.TransformStart(TransformationMatrix.Rotation(element.Rotation, new Point(element.Size.X / 2, element.Size.Y / 2)));

            InternalRender(element, renderArgs);

            gfx.TransformEnd();
            gfx.ClipRegionEnd();

            foreach (var child in element.ChildElements)
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
        private void ApplyEffects<T>(T element, DrawGraphicsEventArgs renderArgs) where T : RenderElement
        {
            if (element?.Lyric?.Effects == null) return;
            foreach (var effect in element.Lyric.Effects)
            {
                var type = effect.GetType();
                if (EffectPlayerResolver.EffectPlayers.ContainsKey(type))
                    EffectPlayerResolver.EffectPlayers[type].ApplyEffect(element, effect, renderArgs);
            }
        }

        private void ComputeElementSizeAndLocation(RenderElement element)
        {
            if (element.ParentElement == null) return;
            switch (element.Dock)
            {
                case ElementDock.Fill:
                    element.Size = element.ParentElement.Size;
                    element.Location = new FloatPoint(0, 0);
                    break;
                case ElementDock.Top:
                    element.Size = new System.Drawing.Point(element.ParentElement.Size.X, element.Size.Y);
                    element.Location = new FloatPoint(0, 0);
                    break;
                case ElementDock.Left:
                    element.Size = new System.Drawing.Point(element.Size.X, element.ParentElement.Size.Y);
                    element.Location = new FloatPoint(0, 0);
                    break;
                case ElementDock.Bottom:
                    element.Size = new System.Drawing.Point(element.ParentElement.Size.X, element.Size.Y);
                    element.Location = new FloatPoint(0, element.ParentElement.Size.Y - element.Size.Y);
                    break;
                case ElementDock.Right:
                    element.Size = new System.Drawing.Point(element.Size.X, element.ParentElement.Size.Y);
                    element.Location = new FloatPoint(element.ParentElement.Size.X - element.Size.X, 0);
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
