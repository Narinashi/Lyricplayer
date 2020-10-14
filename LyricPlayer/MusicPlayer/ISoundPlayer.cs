using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricPlayer.Models;

namespace LyricPlayer.MusicPlayer
{
    public interface ISoundPlayer
    {
        PlayerStatus PlayerStatus { get; }
        TimeSpan CurrentTime { get; }
        FileInfo CurrentFileInfo { get; }

        float Volume { get; set; }
        bool Muted { set; get; }

        void Play();
        void Pause();
        void Stop();

        void Load(FileInfo trackInfo);
    }
}
