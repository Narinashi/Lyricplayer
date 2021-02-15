using GameOverlay.Drawing;
using Newtonsoft.Json;
using System;

namespace LyricPlayer.Model.Elements
{
    public abstract class RenderElement : IDisposable
    {
        public RenderElement()
        { ChildElements = new ElementCollection() { Parent = this }; }

        public RenderElement(Lyric lyric) : this()
        { Lyric = lyric; }

        public Point Location { set; get; }
        public virtual Point Size { set; get; }
        public Rectangle Padding { set; get; }

        /// <summary>
        /// In degree
        /// </summary>
        public float Rotation { set; get; }

        [JsonIgnore]
        public Point AbsoluteLocation => ParentElement == null ?
            Location :
            new Point
            {
                X = ParentElement.AbsoluteLocation.X + Location.X,
                Y = ParentElement.AbsoluteLocation.Y + Location.Y
            };

        [JsonIgnore]
        public Rectangle AbsoluteArea => new Rectangle
        {
            Top = Location.Y,
            Left = Location.X,
            Bottom = Location.Y + Size.Y,
            Right = Location.X + Size.X
        };

        [JsonIgnore]
        public Rectangle AbsoluteRenderArea => new Rectangle
        {
            Left = AbsoluteLocation.X + Padding.Left,
            Top = AbsoluteLocation.Y + Padding.Top,
            Bottom = AbsoluteLocation.Y + Size.Y - Padding.Bottom,
            Right = AbsoluteLocation.X + Size.X - Padding.Right
        };

        [JsonIgnore]
        public Lyric Lyric { get; protected set; }

        [JsonIgnore]
        public RenderElement ParentElement { get; set; }
        public ElementCollection ChildElements { get; protected set; }

        public virtual void Dispose()
        {
            foreach (var element in ChildElements)
                element.Dispose();

            ChildElements.Clear();
            Location = Size = default(Point);
            Rotation = 0;
        }

        public void AlignToLeft()
        {
            if (ParentElement == null) return;
            Location = new Point { X = 0, Y = Location.Y };
        }
        public void AlignToRight()
        {
            if (ParentElement == null) return;
            Location = new Point { X = ParentElement.Size.X - Size.X, Y = Location.Y };
        }
        public void AlignToTop()
        {
            if (ParentElement == null) return;
            Location = new Point { X = Location.X, Y = 0 };
        }
        public void AlighToBottom()
        {
            if (ParentElement == null) return;
            Location = new Point { X = Location.X, Y = ParentElement.Size.Y - Size.Y };
        }
        public void AlignToCenter()
        {
            if (ParentElement == null) return;
            var deltaX = ParentElement.Size.X - Size.X;
            var deltaY = ParentElement.Size.Y - Size.Y;
            Location = new Point { X = deltaX / 2, Y = deltaY / 2 };
        }
        public void AlignCenterVerticaly()
        {
            if (ParentElement == null) return;
            var deltaY = ParentElement.Size.Y - Size.Y;
            Location = new Point { X = Location.X, Y = deltaY / 2 };
        }
        public void AlignCenterHorizontaly()
        {
            if (ParentElement == null) return;
            var deltaX = ParentElement.Size.X - Size.X;
            Location = new Point { X = deltaX / 2, Y = Location.Y };
        }
    }
}
