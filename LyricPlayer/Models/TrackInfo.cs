using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.Models
{
    public class TrackInfo : FileInfo
    {
        public string Title { set; get; }
        public string Album { set; get; }
        public string[] Artists { set; get; }
        public string JoinedArtistNames => Artists?.Any() ?? false ? string.Join(";", Artists) : string.Empty;
        public string TrackName { set; get; }
    }
}
