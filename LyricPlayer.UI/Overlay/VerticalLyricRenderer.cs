using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LyricPlayer.UI.Overlay
{
    class VerticalLyricRenderer : SimpleLyricRenderer
    {
        protected override List<Text> DisplayingLyric
        {
            set => TextHandler.Texts = value;
            get => TextHandler?.Texts;
        }

        TextHandler TextHandler = new TextHandler();

        public VerticalLyricRenderer()
        {
            InterLineSpace = 8;
            DisplayingLyricLinesCount = 5;
            FontName = "Times New Roman";
            FontSize = 15;
            MainLineFontSize = 21;
            FontColor = new Color(220, 220, 220, 255);
            BackgroundColor = new Color(0, 0, 0, 200);
            InfoLocation = new Point(0, 0);
        }

        public override void LyricChanged(TrackLyric trackLyric, Lyric currentLyric)
        {
            TrackLyric = trackLyric;
            var currnetLyricIndex = trackLyric.Lyric.IndexOf(currentLyric);
            if (currnetLyricIndex == -1)
                return;

            var halfSize = DisplayingLyric.Count / 2;
            var skipCount = currnetLyricIndex - halfSize;
            List<Lyric> fakeLyric = new List<Lyric>();

            if (skipCount < 0)
            {
                fakeLyric = Enumerable.Range(0, Math.Abs(skipCount))
                    .Select(x => new Lyric { Text = "..." }).ToList();
                skipCount = 0;
            }

            var displayingLyric = fakeLyric.Concat(TrackLyric.Lyric).Skip(skipCount)
                 .Take(DisplayingLyricLinesCount + 1).ToList();

            var lowOnLines = DisplayingLyricLinesCount - displayingLyric.Count - 1;

            if (lowOnLines > 0)
                displayingLyric.AddRange(Enumerable.Range(0, lowOnLines).Select(x => new Lyric { Text = "..." }));

         

            TextHandler.MoveUp();
            if (displayingLyric.Count < DisplayingLyricLinesCount)
                return;

            for (int index = 0; index < DisplayingLyricLinesCount; index++)
                DisplayingLyric[index].TextToDraw = displayingLyric[index].Text;
        }

        public override void Render(DrawGraphicsEventArgs e)
        {
            if (TrackLyric == null)
                return;

            var gfx = e.Graphics;
            var infoText = $"FPS:{gfx.FPS} Delta:{e.DeltaTime}ms";
            var textSize = gfx.MeasureString(InfoLineFont, infoText);
            InfoSize = textSize;

            gfx.ClearScene(BackgroundColor);
            gfx.BeginScene();

            Point currentLocation = new Point(0,1);

            for (int index = 0; index < DisplayingLyricLinesCount; index++)
            {
                DisplayingLyric[index].RenderSize =
                    gfx.MeasureString(index == (DisplayingLyricLinesCount-2) / 2 ? MainLineFont : TextFont,
                    DisplayingLyric[index].TextToDraw);

                if (!DisplayingLyric[index].LocationSet)
                {
                    DisplayingLyric[index].CurrentLocation = new Point(0, currentLocation.Y);
                    DisplayingLyric[index].DestinationLocation = new Point(0, int.MaxValue);
                    DisplayingLyric[index].LocationSet = true;
                }

                if (DisplayingLyric[index].CurrentLocation.Y < Fixed.AlmostZero)
                {
                    DisplayingLyric.RemoveAt(index);
                    index--;
                    continue;
                }

                if (DisplayingLyric[index].CurrentLocation.Y > DisplayingLyric[index].DestinationLocation.Y)
                    DisplayingLyric[index].CurrentLocation = new Point(0, DisplayingLyric[index].CurrentLocation.Y - 5);

                gfx.DrawText(index == (DisplayingLyricLinesCount-2) / 2
                    ? MainLineFont : TextFont, TextBrush,
                    DisplayingLyric[index].CurrentLocation,
                    DisplayingLyric[index].TextToDraw);

                currentLocation.Y += InterLineSpace + DisplayingLyric[index].RenderSize.Y;
            }



            gfx.DrawText(InfoLineFont, InfoBrush, InfoLocation, infoText);
            gfx.EndScene();
        }
    }

    class TextHandler
    {
        public List<Text> Texts { set; get; }

        public void MoveUp()
        {
            if (!Texts.Any())
                return;

            Texts[0].DestinationLocation = new Point(0, 0);

            Texts.Add(new Text
            {
                TextToDraw = "",
                CurrentLocation = Texts?.LastOrDefault()?.CurrentLocation ?? new Point(0, 0),
                LocationSet = Texts?.LastOrDefault()?.LocationSet ?? false
            });

            for (int index = 1; index < (Texts?.Count ?? 0); index++)
                Texts[index].DestinationLocation = Texts[index - 1].CurrentLocation;
        }
    }
}
