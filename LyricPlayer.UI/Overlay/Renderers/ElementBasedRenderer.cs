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

        public void LyricChanged(TrackLyric trackLyric, Lyric currentLyric)
        {
            while (Rendering)
            { }
            Console.WriteLine((currentLyric.Element as TextElement)?.Text ?? "");
            ChangingLyric = true;

            RootElement.ChildElements.Clear();
            RootElement.ChildElements.Add(currentLyric.Element);

            ChangingLyric = false;
        }

        public void Render(DrawGraphicsEventArgs renderArgs)
        {
            var type = RootElement.GetType();
            while (ChangingLyric)
            { }

            Rendering = true;

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
                Padding = new System.Drawing.Rectangle(100, 100, size.X - 100, size.Y - 100)
            };
        }

        public void Destroy(Graphics gfx)
        {
            RootElement?.Dispose();
            RootElement = null;
        }
        
    }
}
