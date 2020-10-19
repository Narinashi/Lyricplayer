using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LyricPlayer.UI.Overlay
{
    class SimpleLyricRenderer : ILyricOverlayRenderer
    {
        public string FontName { set; get; }
        public float FontSize { set; get; }
        public float MainLineFontSize { set; get; }

        public Color FontColor { set; get; }
        public Color BackgroundColor { set; get; }

        public int DisplayingLyricLinesCount
        {
            get => DisplayingLyric?.Count ?? 0;
            set => DisplayingLyric = Enumerable.Range(0, value % 2 == 0 ? value + 1 : value)
                .Select(x =>
                new Text
                {
                    TextToDraw = "...",
                    CurrentLocation = new Point(0, 0)
                }).ToList();
        }
        public int InterLineSpace { set; get; }

        protected virtual List<Text> DisplayingLyric { set; get; }

        protected static readonly Point Zero = new Point(0, 0);

        protected SolidBrush TextBrush;
        protected SolidBrush InfoBrush;
        protected Font TextFont;
        protected Font MainLineFont;
        protected Font InfoLineFont;
        protected TrackLyric TrackLyric;
        protected Point InfoLocation;
        protected Point InfoSize;
        protected Graphics Gfx;

        public SimpleLyricRenderer()
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

        public virtual void Destroy(Graphics gfx)
        {
            MainLineFont.Dispose();
            InfoBrush.Dispose();
            InfoLineFont.Dispose();
            TextFont.Dispose();
        }

        public virtual void LyricChanged(TrackLyric trackLyric, Lyric currentLyric)
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
                 .Take(DisplayingLyricLinesCount).ToList();

            var lowOnLines = DisplayingLyricLinesCount - displayingLyric.Count;

            if (lowOnLines > 0)
                displayingLyric.AddRange(Enumerable.Range(0, lowOnLines).Select(x => new Lyric { Text = "..." }));

            for (int index = 0; index < DisplayingLyricLinesCount; index++)
                DisplayingLyric[index].TextToDraw = displayingLyric[index].Text;
        }

        public virtual void Render(DrawGraphicsEventArgs e)
        {
            if (TrackLyric == null)
                return;

            var gfx = e.Graphics;
            var infoText = $"FPS:{gfx.FPS} Delta:{e.DeltaTime}ms";
            var textSize = gfx.MeasureString(InfoLineFont, infoText);
            InfoSize = textSize;

            gfx.ClearScene(BackgroundColor);
            gfx.BeginScene();

            Point currentLocation = new Point(0, textSize.Y);
            gfx.DrawText(InfoLineFont, InfoBrush, InfoLocation, infoText);

            for (int index = 0; index < DisplayingLyricLinesCount; index++)
            {
                gfx.DrawText(index == DisplayingLyricLinesCount / 2 ? MainLineFont : TextFont, TextBrush, currentLocation, DisplayingLyric[index].TextToDraw);
                currentLocation.Y += InterLineSpace + gfx.MeasureString(index == DisplayingLyricLinesCount / 2 ? MainLineFont : TextFont, DisplayingLyric[index].TextToDraw).Y;
            }

            gfx.EndScene();
        }

        public virtual void Setup(Graphics gfx)
        {
            TextFont = gfx.CreateFont(FontName, FontSize, wordWrapping: true);
            MainLineFont = gfx.CreateFont(FontName, MainLineFontSize, true, wordWrapping: true);
            InfoLineFont = gfx.CreateFont(FontName, FontSize - 2);

            TextBrush = gfx.CreateSolidBrush(FontColor);
            InfoBrush = gfx.CreateSolidBrush(FontColor);
            Gfx = gfx;
        }
    }

    class Text
    {
        public string TextToDraw { set; get; }
        public Point CurrentLocation { set; get; }
        public Point DestinationLocation { set; get; }
        public Point RenderSize { set; get; }
        public bool LocationSet { set; get; }
    }
}
