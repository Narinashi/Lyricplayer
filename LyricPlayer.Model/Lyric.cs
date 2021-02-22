using LyricPlayer.Model.Effects;
using LyricPlayer.Model.Elements;
using System.Collections.Generic;

namespace LyricPlayer.Model
{
    public class Lyric
    {
        /// <summary>
        /// in millisecond
        /// </summary>
        public int StartAt { set; get; }

        /// <summary>
        /// in millisecond
        /// </summary>
        public int Duration { set; get; }

        /// <summary>
        /// in millisecond
        /// </summary>
        public int EndAt => StartAt + Duration;
        private RenderElement _Element;
        public RenderElement Element
        {
            set { value.Lyric = this; _Element = value; }
            get => _Element;
        }

        public List<Effect> Effects { set; get; }
    }
}
