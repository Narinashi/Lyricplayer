using GameOverlay.Windows;
using LyricPlayer.LyricEffects;
using System;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    class ColorChangeEffectPlayer : LyricEffectPlayerBase<ColorChangeEffect>
    {
        private ColorDelta ForeColorDelta { set; get; }
        private ColorDelta BackgroundColorDelta { set; get; }

        protected override void ApplyEffect(LyricHolder holder, ColorChangeEffect effect, DrawGraphicsEventArgs drawEventArg)
        {
            if (effect.Instant)
            {
                holder.BackgroundColor = effect.ForeColorChangeFrom.ToOverlayColor();
                holder.ForeColor = effect.ForeColorChangeFrom.ToOverlayColor();
                return;
            }

            if (!Initiated)
            {
                InitDeltas(effect, drawEventArg);
                holder.BackgroundColor = effect.BackgroundColorChangeFrom.ToOverlayColor();
                holder.ForeColor = effect.ForeColorChangeFrom.ToOverlayColor();
                Initiated = true;
            }

            holder.BackgroundColor = holder.BackgroundColor.Add(BackgroundColorDelta);
            holder.ForeColor = holder.ForeColor.Add(ForeColorDelta);
        }

        private void InitDeltas(ColorChangeEffect effect, DrawGraphicsEventArgs drawEventArg)
        {
            ForeColorDelta = new ColorDelta
            {
                A = (effect.ForeColorChangeTo.A - effect.ForeColorChangeFrom.A) / (2.55f * effect.Duration /** (drawEventArg.Graphics.FPS==0 ? 1 : drawEventArg.Graphics.FPS)*/),
                B = (effect.ForeColorChangeTo.B - effect.ForeColorChangeFrom.B) / (2.55f * effect.Duration /** (drawEventArg.Graphics.FPS==0 ? 1 : drawEventArg.Graphics.FPS)*/),
                R = (effect.ForeColorChangeTo.R - effect.ForeColorChangeFrom.R) / (2.55f * effect.Duration /** (drawEventArg.Graphics.FPS==0 ? 1 : drawEventArg.Graphics.FPS)*/),
                G = (effect.ForeColorChangeTo.G - effect.ForeColorChangeFrom.G) / (2.55f * effect.Duration /** (drawEventArg.Graphics.FPS==0 ? 1 : drawEventArg.Graphics.FPS)*/),
            };
            BackgroundColorDelta = new ColorDelta
            {
                A = (effect.BackgroundColorChangeTo.A - effect.BackgroundColorChangeFrom.A) / (2.55f * effect.Duration /** (drawEventArg.Graphics.FPS==0 ? 1 : drawEventArg.Graphics.FPS)*/),
                B = (effect.BackgroundColorChangeTo.B - effect.BackgroundColorChangeFrom.B) / (2.55f * effect.Duration /** (drawEventArg.Graphics.FPS==0 ? 1 : drawEventArg.Graphics.FPS)*/),
                R = (effect.BackgroundColorChangeTo.R - effect.BackgroundColorChangeFrom.R) / (2.55f * effect.Duration /** (drawEventArg.Graphics.FPS==0 ? 1 : drawEventArg.Graphics.FPS)*/),
                G = (effect.BackgroundColorChangeTo.G - effect.BackgroundColorChangeFrom.G) / (2.55f * effect.Duration /** (drawEventArg.Graphics.FPS==0 ? 1 : drawEventArg.Graphics.FPS)*/),
            };
        }
    }
    struct ColorDelta
    {
        public float A;
        public float R;
        public float B;
        public float G;
        public override string ToString()
        {
            return $"R:{R} G:{G} B:{B} A:{A}";
        }
    }
}
