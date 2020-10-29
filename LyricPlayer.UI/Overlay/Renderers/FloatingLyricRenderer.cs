using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Models;
using System;
using System.IO;
using System.Linq;

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
        Image Image { set; get; }
        byte[] ImageContent { set; get; }
        TransformationMatrix TransformMatrix { set; get; }
        Rectangle BackgroundRenderRectangle { set; get; }
        bool BackgroundChanged { set; get; }
        public FloatingLyricRenderer()
        {
            CurrentLyricText = "...";

            FontName = "Antonio";
            FontSize = 15;
            MainLineFontSize = 38;
            FontColor = new Color(220, 220, 220, 255);
            BackgroundColor = new Color(0, 0, 0, 195);
            Noise = new FastNoise();
            Noise.SetFrequency(0.001f);
            TransformMatrix = TransformationMatrix.Identity;
        }


        public void Destroy(Graphics gfx)
        {
            MainLineFont.Dispose();
            MainLineBrush.Dispose();
        }

        public virtual void LyricChanged(TrackLyric trackLyric, Lyric currentLyric)
        {
            if (trackLyric != TrackLyric)
                if (Directory.Exists("Backgrounds"))
                {
                    var files = Directory.GetFiles("Backgrounds").Where(x => x.EndsWith(".jpg")).ToList();
                    if (files.Any())
                        ImageContent = File.ReadAllBytes(files[1/*new Random().Next(0, files.Count)*/]);

                    BackgroundRenderRectangle = CalculateBackgroundImageLocation();
                    Reset();
                    BackgroundChanged = true;
                }

            TrackLyric = trackLyric;

            CurrentLyricText = currentLyric.Text;
        }

        public virtual void Render(DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            if (BackgroundChanged)
            {
                Image?.Dispose();
                Image = ImageContent != null ? gfx.CreateImage(ImageContent) : null;

                using (System.Drawing.Bitmap bitmap = System.Drawing.Bitmap.FromStream(new MemoryStream(ImageContent)) as System.Drawing.Bitmap)
                    BackgroundColor = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2).ToOverlayColor(BackgroundColor.A);

                BackgroundChanged = false;
            }


            gfx.BeginScene();
            gfx.ClearScene(BackgroundColor);

            if (Image != null)
            {
                TransformMatrix = TransformationMatrix.Rotation(Noise.GetPerlin(timeCounter, timeCounter / 3) / 24, new Point { X = OverlayParent.Width / 2, Y = OverlayParent.Height / 2 });

                gfx.TransformStart(TransformMatrix);

                gfx.DrawImage(Image, BackgroundRenderRectangle, BackgroundColor.A, true);
                gfx.TransformEnd();
            }

            TransformMatrix = TransformationMatrix.Rotation(Noise.GetCubic(timeCounter, 1) / 6, new Point { X = OverlayParent.Width / 2, Y = OverlayParent.Height / 2 });
            gfx.TransformStart(TransformMatrix);
            MainLineSize = gfx.MeasureString(MainLineFont, CurrentLyricText);

            var deltaX = OverlayParent.Width - MainLineSize.X;
            var deltaY = OverlayParent.Height - MainLineSize.Y;

            var deltaTime = e.DeltaTime / 1000f;
            timeCounter += deltaTime * (float)Math.Pow(Trauma, 0.3f) * TraumaMult * Program.Mulitplier;
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

            gfx.TransformEnd();

            var info = $"FPS:{gfx.FPS} delta:{e.DeltaTime}ms";
            gfx.DrawText(MainLineFont, 9.5f, MainLineBrush, 0, 0, info);


            var copyrightTextSize = gfx.MeasureString(MainLineFont, 10, TrackLyric?.Copyright ?? "");
            var copyrightLocation = new Point
            {
                //X = OverlayParent.Width > copyrightTextSize.X ? OverlayParent.Width - copyrightTextSize.X : 0,
                Y = OverlayParent.Height > copyrightTextSize.Y ? OverlayParent.Height - copyrightTextSize.Y : 0
            };
            gfx.DrawText(MainLineFont, 10, MainLineBrush, copyrightLocation, TrackLyric?.Copyright ?? "");

            OverlayParent.IsTopmost = true;
            gfx.EndScene();
        }

        public void Setup(Graphics gfx)
        {
            MainLineFont = gfx.CreateFont(FontName, MainLineFontSize, true, wordWrapping: true);
            MainLineBrush = gfx.CreateSolidBrush(FontColor);
            gfx.TextAntiAliasing = true;
            gfx.PerPrimitiveAntiAliasing = true;
            BackgroundRenderRectangle = CalculateBackgroundImageLocation();
        }

        protected void Reset()
        {
            var maxSize = (float)Math.Sqrt((OverlayParent.Width * OverlayParent.Height)) * 3;

            Trauma = 1;
            TraumaMult = maxSize / 10f; //the power of movement
            TraumaMag = maxSize / 27f; //the range of movment
            TraumaDecay = 0.000000000001f;
            timeCounter = 0;
        }

        protected Point GetPoint(float time, float seed)
        {
            var f1 = Noise.GetPerlin(seed, seed - time);
            var f2 = Noise.GetCubic(seed + 3, seed + time) / 1.5f;
            return new Point
            {
                X = f1,
                Y = f2
            };
        }

        protected Rectangle CalculateBackgroundImageLocation()
        {
            var result = new Rectangle();

            //if (Image.Width > OverlayParent.Width * 1.2f)
            //{
            //    result.Left = -Image.Width * 0.1f;
            //    result.Right = OverlayParent.Width * 1.1f;
            //}
            //else
            //{
            result.Left = 0;
            result.Right = OverlayParent.Width;
            //}

            //if (Image.Height > OverlayParent.Height * 1.2f)
            //{
            //    result.Top = -Image.Height * 0.1f;
            //    result.Bottom = OverlayParent.Height * 1.1f;
            //}
            //else
            //{
            result.Top = -OverlayParent.Height * 0.15f;
            result.Bottom = OverlayParent.Height * 1.15f;
            //}
            return result;
        }


    }


}
