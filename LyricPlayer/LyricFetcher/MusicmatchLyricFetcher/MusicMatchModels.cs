using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.LyricFetcher.MusicmatchLyricFetcher
{
    public class Time
    {
        public double total { get; set; }
        public int minutes { get; set; }
        public int seconds { get; set; }
        public int hundredths { get; set; }
    }

    public class MusicmatchLyricMDL
    {
        public string text { get; set; }
        public Time time { get; set; }
    }
}
