using LyricPlayer.Models;
using LyricPlayer.SoundEngine;
using System;

namespace LyricPlayer.LyricEngine
{
    public interface ILyricEngine
    {
        PlayerStatus Status { get; }
        TimeSpan CurrentTime { get; set; }
        TrackLyric TrackLyric { get; }
        int Offset { set; get; }
        event EventHandler<Lyric> LyricChanged;

        void Start();
        void Pause();
        void Resume();
        void Stop();
        void Load(TrackLyric lyric, ISoundEngine soundEngine);
    }
}
