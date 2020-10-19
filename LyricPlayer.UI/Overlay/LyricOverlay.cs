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
        public int FPS
        {
            set
            {
                if (Overlay != null)
                    Overlay.FPS = value;
            }
            get => Overlay?.FPS ?? 0;
        }

        protected ILyricOverlayRenderer LyricRenderer { set; get; }

        public LyricOverlay()
        {
            var token = File.ReadAllText("Token.Token");
            Player = new NarinoMusicPlayer(token);
            Player.LyricChanged += PlayerLyricChanged;
            LyricRenderer = new SimpleLyricRenderer();
            OverlaySize = new Size(480, 300);
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
            Overlay.FPS = 60;
        }

        private void OverlayDrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            Overlay.IsTopmost = true;
            LyricRenderer.Render(e);
        }

        private void OverlayDestroyGraphics(object sender, DestroyGraphicsEventArgs e)
            => LyricRenderer.Destroy(e.Graphics);
        

        private void OverlaySetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            LyricRenderer.Setup(e.Graphics);
            e.Graphics.MeasureFPS = true;
        }

        private void PlayerLyricChanged(object sender, Models.Lyric e)
           => LyricRenderer.LyricChanged(Player.Lyric, e);

    }
}
