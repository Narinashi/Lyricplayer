using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using System;
using System.Collections.Generic;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    internal class TextElementRenderer : ElementRenderer<TextElement>
    {
        private static Dictionary<string, Font> Fonts { set; get; } = new Dictionary<string, Font>();
        private static Dictionary<Color, IBrush> Brushes { set; get; } = new Dictionary<Color, IBrush>();

        protected override void InternalRenderPreparation(TextElement element, DrawGraphicsEventArgs renderArgs)
        {
            var gfx = renderArgs.Graphics;
            if (!Fonts.ContainsKey(element.FontName))
                Fonts.Add(element.FontName, gfx.CreateFont(element.FontName, element.FontSize, element.Bold, element.Italic, element.WordWrap));

            if (string.IsNullOrEmpty(element.Text))
                element.Text = "...";

            element.Text = element.Text.Replace("\n", " ");
            var textSize = gfx.MeasureString(Fonts[element.FontName], element.FontSize, element.Text);

            if (element.AutoSize)
            {
                var tp = textSize.ToDrawingPoint();
                element.Size = new System.Drawing.Point(Math.Min(tp.X, element.ParentElement.Size.X),
                                                            Math.Min(tp.Y, element.ParentElement.Size.Y));
            }

            if (element.Size.X < element.LineWidthBreakTreshold * textSize.X)
            {
                var words = element.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var stringToAdd = element.Text = string.Empty;

                for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
                {
                    var tempLineSize = gfx.MeasureString(Fonts[element.FontName], element.FontSize, stringToAdd + words[wordIndex] + " ");
                    if (tempLineSize.X > element.ParentElement.Size.X * element.LineWidthBreakTreshold)
                    {
                        if (string.IsNullOrEmpty(stringToAdd))
                            element.Text += words[wordIndex] + "\n";
                        else
                        {
                            element.Text += stringToAdd + "\n";
                            stringToAdd = string.Empty;
                        }
                    }
                    else stringToAdd += words[wordIndex] + " ";
                }

                if (element.AutoSize)
                {
                    var tp = textSize.ToDrawingPoint();
                    element.Size = new System.Drawing.Point(Math.Min(tp.X, element.ParentElement.Size.X),
                                                                Math.Min(tp.Y, element.ParentElement.Size.Y));
                }

            }

            var dummyTextSize = gfx.MeasureString(Fonts[element.FontName], element.FontSize, "A");
            if (element.Size.X < dummyTextSize.X || element.Size.Y < dummyTextSize.Y)
                element.Size = new System.Drawing.Point(
                                                (int)(element.Size.X < dummyTextSize.X ? dummyTextSize.X : element.Size.X),
                                                (int)(element.Size.Y < dummyTextSize.Y ? dummyTextSize.Y : element.Size.Y));
        }

        protected override void InternalRender(TextElement element, DrawGraphicsEventArgs renderArgs)
        {
            var gfx = renderArgs.Graphics;

            if (!Fonts.ContainsKey(element.FontName))
                Fonts.Add(element.FontName, gfx.CreateFont(element.FontName, element.FontSize, element.Bold, element.Italic, element.WordWrap));
            if (!Brushes.ContainsKey(element.TextColor.ToOverlayColor()))
                Brushes.Add(element.TextColor.ToOverlayColor(), gfx.CreateSolidBrush(element.TextColor.ToOverlayColor()));
            if (!Brushes.ContainsKey(element.BackgroundColor.ToOverlayColor()))
                Brushes.Add(element.BackgroundColor.ToOverlayColor(), gfx.CreateSolidBrush(element.BackgroundColor.ToOverlayColor()));

            var font = Fonts[element.FontName];
            var textLocation = !element.AutoSize ? CalculateTextLocation(element, gfx) : element.AbsoluteLocation;

            if (element.BackgroundColor.A == 0)
                gfx.DrawText(font, font.FontSize,
                    Brushes[element.TextColor.ToOverlayColor()],
                    textLocation.X + element.Padding.Left,
                    textLocation.Y + element.Padding.Top,
                    element.Text);
            else
                gfx.DrawTextWithBackground(font, font.FontSize,
                    Brushes[element.TextColor.ToOverlayColor()],
                    Brushes[element.BackgroundColor.ToOverlayColor()],
                    textLocation.X + element.Padding.Left,
                    textLocation.Y + element.Padding.Top,
                    element.Text);
        }

        private FloatPoint CalculateTextLocation(TextElement element, Graphics gfx)
        {
            var location = new System.Drawing.Point(0, 0);
            var textSize = gfx.MeasureString(Fonts[element.FontName], element.FontSize, element.Text);
            switch (element.HorizontalAlignment)
            {
                case TextHorizontalAlignment.Left:
                    location.X = 0; break;
                case TextHorizontalAlignment.Right:
                    location.X = element.Size.X - (int)textSize.X; break;
                case TextHorizontalAlignment.Center:
                    location.X = (element.Size.X - (int)textSize.X) / 2; break;
            }
            switch (element.VerticalAlignment)
            {
                case TextVerticalAlignment.Top:
                    location.Y = 0; break;
                case TextVerticalAlignment.Center:
                    location.Y = (element.Size.Y - (int)textSize.Y) / 2; break;
                case TextVerticalAlignment.Bottm:
                    location.Y = element.Size.Y - (int)textSize.Y; break;
            }
            return new FloatPoint(location.X + element.AbsoluteLocation.X, location.Y + element.AbsoluteLocation.Y);
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

        public override void Dispose() => Destroy(null);

    }
}
