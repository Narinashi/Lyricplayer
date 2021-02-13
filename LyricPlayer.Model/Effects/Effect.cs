using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.Model.Effects
{
    public class Effect
    {
        public int Duration { set; get; }
        public bool IsInstant => Duration <= 0;
    }
}
