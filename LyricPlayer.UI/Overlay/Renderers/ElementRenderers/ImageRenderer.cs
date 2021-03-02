using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    class ImageRenderer : ElementRenderer<ImageElement>
    {
        static Dictionary<ImageElement, Image> ImageElements { set; get; } = new Dictionary<ImageElement, Image>();

        protected override void InternalRenderPreparation(ImageElement element, DrawGraphicsEventArgs renderArgs)
        {
            base.InternalRenderPreparation(element, renderArgs);
            if (element.AutoSize) return;
            if (element.ParentElement == null) return;
            if (!ImageElements.ContainsKey(element)) return;

            var image = ImageElements[element];
            if (image.Bitmap?.IsDisposed ?? true) return;

            var ratio = image.Width / image.Height;
            if (element.Dock == ElementDock.None)
            {
                FillAsMuchAsPossible(element, image);
                return;
            }



            var currentHeight = element.Size.Y;
           // element.Size = new System.Drawing.Point(ratio * currentHeight)
        }

        private void FillAsMuchAsPossible(ImageElement element, Image image)
        {
            var ratio = image.Width / image.Height;
            if (image.Width <= element.ParentElement.Size.X && image.Height <= element.ParentElement.Size.Y)
            {
                element.Size = new System.Drawing.Point((int)image.Width,(int)image.Height);
                return;
            }
            if(ratio < 1)
            {
                //var multiplier = element.ParentElement.Size.X / image.Width;
                //element.Size = new System.Drawing.Point(multiplier * image.Width, multiplier * image.Height * ratio);
            }
        }

        protected override void InternalRender(ImageElement element, DrawGraphicsEventArgs renderArgs)
        {
            if (element.Initializing)
                return;
            if (!ImageElements.ContainsKey(element))
            {
                InitImage(element, renderArgs.Graphics);
                return;
            }
            var gfx = renderArgs.Graphics;
            gfx.DrawImage(ImageElements[element], element.AbsoluteRenderArea.ToOverlayRectangle(), element.Opacity);
        }

        private void InitImage(ImageElement element, Graphics gfx)
        {
            if (element.ImageContent != null)
                ImageElements.Add(element, gfx.CreateImage(element.ImageContent));
            else if (File.Exists(element.ImagePath))
                ImageElements.Add(element, gfx.CreateImage(element.ImagePath));
            else if (Uri.IsWellFormedUriString(element.ImagePath, UriKind.Absolute))
            {
                element.Initializing = true;
                Task.Run(() =>
                {
                    using (var webClient = new WebClient())
                        element.ImageContent = webClient.DownloadData(element.ImagePath);

                    element.Initializing = false;
                });
            }
        }

        public override void Destroy(Graphics gfx) { }
        public override void Dispose() { }
        public override void Setup(Graphics gfx) { }
        public override void Cleanup()
        {
            foreach (var image in ImageElements)
                image.Value.Dispose();
            ImageElements.Clear();
        }
    }
}
