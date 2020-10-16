using GameOverlay.Drawing;
using GameOverlay.Windows;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LyricPlayer.UI
{
    class Program
    {
        public static void Main()
        {
            var window = new GraphicsWindow(0, 0, 400, 200);
            var process = Process.GetProcessesByName("Taskmgr").FirstOrDefault();
            window.FPS = 120;
            
            Point point = new Point();
            Font font = null;
            SolidBrush color = null;
            SolidBrush bgColor = null;
            window.SetupGraphics += (s, e) =>
            {
                window.Graphics.TextAntiAliasing = true;
                //window.Graphics.
                font = window.Graphics.CreateFont("Time New Roman", 15, true,false,true);
                color = window.Graphics.CreateSolidBrush(200, 200, 200);
                point = new Point(10, 10);
                bgColor = window.Graphics.CreateSolidBrush(0f, 0f, 0f, 0.3f);
            };
            window.DestroyGraphics += (s, e) => {
                font.Dispose();
                color.Dispose();
                bgColor.Dispose(); };
            window.DrawGraphics += (s, e) =>
            {
                window.IsTopmost = true;
                var gfx = e.Graphics;
                e.Graphics.BeginScene();
                e.Graphics.ClearScene(bgColor);

                var padding = 8;

                var infoText = new StringBuilder()
                .Append("FPS: ").Append(window.FPS.ToString().PadRight(padding))
                .Append("FrameTime: ").Append(e.FrameTime.ToString().PadRight(padding))
                .Append("FrameCount: ").Append(e.FrameCount.ToString().PadRight(padding))
                .Append("DeltaTime: ").Append(e.DeltaTime.ToString().PadRight(padding))
                .ToString();
                var size = e.FrameTime%120 / 25f + 10;
                if (size < 1)
                    size = 1;

                e.Graphics.DrawText(gfx.CreateFont("Times New Roman",size), color, point, infoText);
                gfx.EndScene();

            };
            window.Create();

            //window.PlaceAbove(process.MainWindowHandle);
           // window.FitTo(process.MainWindowHandle, true);


            window.Show();
            var x = window.IsVisible;
            window.Join();
        }
    }
}
