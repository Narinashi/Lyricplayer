using GameOverlay.Windows;
using LyricPlayer.Model.Effects;
using LyricPlayer.Model.Elements;
using System;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    internal class SpinEffectPlayer : EffectPlayer<SpinEffect>
    {
        private FastNoise Noise { set; get; }
        public SpinEffectPlayer()
        {
            Noise = new FastNoise(/*new Random().Next(int.MaxValue)*/);
        }

        protected override void InternalApplyEffect(RenderElement element, SpinEffect effect, DrawGraphicsEventArgs renderArgs)
        {
            if (element.Rotation == null)
                element.Rotation = new RotationInfo();

            var deltaTime = renderArgs.DeltaTime / 1000f;

            var randomFloat =//Math.Abs(
                Noise.GetNoise(renderArgs.DeltaTime, renderArgs.FrameCount) / 2.5f - 
                Noise.GetCubic(renderArgs.FrameCount, renderArgs.DeltaTime) / 3.5f;
                //);
            element.Rotation.Rotation += randomFloat * effect.RotationSpeed;

            effect.Trauma -= deltaTime * effect.TraumaDecay * (effect.Trauma + 0.7f);
        }

        protected override void Setup() { }
    }
}
