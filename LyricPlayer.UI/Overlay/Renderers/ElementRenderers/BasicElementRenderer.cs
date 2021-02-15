using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model.Elements;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal class BasicElementRenderer : ElementRenderer<BasicElement>
    {
        private IBrush Brush { set; get; }
        public override void Destroy(Graphics gfx)
        { Brush?.Brush.Dispose(); }

        protected override void InternalRender(BasicElement element, DrawGraphicsEventArgs renderArgs)
        {
            if (Brush == null)
                Setup(renderArgs.Graphics);

            var gfx = renderArgs.Graphics;
            gfx.DrawRectangleEdges(Brush, element.AbsoluteArea, 3);
            gfx.DrawRectangleEdges(Brush, element.AbsoluteRenderArea, 3);
        }

        public override void Setup(Graphics gfx)
        { Brush = gfx.CreateSolidBrush(0, 0, 0); }

        public override void Dispose()
        { Destroy(null); }
    }
}
