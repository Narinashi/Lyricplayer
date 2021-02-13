using LyricPlayer.UI.Overlay.EffectPlayers;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace LyricPlayer.UI
{
    internal static class Extentions
    {
        public static GameOverlay.Drawing.Color ToOverlayColor(this Color color, float alpha = -1) =>
             new GameOverlay.Drawing.Color { A = alpha==-1 ? color.A/255f : alpha, B = color.B / 255f, G = color.G / 255f, R = color.R / 255f };

        public static GameOverlay.Drawing.Point ToOverlayPoint(this Point point) =>
            new GameOverlay.Drawing.Point { X = point.X, Y = point.Y };

        public static GameOverlay.Drawing.Point ToOverlaySize(this Size size) =>
            new GameOverlay.Drawing.Point { X = size.Width, Y = size.Height };

        public static GameOverlay.Drawing.Color Add(this GameOverlay.Drawing.Color color, GameOverlay.Drawing.Color secondColor) => new GameOverlay.Drawing.Color
        {
            A = (((color.A + secondColor.A) % 1f) + 1f) % 1f,
            B = (((color.B + secondColor.B) % 1f) + 1f) % 1f,
            G = (((color.G + secondColor.G) % 1f) + 1f) % 1f,
            R = (((color.R + secondColor.R) % 1f) + 1f) % 1f,
        };

        public static bool HasType<T, U>(this List<U> list) 
            => list.FirstOrDefault(x => x.GetType() == typeof(T)) != null;
    }
}
