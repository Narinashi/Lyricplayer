using Newtonsoft.Json;

namespace LyricPlayer.Model.Elements
{
    public class ImageElement : RenderElement
    {
        public ImageElement(Lyric lyric) : base(lyric) { }
        public ImageElement() : base() { }

        [JsonIgnore]
        public byte[] ImageContent { set; get; }
    }
}
