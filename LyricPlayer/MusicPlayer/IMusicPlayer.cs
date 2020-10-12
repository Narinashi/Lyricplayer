using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricPlayer.Models;

namespace LyricPlayer.MusicPlayer
{
    public interface IMusicPlayer
    {
        MusicPlayerStatus PlayerStatus { get; }
        TimeSpan CurrentTime { get; }
        TrackInfo CurrentTrack { get; }

        float Volume { get; set; }
        bool Muted { set; get; }

        void Play();
        void Pause();
        void Stop();

        void Load(FileInfo trackInfo);
    }
}
