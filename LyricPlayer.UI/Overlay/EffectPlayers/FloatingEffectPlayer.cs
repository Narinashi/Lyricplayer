using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model.Elements;
using LyricPlayer.UI.Overlay.Elements;
using System;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    internal class FloatingEffectPlayer : EffectPlayer<RenderElement>
    {
        private FastNoise Noise { set; get; }
      
        protected float timeCounter;

        protected override void InternalApplyEffect(RenderElement element, DrawGraphicsEventArgs renderArgs)
        {
            var deltaTime = renderArgs.DeltaTime / 1000f;
            var point = GetPoint(1, timeCounter);

            timeCounter += deltaTime * (float)Math.Pow(Trauma, 0.3f) * TraumaMult;

            point.X *= TraumaMag * Trauma;
            point.Y *= TraumaMag * Trauma;

            element.Rotation += Noise.GetCubic(timeCounter, 1) / 10;
            element.Location = new Point
            {
                X = element.Location.X + point.X,
                Y = element.Location.Y + point.Y
            };

            Trauma -= deltaTime * TraumaDecay * (Trauma + 0.3f);
        }

        protected override void Setup()
        {
            Noise = new FastNoise();
            Noise.SetFrequency(0.001f);
            Reset();
        }

        protected Point GetPoint(float time, float seed)
        {
            var f1 = Noise.GetPerlin(seed, seed - time);
            var f2 = Noise.GetCubic(seed, seed + time);
            return new Point
            {
                X = f1,
                Y = f2
            };
        }

        protected void Reset()
        {
            Trauma = 1;
            TraumaMult = 12; //the power of movement
            TraumaMag = 4; //the range of movment
            TraumaDecay = 0.000000000001f;
            timeCounter = 0;
        }
    }
}
