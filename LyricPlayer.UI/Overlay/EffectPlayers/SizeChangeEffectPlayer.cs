using GameOverlay.Windows;
using LyricPlayer.LyricEffects;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    class SizeChangeEffectPlayer : LyricEffectPlayerBase<SizeChangeEffect>
    {
        protected override void ApplyEffect(LyricHolder holder, SizeChangeEffect effect, DrawGraphicsEventArgs drawEventArg)
        {
            if (effect.Instant || effect.SizeTo - holder.FontSize < Fixed.AlmostZero)
            {
                holder.FontSize = effect.SizeTo;
                return;
            }

            if (!Initiated)
            {
                holder.FontSize = effect.SizeFrom;
                Initiated = true;
                return;
            }

            holder.FontSize += (effect.SizeTo - effect.SizeFrom) / drawEventArg.DeltaTime;

            if (holder.FontSize > effect.SizeTo)
                holder.FontSize = effect.SizeTo;
        }
    }
}
