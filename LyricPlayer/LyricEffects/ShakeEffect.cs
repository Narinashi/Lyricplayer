using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LyricPlayer.LyricEffects
{
    public class ShakeEffect : LyricEffect
    {
        public float Trauma { set; get; }
        public float TraumaMult { set; get; }
        public float TraumaRange { set; get; }
        public float TraumaDecay { set; get; }
        [JsonIgnore]
        public float timeCount { set; get; }
    }
}
