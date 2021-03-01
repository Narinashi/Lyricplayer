using Newtonsoft.Json;

namespace LyricPlayer.Model.Elements
{
    public class ImageElement : RenderElement
    {
        public ImageElement() : base() { }

        
        public byte[] ImageContent { set; get; }
        public float Opacity { set; get; } = 0.75f;
        public string ImagePath { set; get; }

        [JsonIgnore]
        public bool Initializing { set; get; }
        public override void Dispose()
        {
            base.Dispose();
            ImageContent = null;
            ImagePath = null;
        }
    }
}
