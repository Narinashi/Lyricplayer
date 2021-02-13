using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer
{
    static class Extentions
    {
        public static string ReplaceToValidFileName(this string filename) => filename.Replace("\"", "").Replace("/", "")
                .Replace("\\", "").Replace("*", "").Replace(":", "").Replace("<", "").
                Replace(">", "").Replace("?", "");
    }
}
