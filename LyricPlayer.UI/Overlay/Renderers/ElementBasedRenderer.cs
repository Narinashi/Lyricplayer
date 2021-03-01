using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.SoundEngine;
using LyricPlayer.UI.Overlay.Renderers.ElementRenderers;
using System.Threading;

namespace LyricPlayer.UI.Overlay.Renderers
{
    internal class ElementBasedRenderer : IElementBasedRender
    {
        public RenderElement RootElement { get; set; }
        TrackLyric CurrentPlayingTrack { set; get; }
        ISoundEngine SoundEngine { set; get; }

        private bool Rendering { set; get; }
        private bool ChangingLyric { set; get; }

        public void Setup(Graphics gfx)
        {
            if (RootElement == null)
                RootElement = new BasicElement { Size = new System.Drawing.Point(1, 1) };
        }

        public void TrackChanged(TrackLyric trackLyric)
        {
            while (Rendering) { Thread.Sleep(1); }
            ChangingLyric = true;

            RootElement.Dispose();
            RendererResolver.Cleanup();

            CurrentPlayingTrack = trackLyric;
            RootElement = CurrentPlayingTrack.RootElement;

            ChangingLyric = false;
        }

        public void Render(DrawGraphicsEventArgs renderArgs)
        {
            if (ChangingLyric)
                return;

            Rendering = true;

            var gfx = renderArgs.Graphics;

            gfx.ClearScene();
            RootElement.Size = new System.Drawing.Point(gfx.Width, gfx.Height);

            var type = RootElement.GetType();
            if (RendererResolver.Renderers.ContainsKey(type))
                RendererResolver.Renderers[type].Render(RootElement, SoundEngine, renderArgs);

            Rendering = false;
        }

        public void Init(ISoundEngine engine, System.Drawing.Point size)
        {
            SoundEngine = engine;
            RootElement = new BasicElement
            {
                Size = size,
                BackgroundColor = System.Drawing.Color.FromArgb(180, 20, 20, 20)
                //Padding = new System.Drawing.Rectangle(100, 100, size.X - 100, size.Y - 100)
            };
        }

        public void Destroy(Graphics gfx)
        {
            RootElement?.Dispose();
            RootElement = null;
        }

    }
}
