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

        public RenderElement Element { set; get; }

        public List<Effect> Effects { set; get; }
    }
}
