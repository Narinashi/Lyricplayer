using GameOverlay.Windows;
using LyricPlayer.LyricEffects;
using System.Collections.Generic;
using System.Linq;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    class DriftEffectPlayer : LyricEffectPlayerBase<DriftEffect>
    {
        protected override void ApplyEffect(LyricHolder holder, DriftEffect effect, DrawGraphicsEventArgs drawEventArg)
        {
            if (effect.Instant)
            {
                holder.CurrentLocation = effect.LocationTo.ToOverlayPoint();
                return;
            }
           
            if (!Initiated)
            {
                holder.CurrentLocation = effect.LocationFrom.ToOverlayPoint();
                Initiated = true;
            }

            holder.CurrentLocation = new GameOverlay.Drawing.Point
            {
                X = holder.CurrentLocation.X +
                (effect.LocationTo.X - effect.LocationFrom.X) / drawEventArg.DeltaTime,
                Y = holder.CurrentLocation.Y +
                 (effect.LocationTo.Y - effect.LocationFrom.Y) / drawEventArg.DeltaTime
            };

            var XDescending = effect.LocationFrom.X > effect.LocationTo.X;
            var YDescending = effect.LocationFrom.Y > effect.LocationTo.Y;

            if ((XDescending && holder.CurrentLocation.X < effect.LocationTo.X) ||
                !XDescending && holder.CurrentLocation.X > effect.LocationTo.X)
                holder.CurrentLocation = new GameOverlay.Drawing.Point
                {
                    Y = holder.CurrentLocation.Y,
                    X = effect.LocationTo.X
                };

            if ((YDescending && holder.CurrentLocation.Y < effect.LocationTo.Y) ||
                (!YDescending && holder.CurrentLocation.Y > effect.LocationTo.Y))
                holder.CurrentLocation = new GameOverlay.Drawing.Point
                {
                    Y = effect.LocationTo.Y,
                    X = holder.CurrentLocation.X,
                };
        }
    }
}
