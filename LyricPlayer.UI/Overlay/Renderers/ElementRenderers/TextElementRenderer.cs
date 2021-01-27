using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.UI.Overlay.Elements;
using System.Collections.Generic;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal class TextElementRenderer : IElementRenderer<TextElement>
    {
        private static Dictionary<string, Font> Fonts { set; get; } = new Dictionary<string, Font>();
        private static Dictionary<Color, IBrush> Brushes { set; get; } = new Dictionary<Color, IBrush>();
        public void Render(TextElement element, DrawGraphicsEventArgs renderArgs)
        {
            var gfx = renderArgs.Graphics;
            gfx.ClipRegionStart(element.RenderArea);
            gfx.TransformStart(TransformationMatrix.Rotation(element.Rotation));

            if (!Fonts.ContainsKey(element.FontName))
                Fonts.Add(element.FontName, gfx.CreateFont(element.FontName, element.FontSize, element.Bold, element.Italic, element.WordWrap));

            var font = Fonts[element.FontName];

                

            gfx.TransformEnd();
            gfx.ClipRegionEnd();
        }

        public void Destroy(Graphics gfx)
        {
            foreach (var font in Fonts)
                font.Value.Dispose();
            foreach (var brush in Brushes)
                brush.Value.Brush.Dispose();

            Brushes.Clear();
            Fonts.Clear();
        }

        public void Setup(Graphics gfx)
        {
            throw new System.NotImplementedException();
        }
    }
}
