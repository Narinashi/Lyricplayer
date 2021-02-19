using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.LyricEngine;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.UI.Overlay.Renderers.ElementRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LyricPlayer.UI.Overlay.Renderers
{
    internal class ElementBasedRenderer : IElementBasedRender
    {
        public RenderElement RootElement { get; set; }
        TrackLyric CurrentPlayingTrack { set; get; }
        ILyricEngine LyricEngine { set; get; }

        private bool Rendering { set; get; }
        private bool ChangingLyric { set; get; }
        
        public void Setup(Graphics gfx)
        {
            if (RootElement == null)
                RootElement = new BasicElement { Size = new System.Drawing.Point (1, 1) };
        }

        public void LyricChanged(TrackLyric trackLyric)
        {
            while (Rendering)
            { }
            var lyrics = LyricEngine.PlayingLyrics;
            ChangingLyric = true;

            foreach (var lyric in lyrics)
                Console.WriteLine((lyric.Element as TextElement)?.Text ?? "");

            RootElement.ChildElements.Replace(lyrics.Select(x=>x.Element).ToList());
            
            ChangingLyric = false;
        }

        public void Render(DrawGraphicsEventArgs renderArgs)
        {
            var gfx = renderArgs.Graphics;
            RootElement.Size = new System.Drawing.Point(gfx.Width, gfx.Height);

            var type = RootElement.GetType();
            while (ChangingLyric)
            { }

            Rendering = true;

            gfx.ClearScene(new Color(20, 20, 20, 0.35f));
            if (RendererResolver.Renderers.ContainsKey(type))
                RendererResolver.Renderers[type].Render(RootElement, renderArgs);

            Rendering = false;
        }

        public void Init(ILyricEngine lyricEngine, System.Drawing.Point size)
        {
            LyricEngine = lyricEngine;
            RootElement = new BasicElement
            {
                Size = size,
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
