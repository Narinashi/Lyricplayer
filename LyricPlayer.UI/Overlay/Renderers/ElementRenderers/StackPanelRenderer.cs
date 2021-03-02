using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model.Elements;
using System.Linq;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    class StackPanelRenderer : ElementRenderer<StackPanelElement>
    {
        protected override void InternalRender(StackPanelElement element, DrawGraphicsEventArgs renderArgs)
        {
            Point childLocation = new Point(0, 0);
            Point childElementSize = new Point(0, 0);

            for (int index = 0; index < element.ChildElements.Count; index++)
            {
                switch (element.ItemsOrientation)
                {
                    case StackPanelItemRenderRotation.UpToBottom:
                        childLocation = new Point(0, childLocation.Y + childElementSize.Y);
                        break;
                    case StackPanelItemRenderRotation.LeftToRight:
                        childLocation = new Point(childLocation.X + childElementSize.X, 0);
                        break;
                    case StackPanelItemRenderRotation.RightToLeft:
                        childLocation = new Point(element.Size.X - childLocation.X - childElementSize.X, 0);
                        break;
                    case StackPanelItemRenderRotation.BottomToUp:
                        childLocation = new Point(0, element.Size.X - childLocation.X - childElementSize.X);
                        break;
                }

                var child = element.ChildElements[index];
                child.Location = childLocation.ToFloatPoint();
                childElementSize = child.Size.ToOverlayPoint();
            }

            if (element.AutoSize && element.ChildElements.Any())
            {
                var lastElement = (element.ItemsOrientation == StackPanelItemRenderRotation.UpToBottom || element.ItemsOrientation == StackPanelItemRenderRotation.LeftToRight) ?
                     element.ChildElements.Last() : element.ChildElements.First();

                var size = new Point(lastElement.Location.X + lastElement.Size.X, lastElement.Location.Y + lastElement.Size.Y);
                var parentSize = element.ParentElement.Size;
                element.Size = new System.Drawing.Point((int)(parentSize.X < size.X ? parentSize.X : size.X), (int)(parentSize.Y < size.Y ? parentSize.Y : size.Y));
            }
        }

        protected override void InternalRenderPreparation(StackPanelElement element, DrawGraphicsEventArgs renderArgs)
        {
            base.InternalRenderPreparation(element, renderArgs);
            foreach (var child in element.ChildElements)
               RendererResolver.Renderers[child.GetType()].PrepareToRender(child, renderArgs);
        }


        public override void Destroy(Graphics gfx) { }
        public override void Dispose() { }
        public override void Setup(Graphics gfx) { }
    }
}
