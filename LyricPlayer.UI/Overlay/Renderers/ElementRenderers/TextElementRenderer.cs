using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model.Elements;
using System.Collections.Generic;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal class TextElementRenderer : ElementRenderer<TextElement>
    {
        private static Dictionary<string, Font> Fonts { set; get; } = new Dictionary<string, Font>();
        private static Dictionary<Color, IBrush> Brushes { set; get; } = new Dictionary<Color, IBrush>();

        protected override void InternalRender(TextElement element, DrawGraphicsEventArgs renderArgs)
        {
            var gfx = renderArgs.Graphics;

            if (!Fonts.ContainsKey(element.FontName))
                Fonts.Add(element.FontName, gfx.CreateFont(element.FontName, element.FontSize, element.Bold, element.Italic, element.WordWrap));
            if (!Brushes.ContainsKey(element.TextColor))
                Brushes.Add(element.TextColor, gfx.CreateSolidBrush(element.TextColor));
            if (!Brushes.ContainsKey(element.BackGroundColor))
                Brushes.Add(element.BackGroundColor, gfx.CreateSolidBrush(element.BackGroundColor));

            var font = Fonts[element.FontName];

            if (element.BackGroundColor == Color.Transparent)
                gfx.DrawText(font, font.FontSize,
                    Brushes[element.TextColor],
                    element.AbsoluteLocation.X + element.Padding.Left,
                    element.AbsoluteLocation.Y + element.Padding.Top,
                    element.Text);
            else
                gfx.DrawTextWithBackground(font, font.FontSize,
                    Brushes[element.TextColor],
                    Brushes[element.BackGroundColor],
                    element.AbsoluteLocation.X + element.Padding.Left,
                    element.AbsoluteLocation.Y + element.Padding.Top,
                    element.Text);
        }

        public override void Destroy(Graphics gfx)
        {
            foreach (var font in Fonts)
                font.Value.Dispose();
            foreach (var brush in Brushes)
                brush.Value.Brush.Dispose();

            Brushes.Clear();
            Fonts.Clear();
        }

        public override void Setup(Graphics gfx)
        {

        }
    }
}
