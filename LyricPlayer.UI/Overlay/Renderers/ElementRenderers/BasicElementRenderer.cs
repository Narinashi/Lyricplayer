using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.UI.Overlay.Elements;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    class BasicElementRenderer : IElementRenderer<RenderElement>
    {
        public void Destroy(Graphics gfx)
        { }

        public void Render(RenderElement element, DrawGraphicsEventArgs renderArgs)
        {
            var gfx = renderArgs.Graphics;
            gfx.ClipRegionStart(element.RenderArea);
            gfx.TransformStart(TransformationMatrix.Rotation(element.Rotation));

            gfx.ClearScene();//just an empty space

            gfx.TransformEnd();
            gfx.ClipRegionEnd();
        }

        public void Setup(Graphics gfx)
        { }
    }
}
