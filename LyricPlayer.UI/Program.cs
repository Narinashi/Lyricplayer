using System;
using System.IO;

namespace LyricPlayer.UI
{
    internal class Program
    {
        public static float Mulitplier { set; get; } = 1f;
        public static void Main()
        {
            var overlay = new Overlay.LyricOverlay();
            overlay.ShowOverlay(string.Empty);

            if (File.Exists("Tracks.txt"))
            {
                var files = File.ReadAllLines("Tracks.txt");
                foreach (var file in files)
                {
                    if (string.IsNullOrEmpty(file.Trim()) ||
                        file.StartsWith("#") ||
                        file.StartsWith("//"))
                        continue;

                    if (Directory.Exists(file))
                    {
                        var directoryFiles = Directory.GetFiles(file);
                        foreach (var directoryFile in directoryFiles)
                            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = directoryFile });
                    }
                    else if (File.Exists(file))
                        overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = file });
                }
            }

            var time = 0;
            var str = string.Empty;
            while (true)
            {
                var data = Console.ReadKey();

                if (data.Key == ConsoleKey.Spacebar || data.Key == ConsoleKey.S)
                {
                    if (overlay.Player.PlayerStatus == Models.PlayerStatus.Playing)
                        overlay.Player.Pause();
                    else
                        overlay.Player.Play();
                }
                else if (char.IsDigit(data.KeyChar))
                    str += data.KeyChar;
                else if (data.Key == ConsoleKey.Backspace || data.Key == ConsoleKey.BrowserBack && str.Length > 0)
                    str = str.Substring(0, str.Length - 1);
                else if (data.Key == ConsoleKey.N)
                    overlay.Player.Playlist.Next();
                else if (data.Key == ConsoleKey.P)
                    overlay.Player.Playlist.Previous();
                else if (data.Key == ConsoleKey.Enter && int.TryParse(str, out time))
                {
                    overlay.Player.CurrentTime = TimeSpan.FromSeconds(time);
                    str = string.Empty;
                }
                else if (data.Key == ConsoleKey.Enter && string.IsNullOrEmpty(str))
                    overlay.Player.Next();
                else if (data.Key == ConsoleKey.UpArrow)
                    Mulitplier *= 2f;
                else if (data.Key == ConsoleKey.DownArrow)
                    Mulitplier /= 2f;
                else if (data.Key == ConsoleKey.RightArrow)
                    overlay.Player.CurrentTime = overlay.Player.CurrentTime.Add(TimeSpan.FromSeconds(5));
                else if(data.Key == ConsoleKey.LeftArrow)
                {
                    if (overlay.Player.CurrentTime.TotalSeconds <= 5)
                        overlay.Player.CurrentTime = TimeSpan.Zero;
                    else
                        overlay.Player.CurrentTime = overlay.Player.CurrentTime.Subtract(TimeSpan.FromSeconds(5));
                }

                if (String.IsNullOrEmpty(str))
                    Console.Clear();
            }

            overlay.Overlay.Join();
        }
    }
}
