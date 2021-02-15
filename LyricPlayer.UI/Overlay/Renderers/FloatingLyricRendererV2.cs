using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;
using System;
using System.Collections.Generic;

namespace LyricPlayer.UI.Overlay.Renderers
{
    class FloatingLyricRendererV2 : FloatingLyricRenderer
    {
        public override string RendererKey => "FloatingLyricRendererV2";
        //Dictionary<Type, LyricEffectPlayerBase> EffectPlayers { set; get; }
        LyricHolder Holder = new LyricHolder { TextToDraw = "...", Duration = int.MaxValue };
        Dictionary<string, Font> Fonts { set; get; }
        Dictionary<Color, SolidBrush> Brushes { set; get; }
        //List<LyricEffect> CurrentLyricEffects { set; get; }


        public FloatingLyricRendererV2()
        {
            //EffectPlayers = Assembly.GetExecutingAssembly().GetTypes()
            //    .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(LyricEffectPlayerBase)))
            //    .ToDictionary(x => x.BaseType.GenericTypeArguments.First(),
            //    a => Activator.CreateInstance(a) as LyricEffectPlayerBase);

            Fonts = new Dictionary<string, Font>();
            Brushes = new Dictionary<Color, SolidBrush>();

            Holder.FontName = FontName;
            Holder.FontSize = MainLineFontSize;
            Holder.BackgroundColor = BackgroundColor;
            Holder.ForeColor = FontColor;
        }

        public override void LyricChanged(TrackLyric trackLyric, Lyric currentLyric)
        {
            if (currentLyric == trackLyric.Lyric[0])
                Reset();

            //Holder.TextToDraw = currentLyric.Text;
            //CurrentLyricEffects = currentLyric.Effects;

            //foreach (var player in EffectPlayers)
            //    player.Value.Initiated = false;
        }

        public override void Render(DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            //if ((CurrentLyricEffects?.Any() ?? false) && e.FrameCount > 60)
            //{
            //    foreach (var effect in CurrentLyricEffects)
            //        EffectPlayers[effect.GetType()].ApplyEffect(Holder, effect, e);
            //}

            gfx.BeginScene();
            gfx.ClearScene(Holder.BackgroundColor);

            if (!Fonts.ContainsKey(Holder.FontName))
                Fonts.Add(Holder.FontName, gfx.CreateFont(Holder.FontName, Holder.FontSize, true, wordWrapping: true));

            if (!Brushes.ContainsKey(Holder.ForeColor))
                Brushes.Add(Holder.ForeColor, gfx.CreateSolidBrush(Holder.ForeColor));

            Holder.RenderSize = gfx.MeasureString(Fonts[Holder.FontName], Holder.FontSize, Holder.TextToDraw);

            var deltaX = OverlayParent.Width - Holder.RenderSize.X;
            var deltaY = OverlayParent.Height - Holder.RenderSize.Y;

            var deltaTime = e.DeltaTime / 1000f;
            timeCounter += deltaTime * (float)Math.Pow(Trauma, 0.3f) * TraumaMult;
            //Bind the movement to the desired range
            var point = GetPoint(1, timeCounter);

            point.X *= TraumaMag * Trauma;
            point.Y *= TraumaMag * Trauma;

            Holder.CurrentLocation = new Point
            {
                X = (deltaX > 0 ? deltaX / 2 : 0) + point.X,
                Y = (deltaY > 0 ? deltaY / 2 : 0) + point.Y
            };

            //decay faster at higher values
            Trauma -= deltaTime * TraumaDecay * (Trauma + 0.3f);

            gfx.DrawText(Fonts[Holder.FontName], Holder.FontSize, Brushes[Holder.ForeColor], Holder.CurrentLocation, Holder.TextToDraw);

            var info = $"FPS:{gfx.FPS} delta:{e.DeltaTime}ms";
            gfx.DrawText(Fonts[Holder.FontName], 9.5f, Brushes[Holder.ForeColor], 0, 0, info);

            var copyrightTextSize = gfx.MeasureString(Fonts[Holder.FontName], 10, TrackLyric?.Copyright ?? "");
            var copyrightLocation = new Point
            {
                X = OverlayParent.Width > copyrightTextSize.X ? OverlayParent.Width - copyrightTextSize.X : 0,
                Y = OverlayParent.Height > copyrightTextSize.Y ? OverlayParent.Height - copyrightTextSize.Y : 0
            };
            gfx.DrawText(Fonts[Holder.FontName], 10, Brushes[Holder.ForeColor], copyrightLocation, TrackLyric?.Copyright ?? "");

            gfx.EndScene();
        }
    }
}
