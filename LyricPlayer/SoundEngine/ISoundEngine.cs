using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricPlayer.Models;

namespace LyricPlayer.SoundEngine
{
    public interface ISoundEngine
    {
        PlayerStatus Status { get; }
        TimeSpan CurrentTime { get; set; }
        FileInfo CurrentFileInfo { get; }
        event EventHandler TrackStopped;

        int TrackLength { get; }
        float Volume { get; set; }
        bool Muted { set; get; }
        void Play();
        void Pause();
        void Resume();
        void Stop();

        void Load(FileInfo trackInfo);
    }
}
