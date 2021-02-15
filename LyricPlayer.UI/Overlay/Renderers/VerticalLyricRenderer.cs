using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LyricPlayer.UI.Overlay.Renderers
{
    class VerticalLyricRenderer : SimpleLyricRenderer
    {
        public override String RendererKey => "VerticalLyricRenderer";
        protected override List<LyricHolder> DisplayingLyric
        {
            set => TextHandler.Texts = value;
            get => TextHandler?.Texts;
        }

        TextHandler TextHandler = new TextHandler();
        public bool UpToDown { set; get; }
        public VerticalLyricRenderer()
        {
            InterLineSpace = 8;
            DisplayingLyricLinesCount = 5;
            FontName = "Antonio";
            FontSize = 15;
            MainLineFontSize = 21;
            FontColor = new Color(220, 220, 220, 255);
            BackgroundColor = new Color(0, 0, 0, 200);
            InfoLocation = new Point(0, 0);
        }

        public override void LyricChanged(TrackLyric trackLyric, Lyric currentLyric)
        {
            if (trackLyric != TrackLyric)
                DisplayingLyricLinesCount = DisplayingLyricLinesCount;

            TrackLyric = trackLyric;
            var currnetLyricIndex = trackLyric.Lyric.IndexOf(currentLyric);
            if (currnetLyricIndex == -1)
                return;

            var halfSize = DisplayingLyric.Count / 2;
            var skipCount = currnetLyricIndex - halfSize;
            List<Lyric> fakeLyric = new List<Lyric>();

            //if (skipCount < 0)
            //{
            //    fakeLyric = Enumerable.Range(0, Math.Abs(skipCount))
            //        .Select(x => new Lyric { Text = "..." }).ToList();
            //    skipCount = 0;
            //}

            var displayingLyric = fakeLyric.Concat(TrackLyric.Lyric).Skip(skipCount)
                 .Take(DisplayingLyricLinesCount).ToList();

            var lowOnLines = DisplayingLyricLinesCount - displayingLyric.Count +1;

            //if (lowOnLines > 0)
            //    displayingLyric.AddRange(Enumerable.Range(0, lowOnLines).Select(x => new Lyric { Text = "..." }));

          
            //if (displayingLyric.Count < DisplayingLyricLinesCount)
            //    return;

            //TextHandler.MoveUp();
            //for (int index = 0; index < DisplayingLyricLinesCount; index++)
            //{
            //    DisplayingLyric[index].TextToDraw = displayingLyric[index].Text;
            //    DisplayingLyric[index].IsCurrent = displayingLyric[index] == currentLyric;
            //}
        }

        public override void Render(DrawGraphicsEventArgs e)
        {
            if (TrackLyric == null)
                return;

            var gfx = e.Graphics;
            var infoText = $"FPS:{gfx.FPS} Delta:{e.DeltaTime}ms";
            InfoSize = gfx.MeasureString(InfoLineFont, infoText);

            gfx.ClearScene(BackgroundColor);
            gfx.BeginScene();

            Point currentLocation = InfoSize;

            for (int index = 0; index < DisplayingLyricLinesCount; index++)
            {
                if (!DisplayingLyric[index].LocationSet)
                {
                    DisplayingLyric[index].CurrentLocation = new Point(0, currentLocation.Y);
                    DisplayingLyric[index].DestinationLocation = new Point(0, int.MaxValue);
                    DisplayingLyric[index].LocationSet = true;
                }

                if (index > 0)
                {
                    DisplayingLyric[index].CurrentLocation = new Point(0, DisplayingLyric[index - 1].RenderSize.Y + DisplayingLyric[index - 1].CurrentLocation.Y + InterLineSpace);

                    if (DisplayingLyric[index].DestinationLocation.Y < InfoLocation.Y)
                        DisplayingLyric[index].DestinationLocation = new Point(0, InfoLocation.Y + 1);
                    if (DisplayingLyric[index].CurrentLocation.Y < InfoLocation.Y)
                        DisplayingLyric[index].CurrentLocation = new Point(0, InfoLocation.Y + 1);
                }

                if (DisplayingLyric[0].CurrentLocation.Y < InfoSize.Y)
                {
                    DisplayingLyric.RemoveAt(0);
                    index--;
                    continue;
                }

                if (DisplayingLyric[index].CurrentLocation.Y > DisplayingLyric[index].DestinationLocation.Y)
                    DisplayingLyric[index].CurrentLocation = new Point(DisplayingLyric[index].CurrentLocation.X, DisplayingLyric[index].CurrentLocation.Y - 2f * gfx.FPS / 60);

                DisplayingLyric[index].RenderSize =
                  gfx.MeasureString(DisplayingLyric[index].IsCurrent ? MainLineFont : TextFont,
                  DisplayingLyric[index].TextToDraw);

                gfx.DrawText(DisplayingLyric[index].IsCurrent ? MainLineFont : TextFont,
                    TextBrush,
                   DisplayingLyric[index].CurrentLocation,
                    DisplayingLyric[index].TextToDraw);

                currentLocation.Y += InterLineSpace + DisplayingLyric[index].RenderSize.Y;
            }

            gfx.DrawText(InfoLineFont, InfoBrush, InfoLocation, infoText);

            var copyrightTextSize = gfx.MeasureString(InfoLineFont, 10, TrackLyric?.Copyright??"");
            var copyrightLocation = new Point
            {
                X = OverlayParent.Width > copyrightTextSize.X ? OverlayParent.Width - copyrightTextSize.X : 0,
                Y = OverlayParent.Height > copyrightTextSize.Y ? OverlayParent.Height - copyrightTextSize.Y : 0
            };
            gfx.DrawText(MainLineFont, InfoLineFont.FontSize, TextBrush, copyrightLocation, TrackLyric?.Copyright??"");

            gfx.EndScene();
        }
    }

    class TextHandler
    {
        public List<LyricHolder> Texts { set; get; }

        public void MoveUp()
        {
            if (!Texts.Any())
                return;

            Texts[0].DestinationLocation = new Point(0, 0);

            Texts.Add(new LyricHolder
            {
                TextToDraw = "",
                CurrentLocation = Texts?.LastOrDefault()?.CurrentLocation ?? new Point(0, int.MaxValue),
                LocationSet = Texts?.LastOrDefault()?.LocationSet ?? false
            });

            for (int index = 1; index < (Texts?.Count ?? 0); index++)
            { Texts[index].DestinationLocation = Texts[index - 1].CurrentLocation.Y > 1 ? Texts[index - 1].CurrentLocation : new Point(0, 1);}
        }

        public int CurrentLineIndex
        {
            get
            {
                for (var i = 0; i < Texts.Count; i++)
                    if (Texts[i].IsCurrent)
                        return i;
                return -1;
            }
        }
    }
}
