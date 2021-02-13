using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.Model
{
    public class TrackInfo : FileInfo
    {
        public string Title { set; get; }
        public string Album { set; get; }
        public string Artists { set; get; }
        public string TrackName { set; get; }
    }
}
