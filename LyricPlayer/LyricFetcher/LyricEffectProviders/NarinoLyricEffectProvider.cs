using LyricPlayer.Model;
using LyricPlayer.Model.Effects;
using LyricPlayer.Model.Elements;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace LyricPlayer.LyricFetcher.LyricEffectProviders
{
    class NarinoLyricEffectProvider : ILyricEffectProvider
    {
        public void AddEffects(TrackLyric trackLyric)
        {
            var root = trackLyric.RootElement;
            var imgFilePath = @"F:\yin.jpg";
            if (!File.Exists(imgFilePath))
                return;

            root.ChildElements.OfType<TextElement>().Update(x =>
            {
                x.FontName = Fixed.DefaultFontName;
                x.FontSize = Fixed.DefaultFontSize;
                x.TextColor = Color.White;
                x.HorizontalAlignment = TextHorizontalAlignment.Center;
                x.VerticalAlignment = TextVerticalAlignment.Center;
                x.Dock = ElementDock.Fill;
                x.AutoSize = false;
            });

            trackLyric.RootElement = AddBackgroundImages(root, imgFilePath);
        }
        private RenderElement AddBackgroundImages(RenderElement root, string imgFilePath)
        {
            var newRoot = new BasicElement
            {
                Dock = ElementDock.Fill,
                Duration = root.Duration,
                BackgroundColor = Color.FromArgb(180, 12, 12, 12)
            };

            var effect = new SpinEffect
            {
                Duration = int.MaxValue,
                Trauma = 10,
                TraumaDecay = 0.000000000001f,
                TraumaMag = 0.8f,
                TraumaMult = 0.8f,
                RotationSpeed = -0.5f
            };
            var rootStackPanel = new StackPanelElement
            {
                Dock = ElementDock.Fill,
                Duration = root.Duration,
                ItemsOrientation = StackPanelItemRenderRotation.LeftToRight,
            };
            var leftSideImage = new ImageElement
            {
                ImagePath = imgFilePath,
                Dock = ElementDock.Left,
                Duration = root.Duration,
                AutoSize = false,
                Size = new Point(80, 80),
                Effects = new List<Effect> { effect }
            };
            var rightSidePanel = new StackPanelElement
            {
                Dock = ElementDock.Right,
                Duration = root.Duration,
                ItemsOrientation = StackPanelItemRenderRotation.RightToLeft
            };
            var rightSideImage = new ImageElement
            {
                ImagePath = imgFilePath,
                Dock = ElementDock.Right,
                Duration = root.Duration,
                Effects = new List<Effect> { effect },
                AutoSize = false,
                Size = new Point(80, 80),
            };


            rightSidePanel.ChildElements.Add(rightSideImage);
            rootStackPanel.ChildElements.Add(leftSideImage);
            rootStackPanel.ChildElements.Add(rightSidePanel);

            newRoot.ChildElements.Add(rootStackPanel);
            newRoot.ChildElements.Add(root);

            root.Effects = root.Effects ?? new List<Effect>();
            root.Effects.Add(new ShakeEffect
            {
                Duration = int.MaxValue,
                Trauma = 12,
                TraumaDecay = 0.000000000001f,
                TraumaMag = 2.8f,
                TraumaMult = 2f
            });

            return newRoot;
        }
    }
}
