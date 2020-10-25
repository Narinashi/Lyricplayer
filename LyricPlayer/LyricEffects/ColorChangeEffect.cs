using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.LyricEffects
{
    public class ColorChangeEffect : LyricEffect
    {
        public virtual Color BackgroundColorChangeFrom { set; get; }
        public virtual Color ForeColorChangeFrom { set; get; }
        public virtual Color BackgroundColorChangeTo { set; get; }
        public virtual Color ForeColorChangeTo { set; get; }
    }
}
