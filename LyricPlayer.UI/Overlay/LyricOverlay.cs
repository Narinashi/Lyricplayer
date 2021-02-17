using GameOverlay.Windows;
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
        public NarinoMusicPlayer Player { protected set; get; }
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
            var token = File.ReadAllText("Token.Token");
            Player = new SpotifyMusicPlayer(token);
            Renderer = new ElementBasedRenderer();
            var size = DisplayTools.GetPhysicalDisplaySize();
            OverlaySize = new Size(1920, 115);
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
            Player.Initialize();
            Overlay = new GraphicsWindow((int)OverlayLocation.X, (int)OverlayLocation.Y, (int)OverlaySize.Width, (int)OverlaySize.Height);
            Overlay.SetupGraphics += OverlaySetupGraphics;
            Overlay.DestroyGraphics += OverlayDestroyGraphics;
            Overlay.DrawGraphics += OverlayDrawGraphics;

            Player.LyricChanged += (s, e) => Renderer.LyricChanged(Player.Lyric);
            Renderer.Init(Player.LyricEngine, new System.Drawing.Point((int)OverlaySize.Width, (int)OverlaySize.Height));

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
