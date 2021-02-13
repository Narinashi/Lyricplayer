using LyricPlayer.Model.Effects;
using LyricPlayer.Model.Elements;
using System.Collections.Generic;

namespace LyricPlayer.Model
{
    public class Lyric
    {
        /// <summary>
        /// a paragraph or sentence
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        /// can be used to display other type of texts (or even elements)
        /// </summary>
        public List<Lyric> SubTexts { set; get; }
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
