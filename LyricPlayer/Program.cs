using System;
using System.IO;
using System.Windows;

namespace LyricPlayer
{
    class Program
    {
        static void Main()
        {
            var token = File.ReadAllText("Token.Token");
            var fetcher = new LyricFetcher.MusicmatchLyricFetcher.MusicmatchLyricFetcher(token)
            {
                UseProxy = true,
                ProxyUrl = new Uri("http://192.168.1.110:8888")
            };

            var player = new MusicPlayer.NAudioPlayer();
            var info = new Models.FileInfo { FileAddress = @"I:\Neffex\NEFFEX - Never Give Up ☝️ [Copyright Free].mp3" };
            player.Load(info);
            player.Play();

            var lyrics = fetcher.GetLyric(Path.GetFileNameWithoutExtension(info.FileAddress), "", "", "", player.TrackLength);
            var lyricController = new LyricController.NarinoLyricController();
            lyricController.Load(lyrics);
            lyricController.LyricChanged += (s, e) => { Console.WriteLine(e.Text); };
            lyricController.Start();
            lyricController.CurrentTime = player.CurrentTime;
            
            var time = 0;
            while (true)
            {
                var data = Console.ReadLine();
                if(int.TryParse(data,out time))
                {
                    player.CurrentTime = TimeSpan.FromSeconds(time);
                    lyricController.CurrentTime = player.CurrentTime;
                }
            }
        }

    }
}
