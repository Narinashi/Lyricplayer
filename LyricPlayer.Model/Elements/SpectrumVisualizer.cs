using LyricPlayer.Model.Elements.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.Model.Elements
{
    public class SpectrumVisualizer : RenderElement
    {
        public SpectrumVisualizationType VisualizationType { set; get; }
        public int BandCount { set; get; }
        public float BandWidth { set; get; }
        public Color BandColor { set; get; }
        public bool Initialized { set; get; }
        public int BandSpace { set; get; }
        public bool Fill { set; get; }
        public float Multiplier { set; get; }
        public float BandRadius { set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is SpectrumVisualizer other))
                return false;
            return other.BandCount == BandCount &&
                other.BandWidth == BandWidth &&
                other.BandColor == BandColor &&
                other.BandSpace == BandSpace &&
                other.Multiplier == Multiplier &&
                other.BandRadius == BandRadius;
        }

        public override int GetHashCode()
        {
            return BandColor.GetHashCode() +
                    BandSpace +
                    BandCount +
                    Multiplier.GetHashCode() +
                    BandWidth.GetHashCode() +
                    BandRadius.GetHashCode();
        }
    }
}
