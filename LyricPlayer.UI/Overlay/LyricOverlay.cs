using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.MusicPlayer;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace LyricPlayer.UI.Overlay
{
    public class LyricOverlay
    {
        public NarinoMusicPlayer Player { protected set; get; }
        public GraphicsWindow Overlay { protected set; get; }
        public GameOverlay.Drawing.Point OverlayLocation { set; get; }
        public Size OverlaySize { set; get; }
        public int InterLineSpace { set; get; }
        public int FPS
        {
            set
            {
                if (Overlay != null)
                    Overlay.FPS = value;
            }
            get => Overlay?.FPS ?? 0;
        }
        public string FontName { set; get; }
        public float FontSize { set; get; }
        public float MainLineFontSize { set; get; }

        public GameOverlay.Drawing.Color FontColor { set; get; }
        public GameOverlay.Drawing.Color BackgroundColor { set; get; }

        public int DisplayingLyricLinesCount
        {
            get => DisplayingLyric?.Length ?? 0;
            set => DisplayingLyric = Enumerable.Range(0, value).Select(x => "...").ToArray();
        }

        protected string[] DisplayingLyric { set; get; }

        SolidBrush textBrush;
        SolidBrush infoBrush;
        Font textFont;
        Font mainLineFont;
        Font infoLineFont;

        public LyricOverlay()
        {
            var token = File.ReadAllText("Token.Token");
            Player = new NarinoMusicPlayer(token);
            Player.LyricChanged += PlayerLyricChanged;
            DisplayingLyricLinesCount = 5;
            OverlaySize = new Size(480, 180);
            FontColor = new Color(210, 210, 210, 255);
            BackgroundColor = new Color(0, 0, 0, 190);
            InterLineSpace = 10;
        }

        public void ShowOverlay(string processName)
            => ShowOverlay(Process.GetProcessesByName(processName).FirstOrDefault()?.MainWindowHandle ?? IntPtr.Zero);

        public void ShowOverlay(IntPtr processWindowHandle)
        {
            Player.Initialize();
            Overlay = new GraphicsWindow((int)OverlayLocation.X, (int)OverlayLocation.Y, (int)OverlaySize.Width, (int)OverlaySize.Height);
            if (processWindowHandle != IntPtr.Zero)
            {
                Overlay.FitTo(processWindowHandle);
                Overlay.PlaceAbove(processWindowHandle);
            }
            Overlay.SetupGraphics += OverlaySetupGraphics;
            Overlay.DestroyGraphics += OverlayDestroyGraphics;
            Overlay.DrawGraphics += OverlayDrawGraphics;

            Overlay.Create();
            Overlay.Show();
            Overlay.Graphics.TextAntiAliasing = true;
            Overlay.FPS = 30;
        }

        private void OverlayDrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            Overlay.IsTopmost = true;

            var location = new GameOverlay.Drawing.Point(0, 0);
            var gfx = e.Graphics;
            var infoText = $"FPS:{gfx.FPS} Delta:{e.DeltaTime}";
            var textSize = gfx.MeasureString(infoLineFont, infoText);

            gfx.ClearScene(BackgroundColor);
            gfx.BeginScene();
            gfx.DrawText(infoLineFont, infoBrush, location, infoText);

            location.Y = textSize.Y + InterLineSpace;
            for (int index = 0; index < DisplayingLyricLinesCount; index++)
            {
                if (index != DisplayingLyricLinesCount / 2)
                {
                    gfx.DrawText(textFont, textBrush, location, DisplayingLyric[index]);
                    location.Y += InterLineSpace + gfx.MeasureString(textFont, DisplayingLyric[index]).Y;
                }
                else
                {
                    gfx.DrawText(mainLineFont, textBrush, location, DisplayingLyric[index]);
                    location.Y += InterLineSpace + gfx.MeasureString(mainLineFont, DisplayingLyric[index]).Y;
                }
            }

            gfx.EndScene();
        }

        private void OverlayDestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            mainLineFont.Dispose();
            infoBrush.Dispose();
            infoLineFont.Dispose();
            textFont.Dispose();
        }

        private void OverlaySetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            e.Graphics.MeasureFPS = true;
            var gfx = e.Graphics;
            textFont = gfx.CreateFont("Times New Roman", 15, wordWrapping: true);
            mainLineFont = gfx.CreateFont("Times New Roman", 20, true, wordWrapping: true);
            infoLineFont = gfx.CreateFont("Time New Roman", 10f);

            textBrush = gfx.CreateSolidBrush(FontColor);
            infoBrush = gfx.CreateSolidBrush(FontColor);
        }

        private void PlayerLyricChanged(object sender, Models.Lyric e)
        {
            var currnetLyricIndex = Player.Lyric.Lyric.IndexOf(e);
            if (currnetLyricIndex == -1)
                return;

            var halfSize = DisplayingLyric.Length / 2;
            for (int index = halfSize - 1; index >= 0; index--)
            {
                if (currnetLyricIndex - (halfSize - index) < 0)
                    DisplayingLyric[index] = "...";
                else
                    DisplayingLyric[index] = Player.Lyric.Lyric[currnetLyricIndex - (halfSize - index)].Text;
            }

            for (int index = halfSize; index < DisplayingLyric.Length; index++)
            {
                if (currnetLyricIndex + index >= Player.Lyric.Lyric.Count)
                    DisplayingLyric[index] = "...";
                else
                    DisplayingLyric[index] = Player.Lyric.Lyric[currnetLyricIndex + (index - halfSize)].Text;
            }
        }
    }
}
