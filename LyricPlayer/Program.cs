using LyricPlayer.LyricEngine;
using LyricPlayer.LyricFetcher.MusicmatchLyricFetcher;
using LyricPlayer.Models;
using LyricPlayer.PlaylistController;
using LyricPlayer.SoundEngine;
using System;
using System.IO;
using System.Windows;

namespace LyricPlayer
{
    class Program
    {
        static void Main()
        {
            var player = new TestMusicPlayer();
            var info = new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Never Give Up ☝️ [Copyright Free].mp3" };

            player.Initialize();
            player.Playlist.Add(info);

            var time = 0;
            while (true)
            {
                var data = Console.ReadLine();
                if(int.TryParse(data,out time))
                    player.CurrentTime = TimeSpan.FromSeconds(time);

                else if(data=="p")
                    player.Pause();
                else if(data=="r")
                    player.Play();
                
            }
        }
       
    }
    class TestMusicPlayer : MusicPlayer.MusicPlayer<TrackInfo>
    {
        public TestMusicPlayer()
        {
            Playlist = new PlaylistController<TrackInfo>();
        }
        public override void Initialize()
        {
            SoundEngine = new NAudioPlayer();
            LyricEngine = new NarinoLyricEngine();
            (LyricEngine as NarinoLyricEngine).LyricChanged += (s, e) => { Console.WriteLine(e.Text); };
            var token = File.ReadAllText("Token.Token");
            LyricFetcher = new MusicmatchLyricFetcher(token)
            {
                UseProxy = false,
                ProxyUrl = new Uri("http://192.168.1.110:8888")
            };
        }
    }
}
