using LyricPlayer.LyricEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.Models
{
    public class Lyric
    {
        /// <summary>
        /// a paragraph or sentence
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        /// usefull when trying to make word by word synchronized lyrics, childeren of this Field are ignored
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

        /// <summary>
        /// Effects .. what else ?
        /// </summary>
        public List<LyricEffect> Effects { set; get; }
    }
}
