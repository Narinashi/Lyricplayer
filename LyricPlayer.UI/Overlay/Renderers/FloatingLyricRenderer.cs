using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Models;
using System;

namespace LyricPlayer.UI.Overlay.Renderers
{
    class FloatingLyricRenderer : ILyricOverlayRenderer
    {
        public string FontName { set; get; }
        public float FontSize { set; get; }
        public float MainLineFontSize { set; get; }
        public int DisplayingLyricLinesCount { set { } get => 1; }
        public int InterLineSpace { set { } get => 0; }
        public GraphicsWindow OverlayParent { set; get; }
        public Color FontColor { set; get; }
        public Color BackgroundColor { set; get; }

        public float Trauma { set; get; }
        public float TraumaMult { set; get; }
        public float TraumaMag { set; get; }
        public float TraumaDecay { set; get; }

        protected float timeCounter;
        Font MainLineFont { set; get; }
        SolidBrush MainLineBrush { set; get; }
        Point MainLineSize { set; get; }
        Point MainLineLocation { set; get; }
        FastNoise Noise { set; get; }
        string CurrentLyricText { set; get; }
        protected TrackLyric TrackLyric { set; get; }

        public FloatingLyricRenderer()
        {
            CurrentLyricText = "...";

            FontName = "Antonio";
            FontSize = 15;
            MainLineFontSize = 24;
            FontColor = new Color(220, 220, 220, 255);
            BackgroundColor = new Color(0, 0, 0, 120);
            Noise = new FastNoise();
            Noise.SetFrequency(0.001f);
        }


        public void Destroy(Graphics gfx)
        {
            MainLineFont.Dispose();
            MainLineBrush.Dispose();
        }

        public virtual void LyricChanged(TrackLyric trackLyric, Lyric currentLyric)
        {
            TrackLyric = trackLyric;
            if (currentLyric == trackLyric.Lyric[0])
                Reset();

            CurrentLyricText = currentLyric.Text;
        }

        public virtual void Render(DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            gfx.BeginScene();
            gfx.ClearScene(BackgroundColor);

            MainLineSize = gfx.MeasureString(MainLineFont, CurrentLyricText);

            var deltaX = OverlayParent.Width - MainLineSize.X;
            var deltaY = OverlayParent.Height - MainLineSize.Y;

            var deltaTime = e.DeltaTime / 1000f;
            timeCounter += deltaTime * (float)Math.Pow(Trauma, 0.3f) * TraumaMult;
            //Bind the movement to the desired range
            var point = GetPoint(1, timeCounter);

            point.X *= TraumaMag * Trauma;
            point.Y *= TraumaMag * Trauma;

            MainLineLocation = new Point
            {
                X = (deltaX > 0 ? deltaX / 2 : 0) + point.X,
                Y = (deltaY > 0 ? deltaY / 2 : 0) + point.Y
            };

            //decay faster at higher values
            Trauma -= deltaTime * TraumaDecay * (Trauma + 0.3f);

            gfx.DrawText(MainLineFont, MainLineBrush, MainLineLocation, CurrentLyricText);
            var info = $"FPS:{gfx.FPS} delta:{e.DeltaTime}ms";
            gfx.DrawText(MainLineFont, 9.5f, MainLineBrush, 0, 0, info);


            var copyrightTextSize = gfx.MeasureString(MainLineFont,10, TrackLyric?.Copyright??"");
            var copyrightLocation = new Point
            {
                X = OverlayParent.Width > copyrightTextSize.X ? OverlayParent.Width - copyrightTextSize.X : 0,
                Y = OverlayParent.Height > copyrightTextSize.Y ? OverlayParent.Height - copyrightTextSize.Y : 0
            };
            gfx.DrawText(MainLineFont, 10, MainLineBrush, copyrightLocation, TrackLyric?.Copyright??"");


            gfx.EndScene();
        }

        public void Setup(Graphics gfx)
        {
            MainLineFont = gfx.CreateFont(FontName, MainLineFontSize, true, wordWrapping: true);
            MainLineBrush = gfx.CreateSolidBrush(FontColor);
        }

        protected void Reset()
        {
            var maxSize = (float)Math.Sqrt((OverlayParent.Width * OverlayParent.Height));

            Trauma = 1;
            TraumaMult = maxSize / 20f; //the power of movement
            TraumaMag = maxSize / 21f; //the range of movment
            TraumaDecay = 0.000000000001f;
            timeCounter = 0;
        }

        protected Point GetPoint(float time, float seed)
        {
            var f1 = Noise.GetPerlin(seed, time);
            var f2 = Noise.GetPerlin(seed + 3, time)/10;
            return new Point
            {
                X = f1,
                Y = f2
            };
        }
    }
}
