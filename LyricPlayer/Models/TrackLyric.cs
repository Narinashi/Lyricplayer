using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.Models
{
    public class TrackLyric
    {
        public bool Synchronized { set; get; }
        public List<Lyric> Lyric { set; get; }
        public string Copyright { set; get; }

    }
}
