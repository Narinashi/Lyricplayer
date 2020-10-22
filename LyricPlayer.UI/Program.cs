using GameOverlay.Drawing;
using GameOverlay.Windows;
using System;


namespace LyricPlayer.UI
{
    class Program
    {
        static FastNoise noise = new FastNoise();
        public static void Main()
        {
            //Test();
            var overlay = new Overlay.LyricOverlay();
            overlay.ShowOverlay(string.Empty);
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Coming For You 🔥 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - GOT THIS 🔥.mp3" });

            //overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Mystify [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Best of Me 🤘 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Comeback 🔥[Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Crown 👑 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Damn Gurl [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Deep Thoughts [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Fear [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Fight Back [Official Video].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Gibberish [Official Video].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Careless 💔 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Blow Up 💣 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Blessed 🙏 [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Broken Dreams [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Never Give Up ☝️ [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Graveyard [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Graveyard [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Life ✨ [Copyright Free].mp3" });
            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Light It Up🔥🤘 [Copyright Free].mp3" });

            overlay.Player.Playlist.Add(new Models.TrackInfo { FileAddress = @"I:\Neffex\NEFFEX - Numb [Copyright Free].mp3" });

            while (true)
            {
                Console.ReadLine();

                overlay.Player.Next();
            }

            overlay.Overlay.Join();
        }


        public static void Test()
        {
            float trauma = 1;
            float traumaMult = 60f; //the power of the shake
            float traumaMag = 18f; //the range of movment
           
            float traumaDecay = 0.01f; //how quickly the shake falls off
            float timeCounter = 0;

            var overlay = new GraphicsWindow()
            {
                FPS = 100,
                Height = 786,
                Width = 1366,
                IsTopmost = true
            };
            string testString = "OVERLOAD";
            Font font = null;
            SolidBrush brush = null;
            Point location = new Point();
            Point renderSize = new Point();

            overlay.SetupGraphics += (s, e) =>
            {
                e.Graphics.MeasureFPS = true;
                font = e.Graphics.CreateFont("Antonio", 125, true);
                brush = e.Graphics.CreateSolidBrush(210, 210, 210, 255);

                e.Graphics.TextAntiAliasing = true;
            };

            overlay.DrawGraphics += (s, e) =>
            {
                var gfx = e.Graphics;
                overlay.IsTopmost = true;
                Console.Title = $"FPS: {gfx.FPS.ToString()}";

                gfx.BeginScene();

                renderSize = e.Graphics.MeasureString(font, testString);
                location = new Point((overlay.Width - renderSize.X) / 2, (overlay.Height - renderSize.Y) / 2);
                gfx.ClearScene(new Color(0, 0, 0, 210));

                var deltaTime = e.DeltaTime / 1000f;
                timeCounter += deltaTime * (float)Math.Pow(trauma, 0.3f) * traumaMult;
                //Bind the movement to the desired range
                var point = GetPoint(1, timeCounter);

                point.X *= traumaMag * trauma; 
                point.Y *= traumaMag * trauma;

                location.X += point.X;
                location.Y += point.Y;
                //decay faster at higher values
                trauma -= deltaTime * traumaDecay * (trauma + 0.3f);


                e.Graphics.DrawText(font, brush, location, testString);

                gfx.EndScene();
            };
            overlay.Create();
            overlay.Show();
            overlay.Join();
        }

        public static Point GetPoint(float time, float seed)
        {
            var f1 = noise.GetPerlin(time, seed);
            var f2 = noise.GetPerlin(seed, time);
            return new Point
            {
                X = f1,
                Y = f2
            };
        }

    }
}
