using LyricPlayer.Model;
using LyricPlayer.MusicPlayer;
using LyricPlayer.UI.Overlay;
using System;
using System.IO;

namespace LyricPlayer.UI
{
    internal class Program
    {
        public static float Mulitplier { set; get; } = 2f;
        static LyricOverlay Overlay { set; get; }
        public static void Main()
        {
            Overlay = new LyricOverlay();

            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.Add, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.Subtract, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.A, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.S, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.D, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.W, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.Up, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.Down, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.Left, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.Right, KeyModifiers.Alt);

            HotKeyManager.HotKeyPressed += HotKeyManager_HotKeyPressed;
            var processName = File.Exists("ProcessName.txt") ? File.ReadAllText("ProcessName.txt") : "";
            Overlay.ShowOverlay(processName);

            if (File.Exists("Tracks.txt") && Overlay.MusicPlayer is NAudioPlayer)
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
                            Overlay.MusicPlayer.Playlist.Add(new TrackInfo { FileAddress = directoryFile });
                    }
                    else if (File.Exists(file))
                        Overlay.MusicPlayer.Playlist.Add(new TrackInfo { FileAddress = file });
                }
            }

            var time = 0;
            var str = string.Empty;
            while (true)
            {
                var data = Console.ReadKey();

                if (data.Key == ConsoleKey.Spacebar)
                {
                    if (Overlay.MusicPlayer.Status == PlayerStatus.Playing)
                        Overlay.MusicPlayer.Pause();
                    else
                        Overlay.MusicPlayer.Play();
                }
                else if (char.IsDigit(data.KeyChar))
                    str += data.KeyChar;
                else if (data.Key == ConsoleKey.Backspace || data.Key == ConsoleKey.BrowserBack && str.Length > 0)
                    str = str.Substring(0, str.Length - 1);
                else if (data.Key == ConsoleKey.N)
                    Overlay.MusicPlayer.Playlist.Next();
                else if (data.Key == ConsoleKey.P)
                    Overlay.MusicPlayer.Playlist.Previous();
                else if (data.Key == ConsoleKey.Enter && int.TryParse(str, out time))
                {
                    Overlay.MusicPlayer.CurrentTime = TimeSpan.FromSeconds(time);
                    str = string.Empty;
                }
                else if (data.Key == ConsoleKey.Enter && string.IsNullOrEmpty(str))
                    Overlay.MusicPlayer.Playlist.Next();

                else if (data.Key == ConsoleKey.UpArrow)
                    Mulitplier *= 1.4f;

                else if (data.Key == ConsoleKey.DownArrow)
                    Mulitplier /= 1.4f;

                else if (data.Key == ConsoleKey.RightArrow)
                    Overlay.MusicPlayer.CurrentTime = Overlay.MusicPlayer.CurrentTime.Add(TimeSpan.FromSeconds(5));

                else if (data.Key == ConsoleKey.LeftArrow)
                {
                    if (Overlay.MusicPlayer.CurrentTime.TotalSeconds <= 5)
                        Overlay.MusicPlayer.CurrentTime = TimeSpan.Zero;
                    else
                        Overlay.MusicPlayer.CurrentTime = Overlay.MusicPlayer.CurrentTime.Subtract(TimeSpan.FromSeconds(5));
                }
                else if (data.Key == ConsoleKey.W)
                    Overlay.Overlay.Height += 10;
                else if (data.Key == ConsoleKey.S)
                    Overlay.Overlay.Height -= 10;
                else if (data.Key == ConsoleKey.D)
                    Overlay.Overlay.Width += 20;
                else if (data.Key == ConsoleKey.A)
                    Overlay.Overlay.Width -= 20;

                //if (String.IsNullOrEmpty(str))
                    //Console.Clear();
            }

            Overlay.Overlay.Join();
        }

        private static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            if (Overlay.Renderer == null)
                return;

            switch (e.Key)
            {
                case System.Windows.Forms.Keys.Add:
                    Overlay.Renderer.Offset += 200;
                    break;

                case System.Windows.Forms.Keys.Subtract:
                    Overlay.Renderer.Offset -= 200;
                    break;

                case System.Windows.Forms.Keys.A:
                    Overlay.Overlay.Move(Overlay.Overlay.X - 20, Overlay.Overlay.Y);
                    break;

                case System.Windows.Forms.Keys.D:
                    Overlay.Overlay.Move(Overlay.Overlay.X + 20, Overlay.Overlay.Y);
                    break;

                case System.Windows.Forms.Keys.W:
                    Overlay.Overlay.Move(Overlay.Overlay.X, Overlay.Overlay.Y - 20);
                    break;

                case System.Windows.Forms.Keys.S:
                    Overlay.Overlay.Move(Overlay.Overlay.X, Overlay.Overlay.Y + 20);
                    break;

                case System.Windows.Forms.Keys.Up:
                    Overlay.Overlay.Height += 20;
                    break;

                case System.Windows.Forms.Keys.Down:
                    Overlay.Overlay.Height -= 20;
                    break;

                case System.Windows.Forms.Keys.Left:
                    Overlay.Overlay.Width -= 20;
                    break;

                case System.Windows.Forms.Keys.Right:
                    Overlay.Overlay.Width += 20;
                    break;

                default:
                    return;
            }
            Console.WriteLine($"Changed Renderer Offset to {Overlay.Renderer.Offset}");
        }
    }
}
