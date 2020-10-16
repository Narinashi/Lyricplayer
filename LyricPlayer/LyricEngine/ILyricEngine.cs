using LyricPlayer.Models;
using System;

namespace LyricPlayer.LyricEngine
{
    public interface ILyricEngine
    {
        PlayerStatus Status { get; }
        TimeSpan CurrentTime { get; set; }
        int Offset { set; get; }

        void Start();
        void Pause();
        void Resume();
        void Stop();
        void Load(TrackLyric lyric);
    }
}
