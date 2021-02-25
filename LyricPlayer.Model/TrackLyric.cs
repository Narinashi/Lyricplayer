using LyricPlayer.Model.Elements;
using System.Collections.Generic;

namespace LyricPlayer.Model
{
    public class TrackLyric
    {
        public RenderElement RootElement { set; get; }
        public bool Synchronized { set; get; }
        public string Copyright { set; get; }
    }
}
