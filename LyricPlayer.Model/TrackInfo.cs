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
        public string AlbumName { set; get; }
        public string ArtistName { set; get; }
        public string TrackName { set; get; }
        public double TrackLength { set; get; }
        public string SpotifyTrackID { set; get; }
    }
}
