using GameOverlay.Drawing;
using System;
using System.Collections.Generic;

namespace LyricPlayer.UI.Overlay.Elements
{
    internal class RenderElement : IDisposable
    {
        public Point Position { set; get; }
        public Point AbsolutePosition => ParentElement == null ?
            Position :
            new Point
            {
                X = ParentElement.AbsolutePosition.X + Position.X,
                Y = ParentElement.AbsolutePosition.Y + Position.Y
            };

        public Point Size { set; get; }
        public Rectangle Padding { set; get; }
        public Rectangle RenderArea => new Rectangle
        {
            Top = Position.Y,
            Left = Position.X,
            Bottom = Position.Y + Size.Y,
            Right = Position.X + Size.X
        };
        /// <summary>
        /// In degree
        /// </summary>
        public float Rotation { set; get; }

        public RenderElement ParentElement { set; get; }
        public List<RenderElement> ChildElements { set; get; } = new List<RenderElement>();

        public virtual void Dispose()
        {
            ChildElements?.ForEach(x => x.Dispose());
            Position = Size = default(Point);
            Rotation = 0;
        }

        public void AlignToLeft()
        {
            if (ParentElement == null) return;
            Position = new Point { X = 0, Y = Position.Y };
        }
        public void AlignToRight()
        {
            if (ParentElement == null) return;
            Position = new Point { X = ParentElement.Size.X - Size.X, Y = Position.Y };
        }
        public void AlignToTop()
        {
            if (ParentElement == null) return;
            Position = new Point { X = Position.X, Y = 0 };
        }
        public void AlighToBottom()
        {
            if (ParentElement == null) return;
            Position = new Point { X = Position.X, Y = ParentElement.Size.Y - Size.Y };
        }
        public void AlignToCenter()
        {
            if (ParentElement == null) return;
            var deltaX = ParentElement.Size.X - Size.X;
            var deltaY = ParentElement.Size.Y - Size.Y;
            Position = new Point { X = deltaX / 2, Y = deltaY / 2 };
        }
        public void AlignCenterVerticaly()
        {
            if (ParentElement == null) return;
            var deltaY = ParentElement.Size.Y - Size.Y;
            Position = new Point { X = Position.X, Y = deltaY / 2 };
        }
        public void AlignCenterHorizontaly()
        {
            if (ParentElement == null) return;
            var deltaX = ParentElement.Size.X - Size.X;
            Position = new Point { X = deltaX / 2, Y = Position.Y };
        }
    }
}
