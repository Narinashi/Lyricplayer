using Newtonsoft.Json;

using System;
using System.Drawing;

namespace LyricPlayer.Model.Elements
{
    public abstract class RenderElement : IDisposable
    {
        public RenderElement()
        { ChildElements = new ElementCollection() { Parent = this }; }

        public RenderElement(Lyric lyric) : this()
        { Lyric = lyric; }

        public FloatPoint Location { set; get; }
        public virtual Point Size { set; get; }
        public Rectangle Padding { set; get; }
        public ElementDock Dock { set; get; }

        /// <summary>
        /// In degree
        /// </summary>
        public float Rotation { set; get; }

        [JsonIgnore]
        public FloatPoint AbsoluteLocation => ParentElement == null ?
            Location :
            new FloatPoint
            {
                X = ParentElement.AbsoluteLocation.X + Location.X,
                Y = ParentElement.AbsoluteLocation.Y + Location.Y
            };

        [JsonIgnore]
        public Rectangle AbsoluteArea => new Rectangle
        {
            Y = (int)AbsoluteLocation.Y,
            X = (int)AbsoluteLocation.X,
            Height = Size.Y,
            Width = Size.X
        };

        [JsonIgnore]
        public Rectangle AbsoluteRenderArea => new Rectangle
        {
            X = (int)AbsoluteLocation.X + Padding.Left,
            Y = (int)AbsoluteLocation.Y + Padding.Top,
            Height = Size.Y - Padding.Bottom,
            Width = Size.X - Padding.Right
        };

        [JsonIgnore]
        public Lyric Lyric { get; set; }

        [JsonIgnore]
        public RenderElement ParentElement { get; set; }
        public ElementCollection ChildElements { get; protected set; }

        public virtual void Dispose()
        {
            foreach (var element in ChildElements)
                element.Dispose();

            ChildElements.Clear();
            Location = default(FloatPoint);
            Size = default(Point);
            Rotation = 0;
        }

        public void AlignToLeft()
        {
            if (ParentElement == null) return;
            Location = new FloatPoint { X = 0, Y = Location.Y };
        }
        public void AlignToRight()
        {
            if (ParentElement == null) return;
            Location = new FloatPoint { X = ParentElement.Size.X - Size.X, Y = Location.Y };
        }
        public void AlignToTop()
        {
            if (ParentElement == null) return;
            Location = new FloatPoint { X = Location.X, Y = 0 };
        }
        public void AlighToBottom()
        {
            if (ParentElement == null) return;
            Location = new FloatPoint { X = Location.X, Y = ParentElement.Size.Y - Size.Y };
        }
        public void AlignToCenter()
        {
            if (ParentElement == null) return;
            var deltaX = ParentElement.Size.X - Size.X;
            var deltaY = ParentElement.Size.Y - Size.Y;
            Location = new FloatPoint { X = deltaX / 2f, Y = deltaY / 2f };
        }
        public void AlignCenterVerticaly()
        {
            if (ParentElement == null) return;
            var deltaY = ParentElement.Size.Y - Size.Y;
            Location = new FloatPoint { X = Location.X, Y = deltaY / 2 };
        }
        public void AlignCenterHorizontaly()
        {
            if (ParentElement == null) return;
            var deltaX = ParentElement.Size.X - Size.X;
            Location = new FloatPoint { X = deltaX / 2, Y = Location.Y };
        }
    }
}
