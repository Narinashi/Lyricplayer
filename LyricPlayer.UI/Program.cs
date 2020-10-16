namespace LyricPlayer.UI
{
    class Program
    {
        public static void Main()
        {
            var overlay = new Overlay.LyricOverlay();
            overlay.ShowOverlay(string.Empty);
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Never Give Up ☝️ [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\NEFFEX - Best of Me 🤘 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Blessed 🙏 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Blow Up 💣 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Broken Dreams [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Careless 💔 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Comeback 🔥[Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Coming For You 🔥 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Crown 👑 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Damn Gurl [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Deep Thoughts [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Fear [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Fight Back [Official Video].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Gibberish [Official Video].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - GOT THIS 🔥.mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Graveyard [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Graveyard [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Life ✨ [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Light It Up🔥🤘 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Mystify [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Numb [Copyright Free].mp3" });


            overlay.Overlay.Join();
        }
    }
}
