using GameOverlay.Drawing;
using System;
using System.Collections.Generic;

namespace LyricPlayer.UI.Overlay.Elements
{
    internal class RenderElement : IDisposable
    {
        public Point Position { set; get; }
        public Point Size { set; get; }
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

      
    }
}
