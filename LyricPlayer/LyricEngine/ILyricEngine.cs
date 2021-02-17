using LyricPlayer.Model;
using LyricPlayer.SoundEngine;
using System;
using System.Collections.Generic;

namespace LyricPlayer.LyricEngine
{
    public interface ILyricEngine
    {
        LyricPlayerStaus Status { get; }
        TimeSpan CurrentTime { get; set; }
        TrackLyric TrackLyric { get; }
        List<Lyric> PlayingLyrics { get; }
        int Offset { set; get; }

        event EventHandler LyricChanged;

        void Load(TrackLyric lyric, ISoundEngine soundEngine);
        void Start();
        void Pause();
        void Resume();
        void Stop();
        void Clear();
        
    }
}
