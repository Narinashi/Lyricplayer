using LyricPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LyricPlayer.PlaylistController
{
    public class PlaylistController<T> where T : TrackInfo
    {
        public T CurrentTrack => CurrentTrackIndex < Tracks?.Count ? Tracks[CurrentTrackIndex] : null;
        public virtual IEnumerable<T> TrackCollection => Tracks.AsEnumerable();
        public event EventHandler<T> TrackChanged;

        protected List<T> Tracks { get; set; }
        protected int CurrentTrackIndex { get; set; }

        public PlaylistController() { Tracks = new List<T>(); }

        public virtual void Add(T track)
        {
            if (File.Exists(track.FileAddress) || track.FileContent != null)
                Tracks.Add(track);

            if (string.IsNullOrEmpty(track.TrackName))
                track.TrackName = Path.GetFileNameWithoutExtension(track.FileAddress);

            if (Tracks.Count == 1)
                OnTrackChanged(CurrentTrack);
        }
        public virtual void Insert(int index, T track)
        {
            if (File.Exists(track.FileAddress) || track.FileContent != null)
                Tracks.Insert(index, track);

            if (string.IsNullOrEmpty(track.TrackName))
                track.TrackName = Path.GetFileNameWithoutExtension(track.FileAddress);

            if (Tracks.Count == 1)
                OnTrackChanged(CurrentTrack);
        }
        public virtual void RemoveAt(int index)
        {
            var removingPlayingTrack = index == CurrentTrackIndex;

            Tracks.RemoveAt(index);

            if (CurrentTrackIndex >= Tracks.Count)
                CurrentTrackIndex = Tracks.Count - 1;

            if (removingPlayingTrack)
                OnTrackChanged(CurrentTrack);
        }
        public virtual void Remove(T track)
        {
            var removingPlayingTrack = CurrentTrack == track;

            Tracks.Remove(track);

            if (CurrentTrackIndex >= Tracks.Count)
                CurrentTrackIndex = Tracks.Count - 1;

            if (removingPlayingTrack)
                OnTrackChanged(CurrentTrack);
        }
        public virtual void Next()
        {
            if (CurrentTrackIndex < 0 || CurrentTrackIndex >= ((Tracks?.Count-1) ?? 0))
                CurrentTrackIndex = 0;
            else
                CurrentTrackIndex++;

            OnTrackChanged(CurrentTrack);
        }
        public virtual void Previous()
        {
            if (CurrentTrackIndex < 0 || CurrentTrackIndex > ((Tracks?.Count-1) ?? 0))
                CurrentTrackIndex = 0;
            else if (CurrentTrackIndex == 0)
                CurrentTrackIndex = Tracks.Count - 1;
            else
                CurrentTrackIndex--;

            OnTrackChanged(CurrentTrack);
        }

        protected virtual void OnTrackChanged(T track)
        {
            TrackChanged?.Invoke(this, track);
        }
    }
}
