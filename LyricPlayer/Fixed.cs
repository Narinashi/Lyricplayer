using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer
{
    static class Fixed
    {
        public const float AlmostZero = 0.0001f;
        public static int RNG(int from,int to)
        {
            var random = new Random();
            return random.Next(from, to);
        }
    }
}
