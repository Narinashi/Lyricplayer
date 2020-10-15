using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricPlayer.Models;

namespace LyricPlayer.LyricFetcher
{
    public interface ILyricFetcher
    {
        TrackLyric GetLyric(string trackName, string Title, string Album, string Artist,double trackLength);
    }
}
