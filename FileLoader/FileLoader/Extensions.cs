using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLoader
{
    static class Extensions
    {
        public static Dictionary<char, int> CharacterCount(this string text)
        {
            return text.GroupBy(c => c)
                       .OrderBy(c => c.Key)
                       .ToDictionary(grp => grp.Key, grp => grp.Count());
        }
    }
}
