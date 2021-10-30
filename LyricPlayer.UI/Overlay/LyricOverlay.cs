using GameOverlay.Windows;
using LyricPlayer.LyricFetcher;
using LyricPlayer.LyricFetcher.MusicmatchLyricFetcher;
using LyricPlayer.MusicPlayer;
using LyricPlayer.UI.Overlay.Renderers;
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
        public AudioPlayer MusicPlayer { protected set; get; }
        public LyricFetcher.LyricFetcher LyricFetcher { protected set; get; }
        public GraphicsWindow Overlay { protected set; get; }
        public IElementBasedRender Renderer { get; protected set; }
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
            MusicPlayer = new NAudioPlayer();
            Renderer = new ElementBasedRenderer();
            LyricFetcher = new MusicxMatchLyricFetcher();
            var size = DisplayTools.GetPhysicalDisplaySize();
            OverlaySize = new Size(size.Width, 80);
            OverlayLocation = new Point(0, 0);
        }

        public void ShowOverlay(string processName)
        {
            TargetProcessName = processName;
            TargetProcess = Process.GetProcessesByName(processName ?? "").FirstOrDefault();
            ShowOverlay(TargetProcess?.MainWindowHandle ?? IntPtr.Zero);
        }

        public void ShowOverlay(IntPtr processWindowHandle)
        {
            Overlay = new GraphicsWindow((int)OverlayLocation.X, (int)OverlayLocation.Y, (int)OverlaySize.Width, (int)OverlaySize.Height);
            Overlay.SetupGraphics += OverlaySetupGraphics;
            Overlay.DestroyGraphics += OverlayDestroyGraphics;
            Overlay.DrawGraphics += OverlayDrawGraphics;

            MusicPlayer.TrackChanged += (s, e) => Renderer.TrackChanged(LyricFetcher.GetLyric(MusicPlayer.CurrentlyPlaying));
            
            Renderer.Init(MusicPlayer, new System.Drawing.Point((int)OverlaySize.Width, (int)OverlaySize.Height));

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
            Renderer.Render(e);
        }

        private void OverlayDestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        { Renderer?.Destroy(e.Graphics); }


        private void OverlaySetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            e.Graphics.MeasureFPS = true;
            e.Graphics.TextAntiAliasing = true;
            Renderer.Setup(e.Graphics);
        }
    }
}
