using LyricPlayer.Models;
using System;

namespace LyricPlayer.LyricController
{
    public interface ILyricController
    {
        PlayerStatus ControllerStatus { get; }
        TimeSpan CurrentTime { get; set; }
        int Offset { set; get; }

        void Start();
        void Pause();
        void Resume();
        void Stop();
        void Load(TrackLyric lyric);
    }
}
