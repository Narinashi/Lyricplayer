using GameOverlay.Drawing;
using Newtonsoft.Json;

namespace LyricPlayer.Model.Elements
{
    public class ImageElement : RenderElement
    {
        public ImageElement(Lyric lyric) : base(lyric) { }
        public ImageElement() : base() { }

        [JsonIgnore]
        public Image Image { set; get; }
    }
}
