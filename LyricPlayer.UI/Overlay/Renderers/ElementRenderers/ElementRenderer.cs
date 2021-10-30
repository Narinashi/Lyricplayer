using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.MusicPlayer;
using LyricPlayer.UI.Overlay.EffectPlayers;
using System;
using System.Linq;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal abstract class ElementRenderer : IDisposable
    {
        public long Offset { set; get; }
        public void Render<T>(T element, AudioPlayer audioPlayer, DrawGraphicsEventArgs renderArgs) where T : RenderElement
        {
            PrepareToRender(element, renderArgs);
            ApplyEffects(element, renderArgs);

            var gfx = renderArgs.Graphics;
            gfx.ClipRegionStart(element.AbsoluteRenderArea.ToOverlayRectangle());

            if (element.Rotation != null)
                gfx.TransformStart(TransformationMatrix.Rotation(element.Rotation.Rotation,
                    element.Rotation.RotationCenter.HasValue ?
                    element.Rotation.RotationCenter.Value.ToOverlayPoint() :
                    new Point(element.AbsoluteLocation.X + (element.Size.X / 2f), element.AbsoluteLocation.Y + (element.Size.Y / 2f))));

            InternalRender(element, audioPlayer, renderArgs);

            var time = audioPlayer.CurrentTime.TotalMilliseconds + Offset;
            foreach (var child in element.ChildElements)
            {
                if (child.EndAt < time) continue;
                if (child.StartAt > time) break;

                var type = child.GetType();
                if (RendererResolver.Renderers.ContainsKey(type))
                    RendererResolver.Renderers[type].Render(child, audioPlayer, renderArgs);
            }

            if (element.Rotation != null)
                gfx.TransformEnd();

            gfx.ClipRegionEnd();
        }

        public void PrepareToRender<T>(T element, DrawGraphicsEventArgs renderArgs) where T : RenderElement
        {
            ComputeElementSizeAndLocation(element);
            InternalRenderPreparation(element, renderArgs);
        }
        private void ApplyEffects<T>(T element, DrawGraphicsEventArgs renderArgs) where T : RenderElement
        {
            if (!(element.Effects?.Any() ?? false)) return;
            foreach (var effect in element.Effects)
            {
                var effectType = effect.GetType();
                if (EffectPlayerResolver.EffectPlayers.Keys.Contains(effectType))
                    EffectPlayerResolver.EffectPlayers[effectType].ApplyEffect(element, effect, renderArgs);
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
        protected abstract void InternalRender(RenderElement element, AudioPlayer audioPlayer, DrawGraphicsEventArgs renderArgs);

        public abstract void Destroy(Graphics gfx);
        public abstract void Setup(Graphics gfx);
        public abstract void Dispose();
        public virtual void Cleanup() { }
    }

    internal abstract class ElementRenderer<T> : ElementRenderer where T : RenderElement
    {
        protected override sealed void InternalRender(RenderElement element, AudioPlayer audioPlayer, DrawGraphicsEventArgs renderArgs)
        {
            InternalRender((T)element, audioPlayer, renderArgs);
        }

        protected override sealed void InternalRenderPreparation(RenderElement element, DrawGraphicsEventArgs renderArgs)
        {
            InternalRenderPreparation((T)element, renderArgs);
        }
        protected abstract void InternalRender(T element, AudioPlayer audioPlayer, DrawGraphicsEventArgs renderArgs);
        protected virtual void InternalRenderPreparation(T element, DrawGraphicsEventArgs renderArgs) { }
    }
}
