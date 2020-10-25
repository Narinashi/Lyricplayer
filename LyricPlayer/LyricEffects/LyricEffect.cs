using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.LyricEffects
{
    public abstract class LyricEffect
    {
        /// <summary>
        /// in Milliseconds
        /// </summary>
        public int Duration { set; get; }
        public bool Instant => Duration <= 0;
    }
}
