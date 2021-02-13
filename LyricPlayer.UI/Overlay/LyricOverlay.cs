using GameOverlay.Windows;
using LyricPlayer.MusicPlayer;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;


namespace LyricPlayer.UI.Overlay
{
    public class LyricOverlay
    {
        public NarinoMusicPlayer Player { protected set; get; }
        public GraphicsWindow Overlay { protected set; get; }
        public Point OverlayLocation { set; get; }
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
        protected Process TargetProcess { set; get; }
        protected string TargetProcessName { set; get; }
        protected Timer Timer { set; get; }

        public LyricOverlay()
        {
            var token = File.ReadAllText("Token.Token");
            Player = new SpotifyMusicPlayer(token);

            var size = DisplayTools.GetPhysicalDisplaySize();
            OverlaySize = new Size(size.Width, 115);
        }

        public void ShowOverlay(string processName)
        {
            TargetProcessName = processName;
            TargetProcess = Process.GetProcessesByName(processName ?? "").FirstOrDefault();
            ShowOverlay(TargetProcess?.MainWindowHandle ?? IntPtr.Zero);
        }

        public void ShowOverlay(IntPtr processWindowHandle)
        {
            Player.Initialize();
            Overlay = new GraphicsWindow((int)OverlayLocation.X, (int)OverlayLocation.Y, (int)OverlaySize.Width, (int)OverlaySize.Height);
            Overlay.SetupGraphics += OverlaySetupGraphics;
            Overlay.DestroyGraphics += OverlayDestroyGraphics;
            Overlay.DrawGraphics += OverlayDrawGraphics;

            Overlay.Create();
            Overlay.Show();
            Overlay.FPS = 90;

            if (!string.IsNullOrEmpty(TargetProcessName))
            {
                Timer = new Timer();
                Timer.Elapsed += (s, e) =>
                {
                    if (!(TargetProcess?.HasExited ?? true))
                        Overlay.PlaceAbove(TargetProcess.MainWindowHandle);
                    else
                    {
                        TargetProcess = Process.GetProcessesByName(TargetProcessName ?? "").FirstOrDefault();
                        Overlay.IsTopmost = true;
                    }
                };
                Timer.Interval = 2500;
                Timer.Start();
            }

        }

        private void OverlayDrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            Overlay.IsTopmost = true;
        }

        private void OverlayDestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        { }


        private void OverlaySetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            e.Graphics.MeasureFPS = true;
            e.Graphics.TextAntiAliasing = true;
        }
    }
}
