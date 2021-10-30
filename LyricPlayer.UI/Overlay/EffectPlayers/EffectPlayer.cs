using GameOverlay.Windows;
using LyricPlayer.Model.Effects;
using LyricPlayer.Model.Elements;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    internal abstract class EffectPlayer
    {
        public void ApplyEffect<T>(RenderElement element, T effect, DrawGraphicsEventArgs renderArgs) where T : ElementEffect
        { InternalApplyEffect(element, effect, renderArgs); }
        protected abstract void InternalApplyEffect(RenderElement element, ElementEffect effect, DrawGraphicsEventArgs renderArgs);
    }

    internal abstract class EffectPlayer<T> : EffectPlayer where T : ElementEffect
    {
        protected override sealed void InternalApplyEffect(RenderElement element, ElementEffect effect, DrawGraphicsEventArgs renderArgs)
        { InternalApplyEffect(element, (T)effect, renderArgs); }

        protected abstract void InternalApplyEffect(RenderElement element, T effect, DrawGraphicsEventArgs renderArgs);
        protected abstract void Setup();
    }
}
