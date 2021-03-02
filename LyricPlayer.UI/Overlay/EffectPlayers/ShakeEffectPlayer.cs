using GameOverlay.Windows;
using LyricPlayer.Model;
using LyricPlayer.Model.Effects;
using LyricPlayer.Model.Elements;
using System;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    internal class ShakeEffectPlayer : EffectPlayer<ShakeEffect>
    {
        private FastNoise Noise { set; get; }
        public ShakeEffectPlayer()
        {
            Noise = new FastNoise(/*new Random().Next(int.MaxValue)*/);
        }

        protected override void InternalApplyEffect(RenderElement element, ShakeEffect effect, DrawGraphicsEventArgs renderArgs)
        {
            if (element.Rotation == null)
                element.Rotation = new RotationInfo();

            var deltaTime = renderArgs.DeltaTime / 1000f;
            var point = GetPoint(1, effect.TimeCounter);

            effect.TimeCounter += deltaTime * (float)Math.Pow(effect.Trauma, 0.7f) * effect.TraumaMult;

            point.X *= effect.TraumaMag * effect.Trauma;
            point.Y *= effect.TraumaMag * effect.Trauma;

            element.Rotation.Rotation = Noise.GetCubic(effect.TimeCounter, 1) / 6;
            element.Location = new FloatPoint
            {
                X = (element.Location.X + point.X),
                Y = (element.Location.Y + point.Y)
            };

            effect.Trauma -= deltaTime * effect.TraumaDecay * (effect.Trauma + 0.7f);
        }


        protected FloatPoint GetPoint(float time, float seed)
        {
            var f1 = Noise.GetCubic(seed, seed - time);
            var f2 = Noise.GetPerlin(seed, seed + time);
            return new FloatPoint
            {
                X = f1,
                Y = f2
            };
        }

        protected override void Setup() { }
    }
}
