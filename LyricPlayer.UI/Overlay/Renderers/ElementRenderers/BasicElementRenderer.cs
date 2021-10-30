using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model.Elements;
using LyricPlayer.MusicPlayer;
using System.Collections.Generic;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal class BasicElementRenderer : ElementRenderer<BasicElement>
    {
        private static Dictionary<Color, IBrush> Brushes { set; get; }
        public override void Destroy(Graphics gfx)
        {
            if (Brushes == null) return;
            foreach (var brush in Brushes)
                brush.Value.Brush.Dispose();
        }

        protected override void InternalRender(BasicElement element, AudioPlayer audioPlayer, DrawGraphicsEventArgs renderArgs)
        {
            if (Brushes == null)
                Setup(renderArgs.Graphics);

            var gfx = renderArgs.Graphics;
            var color = element.BackgroundColor.ToOverlayColor();
            if (!Brushes.ContainsKey(color))
                Brushes.Add(color, gfx.CreateSolidBrush(color));

            gfx.FillRectangle(Brushes[color], element.AbsoluteRenderArea.ToOverlayRectangle());
        }

        public override void Setup(Graphics gfx)
        {
            Brushes = Brushes ?? new Dictionary<Color, IBrush>();
        }

        public override void Dispose()
        { Destroy(null); }
    }
}
