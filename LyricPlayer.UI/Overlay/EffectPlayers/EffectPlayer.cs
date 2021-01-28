using GameOverlay.Windows;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    internal abstract class EffectPlayer<T> where T : Elements.RenderElement
    {
        public abstract int ElementType { get; }
        public void ApplyEffect(T element, DrawGraphicsEventArgs renderArgs)
        {

            InternalApplyEffect(element, renderArgs);
        }

        protected abstract void InternalApplyEffect(T element, DrawGraphicsEventArgs renderArgs);
        protected abstract void Setup();
    }
}
