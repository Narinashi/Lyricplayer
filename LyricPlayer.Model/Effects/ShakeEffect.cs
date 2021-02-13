namespace LyricPlayer.Model.Effects
{
#pragma warning disable CS0659
    public class ShakeEffect : Effect
    {
#pragma warning restore CS0659
        public float Trauma { set; get; }
        public float TraumaMult { set; get; }
        public float TraumaMag { set; get; }
        public float TraumaDecay { set; get; }
        public float TimeCounter { set; get; }

        public override bool Equals(object obj)
        {
            var other = (ShakeEffect)obj;
            return
                Trauma == other.Trauma &&
                TraumaMult == other.TraumaMult &&
                TraumaMag == other.TraumaMag &&
                TraumaDecay == other.TraumaDecay;
        }
    }
}
