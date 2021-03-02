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

        public static void Update<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (action == null)
                throw new NullReferenceException("Operation cannot be null");
            if (collection == null)
                throw new NullReferenceException("Source cannot be null");

            foreach (var item in collection)
                action(item);
        }
    }
}
