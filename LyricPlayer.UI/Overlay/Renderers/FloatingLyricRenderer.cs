using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LyricPlayer.UI.Overlay.Renderers
{
    class FloatingLyricRenderer : ILyricOverlayRenderer
    {
        public virtual string RendererKey => "FloatingLyricRenderer";
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
        Font MeasurementFont { set; get; }
        SolidBrush MainLineBrush { set; get; }
        Point MainLineSize { set; get; }
        Point MainLineLocation { set; get; }
        FastNoise Noise { set; get; }
        List<string> CurrentLyricText { set; get; }
        protected TrackLyric TrackLyric { set; get; }
        Image Image { set; get; }

        System.Drawing.Bitmap RedChannel { set; get; }
        System.Drawing.Bitmap BlueChannel { set; get; }
        System.Drawing.Bitmap GreenChannel { set; get; }

        byte[] ImageContent { set; get; }
        TransformationMatrix TransformMatrix { set; get; }
        Rectangle BackgroundRenderRectangle { set; get; }
        bool BackgroundChanged { set; get; }
        public FloatingLyricRenderer()
        {
            CurrentLyricText = new List<string> { "..." };

            FontName = "Antonio";
            FontSize = 15;
            MainLineFontSize = 50;
            //FontColor = new Color(255, 255, 255, 255);
            FontColor = new Color(240, 237, 255);
            BackgroundColor = new Color(0, 0, 0, 200);
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

            CurrentLyricText.Clear();
            CurrentLyricText.Add(currentLyric.Text);
        }

        public virtual void Render(DrawGraphicsEventArgs e)
        {
            var gfx = e.Graphics;
            if (!(CurrentLyricText?.Any() ?? false))
                return;

            if (BackgroundChanged && ImageContent != null)
            {
                Image?.Dispose();
                Image = ImageContent != null ? gfx.CreateImage(ImageContent) : null;

                using (System.Drawing.Bitmap bitmap = System.Drawing.Bitmap.FromStream(new MemoryStream(ImageContent)) as System.Drawing.Bitmap)
                    BackgroundColor = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2).ToOverlayColor(BackgroundColor.A);

                BackgroundChanged = false;
            }

            gfx.BeginScene();
            gfx.ClearScene(BackgroundColor);

            var point = GetPoint(1, timeCounter);

            if (Image != null)
            {
                TransformMatrix = TransformationMatrix.Rotation(Noise.GetPerlin(timeCounter, timeCounter / 8) / 40, new Point { X = OverlayParent.Width / 2, Y = OverlayParent.Height / 2 });

                gfx.TransformStart(TransformMatrix);

                gfx.DrawImage(Image,
                    BackgroundRenderRectangle.Left - point.X * Program.Mulitplier * 2,
                    BackgroundRenderRectangle.Top - point.Y * Program.Mulitplier * 2,
                    BackgroundRenderRectangle.Right - point.X * Program.Mulitplier * 2,
                    BackgroundRenderRectangle.Bottom - point.Y * Program.Mulitplier * 2,
                    BackgroundColor.A, true);
                gfx.TransformEnd();
            }

            TransformMatrix = TransformationMatrix.Rotation(Noise.GetCubic(timeCounter, 1) / 10, new Point { X = OverlayParent.Width / 2, Y = OverlayParent.Height / 2 });
            gfx.TransformStart(TransformMatrix);

            for (int index = 0; index < CurrentLyricText.Count; index++)
            {
                var line = CurrentLyricText[index];
                var size = gfx.MeasureString(MainLineFont, line);

                if (size.X > gfx.Width * 0.9f)
                {
                    var words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var stringToAdd = string.Empty;

                    for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
                        if (gfx.MeasureString(MainLineFont, stringToAdd + words[wordIndex] + " ").X > gfx.Width * 0.85f)
                        {
                            if (string.IsNullOrEmpty(stringToAdd))
                            {
                                CurrentLyricText.Insert(index, words[wordIndex]);
                                stringToAdd = string.Empty;
                                index++;
                            }
                            else
                            {
                                CurrentLyricText.Insert(index, stringToAdd);
                                stringToAdd = words[wordIndex] + " ";
                                index++;
                            }
                        }
                        else stringToAdd += words[wordIndex] + " ";

                    if (!string.IsNullOrEmpty(stringToAdd))
                        CurrentLyricText.Add(stringToAdd);

                    CurrentLyricText.Remove(line);
                }
            }

            if (!(CurrentLyricText?.Any() ?? false))
            {
                gfx.EndScene();
                return;
            }

            var height = CurrentLyricText.ToList().Sum(x => gfx.MeasureString(MainLineFont, x??"").Y);
            MainLineSize = new Point
            {
                X = gfx.MeasureString(MainLineFont, CurrentLyricText[0]).X,
                Y = height
            };

            var deltaX = OverlayParent.Width - MainLineSize.X;
            var deltaY = OverlayParent.Height - MainLineSize.Y;

            var deltaTime = e.DeltaTime / 1000f;
            timeCounter += deltaTime * (float)Math.Pow(Trauma, 0.3f) * TraumaMult * Program.Mulitplier;
            //Bind the movement to the desired range

            point.X *= TraumaMag * Trauma;
            point.Y *= TraumaMag * Trauma;

            MainLineLocation = new Point
            {
                X = (deltaX > 0 ? deltaX / 2 : deltaX / 2) + point.X,
                Y = (deltaY > 0 ? deltaY / 2 : deltaY / 2) + point.Y
            };

            //decay faster at higher values
            Trauma -= deltaTime * TraumaDecay * (Trauma + 0.3f);

            foreach (var line in CurrentLyricText.ToList())
            {
                gfx.DrawText(MainLineFont, MainLineBrush, MainLineLocation, line);
                MainLineLocation = new Point
                {
                    X = MainLineLocation.X,
                    Y = MainLineLocation.Y + gfx.MeasureString(MainLineFont, line).Y
                };
            }

            gfx.TransformEnd();

           // var info = $"FPS:{gfx.FPS} delta:{e.DeltaTime}ms";
           // gfx.DrawText(MainLineFont, 9.5f, MainLineBrush, 0, 0, info);


            var copyrightTextSize = gfx.MeasureString(MainLineFont, 10, TrackLyric?.Copyright ?? "");
            var copyrightLocation = new Point
            {
                //X = OverlayParent.Width > copyrightTextSize.X ? OverlayParent.Width - copyrightTextSize.X : 0,
                Y = OverlayParent.Height > copyrightTextSize.Y ? OverlayParent.Height - copyrightTextSize.Y : 0
            };
            gfx.DrawText(MainLineFont, 10, MainLineBrush, copyrightLocation, TrackLyric?.Copyright ?? "");

            gfx.EndScene();
        }

        public void Setup(Graphics gfx)
        {
            MainLineFont = gfx.CreateFont(FontName, MainLineFontSize, true, wordWrapping: false);
            MainLineBrush = gfx.CreateSolidBrush(FontColor);
            gfx.TextAntiAliasing = true;
            gfx.PerPrimitiveAntiAliasing = true;
            BackgroundRenderRectangle = CalculateBackgroundImageLocation();
        }

        protected void Reset()
        {
            var maxSize = (float)Math.Sqrt((OverlayParent.Width * OverlayParent.Height)) * 2f;

            Trauma = 1;
            TraumaMult = maxSize / 6f; //the power of movement
            TraumaMag = maxSize / 18f; //the range of movment
            TraumaDecay = 0.000000000001f;
            timeCounter = 0;
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
